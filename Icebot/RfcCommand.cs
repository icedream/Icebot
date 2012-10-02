using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot
{
    /// <summary>
    /// Represents an IRC command
    /// </summary>
    public class RfcCommand : RfcMessage
    {
		public RfcCommand()
		{
		}
		public RfcCommand(string command, params string[] parameters)
		{
			this.Command = command;
			this.Parameters = parameters;
		}

		public string Command
		{
			get; set;
		}
		
		/// <summary>
		/// Wandelt den IRC Befehl in einen Befehl nach RFC zum Versenden an einen IRC Server
		/// </summary>
		/// <returns>String der an den Server gesendet werden kann</returns>
		public override string ToString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			if (IrcSource!=null) 
			{
				s.Append(":");
				s.Append(IrcSource);
				s.Append(" ");
			}
			s.Append(Command);
			if (Parameters!=null && Parameters.Length>1)
			{
				s.Append(" ");
				s.Append(string.Join(" ",Parameters,0,Parameters.Length-1));
			}
			if (Parameters!=null)
			{
				if (Colon) s.Append(" :");
				else s.Append(" ");
				s.Append(Parameters[Parameters.Length-1]);
			}
			s.Append("\r\n");
			return s.ToString();
		}

	}
    }
}
