using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Skull_Island
{
    /// <summary>
    /// Represents the 'How To Play' screen in the game.
    /// </summary>
    public class HowToPlay
    {
        // Fields for managing fade animation
        private float _fadeAlpha = 1.0f;
        private float _fadeSpeed = 0.02f;

        // Fonts for displaying text
        private SpriteFont _font;
        private SpriteFont _largeFont;

        // Text for instructions and titles
        private string _titleText = "HOW TO PLAY";
        private string _promptText = "PRESS ENTER TO PLAY";
        private string _instruction = "CONTROLS:";
        private string _up = "UP:";
        private string _down = "DOWN:";
        private string _left = "LEFT:";
        private string _right = "RIGHT:";
        private string _shoot = "SHOOT:";
        private string _upInstructions = "'W' / UP ARROW";
        private string _downInstructions = "'S' / DOWN ARROW";
        private string _leftInstructions = "'A' / LEFT ARROW";
        private string _rightInstructions = "'D' / RIGHT ARROW";
        private string _shootInstructions = "SPACE";

        // Positioning variables for text elements
        private Vector2 _titlePosition;
        private Vector2 _titlePosition1Fall;
        private Vector2 _titlePosition2Fall;
        private Vector2 _promptPosition;
        private Vector2 _instructionPosition;
        private Vector2 _upPosition;
        private Vector2 _downPosition;
        private Vector2 _leftPosition;
        private Vector2 _rightPosition;
        private Vector2 _shootPosition;
        private Vector2 _upInstructionsPosition;
        private Vector2 _downInstructionsPosition;
        private Vector2 _leftInstructionsPosition;
        private Vector2 _rightInstructionsPosition;
        private Vector2 _shootInstructionsPosition;

        // Variables for oscillation and enter key press handling
        private float _promptBaselineY;
        private float _oscillationSpeed = 2.0f;
        private bool _enterPressed = false;
        private TimeSpan _enterPressTime;

        // Fade effect variables
        private float _fadeValue = 0f;
        private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);
        private Texture2D _fadeTexture;
        private Texture2D _backgroundTexture;
        private Texture2D _fade;

        /// <summary>
        /// Initializes a new instance of the HowToPlay class.
        /// </summary>
        /// <param name="content">Content manager to load resources.</param>
        /// <param name="graphicsDevice">Graphics device to create textures.</param>
        public HowToPlay(ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Initialize textures and fonts
            _fadeTexture = new Texture2D(graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.Black });
            _font = content.Load<SpriteFont>("LanaPixel");
            _largeFont = content.Load<SpriteFont>("CompassProLarge");
            _fade = content.Load<Texture2D>("fade");
            _backgroundTexture = content.Load<Texture2D>("background");

            // Calculate positions for the text elements
            _titlePosition = new Vector2(
                (Skull_Island.ScreenWidth - _largeFont.MeasureString(_titleText).X) / 2,
                Skull_Island.ScreenHeight * 1 / 500
            );
            _titlePosition1Fall = _titlePosition;
            _titlePosition2Fall = _titlePosition;

            Vector2 instructionSize = _font.MeasureString(_instruction);
            _instructionPosition = new Vector2((Skull_Island.ScreenWidth - instructionSize.X) / 2, 200);


            instructionSize = _font.MeasureString(_up);
            _upPosition = new Vector2(Skull_Island.ScreenWidth * 41/100 - instructionSize.X, 250);

            instructionSize = _font.MeasureString(_down);
            _downPosition = new Vector2(Skull_Island.ScreenWidth * 41/100 - instructionSize.X, 300);

            instructionSize = _font.MeasureString(_left);
            _leftPosition = new Vector2(Skull_Island.ScreenWidth * 41/100 - instructionSize.X, 350);

            instructionSize = _font.MeasureString(_right);
            _rightPosition = new Vector2(Skull_Island.ScreenWidth * 41/100 - instructionSize.X, 400);

            instructionSize = _font.MeasureString(_shoot);
            _shootPosition = new Vector2(Skull_Island.ScreenWidth * 41/100 - instructionSize.X, 450);

            _upInstructionsPosition = new Vector2((Skull_Island.ScreenWidth * 42/100), 250);
            _downInstructionsPosition = new Vector2((Skull_Island.ScreenWidth * 42/100), 300);
            _leftInstructionsPosition = new Vector2((Skull_Island.ScreenWidth * 42/100), 350);
            _rightInstructionsPosition = new Vector2((Skull_Island.ScreenWidth * 42/100), 400);
            _shootInstructionsPosition = new Vector2((Skull_Island.ScreenWidth * 42/100), 450);

            _promptPosition = new Vector2(
                (Skull_Island.ScreenWidth - _font.MeasureString(_promptText).X) / 2,
                Skull_Island.ScreenHeight * 13 / 16
            );
            _promptBaselineY = _promptPosition.Y;
        }

        /// <summary>
        /// Updates the state of the HowToPlay screen each frame.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state.</param>
        /// <returns>True if the screen is ready to transition, false otherwise.</returns>
        public bool Update(GameTime gameTime)
        {
            // Fade effect logic
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

            // Update falling title positions
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

            if (!_enterPressed)
            {
                _promptPosition.Y = _promptBaselineY + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * _oscillationSpeed) * 10.0f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_enterPressed)
            {
                _enterPressed = true;
                _enterPressTime = gameTime.TotalGameTime;
            }

            if (_enterPressed)
            {
                float fadeProgress = (float)(gameTime.TotalGameTime - _enterPressTime).TotalSeconds / (float)_fadeDuration.TotalSeconds;
                _fadeValue = MathHelper.Clamp(fadeProgress, 0f, 1f);

                if (_fadeValue >= 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draws the HowToPlay screen content.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw textures.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), Color.White);

            Color black2 = new Color(100, 100, 100);
            Color black1 = new Color(75, 75, 75);
            Color black = new Color(50, 50, 50);

            Color promptColor = _enterPressed ? Color.White : black;
            spriteBatch.DrawString(_font, _promptText, _promptPosition, promptColor);

            // Draw titles 
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition2Fall, black2);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition1Fall, black1);
            spriteBatch.DrawString(_largeFont, _titleText, _titlePosition, black);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.DrawString(_font, _instruction, _instructionPosition, black);
            spriteBatch.DrawString(_font, _up, _upPosition, black);
            spriteBatch.DrawString(_font, _down, _downPosition, black);
            spriteBatch.DrawString(_font, _left, _leftPosition, black);
            spriteBatch.DrawString(_font, _right, _rightPosition, black);
            spriteBatch.DrawString(_font, _shoot, _shootPosition, black);

            spriteBatch.DrawString(_font, _upInstructions, _upInstructionsPosition, Color.White);
            spriteBatch.DrawString(_font, _downInstructions, _downInstructionsPosition, Color.White);
            spriteBatch.DrawString(_font, _leftInstructions, _leftInstructionsPosition, Color.White);
            spriteBatch.DrawString(_font, _rightInstructions, _rightInstructionsPosition, Color.White);
            spriteBatch.DrawString(_font, _shootInstructions, _shootInstructionsPosition, Color.White);


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

    }
}
