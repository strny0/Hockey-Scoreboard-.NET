using System;

namespace HockeyScoreboard
{
    public class PreferencesClass
    {
        private TimeSpan defaultBreakTime;
        private TimeSpan defaultTimeoutTime;

        public TimeSpan DefaultBreakTime { get => defaultBreakTime; set => defaultBreakTime = value; }
        public TimeSpan DefaultTimeoutTime { get => defaultTimeoutTime; set => defaultTimeoutTime = value; }
    }
}
