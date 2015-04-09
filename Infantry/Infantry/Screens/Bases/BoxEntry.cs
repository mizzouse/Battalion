using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Infantry.Managers;

namespace Infantry.Screens
{
    class BoxEntry
    {
        Rectangle box;
        Vector2 boxPosition = Vector2.Zero;
        string text, previousText;
        int caretPos = 0, selectionStart, selectionEnd;
        float blinkTime = 1.0f, currentBlinkTime;
        float fade;
        bool inputEnabled, scrambleInput = false;
        bool releasedMouse = true;
        Dictionary<int, Rectangle> characters = new Dictionary<int, Rectangle>();

        /// <summary>
        /// Gets or sets the text currently in this box
        /// </summary>
        public string Text
        {
            get { return text; }
            set 
            { 
                text = value;
                if (caretPos > text.Length)
                    caretPos = text.Length;

                if (String.IsNullOrEmpty(previousText))
                    previousText = value;
            }
        }

        /// <summary>
        /// Gets or sets the size and/or position of this box entry
        /// </summary>
        public Rectangle Size
        {
            get { return box; }
            set { box = value; }
        }

        /// <summary>
        /// Gets the rectangle used for this button
        /// </summary>
        public Rectangle GetRectangle
        {
            get { return box; }
        }

        /// <summary>
        /// Gets or sets the text position for this entry
        /// Note: If this isnt set, it uses the default box position x/y
        /// </summary>
        public Vector2 Position
        {
            get 
            {
                if (boxPosition != Vector2.Zero)
                    return boxPosition;
                else
                    return new Vector2(Size.X, Size.Y);
            }
            set { boxPosition = value; }
        }

        /// <summary>
        /// Can you type in this box?
        /// </summary>
        public bool InputEnabled
        {
            get { return inputEnabled; }
            set { inputEnabled = value; }
        }

        /// <summary>
        /// Are we scrambling the text?
        /// </summary>
        public bool ScrambleInput
        {
            get { return scrambleInput; }
            set { scrambleInput = value; }
        }

        /// <summary>
        /// Event call raised when the box is clicked on
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
        /// Generic Constructor
        /// </summary>
        public BoxEntry() { }

        /// <summary>
        /// Custom Constructor using a set text in our box
        /// </summary>
        public BoxEntry(string text)
        {
            this.Text = text;
            previousText = text;
        }

        /// <summary>
        /// Updates the box entry
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

            //If we can type in this box, and this box is "selected"
            //lets animate our text
            if (inputEnabled && selected)
            {
                if (blinkTime > 0.0f)
                {
                    currentBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (currentBlinkTime >= blinkTime)
                        currentBlinkTime -= currentBlinkTime;
                }
                handleInput(GameManager.Input);
            }
        }

        /// <summary>
        /// Draws the box entry, this can be overridden to customize
        /// </summary>
        public virtual void Draw(MenuScreen screen, Vector2 position,
            bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.DarkRed : Color.Black;
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * fade;

            color = new Color(color.R, color.G, color.B, screen.Alpha);
            Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 6);

            //Draw our border around our box
            ScreenManager.SpriteBatch.Draw(GameManager.Border, new Rectangle((int)(position.X + Size.X - 1), Size.Y - 1, Size.Width + 2, Size.Height + 2),
                                                                Color.Black);
            //Draw our box
            ScreenManager.SpriteBatch.Draw(GameManager.Pixel, 
                new Rectangle((int)(position.X + Size.X), Size.Y, Size.Width, Size.Height), Color.White);

            //Draw our current string scrambled
            if (scrambleInput && text != previousText)
            {
                string str = "";
                foreach (char c in Text)
                    str += "*";

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, str, position + Position, color,
                    0, origin, scale, SpriteEffects.None, 0);
            }
            else
                //We are not scrambling, just draw current string
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, Text, position + Position, color,
                    0, origin, scale, SpriteEffects.None, 0);

            //Draws our blinking box if needed
            if (inputEnabled && isSelected && text != previousText)
            {
                Vector2 fontSize = new Vector2(GameManager.Font.MeasureString("X").X,
                                               GameManager.Font.MeasureString("Xy").Y);
                if (currentBlinkTime > currentBlinkTime / 1.5f)
                    ScreenManager.SpriteBatch.Draw(GameManager.Pixel,
                        new Rectangle((int)(position.X + Position.X + (caretPos * fontSize.X)),
                            (int)(position.Y + Position.Y), (int)(fontSize.X * scale), (int)fontSize.Y),
                            new Color(Color.Green.R, Color.Green.G, Color.Green.B, screen.Alpha));

                if (selectionStart >= 0 && (selectionEnd > 0 || selectionEnd < 0))
                     ScreenManager.SpriteBatch.Draw(GameManager.Pixel,
                         new Rectangle((int)(position.X + (Position.X + selectionStart)),
                             (int)(position.Y + Position.Y), (int)(selectionStart + selectionEnd + (fontSize.X * scale)), (int)fontSize.Y), 
                             new Color(Color.Green.R, Color.Green.G, Color.Green.B, screen.Alpha / 2));
            }
        }

        /// <summary>
        /// Handles keyboard/mouse input if typing is enabled
        /// </summary>
        private void handleInput(Handlers.InputHandler input)
        {
#if !XBOX || !XBOX360
            //Do we have text to edit that doesnt match our initial text?
            if (!String.IsNullOrEmpty(text) && text != previousText)
            {
                int characterPos = (int)((input.CurrentMouse.X - Position.X) / (GameManager.Font.Spacing));

                //Have we clicked on a position in our text?
                if (input.hasPressed)
                {
                    //Get caret position
                    if (characterPos > text.Length)
                        caretPos = text.Length;
                    else if (characterPos < 0)
                        caretPos = 0;
                    else
                        caretPos = characterPos;
                }

                //Is this the first time we are holding down the mouse?
                else if (input.isHolding && releasedMouse)
                {
                    releasedMouse = false;
                    caretPos = characterPos;
                    selectionStart = caretPos;
                    selectionEnd = caretPos;
                }

                //Are we finally letting go of the mouse?
                else if (input.hasReleased && !releasedMouse)
                    releasedMouse = true;

                else
                {
                    //We must be still holding our mouse, lets update
                    if (!releasedMouse)
                    {
                        if (characterPos > caretPos
                            && caretPos < text.Length)
                        {
                            caretPos++;
                            selectionEnd = caretPos;
                        }

                        if (characterPos < caretPos
                            && caretPos > 0)
                        {
                            caretPos--;
                            selectionStart = caretPos;
                        }
                    }
                }
            }
#endif

            if ((input.isPressed(Keys.Back) || input.isHeld(Keys.Back))
                && caretPos > 0)
                text = text.Remove(--caretPos, 1);
            else if ((input.isPressed(Keys.Delete) || input.isHeld(Keys.Delete))
                && caretPos < text.Length)
                text = text.Remove(caretPos, 1);
            else if (input.isPressed(Keys.Left) && caretPos > 0)
                caretPos--;
            else if (input.isPressed(Keys.Right) && caretPos < text.Length)
                caretPos++;
            else if (input.isPressed(Keys.Home))
                caretPos = 0;
            else if (input.isPressed(Keys.End))
                caretPos = text.Length;
            else
            {
                string letter = input.GetTypedInput;
                if (!String.IsNullOrEmpty(letter))
                    if (GameManager.Font.MeasureString(text + letter).X < Size.Width)
                    {
                        if (caretPos == text.Length)
                            text += letter;
                        else
                            text = text.Insert(caretPos, letter);
                        caretPos++;
                    }
            }
        }
    }
}
