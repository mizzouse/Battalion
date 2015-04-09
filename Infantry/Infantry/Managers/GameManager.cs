using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Infantry.Managers
{
    public class GameManager : GameBase
    {
        private static Game _game;
        private static bool Set = false;

        /// <summary>
        /// Gets or initial sets our XNA game class
        /// </summary>
        public static Game Game
        {
            get { return _game; }
            set
            {
                if (_game != value && !Set)
                {
                    _game = value;
                    Set = true;
                }
            }
        }

        /// <summary>
        /// Our Game Manager class constructor
        /// </summary>
        public GameManager()
            : base()
        {
        }

        /// <summary>
        /// Our Game Manager class constructor with custom window title
        /// </summary>
        /// <param name="text">Our custom window title</param>
        public GameManager(string text)
            : base(text)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
