using System;

namespace HockeyScoreboard
{
    public partial class CustomTypes
    {
        public struct PlayerType
        {
            private string number;
            private bool penaltyRunning;
            private TimeSpan penaltyTimeLeft;
            private TimeSpan penaltyTimeSet;
            private TimeSpan penaltyOffset;
            private bool periodIs2plus2;
            private int scoreAtPeriodStart;

            public string Number { get => number; set => number = value; }
            public bool PenaltyRunning { get => penaltyRunning; set => penaltyRunning = value; }
            public TimeSpan PenaltyTimeLeft { get => penaltyTimeLeft; set => penaltyTimeLeft = value; }
            public TimeSpan PenaltyTimeSet { get => penaltyTimeSet; set => penaltyTimeSet = value; }
            public TimeSpan PenaltyOffset { get => penaltyOffset; set => penaltyOffset = value; }
            public bool PeriodIs2plus2 { get => periodIs2plus2; set => periodIs2plus2 = value; }
            public int ScoreAtPeriodStart { get => scoreAtPeriodStart; set => scoreAtPeriodStart = value; }

        }
    }
}
