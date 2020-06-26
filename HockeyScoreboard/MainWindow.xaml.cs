using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using System.Drawing.Imaging;
using System.Globalization;

namespace HockeyScoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // EVENTS
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vars.Window = new SecondaryWindow(); // Define window
            Vars.Window.Show(); // launch and load view window
            // Load operations TBD
            DefineDefaultVars();
            DefineDefaultPrefs();
            InitializeTimer();
        }
        private void MainTimer_Tick(object sender, EventArgs e)
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
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;

                        StopPenaltyIfRanOut(Vars.Team1, true); StopPenaltyIfRanOut(Vars.Team1, false); // Stop penalty timer if time ran out
                        StopPenaltyIfRanOut(Vars.Team2, true); StopPenaltyIfRanOut(Vars.Team2, false);

                        MoveDownASlot(Vars.Team1); MoveDownASlot(Vars.Team2); // Move Player2 to slot of Player1 if Player1 slot empty

                        TickTimeDownPenalty(Vars.Team1, Vars.Team2, true); TickTimeDownPenalty(Vars.Team1, Vars.Team2, false);   // Tick Time Down
                        TickTimeDownPenalty(Vars.Team2, Vars.Team1, true); TickTimeDownPenalty(Vars.Team2, Vars.Team1, false);   // Function checks if penalties for each player are running
                        break;
                    case CustomTypes.GameState.Break:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Game.StopwatchPeriod.Reset();
                            Vars.Game.TimeLeft = TimeSpan.Zero;
                        }
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                    case CustomTypes.GameState.Timeout:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
                            Vars.Game.GameState = CustomTypes.GameState.Regular;
                            SetTime(Vars.Game.LastRegularTime);
                        }
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                }
            }
            UpdateAllUI(); ; // UI UPDATE
        } // TIMER
        private void TextBoxTeam1Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vars.Team1.Name = TextBoxTeam1Name.Text;
            Vars.Window.TextBlockTeam1Name.Text = Vars.Team1.Name;
        }
        private void TextBoxTeam2Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vars.Team2.Name = TextBoxTeam2Name.Text;
            Vars.Window.TextBlockTeam2Name.Text = Vars.Team2.Name;
        }
        private void UpDownTeam1Score_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team1.Score = (int)UpDownTeam1Score.Value;
            Vars.Window.LabelScoreTeam1Variable.Content = Vars.Team1.Score.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam1Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team1.Shots = (int)UpDownTeam1Shots.Value;
            Vars.Window.LabelShotsTeam1Variable.Content = Vars.Team1.Shots.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam2Score_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Score = (int)UpDownTeam2Score.Value;
            Vars.Window.LabelScoreTeam2Variable.Content = Vars.Team2.Score.ToString(CultureInfo.InvariantCulture);
        }
        private void UpDownTeam2Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Shots = (int)UpDownTeam2Shots.Value;
            Vars.Window.LabelShotsTeam2Variable.Content = Vars.Team2.Shots.ToString(CultureInfo.InvariantCulture);
        }
        private void ButtonTeam1SelectImage_Click(object sender, RoutedEventArgs e)
        {
            ChangeImage(Vars.Team1, ImageTeam1Logo, Vars.Window.ImageTeam1LogoView);
        }
        private void ButtonTeam2SelectImage_Click(object sender, RoutedEventArgs e)
        {
            ChangeImage(Vars.Team2, ImageTeam2Logo, Vars.Window.ImageTeam2LogoView);
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
            Vars.Window.Close();
            Vars.Window = new SecondaryWindow();
            Vars.Window.Show();

        }
        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Vars.Game.Period = CustomTypes.PeriodState.First; Vars.Game.GameState = CustomTypes.GameState.Regular;
            Vars.Team1.HasTimeout = true; Vars.Team2.HasTimeout = true;
            Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
            SetTime(TimeSpan.FromMinutes(7));
            CancelPenalty(Vars.Team1, false); CancelPenalty(Vars.Team1, true);
            CancelPenalty(Vars.Team2, false); CancelPenalty(Vars.Team2, true);
            Vars.Team1.Score = 0; Vars.Team2.Score = 0;
            UpDownTeam1Score.Value = Vars.Team1.Score; UpDownTeam2Score.Value = Vars.Team2.Score;
            Vars.Team1.Shots = 0; Vars.Team2.Shots = 0;
            UpDownTeam1Shots.Value = Vars.Team1.Shots; UpDownTeam2Shots.Value = Vars.Team2.Shots;
            UpdateAllUI();
        }
        private void ButtonCancelPenaltyTeam1Player1_Click(object sender, RoutedEventArgs e)
        {
            CancelPenalty(Vars.Team1, true);
        }
        private void ButtonCancelPenaltyTeam1Player2_Click(object sender, RoutedEventArgs e)
        {
            CancelPenalty(Vars.Team1, false);
        }
        private void ButtonCancelPenaltyTeam2Player1_Click(object sender, RoutedEventArgs e)
        {
            CancelPenalty(Vars.Team2, true);
        }
        private void ButtonCancelPenaltyTeam2Player2_Click(object sender, RoutedEventArgs e)
        {
            CancelPenalty(Vars.Team2, false);
        }
        // PENALTIES TAB
        private void ButtonSetSpecificPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam1.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam1.Value), false); // It is a slot based system, there are 2 slots for each team,
        }
        private void ButtonSet1minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(1), false);
        }
        private void ButtonSet2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(2), false);
        }
        private void ButtonSet5minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(5), false);
        }
        private void ButtonSet10minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(10), false);
        }
        private void ButtonSet2plus2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, Vars.Team2, ListBoxTeam1Players, TimeSpan.FromMinutes(4), true);
        }
        private void ButtonSetSpecificPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, (TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam2.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam2.Value)), false);
        }
        private void ButtonSet1minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(1), false);
        }
        private void ButtonSet2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(2), false);
        }
        private void ButtonSet5minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(5), false);
        }
        private void ButtonSet10minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(10), false);
        }
        private void ButtonSet2plus2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, Vars.Team1, ListBoxTeam2Players, TimeSpan.FromMinutes(4), true);
        }
        private void ButtonTeamManagerSelectImage_Click(object sender, RoutedEventArgs e)
        {

        }
        // TEAMS TAB
        // SOUND TAB
        // PREFERENCES TAB


    }
}

