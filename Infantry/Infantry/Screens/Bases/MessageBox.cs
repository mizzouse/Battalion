using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Infantry.Managers;
using Infantry.Objects;

namespace Infantry.Screens
{
    class MessageBox : ScreenLayer
    {
        private Rectangle OK;
        private Rectangle Cancel;
        private const string texture = "gradient";
        private string _message;
        private bool okSelected = true;
        Color colorW = Color.White;
        Color colorY = Color.Yellow;
        float fade;
        bool okOnly;

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        /// <summary>
        /// Gets or Sets the text for the box.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _message = "";
                _message = value;
            }
        }

        /// <summary>
        /// Generic Object Constructor that does not set a message string but
        /// does allow the option of an "OK" button or both "Ok/Cancel".
        /// </summary>
        /// <param name="OkOnly"></param>
        public MessageBox(bool OkOnly)
        {
            okOnly = OkOnly;

            IsPopup = true;

            TransOnTime = TimeSpan.FromSeconds(0.2);
            TransOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Message Box Constructor with "Ok, Cancel" text by default
        /// Note: You can either use the internal ok/cancel event
        /// or create one using Accepted/Cancelled events
        /// </summary>
        /// <param name="message">Our message box title</param>
        public MessageBox(string message)
            : this(message, false)
        {
        }

        /// <summary>
        /// Message Box Constructor with optional ok and cancel inclusion
        /// </summary>
        /// <param name="message">Our message box text</param>
        /// <param name="includeText">Are we including both buttons?</param>
        public MessageBox(string message, bool OKonly)
        {
            _message = message;
            okOnly = OKonly;

            IsPopup = true;

            TransOnTime = TimeSpan.FromSeconds(0.2);
            TransOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads the graphic content for this screen. This uses the shared
        /// Content Manager provided by the Game Base class.
        /// </summary>
        public override void LoadContent()
        {
            TextureManager.AddTexture("gradient", "Misc/gradient");
        }

        /// <summary>
        /// Handles user input for our message box
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(Handlers.InputHandler input)
        {
            if (input.MenuUp)
                okSelected = true;
            else if (input.MenuDown)
                okSelected = false;

            if (input.MenuSelect && !okSelected)
            {
                //Raises the cancelled event then exits
                if (Cancelled != null)
                    Cancelled(this, EventArgs.Empty);

                ExitScreen();
            }

            else if (input.MenuSelect)
            {
                //Raises the accepted event call then exits
                if (Accepted != null)
                    Accepted(this, EventArgs.Empty);

                ExitScreen();
            }
            else if (input.MenuCancel)
            {
                //Raises the cancelled event call then exits
                if (Cancelled != null)
                    Cancelled(this, EventArgs.Empty);

                ExitScreen();
            }

            //Check mouse input
#if !XBOX || !XBOX360
            if (input.isHighlighting(OK))
            {
                okSelected = true;
                if (input.hasClickedOn(OK))
                {
                    if (Accepted != null)
                        Accepted(this, EventArgs.Empty);
                    ExitScreen();
                }
            }

            if (!okOnly && input.isHighlighting(Cancel))
            {
                okSelected = false;
                if (input.hasClickedOn(Cancel))
                {
                    if (Cancelled != null)
                        Cancelled(this, EventArgs.Empty);
                    ExitScreen();
                }
            }
#endif
        }

        /// <summary>
        /// Updates the message box text graphics only
        /// </summary>
        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, covered);

            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            fade = Math.Min(fade + fadeSpeed, 1);
        }

        /// <summary>
        /// Draws the message box
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //Darkens any other screen beneath the popup box
            ScreenManager.Fade(Alpha * 2 / 3);

            //Center the message
            Viewport viewport = GameManager.Device.Viewport;
            Vector2 viewSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = ScreenManager.Font.MeasureString(_message + "\nOk" + "\nCancel");
            Vector2 textPos = (viewSize - textSize) / 2;

            //Background includes a border already
            const int h = 32; //Horizontal
            const int v = 16; //Vertical

            Rectangle backgroundRect = new Rectangle((int)textPos.X - h,
                (int)textPos.Y - v, (int)textSize.X + h * 2, (int)textSize.Y + v * 2);

            textPos.Y = (int)textPos.Y + v / 2;

            //Update our button rectangles
            Vector2 okSize = ScreenManager.Font.MeasureString("Ok");
            Vector2 cancelSize = ScreenManager.Font.MeasureString("Cancel");
            OK.X = (int)textPos.X;
            OK.Y = (int)textPos.Y;
            OK.Width = (int)okSize.X;
            OK.Height = ScreenManager.Font.LineSpacing * 2;

            Cancel.X = (int)textPos.X;
            Cancel.Y = (int)textPos.Y + OK.Height;
            Cancel.Width = (int)cancelSize.X;
            Cancel.Height = OK.Height;

            //Fade the alpha color during transitions
            Color color = new Color(255,255,255, Alpha);
            colorY = new Color(colorY.R, colorY.G, colorY.B, Alpha);
            colorW = new Color(colorW.R, colorW.G, colorW.B, Alpha);
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * fade;
            Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Texture2D obj = TextureManager.GetTexture(texture);
            if (obj != null)
                //Draw the rectangle
                ScreenManager.SpriteBatch.Draw(obj, backgroundRect, color);

            //Draw the text
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _message, textPos, colorW, 0,
                origin, 1, SpriteEffects.None, 0);
            if (okOnly)
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "\nOk", textPos, colorY, 0, 
                    origin, scale, SpriteEffects.None, 0);
            else
            {
                if (okSelected)
                {
                    ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "\nOk", textPos, colorY, 0,
                        origin, scale, SpriteEffects.None, 0);
                    ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "\n\nCancel", textPos, colorW, 0,
                        origin, 1, SpriteEffects.None, 0);
                }
                else
                {
                    ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "\nOk", textPos, colorW, 0,
                        origin, 1, SpriteEffects.None, 0);
                    ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "\n\nCancel", textPos, colorY, 0,
                        origin, scale, SpriteEffects.None, 0);
                }
            }
            ScreenManager.SpriteBatch.End();
        }
    }
}
