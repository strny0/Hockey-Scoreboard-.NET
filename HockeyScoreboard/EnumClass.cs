using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HockeyScoreboard
{
    public static class EnumClass
    {
        public enum PeriodState
        {
            First = 1,
            Second = 2,
            Third = 3,
            Extension = 4,
            SN = 5,
        }
        public enum GameState
        {
            Regular = 0,
            Break = 1,
            Timeout = 2
        }
    }
}
