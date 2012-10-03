using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public partial class Bot
    {
        private bool _autoPingReply = true;
        public bool AutoPingReply { get { return _autoPingReply; } set { _autoPingReply = value; } }
    }
}
