using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    /// <summary>
    /// Can hold a raw line for events
    /// </summary>
    public class RawLineEventArgs : EventArgs
    {
        public string RawLine;

        public RawLineEventArgs(string line)
        {
            this.RawLine = line;
        }
    }

    /// <summary>
    /// Can hold a response for events
    /// </summary>
    public class IrcResponseEventArgs : EventArgs
    {
        public IrcResponse Response { get; private set; }

        public IrcResponseEventArgs(IrcResponse e)
        {
            this.Response = e;
        }
    }

    /// <summary>
    /// Can hold a command for events
    /// </summary>
    public class IrcCommandEventArgs : EventArgs
    {
        public IrcCommand Command { get; private set; }

        public IrcCommandEventArgs(IrcCommand e)
        {
            this.Command = e;
        }
    }
}
