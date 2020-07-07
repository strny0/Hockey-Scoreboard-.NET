using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoreboardLibrary
{
    public static class Utility
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : $"{value.Substring(0, maxChars)}...";
        }

    }
}
