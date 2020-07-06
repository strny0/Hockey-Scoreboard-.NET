using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using HockeyScoreboard.Properties;

namespace HockeyScoreboard
{
    /// <summary>
    /// This file contains all Events for the Main Window of HockeyScoreboard.
    /// </summary>
    public partial class MainWindow : Window
    {

        #region InitializationEvents
        public MainWindow()
        {
            InitializeComponent();
            UIUpdateAllMainControls();
            UIUpdateAllRadioButtons();
            UIUpdateTimePresetButtonsPenalty("Major"); UIUpdateTimePresetButtonsPeriod();
            Vars.SecondaryWindow.Show(); // launch and load view window
            DefineDefaultProgramState();
            InitializeTimer();
        }
        #endregion

        #region TeamManagementEvents

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
        
        #endregion

        #region GameManagementEvents

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
            UIUpdateAllMainControls(); ; // UI UPDATE
        }
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
        private void ButtonSetTimePresetA_Click(object sender, RoutedEventArgs e) // PRESET [X]
        {
            SetTime(Settings.Default.PeriodPresetA);
        }
        private void ButtonSetTimePresetB_Click(object sender, RoutedEventArgs e) // PRESET [X]
        {
            SetTime(Settings.Default.PeriodPresetB);
        }
        private void ButtonSetTimePresetC_Click(object sender, RoutedEventArgs e) // PRESET [X]
        {
            SetTime(Settings.Default.PeriodPresetC);
        }
        private void ButtonSetTimePresetD_Click(object sender, RoutedEventArgs e) // PRESET [X]
        {
            SetTime(Settings.Default.PeriodPresetD);
        }
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
                SetTime(Settings.Default.BreakDuration);
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
            UIUpdateSecondaryWindowColorScheme();
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
            UIUpdateAllMainControls();
        }
        
        #endregion

        #region PenaltyTabEvents

        private void ButtonSetSpecificPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, (TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam1.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam1.Value)), false, false);
        }
        private void ButtonSetPenaltyTeam1A_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, Settings.Default.PenaltyTimePresetA, false, false);
        }
        private void ButtonSetPenaltyTeam1B_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, Settings.Default.PenaltyTimePresetB, false, false);
        }
        private void ButtonSetPenaltyTeam1C_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, Settings.Default.PenaltyTimePresetC, false, false);
        }
        private void ButtonSetPenaltyTeam1D_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, Settings.Default.PenaltyTimePresetD, false, false);
        }
        private void ButtonSetMinorPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(2), false, true);
        }
        private void ButtonSetDoubleMinorPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(4), true, false);
        }
        private void ButtonSetSpecificPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, (TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam2.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam2.Value)), false, false);
        }
        private void ButtonSetPenaltyTeam2A_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, Settings.Default.PenaltyTimePresetA, false, false);
        }
        private void ButtonSetPenaltyTeam2B_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, Settings.Default.PenaltyTimePresetB, false, false);
        }
        private void ButtonSetPenaltyTeam2C_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, Settings.Default.PenaltyTimePresetC, false, false);
        }
        private void ButtonSetPenaltyTeam2D_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, Settings.Default.PenaltyTimePresetD, false, false);
        }
        private void ButtonSetMinorPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(2), false, true);
        }
        private void ButtonSetDoubleMinorPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            PenaltyAssignToRightPlayer(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(4), true, false);
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
            if (ComboBoxTeam1.SelectedIndex != -1)
            {
                TeamEditorLoadTeamFromComboBox(ListBoxTeam1Players, ComboBoxTeam1, Vars.Team1);
                ComboBoxTeam1.SelectedIndex = -1;
            }
            else return;
        }
        private void ComboBoxTeam2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTeam2.SelectedIndex != -1)
            {
                TeamEditorLoadTeamFromComboBox(ListBoxTeam2Players, ComboBoxTeam2, Vars.Team2);
                ComboBoxTeam2.SelectedIndex = -1;
            }
        }
        private void ListBoxTeam1Players_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxTeam1Players.SelectedIndex = -1;
        }
        private void ListBoxTeam2Players_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxTeam2Players.SelectedIndex = -1;
        }
        
        #endregion

        #region TeamManagerEvents

        private void ButtonTeamManagerAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            TeamEditorAddPlayer(TextBoxPlayerNameManager.Text, UpDownTeamManager.Value.ToString(CultureInfo.InvariantCulture));
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
            Vars.Game.TeamManagerTeamSavingClassInstance.TeamLogoPath = ReturnImageSourcePath();
            ChangeImageFromPath(Vars.Game.TeamManagerTeamSavingClassInstance.TeamLogoPath, ImageTeamManagerLogo);
        }

        #endregion

        #region PreferencesEvents
        private void ButtonPreferencesSetTime_Click(object sender, RoutedEventArgs e)
        {
            PreferencesChangeTimeSetting(TimeSpan.FromMinutes((int)UpDownPreferencesMinutes.Value) + TimeSpan.FromSeconds((int)UpDownPreferencesSeconds.Value));
            UIUpdateAllRadioButtons();
            UIUpdateTimePresetButtonsPenalty("Major");
            UIUpdateTimePresetButtonsPeriod();
        }

        #region Colors
        private void BorderColorBGMain_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorBackgroundMain = ChangeColorSetting(Settings.Default.ColorBackgroundMain);
            UIUpdateColorButton(Settings.Default.ColorBackgroundMain, BorderColorBGMain);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorBGSecondary_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorBackgroundSecondary = ChangeColorSetting(Settings.Default.ColorBackgroundSecondary);
            UIUpdateColorButton(Settings.Default.ColorBackgroundSecondary, BorderColorBGSecondary);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorBorder_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorBorderBrush = ChangeColorSetting(Settings.Default.ColorBorderBrush);
            UIUpdateColorButton(Settings.Default.ColorBorderBrush, BorderColorBorder);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorIndicatorFree_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorPenaltyIndicatorFree = ChangeColorSetting(Settings.Default.ColorPenaltyIndicatorFree);
            UIUpdateColorButton(Settings.Default.ColorPenaltyIndicatorFree, BorderColorIndicatorFree);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorIndicatorOccupied_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorPenaltyIndicatorOccupied = ChangeColorSetting(Settings.Default.ColorPenaltyIndicatorOccupied);
            UIUpdateColorButton(Settings.Default.ColorPenaltyIndicatorOccupied, BorderColorIndicatorOccupied);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorNormalText_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorTextMain = ChangeColorSetting(Settings.Default.ColorTextMain);
            UIUpdateColorButton(Settings.Default.ColorTextMain, BorderColorNormalText);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorPeriodText_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorTextPeriod = ChangeColorSetting(Settings.Default.ColorTextPeriod);
            UIUpdateColorButton(Settings.Default.ColorTextPeriod, BorderColorPeriodText);
            UIUpdateSecondaryWindowColorScheme();
        }
        private void BorderColorTextTime_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorTextTime = ChangeColorSetting(Settings.Default.ColorTextTime);
            UIUpdateColorButton(Settings.Default.ColorTextTime, BorderColorTextTime);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void BorderColorTextValues_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.Default.ColorTextValues = ChangeColorSetting(Settings.Default.ColorTextValues);
            UIUpdateColorButton(Settings.Default.ColorTextValues, BorderColorTextValues);
            UIUpdateSecondaryWindowColorScheme();
        }

        private void ButtonPreferencesRestoreDefaultColors_Click(object sender, RoutedEventArgs e)
        {
            UIRestoreDefaultColors();
            UIUpdateSecondaryWindowColorScheme();
        }

        #endregion
        private void ButtonPreferencesSave_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            UIUpdateSecondaryWindowColorScheme();
            UIUpdateAllRadioButtons();

            MessageBox.Show("Successfully saved. Preferences will be loaded as they were saved next time the program runs.");
        }


        #endregion

        private void ButtonPreferencesChangeFontNumbers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonPreferencesChangeFontText_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundPlayBuzzer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundPlayHorn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundPlayPeriod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundPlayBreak_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundPlayTimeout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundChangeHorn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundChangeBuzzer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundChangePeriod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundChangeBreak_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSoundChangeTimeout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoChange1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoChange2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoChange3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoChange4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoPlayPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoRestart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoLoad1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoLoad2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoLoad3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonVideoLoad4_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

