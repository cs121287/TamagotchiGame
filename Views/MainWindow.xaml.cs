using System.Windows;
using TamagotchiGame.ViewModels;

namespace TamagotchiGame
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        
        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
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