using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TamagotchiGame.Services
{
    public class AnimationService
    {
        private readonly Image _targetImage;
        private DispatcherTimer _animationTimer;
        private BitmapImage _currentSpriteSheet;
        private int _frameCount;
        private int _currentFrame;
        private double _frameDuration;

        public AnimationService(Image targetImage)
        {
            _targetImage = targetImage;
            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        public void PlayAnimation(string spriteSheetPath, int frameCount, double frameDuration)
        {
            // Stop any current animation
            StopAnimation();

            try
            {
                // Load the sprite sheet
                _currentSpriteSheet = new BitmapImage();
                _currentSpriteSheet.BeginInit();
                _currentSpriteSheet.UriSource = new Uri(spriteSheetPath, UriKind.Relative);
                _currentSpriteSheet.CacheOption = BitmapCacheOption.OnLoad;
                _currentSpriteSheet.EndInit();

                // Store animation parameters
                _frameCount = frameCount;
                _currentFrame = 0;
                _frameDuration = frameDuration;

                // Set up timer for animation
                _animationTimer.Interval = TimeSpan.FromSeconds(frameDuration);

                // Ensure the image control has appropriate settings
                _targetImage.Stretch = Stretch.Uniform;
                _targetImage.StretchDirection = StretchDirection.Both;

                // Set a fixed size to ensure proper display if needed
                // Comment these out if you want the image to size to its container
                //_targetImage.Width = 128;
                //_targetImage.Height = 128;

                // Display first frame
                UpdateFrame();

                // Start animation
                _animationTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading animation: {ex.Message}");

                // Try to load a fallback image
                try
                {
                    _targetImage.Source = new BitmapImage(new Uri("Assets/Images/fallback.png", UriKind.Relative));
                }
                catch
                {
                    // If even fallback fails, clear the image
                    _targetImage.Source = null;
                }
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Move to next frame
            _currentFrame++;
            if (_currentFrame >= _frameCount)
            {
                // Loop back to the first frame
                _currentFrame = 0;
            }

            // Update the displayed frame
            UpdateFrame();
        }

        private void UpdateFrame()
        {
            try
            {
                int frameWidth = _currentSpriteSheet.PixelWidth / _frameCount;
                int frameHeight = _currentSpriteSheet.PixelHeight;

                // Create a new cropped bitmap for the current frame
                CroppedBitmap croppedBitmap = new CroppedBitmap(
                    _currentSpriteSheet,
                    new Int32Rect(_currentFrame * frameWidth, 0, frameWidth, frameHeight)
                );

                // Set it as the image source
                _targetImage.Source = croppedBitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating animation frame: {ex.Message}");
            }
        }

        public void StopAnimation()
        {
            // Stop the timer if it's running
            if (_animationTimer != null && _animationTimer.IsEnabled)
            {
                _animationTimer.Stop();
            }
        }
    }
}