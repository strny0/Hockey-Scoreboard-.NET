using System;
using System.Diagnostics;

namespace HockeyScoreboard
{
    public class GameClass
    {
        private TimeSpan timeLeft;
        private TimeSpan lastSetTime;
        private TimeSpan lastRegularTime;

        public TimeSpan TimeLeft { get => timeLeft; set => timeLeft = value; }
        public TimeSpan LastSetTime { get => lastSetTime; set => lastSetTime = value; }
        public TimeSpan LastRegularTime { get => lastRegularTime; set => lastRegularTime = value; }
        public Stopwatch StopwatchPeriod { get; set; } = new Stopwatch();
        public CustomTypes.GameState GameState { get; set; }
        public CustomTypes.PeriodState Period { get; set; }
        public int InputMinute { get; set; }
        public int InputSecond { get; set; }
        public string TimespanFormatRegular { get; } = @"mm\:ss";
        public string TimespanFormatMilisecond { get; } = @"ss\.f";
        public TeamSavingClass TeamManagerTeamSavingClassInstance { get; set; }
    }
}