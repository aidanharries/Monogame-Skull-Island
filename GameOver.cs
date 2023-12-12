using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Skull_Island
{
    /// <summary>
    /// Represents the game over screen in the game.
    /// </summary>
    public class GameOver
    {
        // Fields for fade effect
        private float _fadeAlpha = 1.0f;                            // Current alpha value for fade effect
        private float _fadeSpeed = 0.02f;                           // Speed of the fade effect

        // Sprite fonts for text display
        private SpriteFont _font;                                   // Font for regular text
        private SpriteFont _largeFont;                              // Font for larger text (titles)

        // Text content and positions
        private string _titleText = "GAME OVER";                    // Game over title text
        private string _promptText = "PRESS ENTER TO PLAY AGAIN";   // Prompt text
        private Vector2 _titlePosition;                             // Position for the title text
        private Vector2 _titlePosition1Fall;                        // Position for first layer of falling title
        private Vector2 _titlePosition2Fall;                        // Position for second layer of falling title
        private Vector2 _promptPosition;                            // Position for the prompt text
        private float _promptBaselineY;                             // Baseline Y position for prompt oscillation
        private float _oscillationSpeed = 2.0f;                     // Speed of the prompt oscillation

        // Variables for enter key handling
        private bool _enterPressed = false;                         // Flag to check if enter is pressed
        private TimeSpan _enterPressTime;                           // Time when enter was pressed

        // Variables for fade transition    
        private float _fadeValue = 0f;                              // Current fade value
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);   // Duration of the fade effect
        private Texture2D _fadeTexture;                             // Texture used for fade effect

        // Background and fade textures
        private Texture2D _backgroundTexture;                       // Texture for the background
        private Texture2D _fade;                                    // Texture for the fade effect overlay

        // Properties for scores
        public int FinalScore { get; set; }                         // Player's final score
        private int _displayFinalScore;                             // Displayed final score (for animation)
        public int HighScore { get; set; }                          // High score
        private int _displayHighScore;                              // Displayed high score (for animation)
        public bool IsNewHighScore { get; set; }                    // Flag to indicate if it's a new high score

        /// <summary>
        /// Constructor for GameOver screen.
        /// </summary>
        /// <param name="content">Content manager to load assets.</param>
        /// <param name="graphicsDevice">Graphics device for creating textures.</param>
        public GameOver(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _fadeTexture = new Texture2D(graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.Black });

            _font = content.Load<SpriteFont>("LanaPixel");
            _largeFont = content.Load<SpriteFont>("CompassProLarge");

            _fade = content.Load<Texture2D>("fade");
            _backgroundTexture = content.Load<Texture2D>("background");

            // Calculate initial positions for text elements
            _titlePosition = new Vector2(
                (Skull_Island.ScreenWidth - _largeFont.MeasureString(_titleText).X) / 2,
                Skull_Island.ScreenHeight * 1 / 500
            );
            _titlePosition1Fall = _titlePosition;
            _titlePosition2Fall = _titlePosition;

            _promptPosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Skull_Island.ScreenHeight * 13 / 16
            );
            _promptBaselineY = _promptPosition.Y;
        }

        /// <summary>
        /// Updates the game over screen logic.
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values.</param>
        /// <returns>True if the screen should transition away.</returns>
        public bool Update(GameTime gameTime)
        {
            // Handle fade effect
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

            // Handle title falling animation
            float fallSpeed = 0.75f;
            float fallLimit = 8.0f;
            if (_titlePosition1Fall.Y - _titlePosition.Y < fallLimit)
            {
                _titlePosition1Fall.Y += fallSpeed;
            }
            if (_titlePosition2Fall.Y - _titlePosition.Y < fallLimit * 2)
            {
                _titlePosition2Fall.Y += fallSpeed;
            }

            // Animate final score and high score display
            if (_displayFinalScore < FinalScore)
            {
                _displayFinalScore += Math.Min(FinalScore - _displayFinalScore, 50);
            }
            if (IsNewHighScore && _displayHighScore < HighScore)
            {
                _displayHighScore += Math.Min(HighScore - _displayHighScore, 50);
            }

            // Oscillate prompt position
            if (!_enterPressed)
            {
                _promptPosition.Y = _promptBaselineY + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * _oscillationSpeed) * 10.0f;
            }

            // Handle enter key press
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_enterPressed)
            {
                _enterPressed = true;
                _enterPressTime = gameTime.TotalGameTime;
            }

            // Handle fade transition
            if (_enterPressed)
            {
                float fadeProgress = (float)(gameTime.TotalGameTime - _enterPressTime).TotalSeconds / (float)_fadeDuration.TotalSeconds;
                _fadeValue = MathHelper.Clamp(fadeProgress, 0f, 1f);

                if (_fadeValue >= 1f)
                {
                    Controller.ResetTimer(); // Reset the timer for the next screen
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draws the game over screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), Color.White);

            // Set colors for different text elements
            Color black2 = new Color(100, 100, 100);
            Color black1 = new Color(75, 75, 75);
            Color black = new Color(50, 50, 50);
            Color promptColor = _enterPressed ? Color.White : black;

            // Draw prompt text
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            // Draw title text with falling effect
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition2Fall, black2);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition1Fall, black1);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition, black);

            // Draw final score
            string finalScoreText = $"FINAL SCORE:";
            string finalScore = $"{_displayFinalScore}";
            Vector2 finalScoreTextPosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(finalScoreText).X) / 2,
                (Skull_Island.ScreenHeight - _font.MeasureString(finalScoreText).Y) * 36/100
            );
            Vector2 finalScorePosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(finalScore).X) / 2,
                (Skull_Island.ScreenHeight - _font.MeasureString(finalScore).Y) * 44/100
            );
            spriteBatch.DrawString(_font, finalScoreText, finalScoreTextPosition, black);
            spriteBatch.DrawString(_font, finalScore, finalScorePosition, Color.White);

            // Determine high score text and color
            string highScoreText = IsNewHighScore ? "NEW HIGH SCORE:" : "HIGH SCORE:";
            Color highScoreColor = IsNewHighScore ? Color.Gold : black;
            string highScore = IsNewHighScore ? $"{_displayHighScore}" : $"{HighScore}";
            Vector2 highScoreTextPosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(highScoreText).X) / 2,
                (Skull_Island.ScreenHeight - _font.MeasureString(highScoreText).Y) * 56/100
            );
            Vector2 highScorePosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(highScore).X) / 2,
                (Skull_Island.ScreenHeight - _font.MeasureString(highScore).Y) * 64/100
            );
            spriteBatch.DrawString(_font, highScoreText, highScoreTextPosition, highScoreColor);
            spriteBatch.DrawString(_font, highScore, highScorePosition, Color.White);

            // Handle fade transition effect
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            if (_enterPressed)
            {
                byte alphaValue = (byte)(_fadeValue * 255);
                Color fadeColor = new Color((byte)255, (byte)255, (byte)255, alphaValue);
                spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), fadeColor);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (_fadeAlpha > 0)
            {
                Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                spriteBatch.Draw(_fade, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), fadeColor);
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        /// <summary>
        /// Resets the game to its initial state.
        /// </summary>
        public void Reset()
        {
            _enterPressed = false;
            _fadeValue = 0f;
            _fadeAlpha = 1.0f;
            FinalScore = 0;
            _displayFinalScore = 0;
            _displayHighScore = 0;
        }
    }
}
