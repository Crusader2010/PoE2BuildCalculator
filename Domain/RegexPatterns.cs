using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain
{
    public static partial class RegexPatterns
    {
        [GeneratedRegex(@"^\s*Item\s*class\s*:\s*(.+?)\s*$", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex ItemClassPattern();

        [GeneratedRegex(@"^\s*Armour\s*:\s*(.+?)\s*$", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex ArmourAmountPattern();
    }
}
