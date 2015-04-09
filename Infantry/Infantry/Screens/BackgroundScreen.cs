using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;
using Infantry.Screens;
using Infantry.Objects;

namespace Infantry.Screens
{
    public class BackgroundScreen : ScreenLayer
    {
        Rectangle _size = GameManager.ViewingSize;
        string texture = "background";
        bool custom = false;

        /// <summary>
        /// Returns the size of our background rectangle
        /// </summary>
        public Rectangle Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                    _size = value;
            }
        }

        /// <summary>
        /// Background Screen Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransOnTime = TimeSpan.FromSeconds(0.5);
            TransOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Background Screen Constructor with a gradient texture
        /// </summary>
        /// <param name="gradient">Do we want to use the gradient texture?</param>
        public BackgroundScreen(bool gradient)
        {
            TransOnTime = TimeSpan.FromSeconds(0.5);
            TransOffTime = TimeSpan.FromSeconds(0.5);

            this.custom = gradient;
            if (gradient)
                texture = "gradient";
        }

        /// <summary>
        /// Loads the texture for our background screen
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            if (custom)
                TextureManager.AddTexture(texture, "Misc/gradient");
            else
                TextureManager.AddTexture(texture, "Background/background");
        }

        /// <summary>
        /// Unloads the texture
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            TextureManager.RemoveTexture(texture);
        }

        /// <summary>
        /// Updates the background screen, this should not transition off even
        /// if covered by another screen till we are moving to zonelist.
        /// </summary>
        /// <param name="screenFocus">Are we in focus?</param>
        /// <param name="covered">Are we covered?</param>
        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, false);
        }

        /// <summary>
        /// Draws the background screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //Viewport viewport = GameManager.Device.Viewport;
            //Rectangle fullScreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = Alpha;

            ScreenManager.SpriteBatch.Begin();

            Texture2D obj = TextureManager.GetTexture(texture);
            if (obj != null)
                ScreenManager.SpriteBatch.Draw(obj, Size, new Color(fade, fade, fade));

            ScreenManager.SpriteBatch.End();
        }
    }
}
