using System;
using System.Windows;

namespace HockeyScoreboard
{
    /// <summary>
    /// Interaction logic for TimeoutDialog.xaml
    /// </summary>
    public partial class TimeoutDialog : Window
    {
        public TimeoutDialog()
        {
            InitializeComponent();
        }

        private bool AnyButtonPressed;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AnyButtonPressed = false;
            ButtonSelectTeam1.Content = Vars.Team1.Name;
            ButtonSelectTeam2.Content = Vars.Team2.Name;
            LabelMessage.Content = $"Timeout length: (default {Properties.Settings.Default.TimeoutDuration.Minutes}:{Properties.Settings.Default.TimeoutDuration.Seconds})";
            UpDownMinutes.Value = Properties.Settings.Default.TimeoutDuration.Minutes;
            UpDownSeconds.Value = Properties.Settings.Default.TimeoutDuration.Seconds;
            if (Vars.Team1.HasTimeout == false)
            {
                ButtonSelectTeam1.IsEnabled = false;
            }
            if (Vars.Team2.HasTimeout == false)
            {
                ButtonSelectTeam2.IsEnabled = false;
            }
        }

        private void ButtonSelectTeam1_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan TimeoutTime = TimeSpan.FromMinutes((int)UpDownMinutes.Value) + TimeSpan.FromSeconds((int)UpDownSeconds.Value);
            Vars.Team1.HasTimeout = false; Vars.Team1.TimeoutRunning = true;
            Vars.Game.GameState = CustomTypes.GameState.Timeout; AnyButtonPressed = true;
            Vars.Game.TimeLeft = TimeoutTime; Vars.Game.LastSetTime = TimeoutTime;
            this.Close();
        }

        private void ButtonSelectTeam2_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan TimeoutTime = TimeSpan.FromMinutes((int)UpDownMinutes.Value) + TimeSpan.FromSeconds((int)UpDownSeconds.Value);
            Vars.Team2.HasTimeout = false; Vars.Team2.TimeoutRunning = true;
            Vars.Game.GameState = CustomTypes.GameState.Timeout; AnyButtonPressed = true;
            Vars.Game.TimeLeft = TimeoutTime; Vars.Game.LastSetTime = TimeoutTime;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (AnyButtonPressed == false)
            {
                Vars.Game.GameState = CustomTypes.GameState.Regular;
            }
        }
    }
}