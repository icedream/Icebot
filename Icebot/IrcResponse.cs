using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Icebot
{
    /// <summary>
    /// Represents a response from an IRC server.
    /// </summary>
    public class IrcResponse
    {
        public string Source
        { get; set; }

        public string SourceNick
        { get { return Source.Split('!', '@')[0]; } }

        public string SourceIdent
        { get { return Source.Split('!', '@')[1]; } }

        public string SourceHost
        { get { return Source.Split('!', '@')[2]; } }

        public string Command
        { get; set; }

        public bool IsNumericReply
        { get { return NumericReply != IrcReplyCode.NonNumeric; } }

        public IrcReplyCode NumericReply
        { get { ushort code = 0; ushort.TryParse(Command, out code); return (IrcReplyCode)code; } }

        public string[] Parameters
        { get; set; }

        public static IrcResponse FromRawLine(string line)
        {
            Match m = Regex.Match(line, @"^[:]*(?<source>[^\s]+) (?<command>[^\s]+) (?<parameters>.+)$");
            if (m == null)
                throw new System.Net.ProtocolViolationException(string.Format("IRC response line not protocol-conform: {0}", line));

            var resp = new IrcResponse();
            resp.Source = m.Groups["source"].Value;
            resp.Command = m.Groups["command"].Value;
            
            // Parse arguments
            var paramsplit = new Queue<string>(m.Groups["parameters"].Value.Split(' '));
            var paramlist = new List<string>();
            // All arguments without leading ":" are to be parsed seperately
            while (paramsplit.Count > 0 && !paramsplit.Peek().StartsWith(":"))
                paramlist.Add(paramsplit.Dequeue());
            // Last argument
            if(paramsplit.Count > 0 )
                paramlist.Add(string.Join(" ", paramsplit.ToArray<string>()).Substring(1));
            resp.Parameters = paramlist.ToArray<string>();
            paramsplit.Clear();
            paramsplit = null;

            return resp;
        }
    }
}
