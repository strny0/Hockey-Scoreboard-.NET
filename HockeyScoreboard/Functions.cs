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
using System.Globalization;

namespace HockeyScoreboard
{
    public partial class MainWindow : Window // DATA
    {
        // CONSTANTS
        private const string Caption = "Error";
        private readonly Brush ColorBrush1 = new SolidColorBrush(Color.FromRgb(241, 205, 70));
        private readonly Brush ColorBrush2 = new SolidColorBrush(Color.FromRgb(105, 108, 133));
        // FUNCTIONS
        private void InitializeTimer()
        {
            DispatcherTimer MainTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                IsEnabled = true,
                Interval = TimeSpan.FromMilliseconds(1)
            };
            MainTimer.Start();
            MainTimer.Tick += MainTimer_Tick;
        }
        private void DefineDefaultVars() // DEFAULT VALUES
        {
            Vars.Team1.Index = 1;
            Vars.Team2.Index = 2;
            Vars.Team1.Name = "Team 1";
            Vars.Team2.Name = "Team 2";
            Vars.Team1.Score = 0;
            Vars.Team2.Score = 0;
            Vars.Team1.Shots = 0;
            Vars.Team2.Shots = 0;
            Vars.Team1.Player1 = new CustomTypes.PlayerType
            {
                PenaltyRunning = false
            };
            Vars.Team1.Player2 = new CustomTypes.PlayerType
            {
                PenaltyRunning = false
            };
            Vars.Team2.Player1 = new CustomTypes.PlayerType
            {
                PenaltyRunning = false
            };
            Vars.Team2.Player2 = new CustomTypes.PlayerType
            {
                PenaltyRunning = false
            };
            Vars.Team1.PlayerList = new List<CustomTypes.PlayerTeamListType>();
            Vars.Team2.PlayerList = new List<CustomTypes.PlayerTeamListType>();
            Vars.Team1.HasTimeout = true;
            Vars.Team2.HasTimeout = true;
            Vars.Game.LastSetTime = TimeSpan.FromMinutes(7);
            Vars.Game.TimeLeft = TimeSpan.FromMinutes(7);
            Vars.Game.InputMinute = 7;
            Vars.Game.InputSecond = 0;
            Vars.Game.Period = CustomTypes.PeriodState.First;
            Vars.Game.GameState = CustomTypes.GameState.Regular;
            ButtonPeriodMinus.IsEnabled = false;

        }
        private void DefineDefaultPrefs() // DEFAULT PREFERENCES
        {
            Vars.Prefs.DefaultBreakTime = TimeSpan.FromMinutes(1);
            Vars.Prefs.DefaultTimeoutTime = new TimeSpan(0, 1, 30);
        }
        private OpenFileDialog DefineImageFileDialog()
        {
            OpenFileDialog LoadImageDialog = new OpenFileDialog
            {
                Title = "Load Image",
                FilterIndex = 6,
                InitialDirectory = Environment.SpecialFolder.MyPictures.ToString(),
            };

            string sep = string.Empty;
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                LoadImageDialog.Filter = $"{LoadImageDialog.Filter}{sep}{codecName} ({c.FilenameExtension})|{c.FilenameExtension}";
                sep = "|";
            }

            LoadImageDialog.Filter = $"{LoadImageDialog.Filter}{sep}All files |*.*";

