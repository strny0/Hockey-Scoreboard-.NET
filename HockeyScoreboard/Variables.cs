using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HockeyScoreboard
{
    public class Vars
    {
        public static TeamClass Team1 = new TeamClass();
        public static TeamClass Team2 = new TeamClass();
        public static GameClass Game = new GameClass();
        public static PreferencesClass Prefs = new PreferencesClass();
        public static SecondaryWindow Window = new SecondaryWindow();
    }

    public class CustomTypes
    {
        public struct PlayerTeamListType
        {
            public string Name;
            public string Number;
        }
        public struct PlayerType
        {
            public string Number;
            public bool PenaltyRunning;
            public TimeSpan PenaltyTimeLeft;
            public TimeSpan PenaltyTimeSet;
            public TimeSpan PenaltyOffset;
            public bool PeriodIs2plus2;
            public int ScoreAtPeriodStart;
        }
        public enum PeriodStates
        {
            First = 1,
            Second = 2,
            Third = 3,
            Extension = 4,
            SN = 5,
        }
        public enum GameStates
        {
            Regular = 0,
            Break = 1,
            Timeout = 2
        }
    }
    public class GameClass
    {
        public TimeSpan TimeLeft;
        public TimeSpan LastSetTime;
        public TimeSpan LastRegularTime;
        public bool DisplayMiliseconds;
        public Stopwatch StopwatchPeriod = new Stopwatch();
        public CustomTypes.GameStates GameState;
        public int InputMinute;
        public int InputSecond;
        public string TextBoxRenderFormat = "00.##";
        public CustomTypes.PeriodStates Period;
        public string TimeFormatRegular = @"mm\:ss";
        public string TimeFormatMilisecond = @"ss\.f";
    }
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
        public List<CustomTypes.PlayerTeamListType> PlayerList;
    }
    public class PreferencesClass
    {
        public TimeSpan DefaultBreakTime;
        public TimeSpan DefaultTimeoutTime;
    }
}






// DISCONTINUED BUT KEPT AS BACKUP JUST IN CASE //
//private void DispatchTimerElement_Tick(object sender, EventArgs e) 
//{

//    if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
//    {
//        ButtonPauseTime.Content = "Start All Clocks";
//        ButtonPausePeriodClock.Content = "Start Period Clock";
//        ButtonPausePenaltyClock.Content = "Start Penalty Clock";
//        Vars.Game.StopwatchPeriod.Stop();
//        Vars.Game.StopwatchPeriod.Reset();
//        Vars.Game.TimeLeft = TimeSpan.Zero;
//        UpdateTimeLabel();
//    }

//    if (Vars.Game.StopwatchPeriod.IsRunning) // PERIOD RUNNING
//    {
//        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
//        ButtonPauseTime.Content = "Pause All Clocks";
//        ButtonPausePeriodClock.Content = "Pause Period Clock";
//        UpdateTimeLabel();
//    }
//    else
//    {
//        ButtonPausePeriodClock.Content = "Start Period Clock";
//    }
//    if (Vars.Game.PenaltyRunning == true)  //////// PENALTIES ////////
//    {
//        ButtonPauseTime.Content = "Pause All Clocks";
//        ButtonPausePenaltyClock.Content = "Pause Penalty Clock";

//        StopPenaltyIfRanOut(Vars.Team1, true); StopPenaltyIfRanOut(Vars.Team1, false); // Stop penalty timer if time ran out
//        StopPenaltyIfRanOut(Vars.Team2, true); StopPenaltyIfRanOut(Vars.Team2, false);

//        MoveDownASlot(Vars.Team1); MoveDownASlot(Vars.Team2); // Move Player2 to slot of Player1 if Player1 slot empty

//        TickTimeDownPenalty(Vars.Team1, true); TickTimeDownPenalty(Vars.Team1, false);   // Tick Time Down
//        TickTimeDownPenalty(Vars.Team2, true); TickTimeDownPenalty(Vars.Team2, false);   // Function checks if penalties for each player are running
//        UpdatePenaltyUIMain(); UpdatePenaltyUISecondary(); // UpdateUI
//    }
//    else
//    {
//        ButtonPausePenaltyClock.Content = "Start Penalty Clock";
//        TickTimeDownPenalty(Vars.Team1, true); TickTimeDownPenalty(Vars.Team1, false);   // Tick Time Down
//        TickTimeDownPenalty(Vars.Team2, true); TickTimeDownPenalty(Vars.Team2, false);   // Function checks if penalties for each player are running
//    }


//    if (Vars.Game.StopwatchPeriod.IsRunning == false && Vars.Game.PenaltyRunning == false)
//    {
//        ButtonPauseTime.Content = "Start All Clocks";
//        ButtonPausePeriodClock.Content = "Start Period Clock";
//        ButtonPausePenaltyClock.Content = "Start Penalty Clock";
//    }
//}