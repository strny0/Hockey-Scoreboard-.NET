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

        public string TeamName { get; set; }
        public string TeamLogoPath { get; set; }
        public List<PlayerTeamListType> PlayerList { get; set; }

    }
}
