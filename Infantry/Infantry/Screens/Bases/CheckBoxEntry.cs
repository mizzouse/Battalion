using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Infantry.Managers;

namespace Infantry.Screens
{
    class CheckBoxEntry
    {
        /// <summary>
        /// Sets where our text will be drawn around our check box.
        /// </summary>
        public enum TextLocation
        {
            /// <summary>
            /// Text will be set to above our box
            /// </summary>
            Above,
            /// <summary>
            /// Text will be set to below our box
            /// </summary>
            Below,
            /// <summary>
            /// Text will be set to the left of our box
            /// </summary>
            Left,
            /// <summary>
            /// Text will be set to the right of our box
            /// </summary>
            Right
        }

        Rectangle box;
        Vector2 boxPosition = Vector2.Zero;
        string text;
        Vector2 textPosition = Vector2.Zero;
        float fade;
        bool drawX = false;

        /// <summary>
        /// Gets or sets the text possibly surrounding this check box.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets the rectangle used for this check box.
        /// </summary>
        public Rectangle GetRectangle
        {
            get { return box; }
        }

        /// <summary>
        /// Gets or sets the box position for this entry.
        /// </summary>
        public Vector2 BoxPosition
        {
            get { return boxPosition; }
            set 
            {
                boxPosition = value;

                if (value.X < GameManager.ViewingSize.X)
                    boxPosition = new Vector2(GameManager.ViewingSize.X, boxPosition.Y);
                else if (value.Y < GameManager.ViewingSize.Y)
                    boxPosition = new Vector2(boxPosition.X, GameManager.ViewingSize.Y);
                else if (value.Y > GameManager.ViewingSize.Height)
                    boxPosition = new Vector2(boxPosition.X, GameManager.ViewingSize.Height);

                if (boxPosition.X + box.Width > GameManager.ViewingSize.Width)
                    boxPosition.X = boxPosition.X - box.Width;
                else if (boxPosition.Y + box.Height > GameManager.ViewingSize.Height)
                    boxPosition.Y = boxPosition.Y - box.Height;

                box = new Rectangle((int)boxPosition.X, (int)boxPosition.Y, box.Width, box.Height);
            }
        }

        /// <summary>
        /// Gets or sets the text position surrounding this box.
        /// Default is to the right side of our box
        /// </summary>
        public TextLocation TextPosition { get; set; }

        /// <summary>
        /// Gets or sets the surrounding text(if any) near the box.
        /// Default is Black.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Our Event call raised when the box is clicked on
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Raises the Selected event.
        /// </summary>
        protected internal virtual void OnSelect()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);

            drawX = !drawX;
        }

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CheckBoxEntry() { TextColor = Color.Black; }

        /// <summary>
        /// Initializes the default checkbox size, usually called by a load content call.
        /// </summary>
        public void Initialize()
        {
            Vector2 vector = ScreenManager.Font.MeasureString("X");
            this.box = new Rectangle(0, 0, (int)vector.X, (int)(vector.Y / 1.2f));
        }

        /// <summary>
        /// Updates the checkbox entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool selected, GameTime gameTime)
        {
            //When the menu selection changes, the entry fades between their appearance.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (selected)
                fade = Math.Min(fade + fadeSpeed, 1);
            else
                fade = Math.Max(fade - fadeSpeed, 0);
        }

        /// <summary>
        /// Draws the checkbox entry and any set text.
        /// </summary>
        public virtual void Draw(MenuScreen screen, Vector2 position,
                    bool selected, GameTime gameTime)
        {
            Color color = Color.DarkRed;
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * fade;

            color = new Color(color.R, color.G, color.B, screen.Alpha);
            Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 6);

            //Draws a highlight around our box
            ScreenManager.SpriteBatch.Draw(GameManager.Border, new Rectangle(box.X - 1, box.Y - 1, box.Width + 2, box.Height + 2), 
                                            new Color(Color.Black.R, Color.Black.G, Color.Black.B, screen.Alpha));
            //Draw our box
            ScreenManager.SpriteBatch.Draw(GameManager.Pixel, box, new Color(Color.White.R, Color.White.G, Color.White.B, screen.Alpha));

            //Draws our check mark if selected
            if (drawX)
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "X", new Vector2(box.X, box.Y), 
                    new Color(color.R, color.G, color.B, screen.Alpha), 0, origin, scale, SpriteEffects.None, 0);

            //Draws our current text surrounding it if any
            if (!String.IsNullOrEmpty(text))
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, text, GetTextLocation(text), 
                    new Color(TextColor.R, TextColor.G, TextColor.B, screen.Alpha), 0, origin, 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Internally gets our text's location based on the box's location
        /// </summary>
        private Vector2 GetTextLocation(string text)
        {
            Vector2 length = ScreenManager.Font.MeasureString(text);
            Vector2 vector = Vector2.Zero;
            switch (TextPosition)
            {
                case TextLocation.Above:
                    vector = new Vector2(boxPosition.X - (length.X / 2), 
                        boxPosition.Y - box.Height);
                    //Lets see if it goes off our screen
                    //in either direction
                    if (vector.X < GameManager.ViewingSize.X)
                        //It does, lets move it
                        vector.X = GameManager.ViewingSize.X;

                    if (vector.X > GameManager.ViewingSize.Width)
                        //It does, lets move it
                        vector.X -= (vector.X - GameManager.ViewingSize.Width);

                    //Does the end of the string go off the edge?
                    if (vector.X + text.Length > GameManager.ViewingSize.Width)
                        //It does, correct this
                        vector.X -= (vector.X + text.Length - GameManager.ViewingSize.Width);
                    break;

                case TextLocation.Below:
                    vector = new Vector2(boxPosition.X - (length.X / 2),
                        boxPosition.Y + box.Height);
                    //Lets see if it goes off our screen
                    if (vector.X < GameManager.ViewingSize.X)
                        //It does, lets move it
                        vector.X = GameManager.ViewingSize.X;

                    if (vector.X > GameManager.ViewingSize.Width)
                        //It does
                        vector.X -= (vector.X - GameManager.ViewingSize.Width);

                    //Does the end of the string go off the edge?
                    if (vector.X + text.Length > GameManager.ViewingSize.Width)
                        //It does
                        vector.X -= (vector.X + text.Length - GameManager.ViewingSize.Width);
                    break;

                case TextLocation.Left:
                    vector = new Vector2(boxPosition.X - box.Width - length.X - ScreenManager.Font.Spacing,
                        boxPosition.Y);

                    //Lets check if it goes off screen
                    if (vector.X < GameManager.ViewingSize.X)
                    {
                        //It does, move it and the box
                        vector.X = GameManager.ViewingSize.X;
                        BoxPosition = new Vector2(vector.X + length.X + ScreenManager.Font.Spacing,
                            vector.Y);
                    }
                    break;

                case TextLocation.Right:
                    vector = new Vector2(boxPosition.X + box.Width + ScreenManager.Font.Spacing, boxPosition.Y);

                    //Lets check if it goes off screen
                    if (vector.X > GameManager.ViewingSize.Width || vector.X + length.X > GameManager.ViewingSize.Width)
                    {
                        //It does, move it and the box
                        vector.X = (GameManager.ViewingSize.Width - length.X);
                        BoxPosition = new Vector2(vector.X - ScreenManager.Font.Spacing, vector.Y);
                    }
                    break;
            }

            return vector;
        }
    }
}
