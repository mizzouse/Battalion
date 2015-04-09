using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using PhysicEditor.Models;
using PhysicEditor.Input;

namespace PhysicEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainWindow : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont mainFont;
        MainDialog Dialog;

        KeyboardInput KeyboardInput;

        public MainWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Dialog = new MainDialog();
            Program.mainDialog = Dialog;
            Dialog.Show();

            KeyboardInput = new KeyboardInput();

            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Load models
            Collection.PopulateModelList();
           // Collection.ReadFile();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            mainFont = Content.Load<SpriteFont>("font1");

            // TODO: use this.Content to load your game content here
        }

        public void LoadModel(Model model, string path)
        {
            model = Content.Load<Model>(path);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //Update camera
            Camera.Update();
            //Update Input
            MouseControl.Update();
            KeyboardInput.InputUpdate(gameTime);

            //Update dialog
            Dialog.UpdateModel();

            //Update model

            if (Collection.Models.Count != 0)
            {
                Collection.Models[Program.CurrentModelID].Update(gameTime);
            }
            

            //Update input handlers

            //Update model rotation

            //Update physics

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           // spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            if (Program.IsWireMode)
            {
                RasterizerState state = new RasterizerState();
                state.FillMode = FillMode.WireFrame;
                spriteBatch.GraphicsDevice.RasterizerState = state;
            }
            else
            {
                RasterizerState state = new RasterizerState();
                state.FillMode = FillMode.Solid;
                spriteBatch.GraphicsDevice.RasterizerState = state;
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            spriteBatch.Begin();

            //Some meaningful data
            spriteBatch.DrawString(mainFont, "Current Model ID: " + Program.CurrentModelID + "", new Vector2(0, 0), Color.Black);
            spriteBatch.DrawString(mainFont, "Active Part: " + Program.CurrentPType + "", new Vector2(0, 10), Color.Black);
            spriteBatch.DrawString(mainFont, "(World) Mouse Position: " + MouseControl.NewMousePosition, new Vector2(0, 20), Color.Black);
            spriteBatch.DrawString(mainFont, "Zoom Value: " + Camera.Zoom, new Vector2(0, 30), Color.Black);
            spriteBatch.DrawString(mainFont, "Axis Lock: " + Fields.Fields.AxisMode, new Vector2(0, 40), Color.Black);
            int i = 40;
            if (Fields.Fields.AdjustingModel)
            {
                spriteBatch.DrawString(mainFont, "Model Mode: " + Fields.Fields.Models, new Vector2(0, i+=10), Color.Black);
            }
            if (Fields.Fields.AdjustingSphere)
            {
                spriteBatch.DrawString(mainFont, "Sphere  Mode: " + Fields.Fields.Spheres, new Vector2(0, i+=10), Color.Black);
            }

        /*    int i = 40;
            foreach (ModelBone bone in Collection.Models[Program.CurrentModelID].model.Bones)
            {
                spriteBatch.DrawString(mainFont, "bone: " + bone.Name, new Vector2(0, i+=10), Color.Black);

            }*/
            //If we are placing an anchor point draw the point while user moves it around
            if (Program.IsPlacingAnchor)
            {
                //We will use a little sphere to denote the current position
            }


            GameModel model;

            //Draw current model in center of screen
            if (Collection.Models.Count != 0)
            {
                model =  Collection.GetGameModelByID(Program.CurrentModelID);
                if (model != null)
                {
                    model.Draw(gameTime);
                    RenderSphere.Render(model.Spheres, GraphicsDevice, Color.HotPink);
                }                
            }
            //Draw all physics associated with model

           
            



            if (Collection.Models.Count == 0)
            {
                spriteBatch.End();
                return;
            }



            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Returns content entry of a model at specified path
        /// </summary>
        public Model LoadModelContent(string path)
        {
            return Content.Load<Model>(path);
        }
    }
}
