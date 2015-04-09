using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Screens;
using Infantry.Objects;

namespace Infantry.Managers
{
    public class ScreenManager : DrawableGameComponent
    {
        private static List<ScreenLayer> _screens = new List<ScreenLayer>();
        private static List<ScreenLayer> _updateScreens = new List<ScreenLayer>();
        private static SpriteBatch _spriteBatch;
        private static SpriteFont _font;
        private static bool _initialized = false;
        static List<string> names = new List<string>();

        /// <summary>
        /// Does a certain screen exist?
        /// </summary>
        /// <param name="type">Screen type</param>
        /// <returns>Returns true if found, false if not</returns>
        public static bool ScreenExists(string type)
        {
            foreach (ScreenLayer screen in _screens.ToArray())
                if (screen.GetType().Name.ToLower() == type)
                    return true;

            return false;
        }

        /// <summary>
        /// Gets a specified screen layer, returns null if not found.
        /// </summary>
        public static ScreenLayer GetScreen(string name)
        {
            foreach (ScreenLayer screen in _screens.ToArray())
                if (screen.GetType().Name.ToLower() == name)
                    return screen;

            return null;
        }

        /// <summary>
        /// Gets a copied collection of all the screens our manager currently has
        /// </summary>
        /// <returns></returns>
        public static ScreenLayer[] GetScreens()
        {
            return _screens.ToArray();
        }

        /// <summary>
        /// A default sprite batch used by all screens
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        /// <summary>
        /// Our default font shared by all screens
        /// </summary>
        public static SpriteFont Font
        {
            get { return _font; }
        }

        /// <summary>
        /// Is the Screen Manager initialized? Used for setup
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// Returns a list of the current screens
        /// </summary>
        public static string TracedScreens
        {
            get
            {
                //Updates our list
                TraceScreens();
                
                string output = string.Join(", ", names);
                return output;
            }
        }

        /// <summary>
        /// Our screen manager constructor
        /// </summary>
        /// <param name="game"></param>
        public ScreenManager(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Loads all basic content for our Screen Manager
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GameManager.Device);
            _font = GameManager.Contents.Load<SpriteFont>("Fonts/Lucida Console");
            GameManager.Font = _font;

            //Loads our fade in/out texture
            TextureManager.AddTexture("blank", "Misc/blank");
            foreach (ScreenLayer screen in _screens)
                screen.LoadContent();
        }

        /// <summary>
        /// Unloads all content for all screens and the screen manager itself
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (ScreenLayer screen in _screens)
                screen.UnloadContent();
        }

        /// <summary>
        /// Initializes each screen and the screen manager itself
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _initialized = true;
        }

        /// <summary>
        /// Allows each screen to run logic
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Lets make a copy of the master list
            _updateScreens.Clear();

            foreach (ScreenLayer screen in _screens)
                _updateScreens.Add(screen);

            bool hasFocus = !Game.IsActive;
            bool covered = false;

            while (_updateScreens.Count > 0)
            {
                ScreenLayer screen = _updateScreens[_updateScreens.Count - 1];
                _updateScreens.RemoveAt(_updateScreens.Count - 1);

                // Update the screen.
                screen.Update(gameTime, hasFocus, covered); 

                if (screen.ScreenState == ScreenState.On ||
                    screen.ScreenState == ScreenState.Active)
                {
                    if (!hasFocus)
                    {
                        screen.HandleInput(GameManager.Input);
                        hasFocus = true;
                    }

                    if (!screen.IsPopup)
                        covered = true;
                }
            }
        }

        /// <summary>
        /// Prints a list of all the screens
        /// </summary>
        static void TraceScreens()
        {
            names.Clear();
            foreach (ScreenLayer screen in _screens)
                names.Add(screen.GetType().Name);

            //For debugging purposes later
            Trace.WriteLine(string.Join(", ", names.ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (ScreenLayer screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the manager
        /// </summary>
        public static void AddScreen(ScreenLayer screen)
        {
            _screens.Add(screen);
            if (_initialized)
                screen.LoadContent();
        }

        /// <summary>
        /// Removes a screen from the manager
        /// </summary>
        /// <param name="screen"></param>
        public static void RemoveScreen(ScreenLayer screen)
        {
            if (screen != null)
                screen.UnloadContent();

            _screens.Remove(screen);
            _updateScreens.Remove(screen);
        }

        /// <summary>
        /// Draws a translucent black sprite, used for fading screens
        /// and darkening the background behind popups.
        /// </summary>
        /// <param name="alpha"></param>
        public static void Fade(int alpha)
        {
            Viewport viewport = GameManager.Device.Viewport;

            _spriteBatch.Begin();

             Texture2D texture = (TextureManager.GetTextureObj("blank") == null ? null : TextureManager.GetTextureObj("blank").BaseTexture);
             if (texture != null)
                 _spriteBatch.Draw(texture, new Rectangle(0, 0, viewport.Width, viewport.Height),
                     new Color(0, 0, 0, (byte)alpha));
            _spriteBatch.End();
        }
    }
}