            return LoadImageDialog;

        }
        private void ChangeImage(TeamClass Team, Image ImageChangedMain, Image ImageChangedSecondary)
        {
            OpenFileDialog ImageDialog = DefineImageFileDialog();
            Nullable<bool> Result = ImageDialog.ShowDialog();

            if (Result == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(ImageDialog.FileName))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
                Team.LogoSource = bitmap;
                ImageChangedMain.Source = Team.LogoSource;
                ImageChangedSecondary.Source = Team.LogoSource;
            }
        }
        private void MatchTimeSpanOffsets(TeamClass Team, bool Player1)
        {
            if (Team.Player1.PenaltyOffset.Milliseconds != Vars.Game.StopwatchPeriod.Elapsed.Milliseconds && Player1 == true)
            {
                Team.Player1.PenaltyOffset = TimeSpan.FromHours(Team.Player1.PenaltyOffset.Hours) + TimeSpan.FromMinutes(Team.Player1.PenaltyOffset.Minutes) + TimeSpan.FromSeconds(Team.Player1.PenaltyOffset.Seconds) + TimeSpan.FromMilliseconds(Vars.Game.StopwatchPeriod.Elapsed.Milliseconds);
            }
            if (Team.Player2.PenaltyOffset.Milliseconds != Vars.Game.StopwatchPeriod.Elapsed.Milliseconds && Player1 == false)
            {
                Team.Player2.PenaltyOffset = TimeSpan.FromHours(Team.Player2.PenaltyOffset.Hours) + TimeSpan.FromMinutes(Team.Player2.PenaltyOffset.Minutes) + TimeSpan.FromSeconds(Team.Player2.PenaltyOffset.Seconds) + TimeSpan.FromMilliseconds(Vars.Game.StopwatchPeriod.Elapsed.Milliseconds);
            }
        }
        private void TickTimeDownPenalty(TeamClass Team, TeamClass OtherTeam, bool Player1)
        {
            switch (Player1)
            {
                case true:
                    if (Team.Player1.PenaltyRunning)
                    {
                        if (Team.Player1.PeriodIs2plus2 == false)
                        {
                            MatchTimeSpanOffsets(Team, Player1);
                            Team.Player1.PenaltyTimeLeft = Team.Player1.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset);
                        }
                        else
                        {
                            if (OtherTeam.Score > Team.Player1.ScoreAtPeriodStart)
                            {
                                if (Team.Player1.PenaltyTimeLeft < TimeSpan.FromMinutes(2))
                                {
                                    Team.Player1.PeriodIs2plus2 = false;
                                }
                                else
                                {
                                    Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player1.PenaltyTimeSet = TimeSpan.FromMinutes(2);
                                    Team.Player1.PeriodIs2plus2 = false; Team.Player1.PenaltyTimeLeft = TimeSpan.FromMinutes(2);
                                }
                            }
                            else
                            {
                                MatchTimeSpanOffsets(Team, Player1);
                                Team.Player1.PenaltyTimeLeft = Team.Player1.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset);
                            }
                        }
                    }
                    break;
                case false:
                    if (Team.Player2.PenaltyRunning)
                    {
                        if (Team.Player2.PeriodIs2plus2 == false)
                        {
                            MatchTimeSpanOffsets(Team, Player1);
                            Team.Player2.PenaltyTimeLeft = Team.Player2.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset);
                        }
                        else
                        {
                            if (OtherTeam.Score > Team.Player2.ScoreAtPeriodStart)
                            {
                                if (Team.Player2.PenaltyTimeLeft < TimeSpan.FromMinutes(2))
                                {
                                    Team.Player2.PeriodIs2plus2 = false;
                                }
                                else
                                {
                                    Team.Player2.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player2.PenaltyTimeSet = TimeSpan.FromMinutes(2);
                                    Team.Player2.PeriodIs2plus2 = false; Team.Player2.PenaltyTimeLeft = TimeSpan.FromMinutes(2);
                                }
                            }
                            else
                            {
                                MatchTimeSpanOffsets(Team, Player1);
                                Team.Player2.PenaltyTimeLeft = Team.Player2.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset);
                            }
                        }
                    }
                    break;
            }
        }
        private void StopPenaltyIfRanOut(TeamClass Team, bool Player1)
        {
            switch (Player1)
            {
                case true:
                    if ((Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset) > Team.Player1.PenaltyTimeSet)
                    {
                        CancelPenalty(Team, true);
                    }
                    break;
                case false:
                    if ((Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset) > Team.Player2.PenaltyTimeSet)
                        CancelPenalty(Team, false);
                    break;
            }

        }
        private void MoveDownASlot(TeamClass Team)
        {
            if (Team.Player2.PenaltyRunning == true && Team.Player1.PenaltyRunning == false)
            {
                Team.Player1.PeriodIs2plus2 = Team.Player2.PeriodIs2plus2;
                Team.Player2.PeriodIs2plus2 = false;
                Team.Player1.ScoreAtPeriodStart = Team.Player2.ScoreAtPeriodStart;
                Team.Player1.PenaltyTimeLeft = Team.Player2.PenaltyTimeLeft;
                Team.Player1.PenaltyTimeSet = Team.Player2.PenaltyTimeLeft;
                Team.Player2.PenaltyTimeLeft = TimeSpan.Zero;
                Team.Player1.Number = Team.Player2.Number;
                Team.Player2.Number = String.Empty;
                Team.Player1.PenaltyRunning = true;
                Team.Player2.PenaltyRunning = false;
                Team.Player2.PenaltyOffset = TimeSpan.Zero;
                Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed;


            }
        }
        private void UpdateTimeLabel()
        {
            if (Vars.Game.TimeLeft < TimeSpan.FromMinutes(1))
            {
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatMilisecond, CultureInfo.InvariantCulture); // update time Main window
                Vars.Window.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatMilisecond, CultureInfo.InvariantCulture); // update time Secondary window
            }
            else
            {
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture); // update time Main window
                Vars.Window.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture); // update time Secondary window
            }
            ProgressBarGameTime.Maximum = Vars.Game.LastSetTime.TotalSeconds; ProgressBarGameTime.Value = Vars.Game.TimeLeft.TotalSeconds;
        }
        private void UpdatePenaltyUIMain()
        {
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
                LabelT1P1TimeLeftVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Team1Indicator1.Background = Brushes.Red;
            }
            else
            {
                LabelT1P1NumberVariable.Content = "";
                LabelT1P1TimeLeftVariable.Content = "";
                Team1Indicator1.Background = Brushes.Green;
            }


            if (Vars.Team1.Player2.PenaltyRunning)
            {
                LabelT1P2NumberVariable.Content = Vars.Team1.Player2.Number;
                LabelT1P2TimeLeftVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Team1Indicator2.Background = Brushes.Red;
            }
            else
            {
                LabelT1P2NumberVariable.Content = "";
                LabelT1P2TimeLeftVariable.Content = "";
                Team1Indicator2.Background = Brushes.Green;
            }

            if (Vars.Team2.Player1.PenaltyRunning)
            {
                LabelT2P1NumberVariable.Content = Vars.Team2.Player1.Number;
                LabelT2P1TimeLeftVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Team2Indicator1.Background = Brushes.Red;
            }
            else
            {
                LabelT2P1NumberVariable.Content = "";
                LabelT2P1TimeLeftVariable.Content = "";
                Team2Indicator1.Background = Brushes.Green;
            }

            if (Vars.Team2.Player2.PenaltyRunning)
            {
                LabelT2P2NumberVariable.Content = Vars.Team2.Player2.Number;
                LabelT2P2TimeLeftVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                LabelT2P2NumberVariable.Content = "";
                LabelT2P2TimeLeftVariable.Content = "";
                Team2Indicator2.Background = Brushes.Green;
            }
        }
        private void UpdatePenaltyUISecondary()
        {
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                Vars.Window.LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
                Vars.Window.LabelT1P1TimeLeftVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Vars.Window.Team1Indicator1.Background = Brushes.Red;
            }
            else
            {
                Vars.Window.LabelT1P1NumberVariable.Content = "";
                Vars.Window.LabelT1P1TimeLeftVariable.Content = "";
                Vars.Window.Team1Indicator1.Background = Brushes.Green;
            }


            if (Vars.Team1.Player2.PenaltyRunning)
            {
                Vars.Window.LabelT1P2NumberVariable.Content = Vars.Team1.Player2.Number;
                Vars.Window.LabelT1P2TimeLeftVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Vars.Window.Team1Indicator2.Background = Brushes.Red;
            }
            else
            {
                Vars.Window.LabelT1P2NumberVariable.Content = "";
                Vars.Window.LabelT1P2TimeLeftVariable.Content = "";
                Vars.Window.Team1Indicator2.Background = Brushes.Green;
            }

            if (Vars.Team2.Player1.PenaltyRunning)
            {
                Vars.Window.LabelT2P1NumberVariable.Content = Vars.Team2.Player1.Number;
                Vars.Window.LabelT2P1TimeLeftVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Vars.Window.Team2Indicator1.Background = Brushes.Red;
            }
            else
            {
                Vars.Window.LabelT2P1NumberVariable.Content = "";
                Vars.Window.LabelT2P1TimeLeftVariable.Content = "";
                Vars.Window.Team2Indicator1.Background = Brushes.Green;
            }

            if (Vars.Team2.Player2.PenaltyRunning)
            {
                Vars.Window.LabelT2P2NumberVariable.Content = Vars.Team2.Player2.Number;
                Vars.Window.LabelT2P2TimeLeftVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular, CultureInfo.InvariantCulture);
                Vars.Window.Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                Vars.Window.LabelT2P2NumberVariable.Content = "";
                Vars.Window.LabelT2P2TimeLeftVariable.Content = "";
                Vars.Window.Team2Indicator2.Background = Brushes.Green;
            }
        }
        private void UpdateAllUI() // ALL UI
        {
            if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime)
            {
                ButtonPauseTime.Content = "Start Time";
            }

            if (Vars.Game.StopwatchPeriod.IsRunning)
            {
                ButtonPauseTime.Content = "Pause Time";
                UpdatePenaltyUIMain();
                UpdatePenaltyUISecondary();
            }
            if (Vars.Game.StopwatchPeriod.IsRunning == false)
            {
                ButtonPauseTime.Content = "Start Time";
            }

            if (Vars.Team1.TimeoutRunning)
            {
                LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush1;
                Vars.Window.LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush1;
            }
            else
            {
                LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush2;
                Vars.Window.LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush2;
                if (Vars.Team1.HasTimeout)
                {
                    LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Hidden;
                    Vars.Window.LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Hidden;
                }
                else
                {
                    LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Visible;
                    Vars.Window.LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Visible;
                }
            }
            if (Vars.Team2.TimeoutRunning)
            {
                LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush1;
                Vars.Window.LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush1;
            }
            else
            {
                LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush2;
                Vars.Window.LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush2;
                if (Vars.Team2.HasTimeout)
                {
                    LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Hidden;
                    Vars.Window.LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Hidden;
                }
                else
                {
                    LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Visible;
                    Vars.Window.LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Visible;
                }
            }
            switch (Vars.Game.GameState)
            {
                case CustomTypes.GameState.Regular:
                    LabelTimeText.Content = "Game"; Vars.Window.LabelTimeText.Content = "Game";
                    ButtonBreakMode.Content = "Enter Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonTimeout.IsEnabled = true;
                    ButtonBreakMode.IsEnabled = true;
                    break;
                case CustomTypes.GameState.Break:
                    LabelTimeText.Content = "Break"; Vars.Window.LabelTimeText.Content = "Break";
                    ButtonBreakMode.Content = "Leave Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonBreakMode.IsEnabled = true;
                    ButtonTimeout.IsEnabled = false;
                    break;
                case CustomTypes.GameState.Timeout:
                    LabelTimeText.Content = "Timeout"; Vars.Window.LabelTimeText.Content = "Timeout";
                    ButtonBreakMode.Content = "Enter Break Mode";
                    ButtonTimeout.Content = "Cancel Timeout";
                    ButtonBreakMode.IsEnabled = false;
                    ButtonTimeout.IsEnabled = true;
                    break;
            }
            UpdateTimeLabel();
        }
        private void SetTime(TimeSpan InputTime)
        {
            switch (Vars.Game.GameState)
            {
                case CustomTypes.GameState.Regular:
                    Vars.Game.StopwatchPeriod.Reset();
                    Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;

                    Vars.Team1.Player1.PenaltyTimeSet = Vars.Team1.Player1.PenaltyTimeLeft; Vars.Team1.Player1.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team1.Player2.PenaltyTimeSet = Vars.Team1.Player2.PenaltyTimeLeft; Vars.Team1.Player2.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team2.Player1.PenaltyTimeSet = Vars.Team2.Player1.PenaltyTimeLeft; Vars.Team2.Player1.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team2.Player2.PenaltyTimeSet = Vars.Team2.Player2.PenaltyTimeLeft; Vars.Team2.Player2.PenaltyOffset = TimeSpan.Zero;
                    break;
                case CustomTypes.GameState.Break:
                    Vars.Game.StopwatchPeriod.Reset(); Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;
                    break;
                case CustomTypes.GameState.Timeout:
                    Vars.Game.StopwatchPeriod.Reset(); Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;
                    break;
            }
            UpdateTimeLabel();
            ButtonPauseTime.Content = "Start Time";
        }
        private void ChangePeriod(CustomTypes.PeriodState PeriodVariable)
        {

            switch (PeriodVariable)
            {
                case CustomTypes.PeriodState.First:
                    LabelPeriod.Content = "1";
                    Vars.Window.LabelPeriodVariable.Content = "1";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = false;
                    break;
                case CustomTypes.PeriodState.Second:
                    LabelPeriod.Content = "2";
                    Vars.Window.LabelPeriodVariable.Content = "2";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.Third:
                    LabelPeriod.Content = "3";
                    Vars.Window.LabelPeriodVariable.Content = "3";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.Extension:
                    LabelPeriod.Content = "P";
                    Vars.Window.LabelPeriodVariable.Content = "P";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.SN:
                    LabelPeriod.Content = "Sn";
                    Vars.Window.LabelPeriodVariable.Content = "Sn";
                    ButtonPeriodPlus.IsEnabled = false;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
            }
        }
        private void CancelPenalty(TeamClass Team, bool Player1)
        {
            switch (Player1)
            {
                case true:
                    Team.Player1.PenaltyOffset = TimeSpan.Zero; Team.Player1.PenaltyRunning = false;
                    Team.Player1.PenaltyTimeLeft = TimeSpan.Zero; Team.Player1.PenaltyTimeSet = TimeSpan.Zero;
                    Team.Player1.Number = String.Empty; Team.Player1.PeriodIs2plus2 = false;
                    break;
                case false:
                    Team.Player2.PenaltyOffset = TimeSpan.Zero; Team.Player2.PenaltyRunning = false;
                    Team.Player2.PenaltyTimeLeft = TimeSpan.Zero; Team.Player2.PenaltyTimeSet = TimeSpan.Zero;
                    Team.Player2.Number = String.Empty; Team.Player2.PeriodIs2plus2 = false;
                    MoveDownASlot(Team);
                    break;
            }
            UpdatePenaltyUIMain(); UpdatePenaltyUISecondary();
        }
        private string SelectPlayer(TeamClass Team, ListBox Listbox)
        {

            if (Team.PlayerList.Count != 0 && Listbox.SelectedItems.Count != 0)
            {
                int CheckedItemIndex = Listbox.SelectedItems.Cast<int>().ToArray().First();
                return Team.PlayerList[CheckedItemIndex].Number;
            }
            else
            {
                switch (Team.Index)
                {
                    case 1: return (UpDownPenaltyTeam1Player.Value).ToString(CultureInfo.CurrentCulture);
                    case 2: return (UpDownPenaltyTeam2Player.Value).ToString(CultureInfo.CurrentCulture);
                    default: return String.Empty;
                };
            }
        }
        private void SetPenalty(TeamClass Team, TeamClass OtherTeam, bool Player1, TimeSpan TimeSet, string PlayerNumber, bool Is2plus2)
        {
            switch (Player1)
            {
                case true:
                    Team.Player1.Number = PlayerNumber; Team.Player1.PeriodIs2plus2 = Is2plus2;
                    Team.Player1.PenaltyTimeSet = TimeSet; Team.Player1.PenaltyTimeLeft = TimeSet;
                    Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player1.PenaltyRunning = true;
                    Team.Player1.ScoreAtPeriodStart = OtherTeam.Score;
                    break;
                case false:
                    Team.Player2.Number = PlayerNumber; Team.Player2.PeriodIs2plus2 = Is2plus2;
                    Team.Player2.PenaltyTimeSet = TimeSet; Team.Player2.PenaltyTimeLeft = TimeSet;
                    Team.Player2.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player2.PenaltyRunning = true;
                    Team.Player2.ScoreAtPeriodStart = OtherTeam.Score;
                    break;
            }

            UpdatePenaltyUIMain(); UpdatePenaltyUISecondary();
        }
        private void AssignPenalty(TeamClass Team, TeamClass OtherTeam, ListBox Listbox, TimeSpan TimeSet, bool Is2plus2)
        {
            if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, OtherTeam, true, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, true);
                //MessageBox.Show(String.Format("Number: {0}\nTimeSet: {1}\nTimeLeft: {2}\nStopwatch: {3}\n2+2: {4}", Team.Player1.Number,Team.Player1.PenaltyTimeSet,Team.Player1.PenaltyTimeLeft,Team.Player1.PenaltyStopwatch.IsRunning,Team.Player1.PeriodIs2plus2), "Debug", MessageBoxButton.OK);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, OtherTeam, false, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, false);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == true)
            {
                _ = MessageBox.Show("All penalty slots occupied, cancel running penalties to free up space.", Caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, OtherTeam, true, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, true);
            }
        }
    }
}
