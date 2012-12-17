using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    /// <summary>
    /// Represents an IRC message.
    /// </summary>
    public class IrcMessage
    {
		public IrcMessage()
		{
            this.Command = null;
            this.Parameters = new string[0];
		}

        public IrcMessage(string command)
        {
            this.Command = command;
            this.Parameters = new string[0];
        }

		public IrcMessage(string command, params string[] parameters)
		{
			this.Command = command;
			this.Parameters = parameters;
		}

        public IrcMessage(string command, IEnumerable<string> parameters)
        {
            this.Command = command;
            this.Parameters = parameters.ToArray<string>();
        }

        private IrcMessage(string prefix, string command, IEnumerable<string> parameters)
        {
            this.Prefix = prefix;
            this.Command = command;
            this.Parameters = parameters.ToArray<string>();
        }

        public string Prefix
        { get; set; }

		public string Command
		{ get; set; }

        public string[] Parameters
        { get; set; }

        public override string ToString()
        {
            if (Parameters != null && Parameters.Count() > 0)
            {
                string line = string.Format(
                    "{2} {0} {1}",
                    Command.ToUpper(),
                    string.Join(" ", from p in Parameters select p != null ? (p.Contains(" ") ? ":" : "") + p : ""),
                    Prefix
                );
                return line;
            }
            else
                return Command.ToUpper();
        }

        public static IrcMessage Parse(string rawline)
        {
            int i = 0;
            string prefix = string.Empty, cmd;
            List<string> parameters = new List<string>();

            // Check for a prefix
            if (rawline.StartsWith(":"))
            {
                i++;
                prefix = rawline.Substring(i, (i = rawline.IndexOf(' ', i) + 1) - 1);
            }

            // Parse command
            cmd = rawline.Substring(i, (i = rawline.IndexOf(' ', i) + 1) - 1).ToUpper();

            // Parse parameters
            while (i < rawline.Length)
            {
                var next_space = rawline.IndexOf(' ', i);
                var arg =
                    rawline[i] == ':' // trailing
                    ? rawline.Substring(i + 1)
                    : next_space < 0
                        ? rawline.Substring(i)
                        : rawline.Substring(i, next_space)
                    ;
                parameters.Add(arg);
                i += arg.Length + 1;
            }

            return new IrcMessage(prefix, cmd, parameters);
        }
    }
}
