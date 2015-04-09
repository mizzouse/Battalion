using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;

namespace Infantry.Screens
{
    /// <summary>
    /// Helper class which stores the message to display when
    /// slow loading.
    /// </summary>
    public class MessageDisplay : DrawableGameComponent, IMessageDisplay
    {
        List<NotifyMessage> messages = new List<NotifyMessage>();

        object syncObject = new object();

        static readonly TimeSpan fadeInTime = TimeSpan.FromSeconds(0.25);
        static readonly TimeSpan showTime = TimeSpan.FromSeconds(5);
        static readonly TimeSpan fadeOutTime = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// Generic Constructor with specified text and position
        /// </summary>
        public MessageDisplay(Game game)
            : base(game)
        {
            //Register ourselves as a service
            game.Services.AddService(typeof(IMessageDisplay), this);
        }

        /// <summary>
        /// Updates the messages
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            lock (syncObject)
            {
                int index = 0;
                float targetPosition = 0; //Y coordinate

                while (index < messages.Count)
                {
                    NotifyMessage message = messages[index];

                    //Gradually slide the message up
                    float delta = targetPosition - message.ListPosition;
                    float velocity = (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

                    message.ListPosition += delta * Math.Min(velocity, 1);

                    //Update the age of the message
                    message.Age += gameTime.ElapsedGameTime;
                    if (message.Age < showTime + fadeOutTime)
                    {
                        //This message is still alive
                        index++;

                        //Any messages should be positioned below
                        //this one and push the old above unless already
                        //fading out.
                        if (message.Age < showTime)
                            targetPosition++;
                    }
                    else
                        //Message is old, delete it
                        messages.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Draws our messages in our list
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            lock (syncObject)
            {
                if (messages.Count == 0)
                    return;

                Vector2 position = new Vector2(50, GameManager.Device.Viewport.Height / 4);
                if (ScreenManager.ScreenExists("gamescreen"))
                {
                    UI.ChatBox chat;
                    if ((chat = ChatManager.GetChat("Chat")) != null)
                        //Note: ChatBar.X(starting pos) ChatBar.Height(height length)
                        position = new Vector2(chat.ChatBar.X, chat.ChatBar.Height);
                }

                ScreenManager.SpriteBatch.Begin();

                foreach (NotifyMessage msg in messages)
                {
                    const float scale = 0.75f;
                    float alpha = 1;

                    //Check special text position
                    if (msg.TextPosition != Vector2.Zero)
                        position = msg.TextPosition;

                    if (msg.Age < fadeInTime)
                        //Fade In
                        alpha = (float)(msg.Age.TotalSeconds / fadeInTime.TotalSeconds);

                    else if (msg.Age > showTime)
                    {
                        //Fade Out
                        TimeSpan fadeOut = showTime + fadeOutTime - msg.Age;
                        alpha = (float)(fadeOut.TotalSeconds / fadeOutTime.TotalSeconds);
                    }

                    //Compute message position
                    position.Y = msg.ListPosition * GameManager.Font.LineSpacing * scale;

                    //Compute origin value to right align the text
                    Vector2 origin = GameManager.Font.MeasureString(msg.Text);
                    origin.Y = 0;

                    //Draw the text with a drop shadow
                    ScreenManager.SpriteBatch.DrawString(GameManager.Font, msg.Text, position + Vector2.One,
                                                        Color.Green * alpha, 0, origin, scale, SpriteEffects.None, 0);
                    ScreenManager.SpriteBatch.DrawString(GameManager.Font, msg.Text, position, Color.DarkGreen * alpha, 0,
                                                        origin, scale, SpriteEffects.None, 0);
                }

                ScreenManager.SpriteBatch.End();
            }
        }

        /// <summary>
        /// Shows a new notification message with a specified text position.
        /// </summary>
        public void ShowMessage(string message, Vector2 textPos, params object[] parameters)
        {
            string format = String.Format(message, parameters);
            lock(syncObject)
            {
                float startPos = messages.Count;
                NotifyMessage notify = new NotifyMessage(format, startPos);
                notify.TextPosition = textPos;

                messages.Add(notify);
            }
        }

        /// <summary>
        /// Shows a new notification message.
        /// </summary>
        public void ShowMessage(string message, params object[] parameters)
        {
            ShowMessage(message, Vector2.Zero, parameters);
        }

        /// <summary>
        /// Helper class stores the position and text of a single notification message.
        /// </summary>
        class NotifyMessage
        {
            public string Text;
            public Vector2 TextPosition;
            public float ListPosition;
            public TimeSpan Age;


            public NotifyMessage(string text, float position)
            {
                Text = text;
                ListPosition = position;
                Age = TimeSpan.Zero;
            }
        }
    }
}
