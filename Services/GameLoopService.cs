using System;
using System.Windows.Threading;

namespace TamagotchiGame.Services
{
    public class GameLoopService
    {
        private readonly DispatcherTimer _gameTimer;
        private DateTime _lastUpdateTime;
        
        public event EventHandler<TimeSpan> Update;
        
        public bool IsRunning => _gameTimer.IsEnabled;
        
        public GameLoopService(double updatesPerSecond = 30)
        {
            double intervalSeconds = 1.0 / updatesPerSecond;
            
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(intervalSeconds)
            };
            
            _gameTimer.Tick += GameTimerTick;
            _lastUpdateTime = DateTime.Now;
        }
        
        public void Start()
        {
            _lastUpdateTime = DateTime.Now;
            _gameTimer.Start();
        }
        
        public void Stop()
        {
            _gameTimer.Stop();
        }
        
        private void GameTimerTick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan deltaTime = currentTime - _lastUpdateTime;
            _lastUpdateTime = currentTime;
            
            // Trigger the update event with the time since last update
            Update?.Invoke(this, deltaTime);
        }
    }
}