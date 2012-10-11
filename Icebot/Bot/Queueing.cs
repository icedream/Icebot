using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Icebot
{
    public partial class Bot
    {
        Queue<string>[] _messageQueue = new Queue<string>[3];

        ManualResetEventSlim _asyncQueueLock = new ManualResetEventSlim(false);

        public event EventHandler<RawLineEventArgs> RawLineQueued;
        protected void OnRawLineQueued(string line)
        {
            RawLineEventArgs e = new RawLineEventArgs(line);

            if (RawLineQueued != null)
                RawLineQueued.Invoke(this, e);
        }

        private ushort _queueInterval = 500;
        public ushort QueueInterval { get { return _queueInterval; } set { _queueInterval = value; } }
        private ushort _queueIntervalAfterMessages = 3; // TODO: Find a better name for this.
        public ushort QueueIntervalAfterMessages { get { return _queueInterval; } set { _queueIntervalAfterMessages = value; } }
    }
}
