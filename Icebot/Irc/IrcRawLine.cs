using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Irc
{
    public class IrcRawLine
    {
        public string Command { get; set; }
        public string[] Parameters { get; set; }

        public override string ToString()
        {
            // TODO: Parameter validation
            if (Parameters != null && Parameters.Length > 0)
                return string.Format(
                    "{0} {1}", // COMMAND [Parameters]
                    Command.ToUpper(),
                    (from p in Parameters select p.Contains(' ') ? string.Format(":{0}", p) : p).ToArray<string>()
                );
            else
                return Command.ToUpper();
        }

        internal IrcRawLine()
        {
        }
        public IrcRawLine(string command)
        {
            this.Command = command;
            this.Parameters = new string[0];
        }
        public IrcRawLine(string command, params string[] parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }
    }

    public class IrcStandardReply : IrcRawLine
    {
        public string Source { get; set; }
        public Bot Bot { get; set; }

        public IrcStandardReply(string line, Bot bot)
        {
            this.Bot = bot;

            {
                int sourceStart = line[0] == ':' ? 1 : 0;
                int sourceEnd = line.IndexOf(' ');
                int commandStart = sourceEnd + 1;
                int commandEnd = line.IndexOf(' ', commandStart);
                this.Source = line.Substring(sourceStart, sourceEnd - sourceStart).Substring(1); // :Source
                this.Command = line.Substring(commandStart, commandEnd - commandStart); // Command
                line = line.Substring(commandEnd);
            }

            // Parse all arguments
            var args = new List<string>();
            while (line.Length > 0)
            {
                if (char.IsWhiteSpace(line[0]))
                {
                    line = line.Substring(1);
                    continue;
                }
                else if (line[0] == ':')
                {
                    args.Add(line);
                    line = "";
                    break;
                }
                else
                {
                    string arg = line.Substring(0, line.IndexOf(' '));
                    args.Add(arg);
                    line = line.Substring(arg.Length);
                    continue;
                }
            }

            Parameters = args.ToArray<string>();
        }
    }
}
