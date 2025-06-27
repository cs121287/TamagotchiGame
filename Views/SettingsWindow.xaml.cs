using System;
using System.Windows;
using TamagotchiGame.Models;

namespace TamagotchiGame.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly GameSettings _settings;
        
        public SettingsWindow()
        {
            InitializeComponent();
            
            _settings = GameSettings.Instance;
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            // Game settings
            AutoSaveCheckBox.IsChecked = _settings.AutoSave;
            AutoSaveIntervalComboBox.SelectedIndex = GetIntervalIndex(_settings.AutoSaveIntervalMinutes);
            RunInBackgroundCheckBox.IsChecked = _settings.RunInBackground;
            ShowNotificationsCheckBox.IsChecked = _settings.ShowNotifications;
            StartWithWindowsCheckBox.IsChecked = _settings.StartWithWindows;
            
            // Audio settings
            PlaySoundsCheckBox.IsChecked = _settings.PlaySounds;
            SoundVolumeSlider.Value = _settings.SoundVolume;
            PlayMusicCheckBox.IsChecked = _settings.PlayMusic;
            MusicVolumeSlider.Value = _settings.MusicVolume;
            
            // UI settings
            ThemeComboBox.SelectedIndex = GetThemeIndex(_settings.Theme);
            ShowTipsCheckBox.IsChecked = _settings.ShowTips;
        }
        
        private int GetIntervalIndex(int minutes)
        {
            return minutes switch
            {
                1 => 0,
                5 => 1,
                10 => 2,
                15 => 3,
                30 => 4,
                60 => 5,
                _ => 1 // Default to 5 minutes
            };
        }
        
        private int GetIntervalValue(int index)
        {
            return index switch
            {
                0 => 1,
                1 => 5,
                2 => 10,
                3 => 15,
                4 => 30,
                5 => 60,
                _ => 5 // Default to 5 minutes
            };
        }
        
        private int GetThemeIndex(string theme)
        {
            return theme switch
            {
                "Default" => 0,
                "Dark" => 1,
                "Pastel" => 2,
                "Retro" => 3,
                _ => 0 // Default theme
            };
        }
        
        private string GetThemeValue(int index)
        {
            return index switch
            {
                0 => "Default",
                1 => "Dark",
                2 => "Pastel",
                3 => "Retro",
                _ => "Default" // Default theme
            };
        }
        
        private void SaveSettings()
        {
            // Game settings
            _settings.AutoSave = AutoSaveCheckBox.IsChecked ?? true;
            _settings.AutoSaveIntervalMinutes = GetIntervalValue(AutoSaveIntervalComboBox.SelectedIndex);
            _settings.RunInBackground = RunInBackgroundCheckBox.IsChecked ?? false;
            _settings.ShowNotifications = ShowNotificationsCheckBox.IsChecked ?? true;
            _settings.StartWithWindows = StartWithWindowsCheckBox.IsChecked ?? false;
            
            // Audio settings
            _settings.PlaySounds = PlaySoundsCheckBox.IsChecked ?? true;
            _settings.SoundVolume = (float)SoundVolumeSlider.Value;
            _settings.PlayMusic = PlayMusicCheckBox.IsChecked ?? true;
            _settings.MusicVolume = (float)MusicVolumeSlider.Value;
            
            // UI settings
            _settings.Theme = GetThemeValue(ThemeComboBox.SelectedIndex);
            _settings.ShowTips = ShowTipsCheckBox.IsChecked ?? true;
        }
        
        private void DefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Reset all settings to default values?", "Confirm Reset", 
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Create new settings with default values and load them
                _settings.AutoSave = true;
                _settings.AutoSaveIntervalMinutes = 5;
                _settings.PlaySounds = true;
                _settings.PlayMusic = true;
                _settings.SoundVolume = 0.8f;
                _settings.MusicVolume = 0.5f;
                _settings.RunInBackground = false;
                _settings.ShowNotifications = true;
                _settings.StartWithWindows = false;
                _settings.Theme = "Default";
                _settings.ShowTips = true;
                
                LoadSettings();
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            _settings.SaveSettingsAsync();
            
            DialogResult = true;
            Close();
        }
    }
}