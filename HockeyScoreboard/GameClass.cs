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
        public bool DisplayMiliseconds { get; set; }
        public Stopwatch StopwatchPeriod { get; set; } = new Stopwatch();
        public CustomTypes.GameState GameState { get; set; }
        public int InputMinute { get; set; }
        public int InputSecond { get; set; }
        public string TextBoxRenderFormat { get; set; } = "00.##";
        public CustomTypes.PeriodState Period { get; set; }

        public string TimeFormatRegular { get; } = @"mm\:ss";

        public string TimeFormatMilisecond { get; } = @"ss\.f";
    }
}
