using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace HockeyScoreboard
{
    public partial class MainWindow : Window
    {
        // CONSTANTS
        private List<string> teamLoadingFilePaths = new List<string>();
        private const string ListboxFormat = "{0} - {1}";
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
        private void DefineDefaultProgramState()
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
                Number = "",
                PeriodIsMinor = false,
                PeriodIsDoubleMinor = false,
                PenaltyRunning = false,
                PenaltyOffset = TimeSpan.Zero,
                PenaltyTimeLeft = TimeSpan.Zero,
                PenaltyTimeSet = TimeSpan.Zero,
                OtherTeamScoreAtPenaltyStart = 0,
            };
            Vars.Team1.Player2 = new CustomTypes.PlayerType
            {
                Number = "",
                PeriodIsMinor = false,
                PeriodIsDoubleMinor = false,
                PenaltyRunning = false,
                PenaltyOffset = TimeSpan.Zero,
                PenaltyTimeLeft = TimeSpan.Zero,
                PenaltyTimeSet = TimeSpan.Zero,
                OtherTeamScoreAtPenaltyStart = 0,
            };
            Vars.Team2.Player1 = new CustomTypes.PlayerType
            {
                Number = "",
                PeriodIsMinor = false,
                PeriodIsDoubleMinor = false,
                PenaltyRunning = false,
                PenaltyOffset = TimeSpan.Zero,
                PenaltyTimeLeft = TimeSpan.Zero,
                PenaltyTimeSet = TimeSpan.Zero,
                OtherTeamScoreAtPenaltyStart = 0,
            };
            Vars.Team2.Player2 = new CustomTypes.PlayerType
            {
                Number = "",
                PeriodIsMinor = false,
                PeriodIsDoubleMinor = false,
                PenaltyRunning = false,
                PenaltyOffset = TimeSpan.Zero,
                PenaltyTimeLeft = TimeSpan.Zero,
                PenaltyTimeSet = TimeSpan.Zero,
                OtherTeamScoreAtPenaltyStart = 0,
            };
            Vars.Team1.HasTimeout = true;
            Vars.Team2.HasTimeout = true;
            Vars.Game.LastSetTime = TimeSpan.FromMinutes(7);
            Vars.Game.TimeLeft = TimeSpan.FromMinutes(7);
            Vars.Game.InputMinute = 7;
            Vars.Game.InputSecond = 0;
            Vars.Game.Period = CustomTypes.PeriodState.First;
            Vars.Game.GameState = CustomTypes.GameState.Regular;
            Vars.Game.TeamManagerTeamSavingClassInstance = new TeamSavingClass
            {
                PlayerList = new List<TeamSavingClass.PlayerTeamListType>(),
                TeamLogoPath = "",
                TeamName = ""
            };
            Vars.Team1.SelectedTeamList = new List<TeamSavingClass.PlayerTeamListType>();
            Vars.Team2.SelectedTeamList = new List<TeamSavingClass.PlayerTeamListType>();
            ButtonPeriodMinus.IsEnabled = false;

        }
        private void DefineDefaultPrefs() // DEFAULT PREFERENCES
        {
            Vars.Prefs.DefaultTeamDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\Hockey Scoreboard\\Teams\\";
            Vars.Prefs.DefaultBreakTime = new TimeSpan(0, 1, 0);
            Vars.Prefs.DefaultTimeoutTime = new TimeSpan(0, 0, 30);
        }
        private OpenFileDialog DefineImageFileDialog()
        {
            OpenFileDialog LoadImageDialog = new OpenFileDialog
            {
                Title = "Load Image",
                FilterIndex = 6,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
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
        private SaveFileDialog DefineSaveXmlDialog()
        {
            SaveFileDialog SaveXmlDialog = new SaveFileDialog
            {
                Title = "Save Team",
                AddExtension = true,
                FilterIndex = 1,
                InitialDirectory = Vars.Prefs.DefaultTeamDirectory,
                Filter = "XML files(.xml) | *.xml",
                DefaultExt = ".xml"
            };
            return SaveXmlDialog;

        }
        private OpenFileDialog DefineLoadXmlDialog()
        {
            OpenFileDialog LoadXmlDialog = new OpenFileDialog
            {
                Title = "Load Team",
                FilterIndex = 1,
                InitialDirectory = Vars.Prefs.DefaultTeamDirectory,
                Filter = "XML files(.xml) | *.xml",
                DefaultExt = ".xml"
            };
            return LoadXmlDialog;
        }
        private string ReturnImageSourcePath()
        {
            OpenFileDialog ImageDialog = DefineImageFileDialog();
            Nullable<bool> Result = ImageDialog.ShowDialog();

            if (Result == true)
            {
                return ImageDialog.FileName;
            }
            else return "";
        }
        private void ChangeImageFromPath(string ImagePath, Image ImageToChange)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(ImagePath))
            {
                CacheOption = BitmapCacheOption.OnLoad
            };
            ImageToChange.Source = bitmap;
        }
        private void SetTime(TimeSpan InputTime) // SET TIME
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
            UIUpdateGameTime();
            ButtonPauseTime.Content = "Start Time";
        } 
        private void ChangePeriod(CustomTypes.PeriodState PeriodVariable)
        {

            switch (PeriodVariable)
            {
                case CustomTypes.PeriodState.First:
                    LabelPeriod.Content = "1";
                    Vars.SecondaryWindow.LabelPeriodVariable.Content = "1";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = false;
                    break;
                case CustomTypes.PeriodState.Second:
                    LabelPeriod.Content = "2";
                    Vars.SecondaryWindow.LabelPeriodVariable.Content = "2";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.Third:
                    LabelPeriod.Content = "3";
                    Vars.SecondaryWindow.LabelPeriodVariable.Content = "3";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.Extension:
                    LabelPeriod.Content = "P";
                    Vars.SecondaryWindow.LabelPeriodVariable.Content = "P";
                    ButtonPeriodPlus.IsEnabled = true;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
                case CustomTypes.PeriodState.SN:
                    LabelPeriod.Content = "Sn";
                    Vars.SecondaryWindow.LabelPeriodVariable.Content = "Sn";
                    ButtonPeriodPlus.IsEnabled = false;
                    ButtonPeriodMinus.IsEnabled = true;
                    break;
            }
        }
        // UI UPDATES
        private void UIUpdateGameTime()
        {
            if (Vars.Game.TimeLeft < TimeSpan.FromMinutes(1))
            {
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimespanFormatMilisecond, CultureInfo.InvariantCulture); // update time Main window
                Vars.SecondaryWindow.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimespanFormatMilisecond, CultureInfo.InvariantCulture); // update time Secondary window
            }
            else
            {
                LabelTime.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture); // update time Main window
                Vars.SecondaryWindow.LabelTimeVariable.Content = Vars.Game.TimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture); // update time Secondary window
            }
            ProgressBarGameTime.Maximum = Vars.Game.LastSetTime.TotalSeconds; ProgressBarGameTime.Value = Vars.Game.TimeLeft.TotalSeconds;
        }
        private void UIUpdatePenaltyMainWindow()
        {
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
                LabelT1P1TimeLeftVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
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
                LabelT1P2TimeLeftVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
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
                LabelT2P1TimeLeftVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
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
                LabelT2P2TimeLeftVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
                Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                LabelT2P2NumberVariable.Content = "";
                LabelT2P2TimeLeftVariable.Content = "";
                Team2Indicator2.Background = Brushes.Green;
            }
        }
        private void UIUpdatePenaltySecondaryWindow()
        {
            if (Vars.Team1.Player1.PenaltyRunning)
            {
                Vars.SecondaryWindow.LabelT1P1NumberVariable.Content = Vars.Team1.Player1.Number;
                Vars.SecondaryWindow.LabelT1P1TimeLeftVariable.Content = Vars.Team1.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
                Vars.SecondaryWindow.Team1Indicator1.Background = Brushes.Red;
            }
            else
            {
                Vars.SecondaryWindow.LabelT1P1NumberVariable.Content = "";
                Vars.SecondaryWindow.LabelT1P1TimeLeftVariable.Content = "";
                Vars.SecondaryWindow.Team1Indicator1.Background = Brushes.Green;
            }


            if (Vars.Team1.Player2.PenaltyRunning)
            {
                Vars.SecondaryWindow.LabelT1P2NumberVariable.Content = Vars.Team1.Player2.Number;
                Vars.SecondaryWindow.LabelT1P2TimeLeftVariable.Content = Vars.Team1.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
                Vars.SecondaryWindow.Team1Indicator2.Background = Brushes.Red;
            }
            else
            {
                Vars.SecondaryWindow.LabelT1P2NumberVariable.Content = "";
                Vars.SecondaryWindow.LabelT1P2TimeLeftVariable.Content = "";
                Vars.SecondaryWindow.Team1Indicator2.Background = Brushes.Green;
            }

            if (Vars.Team2.Player1.PenaltyRunning)
            {
                Vars.SecondaryWindow.LabelT2P1NumberVariable.Content = Vars.Team2.Player1.Number;
                Vars.SecondaryWindow.LabelT2P1TimeLeftVariable.Content = Vars.Team2.Player1.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
                Vars.SecondaryWindow.Team2Indicator1.Background = Brushes.Red;
            }
            else
            {
                Vars.SecondaryWindow.LabelT2P1NumberVariable.Content = "";
                Vars.SecondaryWindow.LabelT2P1TimeLeftVariable.Content = "";
                Vars.SecondaryWindow.Team2Indicator1.Background = Brushes.Green;
            }

            if (Vars.Team2.Player2.PenaltyRunning)
            {
                Vars.SecondaryWindow.LabelT2P2NumberVariable.Content = Vars.Team2.Player2.Number;
                Vars.SecondaryWindow.LabelT2P2TimeLeftVariable.Content = Vars.Team2.Player2.PenaltyTimeLeft.ToString(Vars.Game.TimespanFormatRegular, CultureInfo.InvariantCulture);
                Vars.SecondaryWindow.Team2Indicator2.Background = Brushes.Red;
            }
            else
            {
                Vars.SecondaryWindow.LabelT2P2NumberVariable.Content = "";
                Vars.SecondaryWindow.LabelT2P2TimeLeftVariable.Content = "";
                Vars.SecondaryWindow.Team2Indicator2.Background = Brushes.Green;
            }
        }
        private void UIReloadControlsValues(TeamClass Team)
        {
            if (Team.Index == 1)
            {
                ChangeImageFromPath(Team.LogoSource, ImageTeam1Logo); ChangeImageFromPath(Team.LogoSource, Vars.SecondaryWindow.ImageTeam1LogoView);
                TextBoxTeam1Name.Text = Team.Name; UpDownTeam1Score.Value = Team.Score;  UpDownTeam1Shots.Value = Team.Shots;
            }
            else if (Team.Index == 2)
            {
                ChangeImageFromPath(Team.LogoSource, ImageTeam2Logo); ChangeImageFromPath(Team.LogoSource, Vars.SecondaryWindow.ImageTeam2LogoView);
                TextBoxTeam2Name.Text = Team.Name; UpDownTeam2Score.Value = Team.Score; UpDownTeam2Shots.Value = Team.Shots;
            }
            else return;

        }
        private void UIUpdateListbox(ListBox UpdatedListbox, TeamSavingClass LoadClass)
        {
            UpdatedListbox.Items.Clear();
            foreach (TeamSavingClass.PlayerTeamListType ListType in LoadClass.PlayerList)
            {
                _ = UpdatedListbox.Items.Add(string.Format(ListboxFormat, ListType.Number, ListType.Name));
            }
        }
        private void UIUpdateComboBoxSelection(ComboBox RefreshedBox)
        {
            RefreshedBox.Items.Clear();
            string searchDirectory = Vars.Prefs.DefaultTeamDirectory;
            string searchPattern = "*.xml";
            TeamSavingClass LoadClass = new TeamSavingClass();
            XmlSerializer xmlSerializer = new XmlSerializer(Vars.Game.TeamManagerTeamSavingClassInstance.GetType());
            teamLoadingFilePaths.Clear();
            
            foreach (var filePath in Directory.GetFiles(searchDirectory, searchPattern))
            {
                teamLoadingFilePaths.Add(filePath);
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    LoadClass = (TeamSavingClass)xmlSerializer.Deserialize(streamReader);
                }
                RefreshedBox.Items.Add(LoadClass.TeamName);
            }
        }
        private void UIUpdateAll() // ALL UI
        {
            if (Vars.Game.StopwatchPeriod.Elapsed > Vars.Game.LastSetTime)
            {
                ButtonPauseTime.Content = "Start Time";
            }

            if (Vars.Game.StopwatchPeriod.IsRunning)
            {
                ButtonPauseTime.Content = "Pause Time";
                UIUpdatePenaltyMainWindow();
                UIUpdatePenaltySecondaryWindow();
            }
            if (Vars.Game.StopwatchPeriod.IsRunning == false)
            {
                ButtonPauseTime.Content = "Start Time";
            }

            if (Vars.Team1.TimeoutRunning)
            {
                LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush1;
                Vars.SecondaryWindow.LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush1;
            }
            else
            {
                LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush2;
                Vars.SecondaryWindow.LabelTimeoutRunningIndicatorTeam1.Foreground = ColorBrush2;
                if (Vars.Team1.HasTimeout)
                {
                    LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Hidden;
                    Vars.SecondaryWindow.LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Hidden;
                }
                else
                {
                    LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Visible;
                    Vars.SecondaryWindow.LabelTimeoutAvailableIndicatorTeam1.Visibility = Visibility.Visible;
                }
            }
            if (Vars.Team2.TimeoutRunning)
            {
                LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush1;
                Vars.SecondaryWindow.LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush1;
            }
            else
            {
                LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush2;
                Vars.SecondaryWindow.LabelTimeoutRunningIndicatorTeam2.Foreground = ColorBrush2;
                if (Vars.Team2.HasTimeout)
                {
                    LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Hidden;
                    Vars.SecondaryWindow.LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Hidden;
                }
                else
                {
                    LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Visible;
                    Vars.SecondaryWindow.LabelTimeoutAvailableIndicatorTeam2.Visibility = Visibility.Visible;
                }
            }
            switch (Vars.Game.GameState)
            {
                case CustomTypes.GameState.Regular:
                    LabelTimeText.Content = "Game"; Vars.SecondaryWindow.LabelTimeText.Content = "Game";
                    ButtonBreakMode.Content = "Enter Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonTimeout.IsEnabled = true;
                    ButtonBreakMode.IsEnabled = true;
                    break;
                case CustomTypes.GameState.Break:
                    LabelTimeText.Content = "Break"; Vars.SecondaryWindow.LabelTimeText.Content = "Break";
                    ButtonBreakMode.Content = "Leave Break Mode";
                    ButtonTimeout.Content = "Timeout";
                    ButtonBreakMode.IsEnabled = true;
                    ButtonTimeout.IsEnabled = false;
                    break;
                case CustomTypes.GameState.Timeout:
                    LabelTimeText.Content = "Timeout"; Vars.SecondaryWindow.LabelTimeText.Content = "Timeout";
                    ButtonBreakMode.Content = "Enter Break Mode";
                    ButtonTimeout.Content = "Cancel Timeout";
                    ButtonBreakMode.IsEnabled = false;
                    ButtonTimeout.IsEnabled = true;
                    break;
            }
            UIUpdateGameTime();
        }

        // PENALTY MANAGEMENT
        private string PenaltySelectPlayer(TeamClass Team, ListBox Listbox)
        {

            if (Team.SelectedTeamList.Count != 0 && Listbox.SelectedItems.Count != 0)
            {
                int SelectedIndex = Listbox.SelectedIndex;
                Listbox.SelectedIndex = -1;
                return Team.SelectedTeamList[SelectedIndex].Number;
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
        private void PenaltySet(TeamClass Team, TeamClass OtherTeam, bool AppliesToPlayer1, TimeSpan TimeSet, string PlayerNumber, bool IsDoubleMinor, bool IsMinor)
        {
            switch (AppliesToPlayer1)
            {
                case true:
                    Team.Player1.Number = PlayerNumber;
                    Team.Player1.PenaltyRunning = true;
                    Team.Player1.PeriodIsDoubleMinor = IsDoubleMinor;
                    Team.Player1.PeriodIsMinor = IsMinor;
                    Team.Player1.OtherTeamScoreAtPenaltyStart = OtherTeam.Score;
                    Team.Player1.PenaltyTimeSet = TimeSet;
                    Team.Player1.PenaltyTimeLeft = TimeSet;
                    Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed;
                    break;
                case false:
                    Team.Player2.Number = PlayerNumber;
                    Team.Player2.PenaltyRunning = true;
                    Team.Player2.PeriodIsDoubleMinor = IsDoubleMinor;
                    Team.Player2.PeriodIsMinor = IsMinor;
                    Team.Player2.OtherTeamScoreAtPenaltyStart = OtherTeam.Score;
                    Team.Player2.PenaltyTimeSet = TimeSet;
                    Team.Player2.PenaltyTimeLeft = TimeSet;
                    Team.Player2.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed;
                    
                    break;
            }

            UIUpdatePenaltyMainWindow(); UIUpdatePenaltySecondaryWindow();
        }
        private void PenaltyAssignToRightPlayer(TeamClass Team, TeamClass OtherTeam, ListBox Listbox, TimeSpan TimeSet, bool IsDoubleMinor, bool IsMinor)
        {
            if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                PenaltySet(Team, OtherTeam, true, TimeSet, PenaltySelectPlayer(Team, Listbox), IsDoubleMinor, IsMinor);
                PenaltyMatchMilisecondOffset(Team, true);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == false)
            {
                PenaltySet(Team, OtherTeam, false, TimeSet, PenaltySelectPlayer(Team, Listbox), IsDoubleMinor, IsMinor);
                PenaltyMatchMilisecondOffset(Team, false);
            }
            else if (Team.Player1.PenaltyRunning == true && Team.Player2.PenaltyRunning == true)
            {
                _ = MessageBox.Show("All penalty slots occupied, cancel running penalties to free up space.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Team.Player1.PenaltyRunning == false && Team.Player2.PenaltyRunning == false)
            {
                PenaltySet(Team, OtherTeam, true, TimeSet, PenaltySelectPlayer(Team, Listbox), IsDoubleMinor, IsMinor);
                PenaltyMatchMilisecondOffset(Team, true);
            }
        }
        private void PenaltyCancel(TeamClass Team, bool AppliesToPlayer1)
        {
            switch (AppliesToPlayer1)
            {
                case true:
                    Team.Player1.Number = String.Empty;
                    Team.Player1.PenaltyRunning = false;
                    Team.Player1.PeriodIsDoubleMinor = false;
                    Team.Player1.PeriodIsMinor = false;
                    Team.Player1.PenaltyOffset = TimeSpan.Zero;
                    Team.Player1.PenaltyTimeLeft = TimeSpan.Zero;
                    Team.Player1.PenaltyTimeSet = TimeSpan.Zero;
                    break;
                case false:
                    Team.Player2.Number = String.Empty;
                    Team.Player2.PenaltyRunning = false;
                    Team.Player2.PeriodIsDoubleMinor = false;
                    Team.Player1.PeriodIsMinor = false;
                    Team.Player2.PenaltyOffset = TimeSpan.Zero; 
                    Team.Player2.PenaltyTimeLeft = TimeSpan.Zero; 
                    Team.Player2.PenaltyTimeSet = TimeSpan.Zero;
                    PenaltyMoveDownASlot(Team);
                    break;
            }
            UIUpdatePenaltyMainWindow(); UIUpdatePenaltySecondaryWindow();
        }
        private void PenaltyMatchMilisecondOffset(TeamClass Team, bool AppliesToPlayer1)
        {
            if (Team.Player1.PenaltyOffset.Milliseconds != Vars.Game.StopwatchPeriod.Elapsed.Milliseconds && AppliesToPlayer1 == true)
            {
                Team.Player1.PenaltyOffset = TimeSpan.FromHours(Team.Player1.PenaltyOffset.Hours) + TimeSpan.FromMinutes(Team.Player1.PenaltyOffset.Minutes) + TimeSpan.FromSeconds(Team.Player1.PenaltyOffset.Seconds) + TimeSpan.FromMilliseconds(Vars.Game.StopwatchPeriod.Elapsed.Milliseconds);
            }
            if (Team.Player2.PenaltyOffset.Milliseconds != Vars.Game.StopwatchPeriod.Elapsed.Milliseconds && AppliesToPlayer1 == false)
            {
                Team.Player2.PenaltyOffset = TimeSpan.FromHours(Team.Player2.PenaltyOffset.Hours) + TimeSpan.FromMinutes(Team.Player2.PenaltyOffset.Minutes) + TimeSpan.FromSeconds(Team.Player2.PenaltyOffset.Seconds) + TimeSpan.FromMilliseconds(Vars.Game.StopwatchPeriod.Elapsed.Milliseconds);
            }
        }
        private void PenaltyTimeProgression(TeamClass Team, TeamClass OtherTeam, bool AppliesToPlayer1)
        {
            switch (AppliesToPlayer1)
            {
                case true:
                    if (Team.Player1.PenaltyRunning)
                    {
                        
                        if (Team.Player1.PeriodIsDoubleMinor == true)
                        {
                            if (OtherTeam.Score > Team.Player1.OtherTeamScoreAtPenaltyStart)
                            {
                                if (Team.Player1.PenaltyTimeLeft < TimeSpan.FromMinutes(2))
                                {
                                    Team.Player1.PeriodIsDoubleMinor = false;
                                }
                                else
                                {
                                    Team.Player1.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player1.PenaltyTimeSet = TimeSpan.FromMinutes(2);
                                    Team.Player1.PeriodIsDoubleMinor = false; Team.Player1.PenaltyTimeLeft = TimeSpan.FromMinutes(2);
                                }
                            }
                            else
                            {
                                PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                                Team.Player1.PenaltyTimeLeft = Team.Player1.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset);
                            }
                        }
                        else if (Team.Player1.PeriodIsMinor == true)
                        {
                            if (OtherTeam.Score > Team.Player1.OtherTeamScoreAtPenaltyStart)
                            {
                                PenaltyCancel(Team, true);
                            }
                            else
                            {
                                PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                                Team.Player1.PenaltyTimeLeft = Team.Player1.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset);
                            }
                        }
                        else
                        {
                            PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                            Team.Player1.PenaltyTimeLeft = Team.Player1.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset);
                        }
                    }
                    break;
                case false:
                    if (Team.Player2.PenaltyRunning)
                    {
                        if (Team.Player2.PeriodIsDoubleMinor == true)
                        {
                            if (OtherTeam.Score > Team.Player2.OtherTeamScoreAtPenaltyStart)
                            {
                                if (Team.Player2.PenaltyTimeLeft < TimeSpan.FromMinutes(2))
                                {
                                    Team.Player2.PeriodIsDoubleMinor = false;
                                }
                                else
                                {
                                    Team.Player2.PenaltyOffset = Vars.Game.StopwatchPeriod.Elapsed; Team.Player2.PenaltyTimeSet = TimeSpan.FromMinutes(2);
                                    Team.Player2.PeriodIsDoubleMinor = false; Team.Player2.PenaltyTimeLeft = TimeSpan.FromMinutes(2);
                                }
                            }
                            else
                            {
                                PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                                Team.Player2.PenaltyTimeLeft = Team.Player2.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset);
                            }
                        }
                        else if (Team.Player2.PeriodIsMinor == true)
                        {
                            if (OtherTeam.Score > Team.Player2.OtherTeamScoreAtPenaltyStart)
                            {
                                PenaltyCancel(Team, false);
                            }
                            else
                            {
                                PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                                Team.Player2.PenaltyTimeLeft = Team.Player2.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset);
                            }
                        }
                        else
                        {
                            PenaltyMatchMilisecondOffset(Team, AppliesToPlayer1);
                            Team.Player2.PenaltyTimeLeft = Team.Player2.PenaltyTimeSet - (Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset);
                        }
                    }
                    break;
            }
        }
        private void PenaltyStopIfRanOutOfTime(TeamClass Team, bool Player1)
        {
            switch (Player1)
            {
                case true:
                    if ((Vars.Game.StopwatchPeriod.Elapsed - Team.Player1.PenaltyOffset) > Team.Player1.PenaltyTimeSet)
                    {
                        PenaltyCancel(Team, true);
                    }
                    break;
                case false:
                    if ((Vars.Game.StopwatchPeriod.Elapsed - Team.Player2.PenaltyOffset) > Team.Player2.PenaltyTimeSet)
                        PenaltyCancel(Team, false);
                    break;
            }

        }
        private void PenaltyMoveDownASlot(TeamClass Team)
        {
            if (Team.Player2.PenaltyRunning == true && Team.Player1.PenaltyRunning == false)
            {
                Team.Player1.PeriodIsDoubleMinor = Team.Player2.PeriodIsDoubleMinor;
                Team.Player2.PeriodIsDoubleMinor = false;
                Team.Player1.PeriodIsMinor = Team.Player2.PeriodIsMinor;
                Team.Player2.PeriodIsMinor = false;
                Team.Player1.OtherTeamScoreAtPenaltyStart = Team.Player2.OtherTeamScoreAtPenaltyStart;
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
        // TEAM MANAGEMENT
        private void TeamEditorAddPlayer(string InputName, string InputNumber)
        {
            TeamSavingClass.PlayerTeamListType TempPlayer = new TeamSavingClass.PlayerTeamListType
            {
                Name = InputName,
                Number = InputNumber
            };
            Vars.Game.TeamManagerTeamSavingClassInstance.PlayerList.Add(TempPlayer); UIUpdateListbox(ListBoxTeamManager, Vars.Game.TeamManagerTeamSavingClassInstance);
        }
        private void TeamEditorRemovePlayer(ListBox EditorListBox)
        {
            try 
            { 
                Vars.Game.TeamManagerTeamSavingClassInstance.PlayerList.RemoveAt(EditorListBox.SelectedIndex); UIUpdateListbox(ListBoxTeamManager, Vars.Game.TeamManagerTeamSavingClassInstance);
            }
            catch 
            {
                MessageBox.Show("No player selected. Unable to remove.");
                return; 
            }
        }
        private void TeamEditorClearPlayerList()
        {
            Vars.Game.TeamManagerTeamSavingClassInstance.PlayerList.Clear(); UIUpdateListbox(ListBoxTeamManager, Vars.Game.TeamManagerTeamSavingClassInstance); ;
        }
        private void TeamEditorSave(string InputTeamName)
        {
            Vars.Game.TeamManagerTeamSavingClassInstance.TeamName = InputTeamName;
            Directory.CreateDirectory(Vars.Prefs.DefaultTeamDirectory);
            SaveFileDialog SaveTeamDialog = DefineSaveXmlDialog();
            SaveTeamDialog.FileName = InputTeamName;
            Nullable<bool> Result = SaveTeamDialog.ShowDialog();
            XmlSerializer xmlSerializer = new XmlSerializer(Vars.Game.TeamManagerTeamSavingClassInstance.GetType());
            if (Result == true)
            {
                using (StreamWriter streamWriter = new StreamWriter(SaveTeamDialog.FileName))
                {

                    xmlSerializer.Serialize(streamWriter, Vars.Game.TeamManagerTeamSavingClassInstance);
                }
            }
        }
        private void TeamEditorLoad()
        {
            TeamSavingClass LoadClass = new TeamSavingClass();
            OpenFileDialog LoadTeamDialog = DefineLoadXmlDialog();
            Nullable<bool> Result = LoadTeamDialog.ShowDialog();
            XmlSerializer xmlSerializer = new XmlSerializer(Vars.Game.TeamManagerTeamSavingClassInstance.GetType());
            if (Result == true)
            {
                using (StreamReader streamReader = new StreamReader(LoadTeamDialog.FileName))
                {
                    LoadClass = (TeamSavingClass)xmlSerializer.Deserialize(streamReader);
                }
                Vars.Game.TeamManagerTeamSavingClassInstance = LoadClass;
                BitmapImage bitmap = new BitmapImage(new Uri(Vars.Game.TeamManagerTeamSavingClassInstance.TeamLogoPath))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
                ImageTeamManagerLogo.Source = bitmap;
                TextBoxTeamNameManager.Text = Vars.Game.TeamManagerTeamSavingClassInstance.TeamName;
                UIUpdateListbox(ListBoxTeamManager, Vars.Game.TeamManagerTeamSavingClassInstance); ;
            }
        }
        private void TeamEditorLoadTeamFromComboBox(ListBox OutputListBox, ComboBox InputComboBox, TeamClass Team)
        {
            MessageBoxResult result = MessageBox.Show("This will override your current team setup. Do you wish to continue?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                OutputListBox.Items.Clear();
                TeamSavingClass LoadClass = new TeamSavingClass();
                XmlSerializer Xser = new XmlSerializer(Vars.Game.TeamManagerTeamSavingClassInstance.GetType());
                using (StreamReader streamReader = new StreamReader(teamLoadingFilePaths[InputComboBox.SelectedIndex]))
                {
                    LoadClass = (TeamSavingClass)Xser.Deserialize(streamReader);
                }

                Team.Name = LoadClass.TeamName;
                Team.LogoSource = LoadClass.TeamLogoPath;
                Team.Score = 0;
                Team.Shots = 0;
                Team.TimeoutRunning = false;
                Team.HasTimeout = true;
                Team.Player1 = new CustomTypes.PlayerType
                {
                    Number = "",
                    PenaltyTimeLeft = TimeSpan.Zero,
                    PenaltyTimeSet = TimeSpan.Zero,
                    PeriodIsDoubleMinor = false,
                    PenaltyOffset = TimeSpan.Zero,
                    PenaltyRunning = false,
                    OtherTeamScoreAtPenaltyStart = 0
                };
                Team.Player2 = new CustomTypes.PlayerType
                {
                    Number = "",
                    PenaltyTimeLeft = TimeSpan.Zero,
                    PenaltyTimeSet = TimeSpan.Zero,
                    PeriodIsDoubleMinor = false,
                    PenaltyOffset = TimeSpan.Zero,
                    PenaltyRunning = false,
                    OtherTeamScoreAtPenaltyStart = 0

                };
                Team.SelectedTeamList = LoadClass.PlayerList;
                UIUpdateListbox(OutputListBox, LoadClass);
                UIReloadControlsValues(Team); UIUpdateAll();
            }
            else return;
        }
    }
}
