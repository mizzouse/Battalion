using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.UI;

namespace Infantry.Managers
{
    public class ChatManager : DrawableGameComponent
    {
        private static SpriteBatch _batch;
        private static List<ChatBox> _chats = new List<ChatBox>();
        private static List<ChatBox> _update = new List<ChatBox>();
        private static bool _initialized = false;

        /// <summary>
        /// Is the Chat Manager initialized?
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// A default sprite batch used by all chats
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get { return _batch; }
        }

        /// <summary>
        /// Gets a specific chat box using the name
        /// </summary>
        public static ChatBox GetChat(string name)
        {
            foreach (ChatBox chat in _chats)
            {
                if (chat.Title == name)
                    return chat;
            }
            return null;
        }

        /// <summary>
        /// Writes a line to a specific chat box
        /// </summary>
        public static void WriteLine(ChatMessage msg, string boxName)
        {
            foreach (ChatBox chat in _chats)
                if (chat.Title == boxName)
                {
                    chat.WriteLine(msg);
                    break;
                }
        }

        /// <summary>
        /// Chat Manager constructor, creates a new component.
        /// </summary>
        /// <param name="game"></param>
        public ChatManager(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Initializes our chat manager component
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach(ChatBox chat in _chats)
                chat.Initialize();

            _initialized = true;
        }

        /// <summary>
        /// Loads the content and initializes the spritebatch
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _batch = new SpriteBatch(GameManager.Device);
            foreach (ChatBox chat in _chats)
                chat.LoadContent();
        }

        /// <summary>
        /// Unloads all content
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (ChatBox chat in _chats)
                chat.UnloadContent();
        }

        /// <summary>
        /// Allows each chat box to run logic
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            //Make a copy of the master list
            _update.Clear();
            foreach (ChatBox chat in _chats)
                _update.Add(chat);

            while (_update.Count > 0)
            {
                ChatBox chat = null;
                if (_update.Count == 1)
                {
                    //Just update the first one
                    chat = _update[1];
                    _update.RemoveAt(1);
                }
                else
                {
                    chat = _update[_update.Count - 1];
                    _update.RemoveAt(_update.Count - 1);
                }

                if (chat != null)
                    chat.Update(gameTime);
            }
        }

        /// <summary>
        /// Tells each chatbox to draw itself
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (ChatBox chat in _chats)
                chat.Draw(gameTime);
        }

        /// <summary>
        /// Adds a new chat box to the manager
        /// </summary>
        public static void AddChatBox(ChatBox box)
        {
            _chats.Add(box);
            if (_initialized)
                box.LoadContent();
        }

        /// <summary>
        /// Removes a chat box from the manager.
        /// </summary>
        public static void RemoveChatBox(ChatBox box)
        {
            if (_initialized)
                box.UnloadContent();

            _chats.Remove(box);
            _update.Remove(box);
        }
    }
}
