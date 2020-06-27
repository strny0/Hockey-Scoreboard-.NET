using System;

namespace HockeyScoreboard
{
    public class PreferencesClass
    {
        private TimeSpan defaultBreakTime = TimeSpan.FromMinutes(1);
        private TimeSpan defaultTimeoutTime = TimeSpan.FromMinutes(1);
        private string defaultTeamDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\Hockey Scoreboard\\Teams\\";

        public TimeSpan DefaultBreakTime { get => defaultBreakTime; set => defaultBreakTime = value; }
        public TimeSpan DefaultTimeoutTime { get => defaultTimeoutTime; set => defaultTimeoutTime = value; }
        public string DefaultTeamDirectory { get => defaultTeamDirectory; set => defaultTeamDirectory = value; }

    }
}
