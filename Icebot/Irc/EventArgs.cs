using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Irc
{
    public class RawEventArgs : EventArgs
    {
        public IrcRawLine Raw { get; private set; }

        public RawEventArgs(IrcRawLine raw)
        {
            this.Raw = raw;
        }
    }

    public class RawEventArgs : EventArgs
    {
        public IrcRawLine Raw { get; private set; }

        public RawEventArgs(IrcRawLine raw)
        {
            this.Raw = raw;
        }
    }
}
