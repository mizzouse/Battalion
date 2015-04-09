using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;

namespace Infantry.Screens
{
    class ButtonEntry
    {
        Rectangle button;
        string text;
        float fade;
        bool customTexture = false;

        /// <summary>
        /// Gets or sets the text of this button
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets the rectangle used for this button
        /// </summary>
        public Rectangle GetRectangle
        {
            get { return button; }
        }

        /// <summary>
        /// Are we using a button texture?
        /// </summary>
        public bool ButtonTexture
        {
            get { return customTexture; }
        }

        /// <summary>
        /// Event raised when the button is selected
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Raises the Selected event
        /// </summary>
        protected internal virtual void OnSelect()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Button Constructor with a specified text
        /// </summary>
        public ButtonEntry(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Button Constructor with a specified text and using a texture?
        /// </summary>
        public ButtonEntry(string text, bool texture)
        {
            this.text = text;
            this.customTexture = texture;
            if (texture)
                TextureManager.AddTexture("buttontexture", "Misc/ButtonTexture");
        }

        /// <summary>
        /// Updates the button entry
        /// </summary>
        /// <param name="selected">Have we selected it?</param>
        public virtual void Update(MenuScreen screen, bool selected, GameTime gameTime)
        {
            //When the menu selection changes, the entries fade between their appearances.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (selected)
                fade = Math.Min(fade + fadeSpeed, 1);
            else
                fade = Math.Max(fade - fadeSpeed, 0);
        }

        /// <summary>
        /// Draws the button entry, this can be overridden to customize appearance
        /// </summary>
        /// <param name="position">Our button position</param>
        /// <param name="selected">Are we selecting it?</param>
        public virtual void Draw(MenuScreen screen, Vector2 Position,
            bool selected, GameTime gameTime)
        {
            Vector2 position = Position;
            if (!String.IsNullOrEmpty(text))
                position.X = Position.X - (ScreenManager.Font.MeasureString(text).X / 2);

            Color color = selected ? Color.Yellow : Color.White;
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * fade;

            color = new Color(color.R, color.G, color.B, screen.Alpha);
            Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);

            //Lets update our button rectangle
            button.X = (int)position.X;
            button.Y = (int)position.Y;
            button.Width = (int)ScreenManager.Font.MeasureString(text).X;
            button.Height = ScreenManager.Font.LineSpacing + 5;

            if (customTexture)
            {
                Rectangle buttonTexture = new Rectangle(button.X - 5 - (int)origin.X, button.Y - (int)origin.Y, button.Width + 15 + (int)origin.X, button.Height);
                if (TextureManager.RenderReady("buttontexture"))
                    ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture("buttontexture"), buttonTexture, Color.White);
            }
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, text, position, color,
                0, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this button requires
        /// </summary>
        /// <returns>Returns the line spacing</returns>
        public virtual int GetHeight(MenuScreen screen)
        {
            return ScreenManager.Font.LineSpacing + 5;
        }
    }
}
