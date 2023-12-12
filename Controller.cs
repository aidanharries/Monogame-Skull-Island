using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skull_Island
{
    /// <summary>
    /// Manages the spawning of enemies and the timing of their appearance
    /// </summary>
    public class Controller
    {
        // Fields
        public static double Timer = 3D;                // Current countdown timer for enemy spawning
        public static double MaxTime = 3D;              // Maximum time between enemy spawns

        private static Random _random = new Random();   // Random number generator

        /// <summary>
        /// Updates the controller logic.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteSheet">Texture for the enemy sprite.</param>
        public static void Update(GameTime gameTime, Texture2D spriteSheet)
        {
            // Decrease the timer based on elapsed game time
            Timer -= gameTime.ElapsedGameTime.TotalSeconds;

            // Check if it's time to spawn a new enemy
            if (Timer <= 0)
            {
                // Randomly select a side for the enemy to appear
                int side = _random.Next(4);

                // Create and add an enemy based on the selected side
                switch (side)
                {
                    case 0:
                        Enemy.Enemies.Add(new Enemy(new Vector2(-500, _random.Next(-500, 2000)), spriteSheet));
                        break;
                    case 1:
                        Enemy.Enemies.Add(new Enemy(new Vector2(2000, _random.Next(-500, 2000)), spriteSheet));
                        break;
                    case 2:
                        Enemy.Enemies.Add(new Enemy(new Vector2(_random.Next(-500, 2000), -500), spriteSheet));
                        break;
                    case 3:
                        Enemy.Enemies.Add(new Enemy(new Vector2(_random.Next(-500, 2000), 2000), spriteSheet));
                        break;
                }

                // Reset the timer and reduce the maximum time if applicable
                Timer = MaxTime;
                if (MaxTime > 0.5)
                {
                    MaxTime -= 0.02D;
                }
            }
        }

        /// <summary>
        /// Resets the timer to its initial state.
        /// </summary>
        public static void ResetTimer()
        {
            Timer = 3D; // Resetting to the initial timer value
            MaxTime = 3D; // Resetting to the initial maximum time value
        }
    }
}
