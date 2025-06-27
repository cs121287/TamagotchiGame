using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TamagotchiGame.Models
{
    public class Pet : INotifyPropertyChanged
    {
        private string _name;
        private int _hunger;
        private int _happiness;
        private int _health;
        private int _energy;
        private int _age;
        private PetState _currentState;
        private DateTime _lastUpdateTime;
        
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        
        public int Hunger
        {
            get => _hunger;
            set { _hunger = Math.Clamp(value, 0, 100); OnPropertyChanged(); }
        }
        
        public int Happiness
        {
            get => _happiness;
            set { _happiness = Math.Clamp(value, 0, 100); OnPropertyChanged(); }
        }
        
        public int Health
        {
            get => _health;
            set { _health = Math.Clamp(value, 0, 100); OnPropertyChanged(); }
        }
        
        public int Energy
        {
            get => _energy;
            set { _energy = Math.Clamp(value, 0, 100); OnPropertyChanged(); }
        }
        
        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }
        
        public PetState CurrentState
        {
            get => _currentState;
            set { _currentState = value; OnPropertyChanged(); }
        }
        
        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set { _lastUpdateTime = value; OnPropertyChanged(); }
        }
        
        public Pet(string name = "Tama")
        {
            Name = name;
            Hunger = 50;
            Happiness = 50;
            Health = 100;
            Energy = 100;
            Age = 0;
            CurrentState = PetState.Idle;
            LastUpdateTime = DateTime.Now;
        }
        
        public void Feed()
        {
            Hunger -= 30;
            Energy += 10;
            CurrentState = PetState.Eating;
        }
        
        public void Play()
        {
            Happiness += 20;
            Energy -= 15;
            Hunger += 5;
            CurrentState = PetState.Playing;
        }
        
        public void Sleep()
        {
            Energy += 40;
            CurrentState = PetState.Sleeping;
        }
        
        public void Medicine()
        {
            Health += 30;
            Happiness -= 10;
            CurrentState = PetState.Sick;
        }
        
        public void UpdateState()
        {
            var timeSinceLastUpdate = DateTime.Now - LastUpdateTime;
            
            // Determine minutes passed and update stats accordingly
            var minutesPassed = timeSinceLastUpdate.TotalMinutes;
            
            // Basic stat decay over time
            Hunger += (int)(minutesPassed * 2);
            Energy -= (int)(minutesPassed * 1);
            Happiness -= (int)(minutesPassed * 1.5);
            
            // Health decreases if hunger or happiness is too low
            if (Hunger > 80 || Happiness < 20)
            {
                Health -= (int)(minutesPassed * 2);
            }
            
            // Update age (1 day per real hour)
            Age += (int)(timeSinceLastUpdate.TotalHours);
            
            // Update state based on stats
            if (Health < 30)
                CurrentState = PetState.Sick;
            else if (Energy < 20)
                CurrentState = PetState.Tired;
            else if (Hunger > 80)
                CurrentState = PetState.Hungry;
            else if (CurrentState != PetState.Playing && CurrentState != PetState.Eating && CurrentState != PetState.Sleeping)
                CurrentState = PetState.Idle;
                
            LastUpdateTime = DateTime.Now;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public enum PetState
    {
        Idle,
        Eating,
        Playing,
        Sleeping,
        Hungry,
        Sick,
        Tired
    }
}