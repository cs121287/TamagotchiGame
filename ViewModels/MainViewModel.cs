using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TamagotchiGame.Models;
using TamagotchiGame.Services;

namespace TamagotchiGame.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Pet _pet;
        private readonly DispatcherTimer _gameTimer;
        private readonly DispatcherTimer _autoSaveTimer;
        private string _currentSpritePath;
        private string _statusMessage;
        private bool _isInteractionEnabled;
        private readonly SoundService _soundService;
        private readonly SaveLoadService _saveLoadService;

        // Add event for pet state changes
        public event EventHandler<PetState> PetStateChanged;

        public Pet Pet
        {
            get => _pet;
            set
            {
                if (_pet != null)
                {
                    // Unsubscribe from old pet's PropertyChanged event
                    _pet.PropertyChanged -= Pet_PropertyChanged;
                }

                _pet = value;

                if (_pet != null)
                {
                    // Subscribe to new pet's PropertyChanged event
                    _pet.PropertyChanged += Pet_PropertyChanged;
                }

                OnPropertyChanged();
            }
        }

        public string CurrentSpritePath
        {
            get => _currentSpritePath;
            set { _currentSpritePath = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool IsInteractionEnabled
        {
            get => _isInteractionEnabled;
            set { _isInteractionEnabled = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _saveLoadService = new SaveLoadService();
            _soundService = new SoundService();

            // Attempt to load existing pet or create new one
            LoadPet();

            // Set up interaction enabled state
            IsInteractionEnabled = true;

            // Initialize sound effects
            InitializeSounds();

            // Set up game timer to update every second
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _gameTimer.Tick += GameTimerTick;
            _gameTimer.Start();

            // Set up auto-save timer
            _autoSaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(GameSettings.Instance?.AutoSaveIntervalMinutes ?? 5)
            };
            _autoSaveTimer.Tick += AutoSaveTimerTick;

            if (GameSettings.Instance?.AutoSave ?? true)
            {
                _autoSaveTimer.Start();
            }

            // Set a default path for the Image control (this won't be used for animation)
            CurrentSpritePath = "/Assets/Images/idle.png";
            UpdateStatusMessage();
        }

        private void LoadPet()
        {
            Task.Run(async () =>
            {
                try
                {
                    Pet = await _saveLoadService.LoadPetAsync();
                    StatusMessage = "Pet loaded successfully!";
                }
                catch
                {
                    Pet = new Pet();
                    StatusMessage = "Created a new pet!";
                }
            });
        }

        private void InitializeSounds()
        {
            if (_soundService == null)
                return;

            try
            {
                _soundService.LoadSoundEffect("feed", "Assets/Sounds/feed.wav");
                _soundService.LoadSoundEffect("play", "Assets/Sounds/play.wav");
                _soundService.LoadSoundEffect("sleep", "Assets/Sounds/sleep.wav");
                _soundService.LoadSoundEffect("medicine", "Assets/Sounds/medicine.wav");
                _soundService.LoadSoundEffect("happy", "Assets/Sounds/happy.wav");
                _soundService.LoadSoundEffect("sick", "Assets/Sounds/sick.wav");

                if (GameSettings.Instance?.PlayMusic ?? true)
                {
                    _soundService.LoadBackgroundMusic("Assets/Sounds/background_music.mp3");
                    _soundService.PlayBackgroundMusic();
                }

                // Apply volume settings
                _soundService.SoundEffectVolume = GameSettings.Instance?.SoundVolume ?? 0.8f;
                _soundService.MusicVolume = GameSettings.Instance?.MusicVolume ?? 0.5f;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing sounds: {ex.Message}");
            }
        }

        private void Pet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check if the CurrentState property changed
            if (e.PropertyName == nameof(Pet.CurrentState))
            {
                // Notify subscribers about the state change
                PetStateChanged?.Invoke(this, Pet.CurrentState);
                UpdateStatusMessage();
            }
        }

        private void GameTimerTick(object sender, EventArgs e)
        {
            Pet.UpdateState();
            UpdateStatusMessage();

            // Check for critical conditions and notify
            CheckCriticalConditions();
        }

        private void AutoSaveTimerTick(object sender, EventArgs e)
        {
            if (GameSettings.Instance?.AutoSave ?? true)
            {
                SavePet();
            }
        }

        private void UpdateStatusMessage()
        {
            StatusMessage = Pet.CurrentState switch
            {
                PetState.Idle => "Your pet is feeling fine.",
                PetState.Eating => "Yum! Your pet is eating.",
                PetState.Playing => "Your pet is having fun!",
                PetState.Sleeping => "Shh! Your pet is sleeping.",
                PetState.Hungry => "Your pet is hungry! Time to feed.",
                PetState.Sick => "Oh no! Your pet is sick. Give medicine.",
                PetState.Tired => "Your pet is tired. Time for a nap.",
                _ => "Your pet is feeling fine."
            };
        }

        private void CheckCriticalConditions()
        {
            if (!(GameSettings.Instance?.ShowNotifications ?? true))
                return;

            if (Pet.Health < 20)
            {
                ShowNotification("Health Critical!", "Your pet needs medicine now!");
                _soundService?.PlaySoundEffect("sick");
            }
            else if (Pet.Hunger > 90)
            {
                ShowNotification("Very Hungry!", "Your pet is starving!");
            }
            else if (Pet.Happiness < 10)
            {
                ShowNotification("Very Unhappy!", "Your pet needs attention!");
            }
        }

        private void ShowNotification(string title, string message)
        {
            // This would integrate with your OS's notification system
            // For now, just show a message box if we're not in design mode
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                });
            }
        }

        // Implement Feed, Play, Sleep, Medicine methods
        public void Feed()
        {
            if (!IsInteractionEnabled)
                return;

            Pet.Feed();
            _soundService?.PlaySoundEffect("feed");

            // Temporary disable interaction to prevent spam
            TemporarilyDisableInteraction();
        }

        public void Play()
        {
            if (!IsInteractionEnabled)
                return;

            Pet.Play();
            _soundService?.PlaySoundEffect("play");

            // Temporary disable interaction to prevent spam
            TemporarilyDisableInteraction();
        }

        public void Sleep()
        {
            if (!IsInteractionEnabled)
                return;

            Pet.Sleep();
            _soundService?.PlaySoundEffect("sleep");

            // Temporary disable interaction for longer
            TemporarilyDisableInteraction(5);
        }

        public void Medicine()
        {
            if (!IsInteractionEnabled)
                return;

            Pet.Medicine();
            _soundService?.PlaySoundEffect("medicine");

            // Temporary disable interaction to prevent spam
            TemporarilyDisableInteraction();
        }

        private void TemporarilyDisableInteraction(int seconds = 2)
        {
            IsInteractionEnabled = false;

            Task.Delay(TimeSpan.FromSeconds(seconds))
                .ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsInteractionEnabled = true;
                    });
                });
        }

        public void SavePet()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _saveLoadService.SavePetAsync(Pet);
                    StatusMessage = "Game saved!";

                    // Automatically clear the message after a short delay
                    await Task.Delay(2000);

                    // Only update if the status message hasn't been changed by something else
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (StatusMessage == "Game saved!")
                        {
                            UpdateStatusMessage();
                        }
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = "Failed to save game!";
                    Console.WriteLine($"Error saving pet: {ex.Message}");
                }
            });
        }

        public void UpdateSettings()
        {
            // Update timers based on settings
            if (GameSettings.Instance?.AutoSave ?? true)
            {
                _autoSaveTimer.Interval = TimeSpan.FromMinutes(GameSettings.Instance?.AutoSaveIntervalMinutes ?? 5);
                _autoSaveTimer.Start();
            }
            else
            {
                _autoSaveTimer.Stop();
            }

            // Update sound settings
            if (_soundService != null)
            {
                _soundService.SoundEnabled = GameSettings.Instance?.PlaySounds ?? true;
                _soundService.MusicEnabled = GameSettings.Instance?.PlayMusic ?? true;
                _soundService.SoundEffectVolume = GameSettings.Instance?.SoundVolume ?? 0.8f;
                _soundService.MusicVolume = GameSettings.Instance?.MusicVolume ?? 0.5f;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}