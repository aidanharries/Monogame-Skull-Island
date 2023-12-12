using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Skull_Island
{
    /// <summary>
    /// Manages the gameplay logic for Skull Island.
    /// </summary>
    public class Gameplay
    {
        // Fields for fade effect
        private float _fadeAlpha = 1.0f;
        private float _fadeSpeed = 0.02f;

        // Camera for the game view
        private Camera _camera;

        // Textures for various game elements
        private Texture2D _playerDown;
        private Texture2D _playerUp;
        private Texture2D _playerRight;
        private Texture2D _playerLeft;
        private Texture2D _fade;
        private Texture2D _background;
        private Texture2D _ball;
        private Texture2D _skull;

        // Health UI elements
        private Texture2D _healthUISpritesheet;
        private Rectangle _labelRectangle;
        private Rectangle _fullHeartRectangle;
        private Rectangle _emptyHeartRectangle;

        // Player and game elements
        public Player Player;
        private List<HeartDrop> _heartDrops = new List<HeartDrop>();
        private Rectangle _heartDropRectangle;
        private List<NukeDrop> _nukeDrops = new List<NukeDrop>();
        private Texture2D _nukeTexture;
        private Rectangle _nukeRectangle; // Rectangle for nuke sprite
        private Rectangle _DropRegion;
        private Random _random = new Random();

        // Sound Effects
        private SoundEffect _hurtSoundEffect;
        private SoundEffect _explosionSoundEffect;
        private SoundEffect _shootingSoundEffect;
        private SoundEffect _pickupHeartSoundEffect;

        // Store the previous keyboard state
        private KeyboardState _previousKeyboardState;

        // Scoring
        public int Score { get; private set; }
        private int _displayScore;
        private SpriteFont _font;

        /// <summary>
        /// Constructor fot the Gameplay class.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="game"></param>
        public Gameplay(ContentManager content, Skull_Island game)
        {
            _camera = new Camera(game.GraphicsDevice);
            Player = new Player();

            // Initialize scoring
            Score = 0;
            _displayScore = 0;

            // Load content and initialize textures
            _font = content.Load<SpriteFont>("LanaPixel");
            _playerDown = content.Load<Texture2D>("Player/playerDown");
            _playerRight = content.Load<Texture2D>("Player/playerRight");
            _playerLeft = content.Load<Texture2D>("Player/playerLeft");
            _playerUp = content.Load<Texture2D>("Player/playerUp");
            _fade = content.Load<Texture2D>("fade");
            _background = content.Load<Texture2D>("background");
            _ball = content.Load<Texture2D>("ball");
            _skull = content.Load<Texture2D>("skull");
            _nukeTexture = content.Load<Texture2D>("nuke");

            Player.Animations[0] = new SpriteAnimation(_playerDown, 4, 8);
            Player.Animations[1] = new SpriteAnimation(_playerUp, 4, 8);
            Player.Animations[2] = new SpriteAnimation(_playerLeft, 4, 8);
            Player.Animations[3] = new SpriteAnimation(_playerRight, 4, 8);
            Player.Animation = Player.Animations[0];

            // Initialize sound effects
            _hurtSoundEffect = content.Load<SoundEffect>("hurt");
            _explosionSoundEffect = content.Load<SoundEffect>("explosion");
            _shootingSoundEffect = content.Load<SoundEffect>("shoot");
            _pickupHeartSoundEffect = content.Load<SoundEffect>("pickupHeart");

            // Initialize the previous keyboard state
            _previousKeyboardState = Keyboard.GetState();

            // Initialize health UI elements
            _healthUISpritesheet = content.Load<Texture2D>("healthUI");
            _labelRectangle = new Rectangle(0, 0, 64, 52);
            _fullHeartRectangle = new Rectangle(64, 0, 64, 52);
            _emptyHeartRectangle = new Rectangle(128, 0, 64, 52);

            // Initialize heart drop region and rectangle
            int regionSize = 1080;
            int backgroundWidth = 2496;
            int backgroundHeight = 2496;

            int leftX = (-500 + (backgroundWidth - regionSize) / 2);
            int topY = (-500 + (backgroundHeight - regionSize) / 2);
            _DropRegion = new Rectangle(leftX, topY, regionSize, regionSize);

            int insetAmount = 2;
            _heartDropRectangle = new Rectangle
            (
                _fullHeartRectangle.X + insetAmount, _fullHeartRectangle.Y + insetAmount, 
                _fullHeartRectangle.Width - 2 * insetAmount, _fullHeartRectangle.Height - 2 * insetAmount
            );
            _nukeRectangle = new Rectangle(0, 0, 64, 64);
        }

        /// <summary>
        /// Updates the game logic each frame.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state.</param>
        public void Update(GameTime gameTime)
        {
            // Update fade effect
            if (_fadeAlpha > 0)
            {
                _fadeAlpha -= _fadeSpeed;
                _fadeAlpha = MathHelper.Clamp(_fadeAlpha, 0f, 1f);
            }

            // Update player and controller
            Player.Update(gameTime);
            if (!Player.Dead)
            {
                Controller.Update(gameTime, _skull);
            }

            // Update camera
            _camera.Position = Player.Position;
            _camera.Update(gameTime);

            // Update projectiles
            foreach (Projectile projectile in Projectile.Projectiles)
            {
                projectile.Update(gameTime);
            }

            // Update enemies
            foreach (Enemy enemy in Enemy.Enemies)
            {
                enemy.Update(gameTime, Player.Position, Player.Dead);
                int sum = 32 + enemy.Radius;
                if (Vector2.Distance(Player.Position, enemy.Position) < sum && !Player.Dead)
                {
                    Player.TakeHit();
                    enemy.Dead = true;

                    // Play the hurt sound effect
                    _hurtSoundEffect.Play();

                    // Play the explosion sound effect when the enemy dies
                    float explosionVolume = 0.5f; // 50% of the full volume
                    _explosionSoundEffect.Play(explosionVolume, 0.0f, 0.0f);
                }
            }

            // Get the current keyboard state
            var currentKeyboardState = Keyboard.GetState();

            // Check if space key was just pressed
            if (currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
            {
                // Play the shooting sound effect
                _shootingSoundEffect.Play();
            }

            // Update the previous keyboard state
            _previousKeyboardState = currentKeyboardState;

            foreach (Projectile projectile in Projectile.Projectiles)
            {
                foreach (Enemy enemy in Enemy.Enemies)
                {
                    int sum = projectile.Radius + enemy.Radius;
                    if (Vector2.Distance(projectile.Position, enemy.Position) < sum)
                    {
                        projectile.Collided = true;
                        if (!enemy.Dead)
                        {
                            Score += 100;
                        }
                        enemy.Dead = true;

                        // Play the explosion sound effect when the enemy dies
                        float explosionVolume = 0.5f; // 50% of the full volume
                        _explosionSoundEffect.Play(explosionVolume, 0.0f, 0.0f);

                        // Single random number to decide on drop type
                        double dropChance = _random.NextDouble();
                        if (_DropRegion.Contains(enemy.Position) && Player.HitCount > 0)
                        {
                            if (dropChance < 0.10) // 10% chance for a nuke drop
                            {
                                _nukeDrops.Add(new NukeDrop(enemy.Position, _nukeTexture, _nukeRectangle));
                            }
                            else if (dropChance < 0.35) // Additional 25% chance for a heart drop (total 35%)
                            {
                                _heartDrops.Add(new HeartDrop(enemy.Position, _healthUISpritesheet, _heartDropRectangle));
                            }
                            // If the number is greater than 0.35, no drop occurs
                        }
                    }
                }
            }

            // Update heart drops
            for (int i = _heartDrops.Count - 1; i >= 0; i--)
            {
                HeartDrop heartDrop = _heartDrops[i];
                heartDrop.Update(gameTime);

                if (heartDrop.ShouldDespawn())
                {
                    _heartDrops.RemoveAt(i);
                }
                else
                {
                    int enlargementFactor = 10; // Increase the size of the collision area
                    Rectangle heartDropArea = new Rectangle
                    (
                        (int)heartDrop.Position.X - _heartDropRectangle.Width / 2 - enlargementFactor,
                        (int)heartDrop.Position.Y - _heartDropRectangle.Height / 2 - enlargementFactor,
                        _heartDropRectangle.Width + 2 * enlargementFactor,
                        _heartDropRectangle.Height + 2 * enlargementFactor
                    );

                    if (heartDropArea.Contains(Player.Position))
                    {
                        _pickupHeartSoundEffect.Play();
                        Player.HitCount = Math.Max(Player.HitCount - 1, 0);
                        Score += 25; 
                        _heartDrops.RemoveAt(i);
                    }
                }
            }

            // Update nuke drops
            for (int i = _nukeDrops.Count - 1; i >= 0; i--)
            {
                NukeDrop nukeDrop = _nukeDrops[i];
                nukeDrop.Update(gameTime);

                if (nukeDrop.ShouldDespawn())
                {
                    _nukeDrops.RemoveAt(i);
                }
                else
                {
                    int enlargementFactor = 10; // Increase the size of the collision area
                    Rectangle nukeDropArea = new Rectangle
                    (
                        (int)nukeDrop.Position.X - _nukeRectangle.Width / 2 - enlargementFactor,
                        (int)nukeDrop.Position.Y - _nukeRectangle.Height / 2 - enlargementFactor,
                        _nukeRectangle.Width + 2 * enlargementFactor,
                        _nukeRectangle.Height + 2 * enlargementFactor
                    );

                    if (nukeDropArea.Contains(Player.Position))
                    {
                        Score += Enemy.Enemies.Count * 100;

                        // Play the explosion sound effect when the enemy dies
                        float explosionVolume = 0.5f; // 50% of the full volume
                        _explosionSoundEffect.Play(explosionVolume, 0.0f, 0.0f);

                        Enemy.Enemies.Clear();
                        Score += 25;
                        _nukeDrops.RemoveAt(i);
                    }
                }
            }

            // Gradually update the displayed score
            if (_displayScore < Score)
            {
                _displayScore += Math.Min(Score - _displayScore, 25);
            }

            // Remove collided projectiles and dead enemies
            Projectile.Projectiles.RemoveAll(p => p.Collided);
            Enemy.Enemies.RemoveAll(e => e.Dead);
        }

        /// <summary>
        /// Draws the game elements to the screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state.</param>
        /// <param name="spriteBatch">SpriteBatch for drawing textures.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Begin camera SpriteBatch
            spriteBatch.End();
            spriteBatch.Begin(_camera);

            // Draw game elements
            spriteBatch.Draw(_background, new Vector2(-500, -500), Color.White);
            foreach (var heartDrop in _heartDrops)
            {
                heartDrop.Draw(spriteBatch);
            }
            foreach (var nukeDrop in _nukeDrops)
            {
                nukeDrop.Draw(spriteBatch);
            }
            foreach (Enemy enemy in Enemy.Enemies)
            {
                enemy.Animation.Draw(spriteBatch);
            }
            foreach (Projectile projectile in Projectile.Projectiles)
            {
                spriteBatch.Draw(_ball, new Vector2(projectile.Position.X - 48, projectile.Position.Y - 48), Color.White);
            }
            if (!Player.Dead)
            {
                Player.Animation.Draw(spriteBatch);
            }

            // End SpriteBatch and begin UI SpriteBatch
            spriteBatch.End();
            spriteBatch.Begin();

            // Draw UI elements
            float healthUIYPosition = Skull_Island.ScreenHeight - _labelRectangle.Height - 10;
            spriteBatch.Draw(_healthUISpritesheet, new Vector2(10, healthUIYPosition), _labelRectangle, Color.White);

            Vector2 heartPosition = new Vector2(10 + _labelRectangle.Width, healthUIYPosition);
            for (int i = 0; i < Player.MAX_HITS; i++)
            {
                Rectangle heartRect = (Player.MAX_HITS - Player.HitCount > i) ? _fullHeartRectangle : _emptyHeartRectangle;
                spriteBatch.Draw(_healthUISpritesheet, heartPosition, heartRect, Color.White);
                heartPosition.X += _fullHeartRectangle.Width - 15;
            }

            string scoreText = $"SCORE:";
            string currentScore = $"{_displayScore}";
            spriteBatch.DrawString(_font, scoreText, new Vector2(10, 5), Color.White);
            spriteBatch.DrawString(_font, currentScore, new Vector2(10, 50), Color.White);

            // End SpriteBatch and begin fade SpriteBatch
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw Fade Effect
            if (_fadeAlpha > 0)
            {
                Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                spriteBatch.Draw(_fade, new Rectangle(0, 0, Skull_Island.ScreenWidth, Skull_Island.ScreenHeight), fadeColor);
            }

            // End SpriteBatch
            spriteBatch.End();
            spriteBatch.Begin();
        }

        /// <summary>
        /// Resets the game to its initial state.
        /// </summary>
        public void ResetGame()
        {
            Player = new Player();

            Player.Animations[0] = new SpriteAnimation(_playerDown, 4, 8);
            Player.Animations[1] = new SpriteAnimation(_playerUp, 4, 8);
            Player.Animations[2] = new SpriteAnimation(_playerLeft, 4, 8);
            Player.Animations[3] = new SpriteAnimation(_playerRight, 4, 8);
            Player.Animation = Player.Animations[0];

            Player.HitCount = 0;

            Score = 0;
            _displayScore = 0;

            _camera.Position = Vector2.Zero;
            _camera.Update(new GameTime());

            Projectile.Projectiles.Clear();
            Enemy.Enemies.Clear();
            _heartDrops.Clear();
            _nukeDrops.Clear();

            _fadeAlpha = 1.0f;
        }
    }
}
