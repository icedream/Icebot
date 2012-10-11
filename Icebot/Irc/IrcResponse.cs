using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Icebot.Irc
{
    /// <summary>
    /// Represents a response from an IRC server.
    /// </summary>
    public class IrcStandardReply
    {
        public Bot Bot
        { get; set; }

        public IrcStandardReply Message
        { get; set; }
    }

    /// <summary>
    /// Represents a numeric response from an IRC server.
    /// </summary>
    public class IrcNumericReply : IrcStandardReply
    {
        public bool IsValid
        { get { return Reply != IrcReplyCode.NonNumeric; } }

        public IrcReplyCode Reply
        { get { ushort code = 0; ushort.TryParse(Command, out code); return (IrcReplyCode)code; } }
    }
}
