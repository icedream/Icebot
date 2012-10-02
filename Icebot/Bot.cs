using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;

namespace Icebot
{
    public class Bot
    {
        Thread _asyncReadingThread;
        ManualResetEventSlim _asyncReadingLock = new ManualResetEventSlim(true);
        TaskScheduler _scheduler;
        TaskFactory _taskfactory;

        Stream _stream;
        StreamReader _reader;
        StreamWriter _writer;

        static readonly Regex RX_RECV = new Regex("^[:]*(?<source>.+) (?<reply>[A-Z0-9]+) (?<parameterstring>.+)$", RegexOptions.Compiled | RegexOptions.Singleline);

        ILog _log;

        public Bot()
        {
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _taskfactory = new TaskFactory(_scheduler);
        }

        private void _lineHandler(string line)
        {
            // Parse the message via Regular Expressions
            var match = RX_RECV.Match(line);
            if (match == null)
            {
                _log.Error("Received line is not RFC-conform, ignoring.");
                return;
            }

            // Split the message up
            RfcMessage msg = null;
            ushort replyCode = 0;
            if (ushort.TryParse(match.Groups["reply"].Value, out replyCode))
            {
                msg = new RfcReply();
                ((RfcReply)(msg = new RfcReply())).Code = (RfcReplyCode)replyCode;
            }
            else
            {
                ((RfcCommand)(msg = new RfcCommand())).Command = match.Groups["reply"].Value;
            }
            msg.IrcSource = match.Groups["source"].Value;
        }

        private void _asyncReadingProcedure()
        {
            _log.Debug("Asynchronous reading loop started (this is the asyncReadingProcedure thread)");

            while (true)
            {
                try
                {
                    // Wait for the async event to be set
                    _asyncReadingLock.Wait();

                    // Read from stream
                    string recv = _reader.ReadLine();
                    _log.DebugFormat("RECV: {0}", recv);

                    // Asynchronously handle the line
                    _taskfactory.StartNew(() => _lineHandler(recv));
                }
                catch (Exception eng)
                {
                    _log.Error("Error in reading thread", eng);
                }
            }
        }
    }
}
