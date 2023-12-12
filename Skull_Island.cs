using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System;

namespace Skull_Island
{
    /// <summary>
    /// Main game class for Skull Island
    /// </summary>
    public class Skull_Island : Game
    {
        // Constants for screen dimensions
        private const int SCREEN_WIDTH = 1280;
        private const int SCREEN_HEIGHT = 720;

        // Graphics and sprite management
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game screens
        private MainMenu _mainMenu;
        private HowToPlay _howToPlay;
        private Gameplay _gameplay;
        private GameOver _gameOver;

        // Current state of the game
        private GameState _currentState;

        // Background music
        private Song _backgroundMusic1;
        private Song _backgroundMusic2;
        private Song _backgroundMusic3;

        // Screen dimensions properties
        public static int ScreenWidth => SCREEN_WIDTH;
        public static int ScreenHeight => SCREEN_HEIGHT;

        /// <summary>
        /// Constructor for the Skull_Island game.
        /// </summary>
        public Skull_Island()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;

            // Setting the preferred buffer width and height
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            // Initial game state
            _currentState = GameState.MainMenu;

            // Making the mouse cursor visible
            IsMouseVisible = true;
        }

        /// <summary>
        /// Loads game content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize game screens
            _mainMenu = new MainMenu(Content, GraphicsDevice);
            _howToPlay = new HowToPlay(Content, GraphicsDevice);
            _gameplay = new Gameplay(Content, this);
            _gameOver = new GameOver(Content, GraphicsDevice);

            // Load and play background music
            _backgroundMusic1 = Content.Load<Song>("backgroundMusic1");
            _backgroundMusic2 = Content.Load<Song>("backgroundMusic2");
            _backgroundMusic3 = Content.Load<Song>("backgroundMusic3");
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Updates the game state.
        /// </summary>
        /// <param name="gameTime">Game time snapshot.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle game exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Handle background music transitions
            HandleBackgroundMusic(gameTime);

            // Game state management
            switch (_currentState)
            {
                case GameState.MainMenu:
                    if (_mainMenu.Update(gameTime))
                    {
                        _currentState = GameState.HowToPlay;
                    }
                    break;

                case GameState.HowToPlay:
                    if (_howToPlay.Update(gameTime))
                    {
                        _currentState = GameState.Gameplay;
                    }
                    break;

                case GameState.Gameplay:
                    _gameplay.Update(gameTime);
                    if (_gameplay.Player.Dead)
                    {
                        // Handle game over logic
                        int highScore = LoadHighScore();
                        bool isNewHighScore = false;
                        if (_gameplay.Score > highScore)
                        {
                            SaveHighScore(_gameplay.Score);
                            highScore = _gameplay.Score;
                            isNewHighScore = true;
                        }
                        _gameOver.FinalScore = _gameplay.Score;
                        _gameOver.HighScore = highScore;
                        _gameOver.IsNewHighScore = isNewHighScore;
                        _currentState = GameState.GameOver;
                    }
                    break;

                case GameState.GameOver:
                    if (_gameOver.Update(gameTime))
                    {
                        // Reset game and game over screen
                        _gameplay.ResetGame();
                        _gameOver.Reset();
                        _currentState = GameState.Gameplay;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game content.
        /// </summary>
        /// <param name="gameTime">Game time snapshot.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw current game state
            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch);
                    break;

                case GameState.HowToPlay:
                    _howToPlay.Draw(_spriteBatch);
                    break;

                case GameState.Gameplay:
                    _gameplay.Draw(gameTime, _spriteBatch);
                    break;

                case GameState.GameOver:
                    _gameOver.Draw(_spriteBatch);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper method for background music
        /// </summary>
        /// <param name="gameTime">Game time snapshot.</param>
        private void HandleBackgroundMusic(GameTime gameTime)
        {
            switch (_currentState)
            {
                case GameState.MainMenu:
                case GameState.HowToPlay:
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(_backgroundMusic1);
                        MediaPlayer.Volume = 0.25f;
                    }
                    break;

                case GameState.Gameplay:
                    if ((MediaPlayer.Queue.ActiveSong == _backgroundMusic1 || MediaPlayer.Queue.ActiveSong == _backgroundMusic3) && MediaPlayer.Volume > 0)
                    {
                        // Gradually reduce the volume
                        MediaPlayer.Volume -= (float)gameTime.ElapsedGameTime.TotalSeconds / 5.0f;
                        if (MediaPlayer.Volume <= 0)
                        {
                            MediaPlayer.Stop();
                        }
                    }
                    else if ((MediaPlayer.Queue.ActiveSong == _backgroundMusic1 || MediaPlayer.Queue.ActiveSong == _backgroundMusic3) && MediaPlayer.Volume <= 0)
                    {
                        // Ensure the first song is stopped before playing the second
                        MediaPlayer.Play(_backgroundMusic2);
                        MediaPlayer.Volume = 0.25f; // Set initial volume for second song
                    }
                    break;

                case GameState.GameOver:
                    if (MediaPlayer.Queue.ActiveSong == _backgroundMusic2 && MediaPlayer.Volume > 0)
                    {
                        // Gradually reduce the volume
                        MediaPlayer.Volume -= (float)gameTime.ElapsedGameTime.TotalSeconds / 5.0f;
                        if (MediaPlayer.Volume <= 0)
                        {
                            MediaPlayer.Stop();
                        }
                    }
                    else if (MediaPlayer.Queue.ActiveSong == _backgroundMusic2 && MediaPlayer.Volume <= 0)
                    {
                        // Ensure the second song is stopped before playing the third
                        MediaPlayer.Play(_backgroundMusic3);
                        MediaPlayer.Volume = 0.25f; // Set initial volume for third song
                    }
                    break;
            }
        }

        /// <summary>
        /// Saves the high score to a file.
        /// </summary>
        /// <param name="score">Score to save.</param>
        private void SaveHighScore(int score)
        {
            string highScoreFile = "highscore.txt";
            try
            {
                File.WriteAllText(highScoreFile, score.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving high score: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads the high score from a file.
        /// </summary>
        /// <returns>The high score.</returns>
        private int LoadHighScore()
        {
            string highScoreFile = "highscore.txt";
            try
            {
                if (File.Exists(highScoreFile))
                {
                    string scoreText = File.ReadAllText(highScoreFile);
                    return int.TryParse(scoreText, out int score) ? score : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading high score: " + ex.Message);
            }
            return 0;
        }
    }
}