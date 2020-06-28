using System.Collections.Generic;


namespace HockeyScoreboard
{
    public class TeamSavingClass
    {
        public struct PlayerTeamListType
        {
            public string Number;
            public string Name;
        }
        private string teamName;
        private string teamLogoPath;

        public string TeamName { get => teamName; set => teamName = value; }
        public string TeamLogoPath { get => teamLogoPath; set => teamLogoPath = value; }
        public List<PlayerTeamListType> PlayerList { get; set; }
        
    }
}
