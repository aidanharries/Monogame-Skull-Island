using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skull_Island
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class Enemy
    {
        // Static list of all enemies
        public static List<Enemy> Enemies = new List<Enemy>();

        // Fields
        private Vector2 _position = new Vector2(0, 0);
        private int _speed = 150;
        private bool _dead = false;

        // Properties
        public SpriteAnimation Animation;
        public int Radius = 30; // Collision radius

        /// <summary>
        /// Gets the position of the enemy.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        /// <summary>
        /// Gets or sets the dead status of the enemy.
        /// </summary>
        public bool Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }

        /// <summary>
        /// Constructor for creating an Enemy.
        /// </summary>
        /// <param name="newPosition">The starting position of the enemy.</param>
        /// <param name="spriteSheet">The sprite sheet for the enemy's animation.</param>
        public Enemy(Vector2 newPosition, Texture2D spriteSheet)
        {
            _position = newPosition;
            Animation = new SpriteAnimation(spriteSheet, 10, 6);
        }

        /// <summary>
        /// Updates the enemy's position and animation.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="playerPosition">The position of the player.</param>
        /// <param name="isPlayerDead">Indicates whether the player is dead.</param>
        public void Update(GameTime gameTime, Vector2 playerPosition, bool isPlayerDead)
        {
            // Update animation position
            Animation.Position = new Vector2(_position.X - 48, _position.Y - 66);
            Animation.Update(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Move enemy towards the player if the player is not dead
            if (!isPlayerDead)
            {
                Vector2 moveDirection = playerPosition - _position;
                moveDirection.Normalize();
                _position += moveDirection * _speed * deltaTime;
            }
        }
    }
}
