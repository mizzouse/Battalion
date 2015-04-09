using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Infantry.Handlers;
using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.UI
{
    public class ChatMessage
    {
        /// <summary>
        /// The alias of who sent the message
        /// </summary>
        public String Alias;
        /// <summary>
        /// Sets the type of prompt before the message
        /// Example: (Jerks)>
        /// </summary>
        public String Prompt = String.Empty;
        /// <summary>
        /// The person's message
        /// </summary>
        public String Message;
        /// <summary>
        /// The time it was sent
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// The type of message it is
        /// </summary>
        public byte TypeChat;
        /// <summary>
        /// Custom color of the chat message
        /// </summary>
        public Color Color = Color.Transparent;

        public enum Type
        {
            /// <summary>
            /// Public chat message
            /// </summary>
            Public = 0,
            /// <summary>
            /// Private chat message(whisper)
            /// </summary>
            Private = 1,
            /// <summary>
            /// Team message
            /// </summary>
            Team = 2,
            /// <summary>
            /// A squad chat
            /// </summary>
            Squad = 3,
            /// <summary>
            /// A custom private chat like !911 using ;1;
            /// </summary>
            Custom = 4,
            /// <summary>
            /// A personal chat command
            /// </summary>
            PersonalCommand = 5,
            /// <summary>
            /// A system chat command
            /// </summary>
            SystemCommand = 6,
            /// <summary>
            /// A system alert
            /// </summary>
            SystemAlert = 7
        }

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Chat Message Constructor
        /// </summary>
        public ChatMessage(String alias, String message)
        {
            Alias = alias;
            Message = message;
        }
    }

    /// <summary>
    /// This sets the auto scrolling methods
    /// </summary>
    public enum ScrollMethod
    {
        Normal = 0,
        Auto = 1
    }

    public class ChatBox
    {
        string title;
        Rectangle chatbox;
        Rectangle chatbar;
        bool mouseDragging;
        bool active;
        float backgroundAlpha = 0.75f;
        string command, prefix = "> ";
        Color chatColor;
        float textAlpha = 1.0f;
        float padding = 4.0f;
        Vector2 fontSize;
        Vector2 textPosition;
        List<ChatMessage> log = new List<ChatMessage>();
        List<ChatMessage> buffer = new List<ChatMessage>();
        int MaxLines = 10;
        TimeSpan scrollTime = TimeSpan.Zero;
        const float scrollInterval = 0.1f;
        float blinkTime = 1.0f, currentBlinkTime;
        string scrollBuf;
        int scroll, startingPos, cursorPos = 0;
        bool initialized = false;

        /// <summary>
        /// Gets or sets the current title box that will be shown on the chatbar.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        /// Is this box initialized?
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// Does the user want the box transparent?
        /// </summary>
        public bool isTransparent { get; private set; }

        /// <summary>
        /// Gets or sets the alpha(transparency) channel for the background box
        /// <para>This range can be from 0.0 - 1.0f(0-100 %). Default is 75 %</para>
        /// </summary>
        public float BackgroundAlpha
        {
            get { return backgroundAlpha; }
            set
            {
                backgroundAlpha = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// The alpha(transparency) channel for our text inside the box.
        /// <para>The value range is 0.0 - 1.0f(0-100 %). Default is 100 %.</para>
        /// </summary>
        public float TextAlpha
        {
            get { return textAlpha; }
            set
            {
                textAlpha = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Gets or sets the blinking time period for the cursor(in seconds).
        /// <para>If this value is set to 0.0 seconds, the cursor is always on.</para>
        /// <para>Default blinking is 1.0 seconds.</para>
        /// </summary>
        public float BlinkTime
        {
            get { return blinkTime; }
            set
            {
                if (value >= 0.0f)
                    blinkTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds of the chatbox, including the position and size(in pixels).
        /// </summary>
        public Rectangle ChatBounds
        {
            get { return chatbox; }
            set
            {
                if (value != Rectangle.Empty && value.Height - padding * 2.0f >= fontSize.Y
                    && value.Width > fontSize.X)
                {
                    chatbox = value;
                    chatbar = new Rectangle(0, chatbox.Height - 12, chatbox.Width, percent(0.96f, GameManager.ViewingSize.Height));
                    calculateTextArea();
                }
            }
        }

        /// <summary>
        /// Gets the bounds of our chat bar, this bar is the above our
        /// chat box.
        /// </summary>
        public Rectangle ChatBar
        {
            get { return chatbar; }
        }

        /// <summary>
        /// Generic Chatbox Constructor using "Chat" as the chat name
        /// </summary>
        public ChatBox()
        {
            this.title = "Chat";
        }

        /// <summary>
        /// Generic ChatBox Constructor using a special title name
        /// </summary>
        public ChatBox(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// Initializes our chatbox and sets sizes for it
        /// </summary>
        public void Initialize()
        {
            fontSize = new Vector2(GameManager.Font.MeasureString("X").X,
                GameManager.Font.MeasureString("Xy").Y);
            this.ChatBounds = new Rectangle(0, (int)(GameManager.ViewingSize.Height * 0.30f),
                GameManager.ViewingSize.Width, (int)(GameManager.ViewingSize.Height * 0.30f));
            isTransparent = false;

            command = prefix;
            scrollBuf = prefix;

            initialized = true;
        }

        /// <summary>
        /// Loads our chatbar texture
        /// </summary>
        public void LoadContent()
        {
            if (!initialized)
                Initialize();

            TextureManager.AddTexture("ButtonTexture", "Misc/ButtonTexture");
        }

        /// <summary>
        /// Unloads our chatbar texture
        /// </summary>
        public void UnloadContent()
        {
            TextureManager.RemoveTexture("ButtonTexture");
        }

        /// <summary>
        /// Updates our input and scrolling time
        /// </summary>
        public void Update(GameTime gameTime)
        {
            HandleInput(GameManager.Input);
            if (scrollTime.TotalMilliseconds > TimeSpan.Zero.TotalMilliseconds)
                scrollTime -= gameTime.ElapsedGameTime;

            if (blinkTime > 0.0f)
            {
                currentBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (currentBlinkTime >= blinkTime)
                    currentBlinkTime -= currentBlinkTime;
            }
        }

        /// <summary>
        /// Draws our chatscreen and any text in it
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            ChatManager.SpriteBatch.Begin();
            if (active)
            {
                if (TextureManager.RenderReady("ButtonTexture")
                    && !isTransparent)
                {
                    //Draws our chatbox
                    ChatManager.SpriteBatch.Draw(TextureManager.GetTexture("ButtonTexture"), ChatBounds,
                        new Color(Color.Black.R, Color.Black.G, Color.Black.B, backgroundAlpha));
                    //Draws our chatbar
                    ChatManager.SpriteBatch.Draw(TextureManager.GetTexture("ButtonTexture"), chatbar,
                        new Color(Color.DarkGray.R, Color.DarkGray.G, Color.DarkGray.B, backgroundAlpha));
                    //Draws our chat title
                    ChatManager.SpriteBatch.DrawString(GameManager.Font, title.ToUpper(),
                        new Vector2(chatbar.Width / 2 - (GameManager.Font.MeasureString(title).X / 2),
                            chatbar.Height - fontSize.Y), new Color(Color.SkyBlue.R, Color.SkyBlue.G, Color.SkyBlue.B, textAlpha));
                }

                //Draws the logs first
                int i = 0;
                string header;
                foreach (ChatMessage msg in buffer.ToList())
                {
                    if (msg.Prompt != String.Empty)
                    {
                        header = StringHandler.WrapAlias(msg.Alias,
                            (int)textPosition.X,
                            msg.Prompt,
                            (int)StringHandler.StringPosition.Left);
                    }
                    else
                        header = StringHandler.WrapAlias(msg.Alias,
                            (int)textPosition.X,
                            (int)StringHandler.StringPosition.Left);

                    ChatManager.SpriteBatch.DrawString(GameManager.Font, header + prefix + msg.Message,
                        textPosition + new Vector2(0.0f, fontSize.Y * i++), new Color(chatColor.R, chatColor.G, chatColor.B, textAlpha));
                }

                //Draws our current string
                ChatManager.SpriteBatch.DrawString(GameManager.Font, scrollBuf,
                    new Vector2(textPosition.X + GameManager.Font.MeasureString(prefix).X, textPosition.Y),
                    new Color(Color.White.R, Color.White.G, Color.White.B, textAlpha));

                //Draws our blinking box if needed
                if (currentBlinkTime > currentBlinkTime / 1.5f)
                    ChatManager.SpriteBatch.Draw(GameManager.Pixel, new Rectangle((int)(textPosition.X + ((prefix.Length + cursorPos) * fontSize.X)),
                        (int)(textPosition.Y * fontSize.Y),
                        (int)fontSize.X, 1), new Color(Color.Green.R, Color.Green.G, Color.Green.B, textAlpha));
            }
            ChatManager.SpriteBatch.End();
        }

        /// <summary>
        /// Sets the max lines, text position then auto scrolls text
        /// </summary>
        void calculateTextArea()
        {
            MaxLines = (int)((ChatBounds.Height - padding * 2.0f) / fontSize.Y);
            textPosition = new Vector2(ChatBounds.X + padding + GameManager.GameSettings.PromptWidth, 
                ChatBounds.Y + padding);
            AutoScroll();
        }

        /// <summary>
        /// Adjusts the vertical lines automatically
        /// </summary>
        void AutoScroll()
        {
            buffer.Clear();
            if (log.Count > MaxLines)
            {
                for (int i = 0; i < MaxLines; i++)
                    buffer.Add(log[(log.Count + scroll) - (MaxLines - i)]);
            }
            else
                for (int i = 0; i < MaxLines; i++)
                    if (i < log.Count)
                        buffer.Add(log[i]);
        }

        /// <summary>
        /// Parses the message and displays it in the chat box.
        /// </summary>
        public void WriteLine(ChatMessage msg)
        {
            if (msg == null)
                return;

            switch (msg.TypeChat)
            {
                default:
                case (byte)ChatMessage.Type.Public:
                    chatColor = Color.White;
                    break;
                case (byte)ChatMessage.Type.Private:
                    chatColor = Color.Green;
                    break;
                case (byte)ChatMessage.Type.Team:
                    chatColor = Color.Yellow;
                    break;
                case (byte)ChatMessage.Type.Custom:
                    chatColor = Color.Gold;
                    break;
                case (byte)ChatMessage.Type.Squad:
                    msg.Prompt = "#";
                    chatColor = Color.MediumPurple;
                    break;
                case (byte)ChatMessage.Type.SystemAlert:
                    chatColor = Color.Red;
                    break;
            }

            //Did this packet come with a special color?
            if (msg.Color != Color.Transparent)
                chatColor = msg.Color;

            writeline(msg);
        }

        void writeline(ChatMessage msg)
        {
            string text = "";
            float test = ChatBounds.Width - (padding + GameManager.Font.MeasureString("_").X);

            foreach(char c in msg.Message)
            {
                if (GameManager.Font.MeasureString(text + c).X > test)
                {
                    ChatMessage chat = new ChatMessage(msg.Alias, text);
                    chat.Prompt = msg.Prompt;
                    chat.Time = msg.Time;
                    chat.TypeChat = msg.TypeChat;
                    chat.Color = msg.Color;

                    log.Add(chat);

                    text = "";
                }

                text += c;
            }

            if (text != "")
            {
                if (text.Length > 0)
                {
                    //Found more text thats less than our box width
                    ChatMessage chat = new ChatMessage(msg.Alias, text);
                    chat.Prompt = msg.Prompt;
                    chat.Time = msg.Time;
                    chat.TypeChat = msg.TypeChat;
                    chat.Color = msg.Color;

                    log.Add(chat);
                }
                else
                {
                    //Found more, make sure it just isnt a space or period
                    if (text[0] != ' ' || text[0] != '.')
                    {
                        ChatMessage chat = new ChatMessage(msg.Alias, text);
                        chat.Prompt = msg.Prompt;
                        chat.Time = msg.Time;
                        chat.TypeChat = msg.TypeChat;
                        chat.Color = msg.Color;

                        log.Add(chat);
                    }
                }
            }
            AutoScroll();
        }

        /// <summary>
        /// Helper function to determine our size for chat bars
        /// </summary>
        private int percent(float amount, int max)
        {
            return (int)(max - (max * amount));
        }

        /// <summary>
        /// Updates our chat input
        /// </summary>
        private void HandleInput(InputHandler input)
        {
            //Have we turned on/off the chatbox?
            if (input.isPressed(Keys.F12))
                active = !active;

            if (active)
            {
                //Check window resizing
                HandleMouse(input);

                //Check backspace input
                if ((input.isPressed(Keys.Back) || input.isHeld(Keys.Back))
                    && command != prefix)
                {
                    command = command.Remove(--cursorPos, 1);
                    if (GameManager.Font.MeasureString(command).X <= ChatBounds.Width)
                        stringToDraw(ScrollMethod.Normal, command, 0);
                    else
                    {
                        startingPos -= 1;
                        stringToDraw(ScrollMethod.Auto, command, startingPos);
                    }
                }

                //Check spacebar input
                else if (input.isPressed(Keys.Delete) || input.isHeld(Keys.Delete))
                {
                    command = command.Insert(cursorPos, " ");
                    cursorPos += 1;

                    if (GameManager.Font.MeasureString(command).X <= ChatBounds.Width)
                        stringToDraw(ScrollMethod.Normal, command, 0);
                    else
                    {
                        startingPos += 1;
                        stringToDraw(ScrollMethod.Auto, command, startingPos);
                    }
                }

                //Check enter input
                else if (input.isPressed(Keys.Enter) && command != prefix)
                {
                    startingPos = 0;
                    cursorPos = 0;

                    //Strip the prefix before sending
                    SendMessage(command.Substring(prefix.Length));

                    command = prefix;
                    scrollBuf = prefix;
                }

                else
                {
                    //Get typed letter
                    string letter = input.GetTypedInput;

                    if (cursorPos == command.Length)
                        command += letter;
                    else
                        command = command.Insert(cursorPos, letter);
                    cursorPos += 1;

                    if (GameManager.Font.MeasureString(command).X <= ChatBounds.Width)
                        stringToDraw(ScrollMethod.Normal, command, 0);
                    else
                    {
                        startingPos += 1;
                        stringToDraw(ScrollMethod.Auto, command, startingPos);
                    }
                    return;
                }

                //Scroll Up
                if (input.MenuUp || input.isPressed(Keys.PageUp))
                {
                    if (scroll > -(log.Count - MaxLines) && scrollTime <= TimeSpan.Zero)
                    {
                        scroll -= 1;
                        AutoScroll();
                        scrollTime = TimeSpan.FromSeconds(scrollInterval);
                    }
                }

                //Scroll Down
                if (input.MenuDown || input.isPressed(Keys.PageDown))
                {
                    if (scroll < 0 && scrollTime <= TimeSpan.Zero)
                    {
                        scroll += 1;
                        AutoScroll();
                        scrollTime = TimeSpan.FromSeconds(scrollInterval);
                    }
                }

                //Scroll Left
                if (input.MenuLeft && command != prefix)
                {
                    if (cursorPos > 0 && scrollTime <= TimeSpan.Zero)
                    {
                        cursorPos -= 1;
                        scrollTime = TimeSpan.FromSeconds(scrollInterval);
                        if (GameManager.Font.MeasureString(command).X <= ChatBounds.Width)
                            stringToDraw(ScrollMethod.Normal, command, 0);
                        else
                        {
                            startingPos -= 1;
                            stringToDraw(ScrollMethod.Auto, command, startingPos);
                        }
                    }
                }

                //Scroll Right
                if (input.MenuRight)
                {
                    if (cursorPos < command.Length && scrollTime <= TimeSpan.Zero)
                    {
                        cursorPos += 1;
                        scrollTime = TimeSpan.FromSeconds(scrollInterval);
                        if (GameManager.Font.MeasureString(command).X <= ChatBounds.Width)
                            stringToDraw(ScrollMethod.Normal, command, 0);
                        else
                        {
                            startingPos += 1;
                            stringToDraw(ScrollMethod.Auto, command, startingPos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the mouse resizing of our chatbox
        /// </summary>
        private void HandleMouse(InputHandler input)
        {
            //Check if the user is trying to resize
            if (input.isDragging(chatbar))
            {
                //Prevent user from making it smaller or larger than current window
                int height = GameManager.ViewingSize.Height;
                if (height - input.CurrentMouse.Y > 0 && height - input.CurrentMouse.Y < height - padding * 2.0f)
                {
                    chatbar.Y = input.CurrentMouse.Y;
                    chatbar.Height = percent(0.96f, GameManager.ViewingSize.Height);

                    chatbox.Y = input.CurrentMouse.Y + 12;
                    chatbox.Height = height - input.CurrentMouse.Y;

                    //Reset the buffers
                    calculateTextArea();
                }
            }
        }

        /// <summary>
        /// Draws our current string
        /// </summary>
        private void stringToDraw(ScrollMethod type, string text, int position)
        {
            switch (type)
            {
                case ScrollMethod.Normal:
                    scrollBuf = text;
                    break;
                case ScrollMethod.Auto:
                    scrollBuf = text.Substring(position);
                    break;
            }
        }

        /// <summary>
        /// Parses our message then sends it
        /// </summary>
        private void SendMessage(string msg)
        {
            int type;

            //Get the type of message
            switch (msg[0])
            {
                case '?':
                    if (msg.Length > 0)
                        type = (int)ChatMessage.Type.PersonalCommand;
                    else
                        type = (int)ChatMessage.Type.Public;
                    break;

                case '*':
                    type = (int)ChatMessage.Type.SystemCommand;
                    break;

                case ':':
                    type = (int)ChatMessage.Type.Private;
                    break;

                case '/':
                case '\'':
                    type = (int)ChatMessage.Type.Team;
                    break;

                case '#':
                    type = (int)ChatMessage.Type.Squad;
                    break;

                case ';':
                    type = (int)ChatMessage.Type.Custom;
                    break;

                default:
                    type = (int)ChatMessage.Type.Public;
                    break;
            }

            //Send it
            Network.CS_Chat chat = new Network.CS_Chat(type, msg);
        }

        /// <summary>
        /// Returns the name of this object
        /// </summary>
        public override string ToString()
        {
            return title.ToString();
        }
    }
}
