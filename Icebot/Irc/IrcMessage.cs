using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Irc
{
    public class IrcMessage
    {
        public IrcMessageType Type { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }
        public bool IsPublic
        {
            get
            {
                // TODO: Use server's information about channel name prefixes to check if target is a channel
                return Target.Contains("#");
            }
        }
    }

    public class ReceivedIrcMessage : IrcMessage
    {
        public IrcStandardReply Reply { get; set; }
        public IrcMessage Message { get; set; }

        public ReceivedIrcMessage(IrcStandardReply reply)
        {
            Reply = reply;

            var msg = new IrcMessage();
            msg.Target = reply.Parameters[0];
            msg.Type = reply.Command.Equals("PRIVMSG", StringComparison.OrdinalIgnoreCase) ? MessageType.PrivMsg : MessageType.Notice;
            msg.Text = reply.Parameters[1];

            Message = msg;
        }
    }
}
