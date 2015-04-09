using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;
using Infantry.Objects;

namespace Infantry.Screens
{
    public class LoadingScreen : ScreenLayer
    {
        Thread backgroundThread;
        EventWaitHandle backgroundExit;
        GameTime loadingTime;
        TimeSpan animationTimer;
        bool _loadingSlow;
        bool _screensExited;
        int idx = 50;

        ScreenLayer[] _loadedScreens;

        private LoadingScreen(bool loadingSlow, ScreenLayer[] screens)
        {
            _loadingSlow = loadingSlow;
            _loadedScreens = screens;

            /*
            if (loadingSlow)
                TransOnTime = TimeSpan.FromSeconds(10.5);
            else
                TransOnTime = TimeSpan.FromSeconds(0.5);
             */
            TransOnTime = TimeSpan.FromSeconds(0.5);

            //If this is going to be a slow load, create a background thread
            if (loadingSlow)
            {
                backgroundThread = new Thread(BackgroundWorker);
                backgroundExit = new ManualResetEvent(false);
            }
        }

        /// <summary>
        /// Loads each screen first before moving into the next screen
        /// </summary>
        /// <param name="loadingSlow">Are we loading slow?</param>
        /// <param name="screens">What screens are needed next</param>
        public static void Load(bool loadingSlow, params ScreenLayer[] screens)
        {
            foreach (ScreenLayer screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            LoadingScreen loading = new LoadingScreen(loadingSlow, screens);
            ScreenManager.AddScreen(loading);
        }

        /// <summary>
        /// Loads our text content
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            //TextureManager.AddTexture("loading", "Fonts/3D/Dark/Loading");
        }

        /// <summary>
        /// Updates the screens
        /// </summary>
        /// <param name="screenFocus">Are we in focus?</param>
        /// <param name="covered">Are we covered?</param>
        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, covered);

            if (_screensExited)
            {
                //Start up the background thread and draw our animation
                if (backgroundThread != null)
                {
                    loadingTime = gameTime;
                    backgroundThread.Start();
                }

                ScreenManager.RemoveScreen(this);
                foreach (ScreenLayer screen in _loadedScreens)
                    if (screen != null)
                        ScreenManager.AddScreen(screen);

                //Signal the thread to exit
                if (backgroundThread != null)
                {
                    backgroundExit.Set();
                    backgroundThread.Join();
                }

                //Once the load is finished, reset the timer
                GameManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Draws our active screens
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == Screens.ScreenState.Active)
                && (ScreenManager.GetScreens().Length == 1))
                _screensExited = true;

            if (_loadingSlow)
            {
                //Animates our Loading... message
                string text = "Loading";
                animationTimer += gameTime.ElapsedGameTime;

                Viewport viewport = GameManager.Device.Viewport;
                Vector2 viewSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = ScreenManager.Font.MeasureString(text);
                Vector2 textPos = (viewSize - textSize) / 2;
                Color color = Color.GreenYellow;
                color.A = Alpha;

                int dots = (int)(animationTimer.TotalSeconds * 5) % 10;
                text += new string('.', dots);

                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, text, textPos, color);
                /*
                if (TextureManager.RenderReady("loading") && idx-- > 10)
                    ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture("loading"), new Vector2(viewSize.X / 5, viewSize.Y / 2.5f),
                        null, Color.White, 0, Vector2.Zero, 0.2f, SpriteEffects.None, 0);

                if (idx <= 0)
                    idx = 50;
                */
                ScreenManager.SpriteBatch.End();
            }
        }

        /// <summary>
        /// Worker thread that draws our animation when loading slow
        /// </summary>
        void BackgroundWorker()
        {
            long lastTime = System.Diagnostics.Stopwatch.GetTimestamp();

            while (!backgroundExit.WaitOne(1000 / 30))
            {
                GameTime gameTime = GetGameTime(ref lastTime);
                if (!DrawAnimation(gameTime))
                    UpdateGameState();

                //Did we succeed?
                //GameState is updated by our network handler
                if (GameManager.GameState == State.ZoneSelect
                    || GameManager.GameState == State.InGame)
                    return;
            }
        }

        /// <summary>
        /// Method to figure out how long its been since last thread update
        /// </summary>
        GameTime GetGameTime(ref long lastTime)
        {
            long current = System.Diagnostics.Stopwatch.GetTimestamp();
            long elapsed = current - lastTime;
            lastTime = current;

            TimeSpan elapsedTime = TimeSpan.FromTicks(elapsed *
                                TimeSpan.TicksPerSecond /
                                System.Diagnostics.Stopwatch.Frequency);

            return new GameTime(loadingTime.TotalGameTime + elapsedTime, elapsedTime);
        }

        /// <summary>
        /// Calls directly into our drawing method from our background thread
        /// to update the animation while loading.
        /// </summary>
        bool DrawAnimation(GameTime gameTime)
        {
            if ((GameManager.Device == null) || GameManager.Device.IsDisposed)
                return false;

            try
            {
                GameManager.Device.Clear(Color.Black);

                //Draw the loading screen
                Draw(gameTime);

                //If we have any notification messages, show it
                if (GameManager.MsgDisplay != null)
                {
                    GameManager.MsgDisplay.Update(gameTime);
                    GameManager.MsgDisplay.Draw(gameTime);
                }

                GameManager.Device.Present();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the game state and send them back to the previous
        /// screen if any problems occured not network related.
        /// </summary>
        void UpdateGameState()
        {
            switch (GameManager.GameState)
            {
                case State.LoggingIn:
                    GameManager.GameState = State.LogInScreen;
                    Load(false, new LoginScreen());
                    break;

                case State.EnteringZone:
                    GameManager.GameState = State.ZoneSelect;
                    Load(false, new ZoneListScreen());
                    break;
            }
        }
    }
}
