using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows.Media;

namespace TamagotchiGame.Services
{
    public class SoundService
    {
        private readonly Dictionary<string, SoundPlayer> _soundEffects;
        private readonly MediaPlayer _backgroundMusicPlayer;
        
        private float _soundEffectVolume = 1.0f;
        private float _musicVolume = 0.5f;
        private bool _soundEnabled = true;
        private bool _musicEnabled = true;
        
        public float SoundEffectVolume
        {
            get => _soundEffectVolume;
            set
            {
                _soundEffectVolume = Math.Clamp(value, 0f, 1f);
                // Note: SoundPlayer doesn't support volume control directly
            }
        }
        
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Clamp(value, 0f, 1f);
                _backgroundMusicPlayer.Volume = _musicVolume;
            }
        }
        
        public bool SoundEnabled
        {
            get => _soundEnabled;
            set => _soundEnabled = value;
        }
        
        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                if (_musicEnabled)
                {
                    _backgroundMusicPlayer.Play();
                }
                else
                {
                    _backgroundMusicPlayer.Pause();
                }
            }
        }
        
        public SoundService()
        {
            _soundEffects = new Dictionary<string, SoundPlayer>();
            _backgroundMusicPlayer = new MediaPlayer();
            
            // Initialize with default volume
            _backgroundMusicPlayer.Volume = _musicVolume;
        }
        
        public void LoadSoundEffect(string soundName, string filePath)
        {
            try
            {
                if (_soundEffects.ContainsKey(soundName))
                {
                    _soundEffects[soundName].Dispose();
                }
                
                _soundEffects[soundName] = new SoundPlayer(filePath);
                _soundEffects[soundName].LoadAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sound effect '{soundName}': {ex.Message}");
            }
        }
        
        public void PlaySoundEffect(string soundName)
        {
            if (!_soundEnabled || !_soundEffects.ContainsKey(soundName))
                return;
            
            try
            {
                _soundEffects[soundName].Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound effect '{soundName}': {ex.Message}");
            }
        }
        
        public void LoadBackgroundMusic(string filePath)
        {
            try
            {
                _backgroundMusicPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
                _backgroundMusicPlayer.MediaEnded += (s, e) =>
                {
                    _backgroundMusicPlayer.Position = TimeSpan.Zero;
                    _backgroundMusicPlayer.Play();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading background music: {ex.Message}");
            }
        }
        
        public void PlayBackgroundMusic()
        {
            if (_musicEnabled)
            {
                _backgroundMusicPlayer.Play();
            }
        }
        
        public void PauseBackgroundMusic()
        {
            _backgroundMusicPlayer.Pause();
        }
        
        public void StopBackgroundMusic()
        {
            _backgroundMusicPlayer.Stop();
        }
        
        public void Dispose()
        {
            // Clean up resources
            foreach (var soundPlayer in _soundEffects.Values)
            {
                soundPlayer.Dispose();
            }
            
            _soundEffects.Clear();
            _backgroundMusicPlayer.Close();
        }
    }
}