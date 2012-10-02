using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Icebot
{
    /// <summary>
    /// Represents an IRC message. This is the base class for RfcCommand and RfcReply.
    /// </summary>
    public class RfcMessage
    {
        private string[] parameters;
        private string ircSource;

        /// <summary>
        /// Parameters of the message
        /// </summary>
        public string[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Source hostmask of the message (in the form nick!user@host)
        /// </summary>
        public string IrcSource
        {
            get { return ircSource; }
            set { ircSource = value; }
        }

        /// <summary>
        /// Source nickname of the message
        /// </summary>
        public string SourceNick
        {
            get
            {
                return ExtractString(@"^(.*)!", ircSource);
            }
        }

        /// <summary>
        /// Source username of the message
        /// </summary>
        public string SourceUser
        {
            get
            {
                return ExtractString(@"!(.*)@", ircSource);
            }
        }

        /// <summary>
        /// Source hostname of the message
        /// </summary>
        public string SourceHost
        {
            get
            {
                return ExtractString(@"@(.*)$", ircSource);
            }
        }

        /// <summary>
        /// Parameter line string of the message
        /// </summary>
        public string ParameterText
        {
            get
            {
                if (Parameters != null)
                    return string.Join(" ", Parameters);
                else return null;
            }
            set
            {
                if (value != null)
                    Parameters = new string[] { value };
                else Parameters = null;
            }
        }

        protected string ExtractString(string mask, string text)
        {
            Regex r = new Regex(mask);
            Match m = r.Match(text);
            if (m.Success && m.Groups.Count >= 1) return m.Groups[1].Value;
            else return text;
        }
    }
}
