using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public class IrcMessage
    {
        public MessageType Type { get; set; }
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
        public IrcResponse Response { get; set; }
        public IrcMessage Message { get; set; }

        public ReceivedIrcMessage(IrcResponse response)
        {
            Response = response;

            var msg = new IrcMessage();
            msg.Target = response.Parameters[0];
            msg.Type = response.Command.Equals("PRIVMSG", StringComparison.OrdinalIgnoreCase) ? MessageType.PrivMsg : MessageType.Notice;
            msg.Text = response.Parameters[1];

            Message = msg;
        }
    }
}
