using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace HockeyScoreboard
{
    /// <summary>
    /// This file contains all Events for the Main Window of HockeyScoreboard.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vars.SecondaryWindow.Show(); // launch and load view window
            // Load operations TBD
            DefineDefaultProgramState();
            DefineDefaultPrefs();
            InitializeTimer();
        }

        /// <summary>
        /// TEAM MANAGEMENT RELATED EVENTS
        /// </summary>

        private void TextBoxTeam1Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vars.Team1.Name = TextBoxTeam1Name.Text;
            Vars.SecondaryWindow.TextBlockTeam1Name.Text = Vars.Team1.Name;
        }
        private void TextBoxTeam2Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vars.Team2.Name = TextBoxTeam2Name.Text;
            Vars.SecondaryWindow.TextBlockTeam2Name.Text = Vars.Team2.Name;
        }
        private void UpDownTeam1Score_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team1.Score = (int)UpDownTeam1Score.Value;
            Vars.SecondaryWindow.LabelScoreTeam1Variable.Content = Vars.Team1.Score.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam1Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team1.Shots = (int)UpDownTeam1Shots.Value;
            Vars.SecondaryWindow.LabelShotsTeam1Variable.Content = Vars.Team1.Shots.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam2Score_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Score = (int)UpDownTeam2Score.Value;
            Vars.SecondaryWindow.LabelScoreTeam2Variable.Content = Vars.Team2.Score.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam2Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Shots = (int)UpDownTeam2Shots.Value;
            Vars.SecondaryWindow.LabelShotsTeam2Variable.Content = Vars.Team2.Shots.ToString(CultureInfo.InvariantCulture);
        }
        private void ButtonTeam1SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Vars.Team1.LogoSource = ReturnImageSourcePath();
            ChangeImageFromPath(Vars.Team1.LogoSource, ImageTeam1Logo);
            ChangeImageFromPath(Vars.Team1.LogoSource, Vars.SecondaryWindow.ImageTeam1LogoView);

        }
        private void ButtonTeam2SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Vars.Team2.LogoSource = ReturnImageSourcePath();
            ChangeImageFromPath(Vars.Team2.LogoSource, ImageTeam2Logo);
            ChangeImageFromPath(Vars.Team2.LogoSource, Vars.SecondaryWindow.ImageTeam2LogoView);
        }
        private void ButtonCancelPenaltyTeam1Player1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyCancel(Vars.Team1, true);
        }
        private void ButtonCancelPenaltyTeam1Player2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyCancel(Vars.Team1, false);
        }
        private void ButtonCancelPenaltyTeam2Player1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyCancel(Vars.Team2, true);
        }
        private void ButtonCancelPenaltyTeam2Player2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyCancel(Vars.Team2, false);
        }

        /// <summary>
        /// GAME MANAGEMENT RELATED EVENTS
        /// </summary>

        private void MainTimer_Tick(object sender, EventArgs e) // TIMER
        {
            if (Vars.Game.StopwatchPeriod.IsRunning) // PERIOD RUNNING
            {
                switch (Vars.Game.GameState)
                {
                    case CustomTypes.GameState.Regular:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Game.StopwatchPeriod.Reset();
                            Vars.Game.TimeLeft = TimeSpan.Zero;
                        }
                        else Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;

                        PenaltyStopIfRanOutOfTime(Vars.Team1, true); PenaltyStopIfRanOutOfTime(Vars.Team1, false); // Stop penalty timer if time ran out
                        PenaltyStopIfRanOutOfTime(Vars.Team2, true); PenaltyStopIfRanOutOfTime(Vars.Team2, false);

                        PenaltyMoveDownASlot(Vars.Team1); PenaltyMoveDownASlot(Vars.Team2); // Move Player2 to slot of Player1 if Player1 slot empty

                        PenaltyTimeProgression(Vars.Team1, Vars.Team2, true); PenaltyTimeProgression(Vars.Team1, Vars.Team2, false);   // Tick Time Down
                        PenaltyTimeProgression(Vars.Team2, Vars.Team1, true); PenaltyTimeProgression(Vars.Team2, Vars.Team1, false);   // Function checks if penalties for each player are running
                        break;
                    case CustomTypes.GameState.Break:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Game.StopwatchPeriod.Reset();
                            Vars.Game.TimeLeft = TimeSpan.Zero;
                        }
                        else Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                    case CustomTypes.GameState.Timeout:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
                            Vars.Game.GameState = CustomTypes.GameState.Regular;
                            SetTime(Vars.Game.LastRegularTime);
                        }
                        else Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                }
            }
            UIUpdateAll(); ; // UI UPDATE
        } // TIMER
        private void ButtonSetTime_Click(object sender, RoutedEventArgs e)
        {
            SetTime(TimeSpan.FromMinutes(Vars.Game.InputMinute) + TimeSpan.FromSeconds(Vars.Game.InputSecond));
        }
        private void UpDownSeconds_ValueChanged(object sender, EventArgs e)
        {
            Vars.Game.InputSecond = (int)UpDownSeconds.Value;
        }
        private void UpDownMinutes_ValueChanged(object sender, EventArgs e)
        {
            Vars.Game.InputMinute = (int)UpDownMinutes.Value;
        }
        private void ButtonPauseTime_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.Game.StopwatchPeriod.IsRunning)
            {
                ButtonPauseTime.Content = "Start Time";
                Vars.Game.StopwatchPeriod.Stop();
            }
            else
            {
                ButtonPauseTime.Content = "Pause Time";
                Vars.Game.StopwatchPeriod.Start();
            }
        }
        private void ButtonRestartTime_Click(object sender, RoutedEventArgs e)
        {
            SetTime(Vars.Game.LastSetTime);
        }
        private void ButtonSetTimePresetA_Click(object sender, RoutedEventArgs e)
        {
            Vars.Team1.Player1.PenaltyTimeLeft = TimeSpan.FromMinutes(1);
            Vars.Team1.Player1.PenaltyTimeSet = TimeSpan.FromMinutes(1);
            //SetTime(TimeSpan.FromMinutes(1)); // temporary
        } // PRESET
        private void ButtonSetTimePresetB_Click(object sender, RoutedEventArgs e)
        {
            SetTime(TimeSpan.FromMinutes(2)); // temporary
        }// PRESET
        private void ButtonSetTimePresetC_Click(object sender, RoutedEventArgs e)
        {
            SetTime(TimeSpan.FromMinutes(5)); // temporary
        }// PRESET
        private void ButtonSetTimePresetD_Click(object sender, RoutedEventArgs e)
        {
            SetTime(TimeSpan.FromMinutes(10)); // temporary
        }// PRESET
        private void ButtonBreakMode_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.Game.GameState == CustomTypes.GameState.Break)
            {
                Vars.Game.GameState = CustomTypes.GameState.Regular;
                SetTime(Vars.Game.LastRegularTime);
            }
            else
            {
                Vars.Game.LastRegularTime = Vars.Game.TimeLeft;
                Vars.Game.GameState = CustomTypes.GameState.Break;
                SetTime(Vars.Prefs.DefaultBreakTime);
            }
        }
        private void ButtonTimeout_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.Game.GameState == CustomTypes.GameState.Timeout)
            {
                Vars.Game.GameState = CustomTypes.GameState.Regular;
                Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
                SetTime(Vars.Game.LastRegularTime);
            }
            else
            {
                Vars.Game.LastRegularTime = Vars.Game.TimeLeft;
                Vars.Game.GameState = CustomTypes.GameState.Timeout;
                Vars.Game.StopwatchPeriod.Stop();
                TimeoutDialog TD = new TimeoutDialog();
                TD.ShowDialog();
            }
        }
        private void ButtonResetTimeout_Click(object sender, RoutedEventArgs e)
        {
            Vars.Team1.HasTimeout = true; Vars.Team2.HasTimeout = true;
        }
        private void ButtonPeriodPlus_Click(object sender, RoutedEventArgs e)
        {
            Vars.Game.Period++;
            ChangePeriod(Vars.Game.Period);
        }
        private void ButtonPeriodMinus_Click(object sender, RoutedEventArgs e)
        {
            Vars.Game.Period--;
            ChangePeriod(Vars.Game.Period);
        }
        private void ButtonNewView_Click(object sender, RoutedEventArgs e)
        {
            Vars.SecondaryWindow.Close();
            Vars.SecondaryWindow = new SecondaryWindow();
            Vars.SecondaryWindow.Show();

        }
        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Vars.Game.Period = CustomTypes.PeriodState.First; Vars.Game.GameState = CustomTypes.GameState.Regular;
            Vars.Team1.HasTimeout = true; Vars.Team2.HasTimeout = true;
            Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
            SetTime(TimeSpan.FromMinutes(7));
            PenaltyCancel(Vars.Team1, false); PenaltyCancel(Vars.Team1, true);
            PenaltyCancel(Vars.Team2, false); PenaltyCancel(Vars.Team2, true);
            Vars.Team1.Score = 0; Vars.Team2.Score = 0;
            UpDownTeam1Score.Value = Vars.Team1.Score; UpDownTeam2Score.Value = Vars.Team2.Score;
            Vars.Team1.Shots = 0; Vars.Team2.Shots = 0;
            UpDownTeam1Shots.Value = Vars.Team1.Shots; UpDownTeam2Shots.Value = Vars.Team2.Shots;
            UIUpdateAll();
        }

        /// <summary>
        /// PENALTY TAB RELATED EVENTS
        /// </summary>

        private void ButtonSetSpecificPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam1.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam1.Value), false); // It is a slot based system, there are 2 slots for each team,
        }
        private void ButtonSet1minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(1), false);
        }
        private void ButtonSet2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(2), false);
        }
        private void ButtonSet5minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(5), false);
        }
        private void ButtonSet10minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(10), false);
        }
        private void ButtonSet2plus2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(4), true);
        }
        private void ButtonSetSpecificPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, (TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam2.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam2.Value)), false);
        }
        private void ButtonSet1minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(1), false);
        }
        private void ButtonSet2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(2), false);
        }
        private void ButtonSet5minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(5), false);
        }
        private void ButtonSet10minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(10), false);
        }
        private void ButtonSet2plus2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(4), true);
        }
        private void ComboBoxTeam1_DropDownOpened(object sender, EventArgs e)
        {
            UIUpdateComboBoxSelection(ComboBoxTeam1);
        }
        private void ComboBoxTeam2_DropDownOpened(object sender, EventArgs e)
        {
            UIUpdateComboBoxSelection(ComboBoxTeam2);
        }
        private void ComboBoxTeam1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TeamEditorLoadTeamFromComboBox(ListBoxTeam1Players, ComboBoxTeam1, Vars.Team1);
        }
        private void ComboBoxTeam2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TeamEditorLoadTeamFromComboBox(ListBoxTeam2Players, ComboBoxTeam2, Vars.Team2);

        }
        private void ListBoxTeam1Players_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxTeam1Players.SelectedIndex = -1;
        }
        private void ListBoxTeam2Players_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxTeam2Players.SelectedIndex = -1;
        }

        /// <summary>
        /// TEAM MANAGER RELATED EVENTS
        /// </summary>

        private void ButtonTeamManagerAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorAddPlayer(TextBoxPlayerNameManager.Text, UpDownTeamManager.Value.ToString());
        }
        private void ButtonTeamManagerRemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorRemovePlayer(ListBoxTeamManager);
        }
        private void ButtonTeamManagerClearList_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorClearPlayerList();
        }
        private void ListBoxTeamManager_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TeamEditorRemovePlayer(ListBoxTeamManager);
        }
        private void ButtonTeamManagerSaveTeam_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorSave(TextBoxTeamNameManager.Text);
        }
        private void ButtonTeamManagerLoadTeam_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorLoad();
        }
        private void ButtonTeamManagerSelectImage_Click(object sender, RoutedEventArgs e)
        {
            Vars.Game.TempEditorTeam.TeamLogoPath = ReturnImageSourcePath();
            ChangeImageFromPath(Vars.Game.TempEditorTeam.TeamLogoPath, ImageTeamManagerLogo);
        }




        // SOUND TAB
        // PREFERENCES TAB


    }
}

