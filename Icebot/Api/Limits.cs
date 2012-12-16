using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Icebot.Api
{
    public static class Limits
    {
        public const int MaximumShortDescriptionLength = 384;
        public const bool LongMustBeLongerThanShortDescription = true;
        public static readonly Regex AlphanumericRegex = new Regex("^[a-zA-Z0-9]*$");
        public static readonly Regex AlphabeticRegex = new Regex("^[a-zA-Z]*$");
        public static readonly Regex UppercaseRegex = new Regex("^[A-Z]*$");
        public static readonly Regex LowercaseRegex = new Regex("^[a-z]*$");
        public static readonly Regex NumericRegex = new Regex("^[0-9]*$");
        public static readonly Regex HexadecimalRegex = new Regex("^[0-9a-fA-F]*$");

        public static bool IsValidCommandName(string name)
        {
            return name.Length > 0 && AlphanumericRegex.IsMatch(name);
        }
    }
}
