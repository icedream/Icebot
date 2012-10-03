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
        // Private members

        TaskScheduler _scheduler;
        TaskFactory _taskfactory;

        Stack<string>[] _messageQueue = new Stack<string>[3];

        Stream _stream;
        StreamReader _reader;
        StreamWriter _writer;

        ILog _log;


        // Event locks

        ManualResetEventSlim _asyncReadingLock = new ManualResetEventSlim(true);
        ManualResetEventSlim _asyncWritingLock = new ManualResetEventSlim(true);
        ManualResetEventSlim _asyncQueueLock = new ManualResetEventSlim(false);



        // Events

        public event EventHandler<IrcResponseEventArgs> ResponseReceived;
        public event EventHandler<IrcResponseEventArgs> NumericReplyReceived;
        public event EventHandler<IrcResponseEventArgs> StandardReplyReceived;
        public event EventHandler<RawLineEventArgs> RawLineReceived;
        public event EventHandler<RawLineEventArgs> RawLineQueued;
        public event EventHandler<RawLineEventArgs> RawLineSent;


        // Public properties
        private ushort _queueInterval = 500;
        public ushort QueueInterval { get { return _queueInterval; } set { _queueInterval = value; } }
        private ushort _queueIntervalAfterMessages = 3; // TODO: Find a better name for this.
        public ushort QueueIntervalAfterMessages { get { return _queueInterval; } set { _queueIntervalAfterMessages = value; } }


        // Constructors

        public Bot()
        {
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _taskfactory = new TaskFactory(_scheduler);

            for (int i = 0; i < _messageQueue.Length; i++)
                _messageQueue[i] = new Stack<string>();
        }



        // Private functions

        private void _lineHandler(string line)
        {
            // Generally, raw line received (even if it is not valid)
            if (RawLineReceived != null)
                RawLineReceived.Invoke(this, new RawLineEventArgs(line));

            // Parse the response
            var response = IrcResponse.FromRawLine(line);

            // Generally, response received
            if (ResponseReceived != null)
                ResponseReceived.Invoke(this, new IrcResponseEventArgs(response));

            // Specifically, response received
            if (response.IsNumericReply && NumericReplyReceived != null)
                NumericReplyReceived.Invoke(this, new IrcResponseEventArgs(response));
            else if (StandardReplyReceived != null)
                StandardReplyReceived.Invoke(this, new IrcResponseEventArgs(response));
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

                    lock (_asyncReadingLock)
                    {
                        // Read from stream
                        string recv = _reader.ReadLine();
                        _log.DebugFormat("RECV: {0}", recv);

                        // Asynchronously handle the line
                        _taskfactory.StartNew(() => _lineHandler(recv));
                    }
                }
                catch (Exception eng)
                {
                    _log.Error("Error in reading thread", eng);
                }
            }
        }

        private void _asyncWritingProcedure()
        {
            _log.Debug("Asynchronous writing loop started (this is the asyncWritingProcedure thread)");

            while (true)
            {
                try
                {
                    // Wait for the queue to be filled
                    _asyncQueueLock.Wait(); // Needs less cpu then Thread.Sleep. Just a bit. At least that. ._.
                    _asyncQueueLock.Reset();
                    lock (_asyncQueueLock)
                    {
                        // Wait for the async event to be set
                        _asyncWritingLock.Wait();

                        ushort msg = 0;

                        // This should emulate the anti-flood of the ZNC bouncer
                        lock (_asyncWritingLock)
                        {
                            for (int i = _messageQueue.Length - 1; i > 0; i--)
                                while (_messageQueue[i].Count > 0)
                                {
                                    string line = _messageQueue[i].Pop();
                                    // TODO: Count timing of the event invocation and subtract it from total wait time
                                    _writer.WriteLine(line);
                                    _writer.Flush();
                                    if (RawLineSent != null)
                                        RawLineSent.Invoke(this, new RawLineEventArgs(line));
                                    if(msg == QueueIntervalAfterMessages)
                                        Thread.Sleep(this.QueueInterval);
                                    else
                                        msg++;
                                }
                        }
                    }
                }
                catch (Exception eng)
                {
                    _log.Error("Error in writing thread", eng);
                }
            }
        }



        // Public functions

        public void SendCommand(string command)
        {
            SendCommand(new IrcCommand(command));
        }

        public void SendCommand(IrcCommand command)
        {
            Raw(command.ToString());
        }

        public void Raw(string line, int priority = 0)
        {
            if (priority < 0 || priority >= _messageQueue.Length)
                throw new ArgumentOutOfRangeException("priority", priority, string.Format("Must be between 0 and {0}", _messageQueue.Length - 1));
            
            _messageQueue[priority].Push(line);
            _asyncQueueLock.Set();

            if (RawLineQueued != null)
                RawLineQueued.Invoke(this, new RawLineEventArgs(line));
        }
    }
}
