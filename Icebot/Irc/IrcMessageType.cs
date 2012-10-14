using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Irc
{
    public enum IrcMessageType
    {
        PrivMsg = 1,
        Notice = 2,
        // InternalDeprecated = 3,
        // InternalDeprecated = 4,
        CtcpRequest = 5,
        CtcpReply = 6
    }
}
