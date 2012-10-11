using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public enum MessageType
    {
        PrivMsg = 1,
        Notice = 2,
        // InternalDeprecated = 3,
        // InternalDeprecated = 4,
        CtcpRequest = 5,
        CtcpReply = 6
    }
}
