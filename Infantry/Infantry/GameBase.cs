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

using Infantry.Handlers;
using Infantry.Helpers;
using Infantry.Settings;
using Infantry.Managers;
using Infantry.Objects;
using Infantry.Screens;
using Infantry.Plugins;

namespace Infantry
{
    public enum State
    {
        LogInScreen,        //We are at the login screen
        LoggingIn,          //We are attempting to log into the account server
        ZoneSelect,         //We are at the zone select screen
        EnteringZone,       //We are entering a zone
        InGame,             //We are in game
        Store,              //Store
        Skill               //Skills/attributes
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class GameBase : Microsoft.Xna.Framework.Game
    {
        #region Base Declarations
        protected static GraphicsDeviceManager graphics = null;
        protected static ContentManager content = null;
        protected static SpriteBatch spriteBatch = null;
        protected static Rectangle _windowBounds;
        protected static Rectangle _viewSize;
        protected static float _aspectRatio = 1.0f;
        private static Color _backgroundColor = Color.Black;
        private static Texture2D _pixel, _border;
        private static SpriteFont _font;
        private static State _gameState = State.LogInScreen;
        private bool _isWindowActive = false;
        private bool _applyChanges = false;
        #endregion

        #region Component Declarations
        private static InputHandler _input = null;
        private static FPSCounter _fps = null;
        private static TextureManager _textureManager = null;
        private static ScreenManager _screenManager = null;
        private static CameraManager _cameraManager = null;
        private static NetworkManager _networkManager = null;
        private static ChatManager _chatManager = null;
        private static MessageDisplay _messageDisplay;
        #endregion

        #region Object Declarations
        private static string accountTicket;
        private static Player _player = null;
        private static Dictionary<long, Vehicle> _vehicles = new Dictionary<long, Vehicle>();
        #endregion

        #region Misc Declarations
        private static readonly Random _random = new Random();
        #endregion

        #region Public Pointers
        /// <summary>
        /// The graphics device used to render our game
        /// </summary>
        public static GraphicsDevice Device
        {
            get { return graphics.GraphicsDevice; }
        }

        /// <summary>
        /// Our content manager used for our game
        /// </summary>
        public static ContentManager Contents
        {
            get { return content; }
        }

        /// <summary>
        /// Our game window size in a retangle
        /// This is the outer edge of our client
        /// </summary>
        public static Rectangle WindowBounds
        {
            get { return _windowBounds; }
        }

        /// <summary>
        /// Our game's current viewing size in a rectangle
        /// This is the inside rendering size in our client window.
        /// </summary>
        public static Rectangle ViewingSize
        {
            get { return _viewSize; }
        }

        /// <summary>
        /// Our base texture used as a default white color for various boxes
        /// </summary>
        public static Texture2D Pixel
        {
            get { return _pixel; }
        }

        /// <summary>
        /// Our base texture used a a default black color border line for various boxes
        /// </summary>
        public static Texture2D Border
        {
            get { return _border; }
        }

        /// <summary>
        /// Our base font used for our game
        /// </summary>
        public static SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Shows our current OS platform
        /// </summary>
        public static PlatformID CurrentPlatform = Environment.OSVersion.Platform;

        /// <summary>
        /// Aspect ratio of our rendering area
        /// </summary>
        public static float AspectRatio
        {
            get { return _aspectRatio; }
        }

        /// <summary>
        /// Our background re-drawing color 
        /// </summary>
        public static Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets if our game window is active
        /// </summary>
        public bool IsWindowActive
        {
            get { return _isWindowActive; }
            set { _isWindowActive = value; }
        }

        /// <summary>
        /// Our input handler which shows the state of a game pad, keyboard and mouse.
        /// </summary>
        public static InputHandler Input
        {
            get { return _input; }
        }

        /// <summary>
        /// Our frames per second counter
        /// </summary>
        public static FPSCounter FPS
        {
            get { return _fps; }
        }

        /// <summary>
        /// What game state are we in?
        /// </summary>
        public static State GameState
        {
            get { return _gameState; }
            set { _gameState = value; }
        }

        /// <summary>
        /// Our Game Settings initialized on startup
        /// </summary>
        public static GameSetting GameSettings
        {
            get { return GameSetting.Default; }
        }

        /// <summary>
        /// Our User Settings initialized on startup
        /// </summary>
        public static UserSetting UserSettings
        {
            get { return UserSetting.GetUserSettings(); }
        }

        /// <summary>
        /// Returns our player object structure which is instanced once
        /// </summary>
        public static Player Player
        {
            get { return _player; }
        }

        /// <summary>
        /// Gets or Sets our account ticket sent from our Account Server
        /// </summary>
        public static string AccountTicket
        {
            get { return accountTicket; }
            set
            {
                if (accountTicket != value)
                    accountTicket = value;
            }
        }

        /// <summary>
        /// Returns a random generated number
        /// </summary>
        public static Random Random
        {
            get { return _random; }
        }

        /// <summary>
        /// Returns the vehicle dictionary for in game vehicles
        /// </summary>
        public static IDictionary<long, Vehicle> Vehicles
        {
            get { return _vehicles; }
        }

        /// <summary>
        /// Returns our interface to the message display service
        /// </summary>
        public static MessageDisplay MsgDisplay
        {
            get { return _messageDisplay; }
        }
        #endregion


        /// <summary>
        /// Base Game Constructor using a custom Window Title
        /// </summary>
        /// <param name="title">Title that will display at the top of our game window</param>
        protected GameBase(string title)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PreparingDeviceSettings);

