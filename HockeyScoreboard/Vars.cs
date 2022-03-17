namespace HockeyScoreboard
{
    public static class Vars
    {
        public static TeamClass Team1 { get; set; } = new TeamClass();
        public static TeamClass Team2 { get; set; } = new TeamClass();
        public static GameClass Game { get; set; } = new GameClass();
        public static SecondaryWindow SecondaryWindow { get; set; } = new SecondaryWindow();
        public static System.Windows.Media.MediaPlayer MPlayer { get; set; } = new System.Windows.Media.MediaPlayer();
    }
}