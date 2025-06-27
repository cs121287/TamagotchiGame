using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TamagotchiGame.Models
{
    public class GameSettings
    {
        private const string SettingsFileName = "game_settings.json";
        
        // Game settings
        public bool AutoSave { get; set; } = true;
        public int AutoSaveIntervalMinutes { get; set; } = 5;
        public bool PlaySounds { get; set; } = true;
        public bool PlayMusic { get; set; } = true;
        public float SoundVolume { get; set; } = 0.8f;
        public float MusicVolume { get; set; } = 0.5f;
        public bool RunInBackground { get; set; } = false;
        public bool ShowNotifications { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        
        // UI settings
        public string Theme { get; set; } = "Default";
        public bool ShowTips { get; set; } = true;
        
        // Static instance for singleton pattern
        private static GameSettings _instance;
        
        public static GameSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadSettings().GetAwaiter().GetResult();
                }
                return _instance;
            }
        }
        
        private GameSettings() { }
        
        private static async Task<GameSettings> LoadSettings()
        {
            try
            {
                string settingsPath = GetSettingsFilePath();
                
                if (!File.Exists(settingsPath))
                {
                    return new GameSettings();
                }
                
                string jsonString = await File.ReadAllTextAsync(settingsPath);
                var settings = JsonSerializer.Deserialize<GameSettings>(jsonString);
                
                return settings ?? new GameSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                return new GameSettings();
            }
        }
        
        public async Task SaveSettingsAsync()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                string settingsPath = GetSettingsFilePath();
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                
                await File.WriteAllTextAsync(settingsPath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
        
        private static string GetSettingsFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TamagotchiGame",
                SettingsFileName);
        }
        
        public static void ReloadSettings()
        {
            _instance = LoadSettings().GetAwaiter().GetResult();
        }
    }
}