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
    public partial class Bot
    {
        TaskScheduler _scheduler;
        TaskFactory _taskfactory;

        Socket _socket;
        Stream _stream;
        StreamReader _reader;
        StreamWriter _writer;

        ManualResetEventSlim _registeredLock = new ManualResetEventSlim(false);

        ILog _log = LogManager.GetLogger(typeof(Bot));

        public event EventHandler<IrcResponseEventArgs> ResponseReceived;
        protected void OnResponseReceived(IrcResponse resp)
        {
            IrcResponseEventArgs e = new IrcResponseEventArgs(resp);
            if (ResponseReceived != null)
                ResponseReceived.Invoke(this, e);
        }
        public event EventHandler<IrcResponseEventArgs> NumericReplyReceived;
        protected void OnNumericReplyReceived(IrcResponse resp)
        {
            IrcResponseEventArgs e = new IrcResponseEventArgs(resp);
            if (NumericReplyReceived != null)
                NumericReplyReceived.Invoke(this, e);
            switch (resp.NumericReply)
            {
                case IrcReplyCode.RPL_MYINFO:
                    _registeredLock.Set();
                    break;
            }
        }
        public event EventHandler<IrcResponseEventArgs> StandardReplyReceived;
        protected void OnStandardReplyReceived(IrcResponse resp)
        {
            IrcResponseEventArgs e = new IrcResponseEventArgs(resp);
            if (StandardReplyReceived != null)
                StandardReplyReceived.Invoke(this, e);
            if (resp.Source.Equals("PING", StringComparison.OrdinalIgnoreCase))
                Pong(resp);
        }
        public event EventHandler<RawLineEventArgs> RawLineReceived;
        protected void OnRawLineReceived(string line)
        {
            RawLineEventArgs e = new RawLineEventArgs(line);
            if (RawLineReceived != null)
                RawLineReceived.Invoke(this, e);
        }
        public event EventHandler<RawLineEventArgs> RawLineSent;
        protected void OnRawLineSent(string line)
        {
            RawLineEventArgs e = new RawLineEventArgs(line);
            if (RawLineSent != null)
                RawLineSent.Invoke(this, e);
        }

        public IPEndPoint Server { get; set; }

        private string _prefix = "!";
        public string Prefix { get { return _prefix; } set { _prefix = value; } }

        public Bot(IPEndPoint server)
        {
            _scheduler = TaskScheduler.Default;
            _taskfactory = new TaskFactory(_scheduler);

            // Fill with empty stacks for the message queueing (actually "stacking")
            for (int i = 0; i < _messageQueue.Length; i++)
                _messageQueue[i] = new Queue<string>();

            this.Server = server;
        }

        public void Start(string nick = "IRCBot", string ident = "bot", string realname = "IRC Bot", string password = null, bool invisible = false, bool receiveWallops = false)
        {
            Connect();
            Thread.Sleep(500);

            // Send password
            if(!string.IsNullOrEmpty(password))
                Pass(password);

            // Send initial nick (or at least try getting that nick)
            Nick(nick);

            // Send user data
            User(ident, receiveWallops, invisible, realname);

            // Asynchronous reading/writing startup
            _taskfactory.StartNew(() => _asyncReadingProcedure(), _asyncCancel.Token);
            _taskfactory.StartNew(() => _asyncWritingProcedure(), _asyncCancel.Token);

            // Wait for being registered
            // TODO: Alternative nicks and other error handling for login
            _registeredLock.Wait();
        }

        internal void Connect()
        {
            _registeredLock.Reset();
            // TODO: Implement SSL support
            // TODO: Implement error handling for connection errors
            _socket = new Socket(Server.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(Server);
            _stream = new NetworkStream(_socket, true);
            // TODO: Implement BufferSize property
            _reader = new StreamReader(_stream, Encoding.UTF8, false, 2048);
            _writer = new StreamWriter(_stream, Encoding.UTF8, 2048);
        }

        internal void Disconnect()
        {
            _stream.Close();
        }

        public void Stop(string reason = "Quitting")
        {
            _asyncCancel.Cancel();
            _writer.WriteLine("QUIT :" + reason);
            while (!_reader.ReadLine().StartsWith("ERROR ", StringComparison.OrdinalIgnoreCase)) ;
            Disconnect();
        }

        private void _lineHandler(string line)
        {
            // Generally, raw line received (even if it is not valid)
            OnRawLineReceived(line);

            // Parse the response
            var response = IrcResponse.FromRawLine(line);

            // Generally, response received
            OnResponseReceived(response);

            // Specifically, response received
            if (response.IsNumericReply)
                OnNumericReplyReceived(response);
            else if (StandardReplyReceived != null)
                OnStandardReplyReceived(response);
        }



        // Public functions

        public void SendCommand(string command)
        {
            SendCommand(new IrcCommand(command));
        }

        public void SendCommand(string command, params string[] parameters)
        {
            SendCommand(new IrcCommand(command, parameters));
        }

        public void SendCommand(IrcCommand command)
        {
            Raw(command.ToString());
        }

        public void Raw(string line, int priority = 0)
        {
            if (priority < 0 || priority >= _messageQueue.Length)
                throw new ArgumentOutOfRangeException("priority", priority, string.Format("Must be between 0 and {0}", _messageQueue.Length - 1));
            
            _messageQueue[priority].Enqueue(line);
            _asyncQueueLock.Set();

            OnRawLineQueued(line);
        }
    }
}
