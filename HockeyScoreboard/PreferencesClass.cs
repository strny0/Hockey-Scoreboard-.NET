using System;

namespace HockeyScoreboard
{
    public class PreferencesClass
    {
        private TimeSpan defaultBreakTime = TimeSpan.FromMinutes(1);
        private TimeSpan defaultTimeoutTime = TimeSpan.FromMinutes(1);

        public TimeSpan DefaultBreakTime { get => defaultBreakTime; set => defaultBreakTime = value; }
        public TimeSpan DefaultTimeoutTime { get => defaultTimeoutTime; set => defaultTimeoutTime = value; }
        public string DefaultTeamDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Hockey Scoreboard\\Teams\\";

    }
}
