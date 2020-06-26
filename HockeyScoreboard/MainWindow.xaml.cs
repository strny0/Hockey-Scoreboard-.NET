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

namespace HockeyScoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            DispatcherTimer MainTimer = new DispatcherTimer(DispatcherPriority.Background);
            MainTimer.IsEnabled = true; MainTimer.Interval = TimeSpan.FromMilliseconds(1);
            MainTimer.Start();
            MainTimer.Tick += MainTimer_Tick;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (Vars.Game.StopwatchPeriod.IsRunning) // PERIOD RUNNING
            {
                switch (Vars.Game.GameState)
                {
                    case CustomTypes.GameStates.Regular:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Game.StopwatchPeriod.Reset();
                            Vars.Game.TimeLeft = TimeSpan.Zero;
                        }
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;

                        StopPenaltyIfRanOut(Vars.Team1, true); StopPenaltyIfRanOut(Vars.Team1, false); // Stop penalty timer if time ran out
                        StopPenaltyIfRanOut(Vars.Team2, true); StopPenaltyIfRanOut(Vars.Team2, false);

                        MoveDownASlot(Vars.Team1); MoveDownASlot(Vars.Team2); // Move Player2 to slot of Player1 if Player1 slot empty

                        TickTimeDownPenalty(Vars.Team1, true); TickTimeDownPenalty(Vars.Team1, false);   // Tick Time Down
                        TickTimeDownPenalty(Vars.Team2, true); TickTimeDownPenalty(Vars.Team2, false);   // Function checks if penalties for each player are running
                        break;
                    case CustomTypes.GameStates.Break:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Game.StopwatchPeriod.Reset();
                            Vars.Game.TimeLeft = TimeSpan.Zero;
                        }
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                    case CustomTypes.GameStates.Timeout:
                        if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime) // if run out, stop counting
                        {
                            Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
                            Vars.Game.GameState = CustomTypes.GameStates.Regular;
                            SetTime(Vars.Game.LastRegularTime);
                        }
                        Vars.Game.TimeLeft = Vars.Game.LastSetTime - Vars.Game.StopwatchPeriod.Elapsed;
                        break;
                }
            }
            UpdateAllUI(); ; // UI UPDATE
        } // TIMER

        private void DefineDefaultVars() // IF NO PREFERENCES FOUND, DEFAULT VALUES
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
            Vars.Game.Period = CustomTypes.PeriodStates.First;
            Vars.Game.GameState = CustomTypes.GameStates.Regular;
            ButtonPeriodMinus.IsEnabled = false;
            Vars.Prefs.DefaultBreakTime = TimeSpan.FromMinutes(1);
            Vars.Prefs.DefaultTimeoutTime = new TimeSpan(0, 1, 30);
        }
        private OpenFileDialog DefineImageFileDialog()
        {
            OpenFileDialog LoadImageDialog = new OpenFileDialog
            {
                Filter = "",
                Title = "Load Image",
                InitialDirectory = Environment.CurrentDirectory
            };
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                LoadImageDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", LoadImageDialog.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }
            LoadImageDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", LoadImageDialog.Filter, sep, "All Files", "*.*");
            LoadImageDialog.DefaultExt = ".png"; // Default file extension 
            return LoadImageDialog;

        }
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
            Vars.Window.LabelScoreTeam1Variable.Content = Vars.Team1.Score.ToString();
        }

        private void UpDownTeam1Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team1.Shots = (int)UpDownTeam1Shots.Value;
            Vars.Window.LabelShotsTeam1Variable.Content = Vars.Team1.Shots.ToString();
        }

        private void UpDownTeam2Score_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Score = (int)UpDownTeam2Score.Value;
            Vars.Window.LabelScoreTeam2Variable.Content = Vars.Team2.Score.ToString();
        }

        private void UpDownTeam2Shots_ValueChanged(object sender, EventArgs e)
        {
            Vars.Team2.Shots = (int)UpDownTeam2Shots.Value;
            Vars.Window.LabelShotsTeam2Variable.Content = Vars.Team2.Shots.ToString();
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
        private void ButtonTeam1SelectImage_Click(object sender, RoutedEventArgs e)
        {
            ChangeImage(Vars.Team1, ImageTeam1Logo, Vars.Window.ImageTeam1LogoView);
        }
        private void ButtonTeam2SelectImage_Click(object sender, RoutedEventArgs e)
        {
            ChangeImage(Vars.Team2, ImageTeam2Logo, Vars.Window.ImageTeam2LogoView);
        }

        private void UpdatePenaltyUIMain()
        {
            LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
            LabelT1P1TimeVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                Team1Indicator1.Background = Brushes.Red;
            }
            else
            {
                Team1Indicator1.Background = Brushes.Green;
            }

            LabelT1P2NumberVariable.Content = Vars.Team1.Player2.Number;
            LabelT1P2TimeVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
            if (Vars.Team1.Player2.PenaltyRunning)
            {
                Team1Indicator2.Background = Brushes.Red;
            }
            else
            {
                Team1Indicator2.Background = Brushes.Green;
            }
            LabelT2P1NameVariable.Content = Vars.Team2.Player1.Number;
            LabelT2P1TimeVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
            if (Vars.Team2.Player1.PenaltyRunning)
            {
                Team2Indicator1.Background = Brushes.Red;
            }
            else
            {
                Team2Indicator1.Background = Brushes.Green;
            }
            LabelT2P2NameVariable.Content = Vars.Team2.Player2.Number;
            LabelT2P2TimeVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
            if (Vars.Team2.Player2.PenaltyRunning)
            {
                Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                Team2Indicator2.Background = Brushes.Green;
            }
        }
        private void UpdatePenaltyUISecondary()
        {
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                Vars.Window.LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
                Vars.Window.LabelT1P1TimeLeftVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
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
                Vars.Window.LabelT1P2TimeLeftVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
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
                Vars.Window.LabelT2P1TimeLeftVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
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
                Vars.Window.LabelT2P2TimeLeftVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimeFormatRegular);
                Vars.Window.Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                Vars.Window.LabelT2P2NumberVariable.Content = "";
                Vars.Window.LabelT2P2TimeLeftVariable.Content = "";
                Vars.Window.Team2Indicator2.Background = Brushes.Green;
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
        private void TickTimeDownPenalty(TeamClass Team, bool Player1)
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
                            if (Team.Score > Team.Player1.ScoreAtPeriodStart)
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
                            if (Team.Score > Team.Player2.ScoreAtPeriodStart)
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
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatMilisecond); // update time Main window
                Vars.Window.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatMilisecond); // update time Secondary window
            }
            else
            {
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatRegular); // update time Main window
                Vars.Window.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimeFormatRegular); // update time Secondary window
            }
        }

        private readonly Brush ColorBrush1 = new SolidColorBrush(Color.FromRgb(241, 205, 70));
        private readonly Brush ColorBrush2 = new SolidColorBrush(Color.FromRgb(105, 108, 133));
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
                case CustomTypes.GameStates.Regular:
                    LabelTimeText.Content = "Game"; Vars.Window.LabelTimeText.Content = "Game";
                    ButtonBreakMode.Content = "Enter Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonTimeout.IsEnabled = true;
                    ButtonBreakMode.IsEnabled = true;
                    break;
                case CustomTypes.GameStates.Break:
                    LabelTimeText.Content = "Break"; Vars.Window.LabelTimeText.Content = "Break";
                    ButtonBreakMode.Content = "Leave Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonBreakMode.IsEnabled = true;
                    ButtonTimeout.IsEnabled = false;
                    break;
                case CustomTypes.GameStates.Timeout:
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
                case CustomTypes.GameStates.Regular:
                    Vars.Game.StopwatchPeriod.Reset();
                    Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;

                    Vars.Team1.Player1.PenaltyTimeSet = Vars.Team1.Player1.PenaltyTimeLeft; Vars.Team1.Player1.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team1.Player2.PenaltyTimeSet = Vars.Team1.Player2.PenaltyTimeLeft; Vars.Team1.Player2.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team2.Player1.PenaltyTimeSet = Vars.Team2.Player1.PenaltyTimeLeft; Vars.Team2.Player1.PenaltyOffset = TimeSpan.Zero;
                    Vars.Team2.Player2.PenaltyTimeSet = Vars.Team2.Player2.PenaltyTimeLeft; Vars.Team2.Player2.PenaltyOffset = TimeSpan.Zero;
                    break;
                case CustomTypes.GameStates.Break:
                    Vars.Game.StopwatchPeriod.Reset(); Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;
                    break;
                case CustomTypes.GameStates.Timeout:
                    Vars.Game.StopwatchPeriod.Reset(); Vars.Game.LastSetTime = InputTime; Vars.Game.TimeLeft = InputTime;
                    break;
            }
            UpdateTimeLabel();
            ButtonPauseTime.Content = "Start Time";
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
            if (Vars.Game.GameState == CustomTypes.GameStates.Break)
            {
                Vars.Game.GameState = CustomTypes.GameStates.Regular;
                SetTime(Vars.Game.LastRegularTime);
            }
            else
            {
                Vars.Game.LastRegularTime = Vars.Game.TimeLeft;
                Vars.Game.GameState = CustomTypes.GameStates.Break;
                SetTime(Vars.Prefs.DefaultBreakTime);
            }
        }
        private void ButtonTimeout_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.Game.GameState == CustomTypes.GameStates.Timeout)
            {
                Vars.Game.GameState = CustomTypes.GameStates.Regular;
                Vars.Team1.TimeoutRunning = false; Vars.Team2.TimeoutRunning = false;
                SetTime(Vars.Game.LastRegularTime);
            }
            else
            {
                Vars.Game.LastRegularTime = Vars.Game.TimeLeft;
                Vars.Game.GameState = CustomTypes.GameStates.Timeout;
                Vars.Game.StopwatchPeriod.Stop();
                TimeoutDialog TD = new TimeoutDialog();
                TD.ShowDialog();
            }
        }
        private void ButtonResetTimeout_Click(object sender, RoutedEventArgs e)
        {
            Vars.Team1.HasTimeout = true; Vars.Team2.HasTimeout = true;
        }

        private void ChangePeriod(CustomTypes.PeriodStates PeriodVariable)
        {

            switch (PeriodVariable)
            {
                case CustomTypes.PeriodStates.First:
                    LabelPeriod.Content = "1";
                    Vars.Window.LabelPeriodVariable.Content = "1";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = false;
                    break;
                case CustomTypes.PeriodStates.Second:
                    LabelPeriod.Content = "2";
                    Vars.Window.LabelPeriodVariable.Content = "2";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodStates.Third:
                    LabelPeriod.Content = "3";
                    Vars.Window.LabelPeriodVariable.Content = "3";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodStates.Extension:
                    LabelPeriod.Content = "P";
                    Vars.Window.LabelPeriodVariable.Content = "P";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodStates.SN:
                    LabelPeriod.Content = "Sn";
                    Vars.Window.LabelPeriodVariable.Content = "Sn";
                    ButtonPeriodPlus.IsEnabled = false;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
            }
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
            Vars.Game.Period = CustomTypes.PeriodStates.First; Vars.Game.GameState = CustomTypes.GameStates.Regular;
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
        private string SelectPlayer(TeamClass Team, ListBox Listbox)
        {

            if (Team.PlayerList.Count != 0 && Listbox.SelectedItems.Count != 0)
            {
                int CheckedItemIndex = Listbox.SelectedItems.Cast<int>().ToArray().First();
                return Team.PlayerList[CheckedItemIndex].Number.ToString();
            }
            else
            {
                switch(Team.Index)
                {
                    case 1: return (UpDownPenaltyTeam1Player.Value).ToString();
                    case 2: return (UpDownPenaltyTeam2Player.Value).ToString();
                    default: return String.Empty;
                };
            }
        }
        private void SetPenalty(TeamClass Team, bool Player1, TimeSpan TimeSet, string PlayerNumber, bool Is2plus2)
        {
            switch (Player1)
            {
                case true:
                    //MessageBox.Show(String.Format("Number: {0}\nTimeSet: {1}\nTimeLeft: {2}\nStopwatch: {3}\n2+2: {4}", Player.Number, Player.PenaltyTimeSet, Player.PenaltyTimeLeft, Player.PenaltyStopwatch.IsRunning, Player.PeriodIs2plus2), "Debug", MessageBoxButton.OK);
                    Team.Player1.Number = PlayerNumber; Team.Player1.PeriodIs2plus2 = Is2plus2;
                    Team.Player1.PenaltyTimeSet = TimeSet; Team.Player1.PenaltyTimeLeft = TimeSet;
                    Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player1.PenaltyRunning = true;
                    Team.Player1.ScoreAtPeriodStart = Team.Score;
                    break;
                case false:
                    Team.Player2.Number = PlayerNumber; Team.Player2.PeriodIs2plus2 = Is2plus2;
                    Team.Player2.PenaltyTimeSet = TimeSet; Team.Player2.PenaltyTimeLeft = TimeSet;
                    Team.Player2.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player2.PenaltyRunning = true;
                    Team.Player2.ScoreAtPeriodStart = Team.Score;
                    break;
            }

            UpdatePenaltyUIMain(); UpdatePenaltyUISecondary();
        }
        private void AssignPenalty(TeamClass Team, ListBox Listbox, TimeSpan TimeSet, bool Is2plus2)
        {
            if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, true, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, true);
                //MessageBox.Show(String.Format("Number: {0}\nTimeSet: {1}\nTimeLeft: {2}\nStopwatch: {3}\n2+2: {4}", Team.Player1.Number,Team.Player1.PenaltyTimeSet,Team.Player1.PenaltyTimeLeft,Team.Player1.PenaltyStopwatch.IsRunning,Team.Player1.PeriodIs2plus2), "Debug", MessageBoxButton.OK);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, false, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, false);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == true)
            {
                MessageBox.Show("All penalty slots occupied, cancel running penalties to free up space.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                SetPenalty(Team, true, TimeSet, SelectPlayer(Team, Listbox), Is2plus2);
                MatchTimeSpanOffsets(Team, true);
            }
        }

        private void ButtonSetSpecificPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam1.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam1.Value), false); // It is a slot based system, there are 2 slots for each team,
        }

        private void ButtonSet1minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes(1), false);
        }

        private void ButtonSet2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes(2), false);
        }

        private void ButtonSet5minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes(5), false);
        }

        private void ButtonSet10minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes(10), false);
        }

        private void ButtonSet2plus2minutePenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team1, ListBoxTeam1Players, TimeSpan.FromMinutes(4), true);
        }

        private void ButtonSetAPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetBPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetCPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetDPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetEPenaltyTeam1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetSpecificPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, (TimeSpan.FromMinutes((int)UpDownMinutesPenaltyTeam2.Value) + TimeSpan.FromSeconds((int)UpDownSecondsPenaltyTeam2.Value)), false);
        }

        private void ButtonSet1minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, TimeSpan.FromMinutes(1), false);
        }

        private void ButtonSet2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, TimeSpan.FromMinutes(2), false);
        }

        private void ButtonSet5minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, TimeSpan.FromMinutes(5), false);
        }

        private void ButtonSet10minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, TimeSpan.FromMinutes(10), false);
        }

        private void ButtonSet2plus2minutePenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {
            AssignPenalty(Vars.Team2, ListBoxTeam2Players, TimeSpan.FromMinutes(4), true);
        }

        private void ButtonSetAPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetBPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetCPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetDPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSetEPenaltyTeam2_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}

