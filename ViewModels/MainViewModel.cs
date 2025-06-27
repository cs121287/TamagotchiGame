using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using TamagotchiGame.Models;

namespace TamagotchiGame.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Pet _pet;
        private readonly DispatcherTimer _gameTimer;
        private string _currentSpritePath;
        
        public Pet Pet
        {
            get => _pet;
            set { _pet = value; OnPropertyChanged(); }
        }
        
        public string CurrentSpritePath
        {
            get => _currentSpritePath;
            set { _currentSpritePath = value; OnPropertyChanged(); }
        }
        
        public MainViewModel()
        {
            Pet = new Pet();
            
            // Set up game timer to update every second
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _gameTimer.Tick += GameTimerTick;
            _gameTimer.Start();
            
            UpdateSprite();
        }
        
        private void GameTimerTick(object sender, EventArgs e)
        {
            Pet.UpdateState();
            UpdateSprite();
        }
        
        private void UpdateSprite()
        {
            // In a real implementation, you'd have different sprites for different states
            // This is a placeholder for the sprite selection logic
            CurrentSpritePath = Pet.CurrentState switch
            {
                PetState.Idle => "/Assets/Images/idle.png",
                PetState.Eating => "/Assets/Images/eating.png",
                PetState.Playing => "/Assets/Images/playing.png",
                PetState.Sleeping => "/Assets/Images/sleeping.png",
                PetState.Hungry => "/Assets/Images/hungry.png",
                PetState.Sick => "/Assets/Images/sick.png",
                PetState.Tired => "/Assets/Images/tired.png",
                _ => "/Assets/Images/idle.png"
            };
        }
        
        public void Feed()
        {
            Pet.Feed();
            UpdateSprite();
        }
        
        public void Play()
        {
            Pet.Play();
            UpdateSprite();
        }
        
        public void Sleep()
        {
            Pet.Sleep();
            UpdateSprite();
        }
        
        public void Medicine()
        {
            Pet.Medicine();
            UpdateSprite();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}