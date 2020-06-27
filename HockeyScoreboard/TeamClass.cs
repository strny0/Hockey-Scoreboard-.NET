using System.Collections.Generic;

namespace HockeyScoreboard
{
	public class TeamClass
	{
		public int Index;
		public bool HasTimeout;
		public bool TimeoutRunning;
		public string Name;
		public int Score;
		public int Shots;
		public System.Windows.Media.ImageSource LogoSource;
		public CustomTypes.PlayerType Player1;
		public CustomTypes.PlayerType Player2;
		public List<TeamSavingClass.PlayerTeamListType> SelectedTeamList;
	}
}
