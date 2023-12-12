using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skull_Island
{
    /// <summary>
    /// Manages a sprite's texture and drawing.
    /// </summary>
    public class SpriteManager
    {
        // Fields
        protected Texture2D Texture;            // Texture for the sprite
        protected Rectangle[] Rectangles;       // Array of rectangles for sprite frames
        protected int FrameIndex = 0;           // Current frame index

        // Properties
        public Vector2 Position = Vector2.Zero; // Position of the sprite
        public Color Color = Color.White;       // Color tint of the sprite
        public Vector2 Origin;                  // Origin point for the sprite rotation
        public float Rotation = 0f;             // Rotation angle
        public float Scale = 1f;                // Scale factor
        public SpriteEffects SpriteEffect;      // Effects applied to the sprite

        /// <summary>
        /// Constructor for SpriteManager.
        /// </summary>
        /// <param name="Texture">Texture for the sprite.</param>
        /// <param name="frames">Number of frames in the sprite.</param>
        public SpriteManager(Texture2D Texture, int frames)
        {
            this.Texture = Texture;
            int width = Texture.Width / frames; // Calculate the width of each frame
            Rectangles = new Rectangle[frames];

            // Initialize rectangles for each frame
            for (int i = 0; i < frames; i++)
            {
                Rectangles[i] = new Rectangle(i * width, 0, width, Texture.Height);
            }
        }

        /// <summary>
        /// Draws the sprite using the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw the sprite.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, 0f);
        }
    }

    /// <summary>
    /// Manages sprite animation.
    /// </summary>
    public class SpriteAnimation : SpriteManager
    {
        // Fields
        private float _timeElapsed;     // Time elapsed since the last frame update
        private float _timeToUpdate;    // Time required to update to the next frame

        // Properties
        public bool IsLooping = true;   // Determines if the animation should loop

        /// <summary>
        /// Sets or gets the number of frames per second for the animation.
        /// </summary>
        public int FramesPerSecond { set { _timeToUpdate = (1f / value); } }

        /// <summary>
        /// Constructor for SpriteAnimation.
        /// </summary>
        /// <param name="Texture">Texture for the animated sprite.</param>
        /// <param name="frames">Number of frames in the animation.</param>
        /// <param name="fps">Frames per second for the animation.</param>
        public SpriteAnimation(Texture2D Texture, int frames, int fps) : base(Texture, frames)
        {
            FramesPerSecond = fps;
        }

        /// <summary>
        /// Updates the animation based on the elapsed game time.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if it's time to update to the next frame
            if (_timeElapsed > _timeToUpdate)
            {
                _timeElapsed -= _timeToUpdate;

                // Update the frame index, reset to 0 if looping
                if (FrameIndex < Rectangles.Length - 1)
                {
                    FrameIndex++;
                }
                else if (IsLooping)
                {
                    FrameIndex = 0;
                }
            }
        }

        /// <summary>
        /// Sets the animation frame to a specific frame.
        /// </summary>
        /// <param name="frame">The frame index to set.</param>
        public void SetFrame(int frame)
        {
            FrameIndex = frame;
        }
    }
}
