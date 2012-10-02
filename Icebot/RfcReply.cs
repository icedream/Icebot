using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    /// <summary>
    /// Numeric Reply
    /// </summary>
    public class RfcReply : RfcMessage
    {
        private RfcReplyCode code;
        /// <summary>
        /// The reply code of the message
        /// </summary>
        public RfcReplyCode Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        /// <summary>
        /// Converts the reply to a raw line which is ready to send directly to the IRC server
        /// </summary>
        /// <returns>String to send directly to the IRC server</returns>
        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (IrcSource != null)
            {
                s.Append(":");
                s.Append(IrcSource);
                s.Append(" ");
            }
            s.Append(((int)Code).ToString("d3"));
            if (Parameters != null && Parameters.Length > 1)
            {
                s.Append(" ");
                s.Append(string.Join(" ", Parameters, 0, Parameters.Length - 1));
            }
            if (Parameters != null)
            {
                s.Append(" :");
                s.Append(Parameters[Parameters.Length - 1]);
            }
            s.Append("\r\n");
            return s.ToString();
        }


    }
}
