using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    public class IrcSource
    {
        public string Hostmask { get; set; }

        public string Nick { get { return Hostmask.Split('@', '!')[0]; } }
        public string Ident { get { return Hostmask.Split('@', '!')[1]; } }
        public string Host { get { return Hostmask.Split('@', '!')[2]; } }
    }
}
