using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Skull_Island
{
    /// <summary>
    /// Represents the main menu of the game.
    /// </summary>
    public class MainMenu
    {
        // Fonts for displaying text
        private SpriteFont _font;
        private SpriteFont _largeFont;

        // Menu text content
        private string _titleText = "SKULL ISLAND";
        private string _promptText = "PRESS ENTER TO PLAY";

        // Title and prompt position management
        private Vector2 _titlePosition;
        private Vector2 _titlePosition1Fall;
        private Vector2 _titlePosition2Fall;
        private Vector2 _promptPosition;

        private float _promptBaselineY; // Base Y position for prompt oscillation
        private float _oscillationSpeed = 2.0f; // Speed of prompt oscillation

        // Enter key handling
        private bool _enterPressed = false;
        private TimeSpan _enterPressTime;

        // Fade effect
        private float _fadeValue = 0f;
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);
        private Texture2D _fadeTexture;

        // Background texture
        private Texture2D _backgroundTexture;

        // Enemy animations
        private SpriteAnimation _enemyAnimation;
        private SpriteAnimation _enemyReflectionAnimation;

        /// <summary>
        /// Initializes a new instance of the MainMenu class
        /// </summary>
        /// <param name="content">Content manager to load resources.</param>
        /// <param name="graphicsDevice">Graphics device for creating textures.</param>
        public MainMenu(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Load and set up textures and fonts
            _fadeTexture = new Texture2D(graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.Black });

            _font = content.Load<SpriteFont>("LanaPixel");
            _largeFont = content.Load<SpriteFont>("CompassProLarge");

            _backgroundTexture = content.Load<Texture2D>("mainMenuBackground");

            // Set up title positions
            _titlePosition = new Vector2(
                (Skull_Island.ScreenWidth - _largeFont.MeasureString(_titleText).X) / 2,
                Skull_Island.ScreenHeight * 1 / 500
            );
            _titlePosition1Fall = _titlePosition;
            _titlePosition2Fall = _titlePosition;

            // Set up prompt position
            _promptPosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Skull_Island.ScreenHeight * 13 / 16
            );
            _promptBaselineY = _promptPosition.Y;

            // Load and set up enemy animations
            Texture2D enemySpriteSheet = content.Load<Texture2D>("skull");
            _enemyAnimation = new SpriteAnimation(enemySpriteSheet, 10, 6);

            Vector2 centerPosition = new Vector2(Skull_Island.ScreenWidth / 2, Skull_Island.ScreenHeight / 2);
            _enemyAnimation.Position = new Vector2(centerPosition.X + 50, (centerPosition.Y - enemySpriteSheet.Height / 2) + 2);

            _enemyReflectionAnimation = new SpriteAnimation(enemySpriteSheet, 10, 6);
            _enemyReflectionAnimation.Position = new Vector2(centerPosition.X + 146, Skull_Island.ScreenHeight - centerPosition.Y + 225);

            _enemyReflectionAnimation.Scale = -1; // Flip vertically
            _enemyReflectionAnimation.Color = new Color(255, 255, 255, 127); // 50% transparency
        }

        /// <summary>
        /// Updates the state of the main menu.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state.</param>
        /// <returns>True if the main menu process is complete, otherwise false.</returns>
        public bool Update(GameTime gameTime)
        {
            // Update title positions for falling effect
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

            // Oscillate prompt position
            if (!_enterPressed)
            {
                _promptPosition.Y = _promptBaselineY + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * _oscillationSpeed) * 10.0f;
            }

            // Update enemy animations
            _enemyAnimation.Update(gameTime);
            _enemyReflectionAnimation.Update(gameTime);

            // Handle enter key press
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_enterPressed)
            {
                _enterPressed = true;
                _enterPressTime = gameTime.TotalGameTime;
            }

            // Fade out when enter is pressed
            if (_enterPressed)
            {
                float fadeProgress = (float)(gameTime.TotalGameTime - _enterPressTime).TotalSeconds / (float)_fadeDuration.TotalSeconds;
                _fadeValue = MathHelper.Clamp(fadeProgress, 0f, 1f);

                if (_fadeValue >= 1f)
                {
                    return true;  // Indicate that the main menu is complete
                }
            }

            return false; // Menu is still active
        }

        /// <summary>
        /// Draws the main menu.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for drawing textures and text.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), Color.White);

            // Draw title and prompt with falling and oscillating effects
            Color black2 = new Color(100, 100, 100); // Lighter shade
            Color black1 = new Color(75, 75, 75);    // Medium shade
            Color black = new Color(50, 50, 50);     // Darker shade

            Color promptColor = _enterPressed ? Color.White : black;
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition2Fall, black2);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition1Fall, black1);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition, black);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw enemy animations
            _enemyAnimation.Draw(spriteBatch);
            _enemyReflectionAnimation.Draw(spriteBatch);

            // Apply fade effect if enter is pressed
            if (_enterPressed)
            {
                byte alphaValue = (byte)(_fadeValue * 255);
                Color fadeColor = new Color((byte)255, (byte)255, (byte)255, alphaValue);
                spriteBatch.Draw(_fadeTexture, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), fadeColor);
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }
    }
}
