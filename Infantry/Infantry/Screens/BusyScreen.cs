using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;
using Infantry.Objects;

namespace Infantry.Screens
{
    public class BusyScreen : ScreenLayer
    {
        TimeSpan animation;

        public string Message = "Loading";

        /// <summary>
        /// Our event completion method.
        /// </summary>
        public event EventHandler<EventArgs> EventCompleted;

        /// <summary>
        /// Our operation completion event with a bool argument.
        /// </summary>
        public event EventHandler<OperationCompletedEvent> OperationCompleted;

        /// <summary>
        /// Constructs a busy screen.
        /// </summary>
        public BusyScreen()
        {
            IsPopup = true;
            TransOnTime = TimeSpan.FromSeconds(0.1);
            TransOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Constructs a busy screen with a specified message
        /// </summary>
        public BusyScreen(string message)
        {
            this.Message = message;
            IsPopup = true;
            TransOnTime = TimeSpan.FromSeconds(0.1);
            TransOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads any needed content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            //TextureManager.AddTexture("spinner", "Misc/spinner");
        }

        /// <summary>
        /// Updates our busy screen.
        /// </summary>
        public override void Update(GameTime gameTime, 
            bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, covered);
        }

        /// <summary>
        /// Draws our busy screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            string text = Message;
            animation += gameTime.ElapsedGameTime;

            Viewport viewport = GameManager.Device.Viewport;
            Vector2 viewSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = ScreenManager.Font.MeasureString(text);

            int dots = (int)(animation.TotalSeconds * 5) % 10;
            text += new string('.', dots);

            Vector2 textPosition = (viewSize - textSize) / 2;
            Color color = new Color(255, 255, 255, Alpha);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, text, textPosition, color);
            ScreenManager.SpriteBatch.End();
        }

        /// <summary>
        /// Signals our event has completed
        /// </summary>
        public virtual void OnEventCompleted()
        {
            if (EventCompleted != null)
                EventCompleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Signals our operation has completed with optional reason
        /// </summary>
        public virtual void OnOperationCompleted(bool result, string reason)
        {
            if (OperationCompleted != null)
            {
                OperationCompletedEvent ope = new OperationCompletedEvent(result);
                ope.Reason = (String.IsNullOrEmpty(reason) ? "" : reason);
                OperationCompleted(this, ope);
            }
        }
    }
}
