using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Skull_Island
{
    /// <summary>
    /// Represents a projectile in the game.
    /// </summary>
    public class Projectile
    {
        // Fields
        private Vector2 _position;          // Current position of the projectile
        private int _speed = 1000;          // Speed of the projectile
        private Direction _direction;       // Direction of the projectile's movement
        private bool _collided = false;     // Flag indicating if the projectile has collided

        // Static list of all projectiles
        public static List<Projectile> Projectiles = new List<Projectile>();

        // Radius of the projectile for collision detection
        public int Radius = 18;

        /// <summary>
        /// Constructor for creating a projectile.
        /// </summary>
        /// <param name="newPosition">Initial position of the projectile.</param>
        /// <param name="newDirection">Direction of the projectile's movement.</param>
        public Projectile(Vector2 newPosition, Direction newDirection)
        {
            _position = newPosition;
            _direction = newDirection;
        }

        /// <summary>
        /// Gets the current position of the projectile.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        /// <summary>
        /// Gets or sets the collided state of the projectile.
        /// </summary>
        public bool Collided
        {
            get { return _collided; }
            set { _collided = value; }
        }

        /// <summary>
        /// Updates the projectile's position based on its direction and speed.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update position based on direction and speed
            switch (_direction)
            {
                case Direction.Right:
                    _position.X += _speed * deltaTime;
                    break;
                case Direction.Left:
                    _position.X -= _speed * deltaTime;
                    break;
                case Direction.Down:
                    _position.Y += _speed * deltaTime;
                    break;
                case Direction.Up:
                    _position.Y -= _speed * deltaTime;
                    break;
            }
        }
    }
}
