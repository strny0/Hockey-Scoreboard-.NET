using System;

namespace HockeyScoreboard
{
    public class PlayerClass
    {
        private TimeSpan penaltyTimeLeft;
        private TimeSpan penaltyTimeSet;
        private TimeSpan penaltyOffset;
        public string Number { get; set; }
        public bool PenaltyRunning { get; set; }
        public TimeSpan PenaltyTimeLeft { get => penaltyTimeLeft; set => penaltyTimeLeft = value; }
        public TimeSpan PenaltyTimeSet { get => penaltyTimeSet; set => penaltyTimeSet = value; }
        public TimeSpan PenaltyOffset { get => penaltyOffset; set => penaltyOffset = value; }
        public bool PeriodIsMinor { get; set; }
        public bool PeriodIsDoubleMinor { get; set; }
        public int OtherTeamScoreAtPenaltyStart { get; set; }
    }
}