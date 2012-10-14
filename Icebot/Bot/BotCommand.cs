using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public class BotCommand
    {
        public string Command;
        public string[] Arguments;
        public ReceivedIrcMessage Message;

        public BotCommand(ReceivedIrcMessage msg)
        {
            Message = msg;

            // Different parsing algorithms for public and private messages
            List<string> arguments = new List<string>();
            string cmdname = "";

            var spl = new Queue<string>(Message.Text.Split(' '));
            bool merge = false;
            string mergedargument = "";
            char mergechar = '\0';

            // Public => [in #channel] !command, Private => [private] command
            if (msg.IsPublic)
            {
                if (!Message.Text.StartsWith(msg.Response.Bot.Prefix))
                    throw new InvalidOperationException("Tried to make a bot command from a received message which does not look like one");

                cmdname = spl.Dequeue().Substring(msg.Response.Bot.Prefix.Length);
            }

            // TODO: Let the bot allow unescaped quotes in a quoted text (as it is now) or do it more exact with quoted characters?
            while (spl.Count > 0)
            {
                string word = spl.Dequeue();
                switch (merge)
                {
                    case true:
                        {
                            mergedargument += " " + word;
                            if (word.EndsWith(mergechar))
                            {
                                mergedargument = mergedargument.Substring(0, mergedargument.Length - 1);
                                arguments.Add(mergedargument);
                                mergedargument = "";
                                merge = !merge;
                            }
                        } break;

                    case false:
                        {
                            if (word.StartsWith("'") || word.StartsWith("\""))
                            {
                                mergechar = word[0];
                                mergedargument = word.Substring(1);
                                merge = !merge;
                            }
                        } break;
                }
            }

            if (merge)
            {
                Message.Response.Bot._log
            }
        }
    }
}
