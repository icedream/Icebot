using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public class IrcSource
    {
        public IrcServer Server { get; set; }

        public string Hostmask { get; set; }

        public string Nick { get { return Hostmask.Split('@', '!')[0]; } }
        public string Ident { get { return Hostmask.Split('@', '!')[1]; } }
        public string Host { get { return Hostmask.Split('@', '!')[2]; } }

        public void Notice(string text)
        {
            Server.Notice(Nick, text);
        }
        public void Message(string text)
        {
            Server.Message(Nick, text);
        }
        public void Action(string text)
        {
            Server.Action(Nick, text);
        }
        public void Ctcp(string cmd)
        {
            Server.Ctcp(Nick, cmd);
        }
        public void Ctcp(string cmd, params string[] arguments)
        {
            Server.Ctcp(Nick, cmd, arguments);
        }
    }

    public class IrcChannelSource
    {
        public IrcSource SourceUser { get; set; }
        public IrcChannel SourceChannel { get; set; }

        public void Kick()
        {
            SourceChannel.Kick(SourceUser.Nick);
        }

        /*
        public void Op()
        {
            SourceChannel.Op(SourceUser.Nick);
        }
        public void Deop()
        {
            SourceChannel.Deop(SourceUser.Nick);
        }
        public void Owner()
        {
            SourceChannel.Owner(SourceUser.Nick);
        }
        public void Deowner()
        {
            SourceChannel.Deowner(SourceUser.Nick);
        }
        public void Protect()
        {
            SourceChannel.Owner(SourceUser.Nick);
        }
        public void Deprotect()
        {
            SourceChannel.Deowner(SourceUser.Nick);
        }
        public void Halfop()
        {
            SourceChannel.Halfop(SourceUser.Nick);
        }
        public void Dehalfop()
        {
            SourceChannel.Dehalfop(SourceUser.Nick);
        }
        public void Voice()
        {
            SourceChannel.Voice(SourceUser.Nick);
        }
        public void Devoice()
        {
            SourceChannel.Devoice(SourceUser.Nick);
        }
        */
    }
}
