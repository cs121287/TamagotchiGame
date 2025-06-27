using System.Windows;
using TamagotchiGame.ViewModels;
using TamagotchiGame.Services;

namespace TamagotchiGame
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private SpriteAnimator _spriteAnimator;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Initialize sprite animator after the PetImage is created
            _spriteAnimator = new SpriteAnimator(PetImage);

            // Subscribe to sprite state changes
            _viewModel.PetStateChanged += (sender, state) =>
            {
                _spriteAnimator.UpdateAnimation(state);
            };

            // Initial animation based on current state
            _spriteAnimator.UpdateAnimation(_viewModel.Pet.CurrentState);
        }

        private void Feed_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Feed();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Play();
        }

        private void Sleep_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Sleep();
        }

        private void Medicine_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Medicine();
        }
    }
}