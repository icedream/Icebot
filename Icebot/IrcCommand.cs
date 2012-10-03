using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    /// <summary>
    /// Represents an IRC command which can be sent to an IRC server.
    /// </summary>
    public class IrcCommand
    {
		public IrcCommand()
		{
            this.Command = null;
            this.Parameters = new string[0];
		}

        public IrcCommand(string command)
        {
            this.Command = command;
            this.Parameters = new string[0];
        }

		public IrcCommand(string command, params string[] parameters)
		{
			this.Command = command;
			this.Parameters = parameters;
		}

		public string Command
		{ get; set; }

        public string[] Parameters
        { get; set; }

        public override string ToString()
        {
            var pa = string.Join(" ", from p in Parameters select (p.Contains(" ") ? ":" : "") + p);
            return string.Format("{0} {1}", Command.ToUpper(), pa);
        }
    }
}
