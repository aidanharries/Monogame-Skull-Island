using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Skull_Island
{
    /// <summary>
    /// Represents the four cardinal directions.
    /// </summary>
    public enum Direction
    {
        Down,
        Up,
        Left,
        Right
    }

    /// <summary>
    /// Represents the player character in the game.
    /// </summary>
    public class Player
    {
        // Fields
        private Vector2 _position = new Vector2(500, 300);              // Starting position
        private int _speed = 400;                                       // Movement speed
        private Direction _direction = Direction.Down;                  // Current direction
        private bool _isMoving = false;                                 // Is the player moving
        private KeyboardState _keyboardStateOld = Keyboard.GetState();  // Previous keyboard state
        private float _hitFlashDuration = 0.2f;                         // Duration to flash red on hit
        private float _hitFlashTimer = 0;                               // Timer for hit flash

        // Constants
        public const int MAX_HITS = 10;                                 // Maximum hits player can take

        // Properties
        public int HitCount = 0;                                        // Current hit count
        public bool Dead = false;                                       // Is the player dead
        public SpriteAnimation Animation;                               // Current animation
        public SpriteAnimation[] Animations = new SpriteAnimation[4];   // Animations for each direction

        /// <summary>
        /// Gets the player's position.
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Sets the player's X position.
        /// </summary>
        /// <param name="newX">The new X position.</param>
        public void SetX(float newX)
        {
            _position.X = newX;
        }

        /// <summary>
        /// Sets the player's Y position
        /// </summary>
        /// <param name="newY">The new Y position.</param>
        public void SetY(float newY)
        {
            _position.Y = newY;
        }

        /// <summary>
        /// Updates the player's state.
        /// </summary>
        /// <param name="gameTime">Game time information.</param>
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _isMoving = false;

            if (Dead)
            {
                _isMoving = false;
            }
            else if (keyboardState.IsKeyDown(Keys.Space))
            {
                _isMoving = false;
            }
            else
            {
                // Check keyboard state for movement keys
                bool movingRight = keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D);
                bool movingLeft = keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A);
                bool movingUp = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);
                bool movingDown = keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S);

                // Determine the direction of the movement
                if (movingRight && movingUp || movingRight && movingDown)
                {
                    _direction = Direction.Right;
                    _isMoving = true;
                }
                else if (movingLeft && movingUp || movingLeft && movingDown)
                {
                    _direction = Direction.Left;
                    _isMoving = true;
                }
                else if (movingRight)
                {
                    _direction = Direction.Right;
                    _isMoving = true;
                }
                else if (movingLeft)
                {
                    _direction = Direction.Left;
                    _isMoving = true;
                }
                else if (movingUp)
                {
                    _direction = Direction.Up;
                    _isMoving = true;
                }
                else if (movingDown)
                {
                    _direction = Direction.Down;
                    _isMoving = true;
                }

                // Apply movement if the player is moving
                if (_isMoving)
                {
                    if (movingRight && _position.X < 1275)
                    {
                        _position.X += _speed * deltaTime;
                    }
                    if (movingLeft && _position.X > 225)
                    {
                        _position.X -= _speed * deltaTime;
                    }
                    if (movingUp && _position.Y > 200)
                    {
                        _position.Y -= _speed * deltaTime;
                    }
                    if (movingDown && _position.Y < 1250)
                    {
                        _position.Y += _speed * deltaTime;
                    }
                }

                // Reduce the hit flash timer
                if (_hitFlashTimer > 0)
                {
                    _hitFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_hitFlashTimer <= 0)
                    {
                        // Reset color to white once the hit flash duration is over
                        foreach (var animation in Animations)
                        {
                            animation.Color = Color.White;
                        }
                    }
                }
            }

            // Set the current animation based on the direction
            Animation = Animations[(int)_direction];
            // Adjust animation position
            Animation.Position = new Vector2(_position.X - 48, _position.Y - 48);

            // Update animation if moving or shooting, otherwise set to idle frame
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Animation.SetFrame(0);
            }
            else if (_isMoving)
            {
                Animation.Update(gameTime);
            }
            else
            {
                Animation.SetFrame(1);
            }

            // Check for space key press to initiate an attack
            if (keyboardState.IsKeyDown(Keys.Space) && _keyboardStateOld.IsKeyUp(Keys.Space) && !Dead)
            {
                // Add a new projectile at the player's position in the current direction
                Projectile.Projectiles.Add(new Projectile(_position, _direction));
            }

            _keyboardStateOld = keyboardState;
        }

        /// <summary>
        /// Handles the player taking a hit.
        /// </summary>
        public void TakeHit()
        {
            if (HitCount < MAX_HITS)
            {
                HitCount++;
                _hitFlashTimer = _hitFlashDuration;

                // Set the color for all player animations
                foreach (var animation in Animations)
                {
                    animation.Color = Color.Red;
                }

                if (HitCount >= MAX_HITS)
                {
                    Dead = true;
                }
            }
        }
    }
}
