using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Infantry.Managers;
using Infantry.UI;

namespace Infantry.Screens
{
    public class GameScreen : ScreenLayer
    {
        private static double delta = 0.0;
        //Atmosphere atmosphere = new Atmosphere();
        ChatBox box = new ChatBox("Chat");

        public GameScreen()
        {
            TransOnTime = TimeSpan.FromSeconds(1.5);
            TransOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads all graphics content
        /// </summary>
        public override void LoadContent()
        {
            CameraManager.SetActiveCamera((int)CameraManager.CameraType.Overhead);
            CameraManager.ActiveCamera.Position = new Vector3(0.0f, 0.0f, -30.0f);
            CameraManager.ActiveCamera.NearPlane = 0.1f;
            CameraManager.ActiveCamera.FarPlane = 5000f;
            CameraManager.SetAllProjections(GameManager.AspectRatio);

            //Sky stuff here

            //Terrain stuff here

            ChatManager.AddChatBox(box);

            //Once loaded, reset the game time so it doesnt play catch up
            GameManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload any graphic content
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, covered);

            delta = gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Updates any input
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(Handlers.InputHandler input)
        {
            /*
            if (input.isPressed(Keys.Up))
                atmosphere.SunDirection += 5f * (float)delta;
            if (input.isPressed(Keys.Down))
                atmosphere.SunDirection -= 5f * (float)delta; */
            if (input.isPressed(Keys.F1))
                CameraManager.SetActiveCamera((int)CameraManager.CameraType.Overhead);
            if (input.isPressed(Keys.F2))
                CameraManager.SetActiveCamera((int)CameraManager.CameraType.FirstPerson);
            if (input.isPressed(Keys.T))
                CameraManager.ActiveCamera.Translate(new Vector3(0, 100f * (float)delta, 0.0f));
            if (input.isHeld(Keys.C))
            {
                CameraManager.ActiveCamera.RotateX(input.MouseMoved.Y);
                CameraManager.ActiveCamera.RotateY(input.MouseMoved.X);
            }
        }

        /// <summary>
        /// Draws the gameplay and our base(screen layer) screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            string message = "FPS - " + GameManager.FPS.FrameCount.ToString();

            //Set the position spot
            Vector2 position = new Vector2(box.ChatBounds.Right - GameManager.Font.Spacing,
                                           box.ChatBounds.Top + GameManager.Font.LineSpacing);
            Color color = new Color(255, 255, 255, Alpha);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, position, color);
            ScreenManager.SpriteBatch.End();
        }
    }
}