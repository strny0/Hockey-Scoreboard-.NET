using System.Collections.Generic;

namespace HockeyScoreboard
{
	public class TeamClass
	{
		public int Index { get; set; }
		public bool HasTimeout { get; set; }
		public bool TimeoutRunning { get; set; }
		public string Name { get; set; }
		public int Score { get; set; }
		public int Shots { get; set; }
		public string LogoSource { get; set; }

		public List<TeamSavingClass.PlayerTeamListType> SelectedTeamList { get; set; }
        public PlayerClass Player1 { get; set; }
		public PlayerClass Player2 { get; set; }
}
}
