using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Infantry.Screens
{
    public class FPSCounter : GameComponent
    {
        private float interval = 1.0f;
        private float lastUpdate = 0.0f;
        private float frameCount = 0;
        private float fps = 0;

        /// <summary>
        /// FPS Counter Update event
        /// </summary>
        public event EventHandler<EventArgs> Updated;

        /// <summary>
        /// How many frames per second
        /// </summary>
        public float FrameCount
        {
            get { return fps; }
        }

        /// <summary>
        /// Our base counter constructor
        /// </summary>
        public FPSCounter(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Updates the fps counter when game updates itself.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCount++;

            lastUpdate += elapsed;
            if (lastUpdate > interval)
            {
                fps = frameCount / lastUpdate;
                frameCount = 0;
                lastUpdate -= interval;

                if (Updated != null)
                    Updated(this, new EventArgs());
            }
        }
    }
}
