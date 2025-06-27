using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using TamagotchiGame.Services;
using TamagotchiGame.Models;

namespace TamagotchiGame
{
    public partial class App : Application
    {
        private SaveLoadService _saveLoadService;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize services
            _saveLoadService = new SaveLoadService();
            
            // Set up global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            
            // Create application data directory if it doesn't exist
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TamagotchiGame");
                
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            // Perform any cleanup or final save operations
            if (MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.DataContext is ViewModels.MainViewModel viewModel)
                {
                    // Save the pet's state when the application exits
                    _saveLoadService.SavePetAsync(viewModel.Pet).Wait();
                }
            }
            
            base.OnExit(e);
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError(e.ExceptionObject as Exception);
            MessageBox.Show($"An unexpected error occurred: {(e.ExceptionObject as Exception)?.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        
        private void LogError(Exception ex)
        {
            if (ex == null)
                return;
                
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TamagotchiGame", 
                    "error_log.txt");
                    
                string logEntry = $"[{DateTime.Now}] {ex.GetType()}: {ex.Message}\n{ex.StackTrace}\n\n";
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // If logging fails, there's not much we can do
            }
        }
    }
}