using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using TamagotchiGame.Models;
using System.Windows.Threading;

namespace TamagotchiGame.Services
{
    public class SpriteAnimator
    {
        private readonly Image _targetImage;
        private DispatcherTimer _animationTimer;
        private BitmapImage _currentSpriteSheet;
        private int _frameCount;
        private int _currentFrame;
        private bool _loop;
        private PetState _currentState;

        // Animation definitions for different states
        private readonly struct AnimationInfo
        {
            public string SpriteSheet { get; }
            public int FrameCount { get; }
            public double FrameDuration { get; } // in seconds
            public bool Loop { get; }

            public AnimationInfo(string spriteSheet, int frameCount, double frameDuration, bool loop = true)
            {
                SpriteSheet = spriteSheet;
                FrameCount = frameCount;
                FrameDuration = frameDuration;
                Loop = loop;
            }
        }

        private readonly Dictionary<PetState, AnimationInfo> _animations = new Dictionary<PetState, AnimationInfo>
        {
            { PetState.Idle, new AnimationInfo("Assets/Images/idle_sheet.png", 4, 0.25) },
            { PetState.Eating, new AnimationInfo("Assets/Images/eating_sheet.png", 6, 0.2, false) },
            { PetState.Playing, new AnimationInfo("Assets/Images/playing_sheet.png", 6, 0.15) },
            { PetState.Sleeping, new AnimationInfo("Assets/Images/sleeping_sheet.png", 4, 0.5) },
            { PetState.Hungry, new AnimationInfo("Assets/Images/hungry_sheet.png", 3, 0.3) },
            { PetState.Sick, new AnimationInfo("Assets/Images/sick_sheet.png", 3, 0.4) },
            { PetState.Tired, new AnimationInfo("Assets/Images/tired_sheet.png", 3, 0.4) }
        };

        public SpriteAnimator(Image targetImage)
        {
            _targetImage = targetImage;
            _currentState = PetState.Idle;
            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += AnimationTimer_Tick;

            // Configure image for best display
            _targetImage.Stretch = Stretch.Uniform;
            _targetImage.StretchDirection = StretchDirection.Both;
        }

        public void UpdateAnimation(PetState state)
        {
            if (_currentState == state && _animationTimer.IsEnabled)
                return; // Already playing this animation

            _currentState = state;

            if (_animations.TryGetValue(state, out AnimationInfo animInfo))
            {
                PlayAnimation(animInfo.SpriteSheet, animInfo.FrameCount, animInfo.FrameDuration, animInfo.Loop);
            }
            else
            {
                // Fallback to idle animation if state not found
                if (_animations.TryGetValue(PetState.Idle, out AnimationInfo fallbackInfo))
                {
                    PlayAnimation(fallbackInfo.SpriteSheet, fallbackInfo.FrameCount, fallbackInfo.FrameDuration, fallbackInfo.Loop);
                }
            }
        }

        private void PlayAnimation(string spriteSheetPath, int frameCount, double frameDuration, bool loop)
        {
            // Stop any current animation
            if (_animationTimer.IsEnabled)
            {
                _animationTimer.Stop();
            }

            try
            {
                // Load sprite sheet
                _currentSpriteSheet = new BitmapImage();
                _currentSpriteSheet.BeginInit();
                _currentSpriteSheet.UriSource = new Uri(spriteSheetPath, UriKind.Relative);
                _currentSpriteSheet.CacheOption = BitmapCacheOption.OnLoad;
                _currentSpriteSheet.EndInit();

                // Store animation parameters
                _frameCount = frameCount;
                _currentFrame = 0;
                _loop = loop;

                // Set up the timer
                _animationTimer.Interval = TimeSpan.FromSeconds(frameDuration);

                // Show first frame
                UpdateFrame();

                // Start the timer
                _animationTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing animation: {ex.Message}");

                // Use a simple placeholder image if animation fails
                try
                {
                    _targetImage.Source = new BitmapImage(new Uri("Assets/Images/fallback.png", UriKind.Relative));
                }
                catch
                {
                    // If even the fallback fails, just clear the image
                    _targetImage.Source = null;
                }
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Move to next frame
            _currentFrame++;

            // Check if we reached the end of the animation
            if (_currentFrame >= _frameCount)
            {
                if (_loop)
                {
                    // Loop back to first frame
                    _currentFrame = 0;
                }
                else
                {
                    // Stop animation and stay on last frame
                    _animationTimer.Stop();
                    return;
                }
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

                // Create a cropped bitmap showing just the current frame
                CroppedBitmap croppedBitmap = new CroppedBitmap(
                    _currentSpriteSheet,
                    new Int32Rect(_currentFrame * frameWidth, 0, frameWidth, frameHeight)
                );

                // Set as image source
                _targetImage.Source = croppedBitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating frame: {ex.Message}");
            }
        }

        public void StopAnimation()
        {
            if (_animationTimer.IsEnabled)
            {
                _animationTimer.Stop();
            }
        }
    }
}