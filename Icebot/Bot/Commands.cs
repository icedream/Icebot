using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public partial class Bot
    {
        public void User(string ident, bool receiveWallops, bool invisible, string realname)
        {
            // TODO: Implement handling for ERR_ALREADYREGISTERED
            SendCommand("user",
                ident,
                (((receiveWallops ? 1 : 0) << 2) + ((invisible ? 1 : 0) << 3)).ToString(),
                "*",
                realname
                );
        }

        /*
        private void Quit(string reason = "Quitting")
        {
            SendCommand("quit", reason);
        }
         */

        public void Pass(string password)
        {
            // TODO: Implement handling for ERR_ALREADYREGISTERED
            SendCommand("pass", password);
        }

        public void Nick(string nickname)
        {
            // TODO: Implement error handling for this command
            SendCommand("nick", nickname);
        }

        public void Privmsg(string target, string message)
        {
            SendCommand("privmsg", target, message);
        }

        public void Privmsg(string[] targets, string message)
        {
            SendCommand("privmsg", string.Join(",", targets), message);
        }

        public void Notice(string target, string message)
        {
            SendCommand("notice", target, message);
        }

        public void Notice(string[] targets, string message)
        {
            SendCommand("notice", string.Join(",", targets), message);
        }

        public void Join(params string[] channels)
        {
            SendCommand("join", string.Join(",", channels));
        }

        public void Part(string channel, string reason = "Leaving")
        {
            SendCommand("join", channel, reason);
        }

        public void Part(string[] channels, string reason = "Leaving")
        {
            SendCommand("join", string.Join(",", channels), reason);
        }

        public void Oper(string username, string password)
        {
            SendCommand("oper", username, password);
        }

        public void Ping(string data)
        {
            SendCommand("ping", data);
        }

        internal void Pong(string data)
        {
            SendCommand("pong", data);
        }

        private void Pong(IrcCommand data)
        {
            SendCommand("pong", data.Parameters[0]);
        }
    }
}
