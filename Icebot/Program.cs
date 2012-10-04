using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Xml;


namespace Icebot
{
    class Program
    {
        public static ILog _log = LogManager.GetLogger(typeof(Program));
        static List<Bot> _bots = new List<Bot>();

        static void Usage()
        {
            Console.WriteLine("Usage: Icebot.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Supported options:");
            Console.WriteLine("\t-c, --config-file FILE");
            Console.WriteLine("\t\tLoads a configuration file instead reading from standard input.");
            //Console.WriteLine("\t-b, --background");
            //Console.WriteLine("\t\tRun the bot in the background.");
            //Console.WriteLine("\t-p, --pid-file FILE");
            //Console.WriteLine("\t\tSave the PID to a specific ile.");
            Console.WriteLine();
            Console.WriteLine("Exit codes:");
            Console.WriteLine("\t0\tBot has shut down successfully.");
            Console.WriteLine("\t-1\tConfiguration could not be read from file.");
            Console.WriteLine("\t-2\tConfiguration could not be parsed.");
            return;
        }

        static int Main(string[] args)
        {
            Queue<string> arguments = new Queue<string>(args);

            // Load configuration from stdin
            string xml = null;
            while(arguments.Count > 0)
            {
                string name = arguments.Dequeue().ToLower();
                switch(name)
                {
                    case "--config-file":
                    case "-c":
                        if(arguments.Count < 1)
                        {
                            Console.Error.WriteLine("You need to specify a configuration file when using {0}.", name);
                            return -1;
                        }
                        try {
                        xml = System.IO.File.ReadAllText(arguments.Dequeue());
                        } catch (System.Security.SecurityException err) {
                            Console.Error.WriteLine("Config file can't be opened under these security circumstances.");
#if DEBUG
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Stacktrace:");
                            Console.Error.WriteLine(err.StackTrace);
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Inner exception: " + err.InnerException.Message);
#endif
                            return -1;
                        } catch (System.IO.DirectoryNotFoundException) {
                            Console.Error.WriteLine("The folder which should contain the config file does not exist.");
                            return -1;
                        }catch(System.IO.FileNotFoundException) {
                            Console.Error.WriteLine("Config file does not exist.");
                            return -1;
                        } catch(Exception err) {
                            Console.Error.WriteLine("Error loading config file: " + err.Message);
#if DEBUG
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Stacktrace:");
                            Console.Error.WriteLine(err.StackTrace);
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Inner exception: " + err.InnerException.Message);
#endif
                            return -1;
                        }
                        break;
                    case "-h":
                        Usage();
                        return 0;
                    default:
                        Console.Error.WriteLine("Warning: Unknown parameter " + name);
                        break;
                }
            }

            if(string.IsNullOrEmpty(xml))
                xml = Console.In.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (XmlException err)
            {
                Console.Error.WriteLine("The configuration has an error:");
                Console.Error.WriteLine("\t" + err.Message);
                Console.Error.WriteLine("\tin line " + err.LineNumber + ", position " + err.LinePosition);
#if DEBUG
                Console.Error.WriteLine();
                Console.Error.WriteLine("Stacktrace:");
                Console.Error.WriteLine(err.StackTrace);
                Console.Error.WriteLine();
                Console.Error.WriteLine("Inner exception: " + err.InnerException.Message);
#endif
                return -2;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine("The configuration has an error:");
                Console.Error.WriteLine("\t" + err.Message);
                Console.Error.WriteLine("\tThere are no details about the position of this error in the configuration.");
#if DEBUG
                Console.Error.WriteLine();
                Console.Error.WriteLine("Stacktrace:");
                Console.Error.WriteLine(err.StackTrace);
                Console.Error.WriteLine();
                Console.Error.WriteLine("Inner exception: " + err.InnerException.Message);
#endif
                return -2;
            }

            // Load log4net configuration
            XmlConfigurator.Configure((XmlElement)doc.SelectSingleNode("//icebot/log4net"));

            var servers = doc.SelectSingleNode("//icebot/servers");
            foreach (var server in servers.ChildNodes.OfType<XmlNode>())
            {
                try
                {
                    _log.DebugFormat("Resolving {0}...", server.Attributes["host"].Value);
                    var dnsEntry = System.Net.Dns.GetHostEntry(server.Attributes["host"].Value);
                    ushort port = 0;
                    if (!ushort.TryParse(server.Attributes["port"].Value, out port))
                        throw new FormatException("Invalid port in configuration");
                    bool ipv6allowed = server.SelectSingleNode("child::no-ipv6") == null;
                    Bot bot = null;
                    foreach (var ip in dnsEntry.AddressList)
                    {
                        try
                        {
                            _log.InfoFormat("Connecting to {0}:{1} (using IP {2})...", server.Attributes["host"].Value, port, ip.ToString());
                            bot = new Bot(new System.Net.IPEndPoint(ip, port));
                            bot.StandardReplyReceived += new EventHandler<IrcResponseEventArgs>(bot_StandardReplyReceived);
                            bot.NumericReplyReceived += new EventHandler<IrcResponseEventArgs>(bot_NumericReplyReceived);
                            bot.RawLineQueued += new EventHandler<RawLineEventArgs>(bot_RawLineQueued);
                            bot.RawLineSent += new EventHandler<RawLineEventArgs>(bot_RawLineSent);
                            bot.Start(
                                nick: server.SelectSingleNode("child::nickname") != null ? server.SelectSingleNode("child::nickname").InnerText : "Icebot",
                                ident: server.SelectSingleNode("child::ident") != null ? server.SelectSingleNode("child::ident").InnerText : "icebot",
                                realname: server.SelectSingleNode("child::realname") != null ? server.SelectSingleNode("child::realname").InnerText : "Icebot IRC Bot",
                                password: server.SelectSingleNode("child::password") != null ? server.SelectSingleNode("child::password").InnerText : null,

                                invisible: server.SelectSingleNode("child::invisible") != null,
                                receiveWallops: server.SelectSingleNode("child::receive-wallops") != null
                            );
                            bot.Join(server.SelectSingleNode("child::channels").InnerText.Split(','));
                            break;
                        }
                        catch(Exception err)
                        {
                            bot = null;
                            _log.Warn(string.Format("Connection to {0}:{1} (using IP {2}) failed", server.Attributes["host"].Value, port, ip.ToString()), err);
                        }
                    }
                    if(bot == null)
                        _log.ErrorFormat("Can't establish a connection to {0}:{1}", server.Attributes["host"].Value, port);
                    else
                        _log.InfoFormat("Connection succeeded.");
                }
                catch (Exception err)
                {
                    _log.Error(string.Format("Could not load server configuration for {0}", server.Attributes["host"].Value), err);
                }
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            return 0;
        }

        static void bot_RawLineSent(object sender, RawLineEventArgs e)
        {
            _log.Debug("Direct SEND: " + e.RawLine);
        }

        static void bot_RawLineQueued(object sender, RawLineEventArgs e)
        {
            _log.Debug("Queued SEND: " + e.RawLine);
        }

        static void bot_NumericReplyReceived(object sender, IrcResponseEventArgs e)
        {
            _log.Debug("Numeric reply: " + e.Response.NumericReply.ToString() + " with " + e.Response.Parameters.Length + " arguments");
        }

        static void bot_StandardReplyReceived(object sender, IrcResponseEventArgs e)
        {
            _log.Debug("Standard reply: " + e.Response.Command + " with " + e.Response.Parameters.Length + " arguments");
        }
    }
}
