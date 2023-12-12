using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skull_Island
{
    /// <summary>
    /// Represents a nuke drop object in the game.
    /// </summary>
    public class NukeDrop
    {
        // Properties
        public Vector2 Position { get; private set; }   // Current position

        // Fields
        private Texture2D _sprite;                      // Texture for the nuke drop sprite
        private Rectangle _spriteRectangle;             // Rectangle for the sprite's source texture
        private float _timer;                           // Timer to track how long the nuke has been present

        /// <summary>
        /// Constructor for the NukeDrop class.
        /// </summary>
        /// <param name="position">Initial position of the nuke drop.</param>
        /// <param name="sprite">Texture for the nuke drop sprite.</param>
        /// <param name="spriteRectangle">Rectangle for the sprite's source texture.</param>
        public NukeDrop(Vector2 position, Texture2D sprite, Rectangle spriteRectangle)
        {
            Position = position;
            _sprite = sprite;
            _spriteRectangle = spriteRectangle;
            _timer = 0f; // Initialize timer to 0
        }

        /// <summary>
        /// Updates the nuke drop, tracking its elapsed time.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Draws the nuke drop sprite at its current position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw the sprite.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centeredPosition = new Vector2(Position.X - _spriteRectangle.Width / 2, Position.Y - _spriteRectangle.Height / 4);
            spriteBatch.Draw(_sprite, centeredPosition, _spriteRectangle, Color.White);
        }

        /// <summary>
        /// Determines if the nuke drop should despawn.
        /// </summary>
        /// <returns>True if the nuke drop should despawn, otherwise false.</returns>
        public bool ShouldDespawn()
        {
            return _timer > 10; // Despawn after 10 seconds
        }

        
    }
}
