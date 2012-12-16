using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Api
{
    public enum CommandType
    {
        /// <summary>
        /// This command is a command which can be used by writing "[prefix][name] [arguments]" in a channel,
        /// for example "!test arg1 arg2 ..."
        /// </summary>
        Channel = 1,
        CTCP = 2,
        Private = 3
    }
}