            GameSetting.Initialize();
            UserSetting.Initialize();
            ApplyResolution();

            Window.Title = title;
            _windowBounds = Window.ClientBounds;

            this.IsMouseVisible = true;

            content = new ContentManager(this.Services);
            content.RootDirectory = "Content";

            //Game Components - Our base initializer will initialize them all for us
            //Network Manager
            _networkManager = new NetworkManager(this);
            Components.Add(_networkManager);

            //Input
            _input = new InputHandler(this);
            Components.Add(_input);

            //FPS Counter
            _fps = new FPSCounter(this);
            Components.Add(_fps);

            //Notification Display Component
            _messageDisplay = new Screens.MessageDisplay(this);
            Components.Add(_messageDisplay);

            //Texture Manager
            _textureManager = new TextureManager(this);
            Components.Add(_textureManager);

            //Screen Manager
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            //Camera Manager
            _cameraManager = new CameraManager(this);
            Components.Add(_cameraManager);

            //Chat Manager
            _chatManager = new ChatManager(this);
            Components.Add(_chatManager);
        }

        /// <summary>
        /// Base Game Constructor using set string "Infantry"
        /// </summary>
        public GameBase()
            : this("Infantry") { }

        /// <summary>
        /// Prepares the graphics device
        /// </summary>
        void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                PresentationParameters param = e.GraphicsDeviceInformation.PresentationParameters;
                if (graphics.PreferredBackBufferHeight == 720)
                    param.MultiSampleCount = 4;
                else
                    param.MultiSampleCount = 2;
            }
        }

        /// <summary>
        /// Applies the resolution change
        /// </summary>
        public void ApplyResolution()
        {
            int resWidth = GameSetting.Default.ResolutionWidth;
            int resHeight = GameSetting.Default.ResolutionHeight;

            if (resWidth < GameSetting.MinimumResolutionWidth || resHeight < GameSetting.MinimumResolutionHeight)
            {
                resWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

#if XBOX || XBOX360
            //Xbox graphics are fixed sizes
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
            graphics.PreferredBackBufferWidth = resWidth;
            graphics.PreferredBackBufferHeight = resHeight;
            graphics.IsFullScreen = GameSetting.Default.FullScreen;
#endif
            _windowBounds = Window.ClientBounds;
            _viewSize = new Rectangle(0, 0, graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight);
            _applyChanges = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _pixel = new Texture2D(Device, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            _border = new Texture2D(Device, 1, 1, false, SurfaceFormat.Color);
            _border.SetData(new[] { Color.Black });

            graphics.DeviceReset += new EventHandler<EventArgs>(DeviceReset);
        }

        /// <summary>
        /// Raised when the graphic device is being reset, 
        /// updates all rendering areas and cameras
        /// </summary>
        void DeviceReset(object sender, EventArgs e)
        {
            int _width = graphics.GraphicsDevice.Viewport.Width;
            int _height = graphics.GraphicsDevice.Viewport.Height;
            _aspectRatio = (float)_width / (float)_height;
            _viewSize = new Rectangle(0, 0, _width, _height);
            //CameraManager.SetAllProjections(_aspectRatio);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            //Updates the input handler
            //Since this update is second in line after game manager,
            //this will update all input.
            Input.InputUpdate();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Input.isPressed(Keys.T))
                Console.WriteLine(ScreenManager.TracedScreens);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            base.Draw(gameTime);

            if (_applyChanges)
            {
                graphics.ApplyChanges();
                _applyChanges = false;
            }
        }

        /// <summary>
        /// When the window is active, itll draw animated graphics
        /// </summary>
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            _isWindowActive = true;
        }

        /// <summary>
        /// When the window isnt active, it wont draw animated graphics
        /// </summary>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            _isWindowActive = false;
        }
    }
}
