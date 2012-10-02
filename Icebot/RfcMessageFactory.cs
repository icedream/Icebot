using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// TODO: Mark code as copied and modified from Schnirc2 (MIT X11 License)

namespace Icebot
{
    /// <summary>
    /// Contains functions to build message structures from raw messages.
    /// </summary>
    public class RfcMessageFactory
    {
        static RfcMessageFactory()
        {
            commandRegex = new Regex(@"^(:(?<prefix>\S*)\s)?(?<command>[A-Za-z]\w*)(((\s(?<parameter>[^:]\S*)){0,14}(\s:(?<parameter>.*))?)|((\s(?<parameter>[^:]\S*)){14}(\s(?<parameter>.*))))?", RegexOptions.Compiled | RegexOptions.Singleline);
            replyRegex = new Regex(@"^:(?<prefix>\S*)\s(?<reply>\d{3})(((\s(?<parameter>[^:]\S*)){0,14}(\s:(?<parameter>.*))?)|((\s(?<parameter>[^:]\S*)){14}(\s(?<parameter>.*))))?", RegexOptions.Compiled | RegexOptions.Singleline);
        }
        private static Regex commandRegex;
        private static Regex replyRegex;

        /// <summary>
        /// Generates an RfcMessage instance from an incoming IRC message.
        /// </summary>
        /// <param name="message">Die Nachricht vom IRC Server</param>
        /// <returns>Erzeugte Message</returns>
        public RfcMessage Build(string message)
        {
            RfcMessage m = null;
            Match match = commandRegex.Match(message);
            if (match.Success)
            {
                m = new RfcCommand();
                ((RfcCommand)m).Command = match.Groups["command"].Value;
            }
            else
            {
                match = replyRegex.Match(message);
                if (match.Success)
                {
                    m = new RfcReply();
                    ((RfcReply)m).Code = (RfcReplyCode)int.Parse(match.Groups["reply"].Value);
                }
            }
            if (match.Success)
            {
                m.IrcSource = match.Groups["prefix"].Value;
                m.Parameters = new string[match.Groups["parameter"].Captures.Count];
                for (int i = 0; i < match.Groups["parameter"].Captures.Count; i++)
                    m.Parameters[i] = match.Groups["parameter"].Captures[i].ToString();
                return m;
            }
            else
                return null;
        }
    }
}
