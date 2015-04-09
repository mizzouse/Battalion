using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Infantry.Helpers;

namespace Infantry.Settings
{
    /// <summary>
    /// Game settings are stored in a custom xml file. Reasoning behind this
    /// is so we can store both on the xbox and a pc.
    /// Note: Default instancing is also in this file for normal setting files.
    /// </summary>
    [Serializable]
    public class GameSetting
    {
        #region Properties
        private static bool save = false;
        private int _resoWidth = 640, _resoHeight = 480;
        public const int MinimumResolutionWidth = 640;
        public const int MinimumResolutionHeight = 480;

        /// <summary>
        /// Current Resolution Width
        /// </summary>
        public int ResolutionWidth
        {
            get { return _resoWidth; }
            set
            {
                if (_resoWidth != value)
                {
                    _resoWidth = value;
                    save = true;
                }
            }
        }

        /// <summary>
        /// Current Resolution Height
        /// </summary>
        public int ResolutionHeight
        {
            get { return _resoHeight; }
            set
            {
                if (_resoHeight != value)
                {
                    _resoHeight = value;
                    save = true;
                }
            }
        }

        private bool _fullScreen = false;
        /// <summary>
        /// Is this in full screen?
        /// </summary>
        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                if (_fullScreen != value)
                {
                    _fullScreen = value;
                    save = true;
                }
            }
        }

        private bool _postScreenEffects = true;
        /// <summary>
        /// Are we using post screen effects after rendering?
        /// Note: Default is true
        /// </summary>
        public bool PostScreenEffects
        {
            get { return _postScreenEffects; }
            set
            {
                if (_postScreenEffects != value)
                {
                    _postScreenEffects = value;
                    save = true;
                }
            }
        }

        private bool _reflections = true;
        /// <summary>
        /// Are we using reflective surfaces?
        /// Note: Default is true
        /// </summary>
        public bool Reflections
        {
            get { return _reflections; }
            set
            {
                if (_reflections != value)
                {
                    _reflections = value;
                    save = true;
                }
            }
        }

        private bool _refractions = true;
        /// <summary>
        /// Are we using refractive surfaces?
        /// Note: Default is true
        /// </summary>
        public bool Refractions
        {
            get { return _refractions; }
            set
            {
                if (_refractions != value)
                {
                    _refractions = value;
                    save = true;
                }
            }
        }

        private bool _shadowMapping = true;
        /// <summary>
        /// Are we using shadow mapping?
        /// Note: Default is true
        /// </summary>
        public bool ShadowMapping
        {
            get { return _shadowMapping; }
            set
            {
                if (_shadowMapping != value)
                {
                    _shadowMapping = value;
                    save = true;
                }
            }
        }

        private DetailLevel _level = DetailLevel.High;
        /// <summary>
        /// Gets or sets our detail level
        /// Note: Default is High
        /// </summary>
        public DetailLevel DetailLvl
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    switch(value)
                    {
                        case DetailLevel.Low:
                            _shadowMapping = false;
                            _reflections = false;
                            _refractions = false;
                            _postScreenEffects = false;
                            break;

                        case DetailLevel.Medium:
                            _shadowMapping = false;
                            _reflections = true;
                            _refractions = true;
                            _postScreenEffects = false;
                            break;

                        case DetailLevel.High:
                            _shadowMapping = true;
                            _reflections = true;
                            _refractions = true;
                            _postScreenEffects = true;
                            break;

                        default:
                            _level = DetailLevel.High;
                            _shadowMapping = true;
                            _reflections = true;
                            _refractions = true;
                            _postScreenEffects = true;
                            break;
                    }
                    save = true;
                }
            }
        }

        private float _soundVolume = 0.8f;
        /// <summary>
        /// Sound volume
        /// </summary>
        public float SoundVolume
        {
            get { return _soundVolume; }
            set
            {
                if (_soundVolume != value)
                {
                    _soundVolume = value;
                    save = true;
                }
            }
        }

        private float _musicVolume = 0.4f;
        /// <summary>
        /// Music Volume
        /// </summary>
        public float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (_musicVolume != value)
                {
                    _musicVolume = value;
                    save = true;
                }
            }
        }

        private float _controllerSensitivity = 0.5f;
        /// <summary>
        /// Our controller sensitivity
        /// </summary>
        public float ControllerSensitivity
        {
            get { return _controllerSensitivity; }
            set
            {
                if (_controllerSensitivity != value)
                {
                    _controllerSensitivity = value;
                    save = true;
                }
            }
        }

        private bool _controllerVibrate = false;
        /// <summary>
        /// Vibrate our controller?
        /// </summary>
        public bool ControllerVibrate
        {
            get { return _controllerVibrate; }
            set
            {
                if (_controllerVibrate != value)
                {
                    _controllerVibrate = value;
                    save = true;
                }
            }
        }

        private float _promptWidth = 8.0f;
        /// <summary>
        /// Sets our prompts max width, default is 8.
        /// </summary>
        public float PromptWidth
        {
            get { return _promptWidth; }
            set
            {
                if (value > 0 && _promptWidth != value)
                {
                    _promptWidth = value;
                    save = true;
                }
            }
        }
        #endregion

        #region Default Settings
        const string SettingsFileName = "GameSettings.xml";
        private static GameSetting _defaultInstance = null;

        /// <summary>
        /// Default instance of our Game Settings
        /// </summary>
        public static GameSetting Default
        {
            get { return _defaultInstance; }
        }
        #endregion

        #region Constructor and Initialize
        /// <summary>
        /// No public constructor! Create the game settings.
        /// </summary>
        private GameSetting()
        {
        }

        /// <summary>
        /// Create game settings.  This constructor helps us to only load the
        /// GameSetting once, not again if GameSetting is recreated by
        /// the Deserialization process.
        /// </summary>
        public static void Initialize()
        {
            _defaultInstance = new GameSetting();
            Load();
        }
        #endregion

        #region Save/Load
        internal static void Load()
        {
            save = true;
            FileStream file = FileHelper.LoadContentFile(SettingsFileName);

            if (file == null || file.Length == 0)
            {
                //File doesnt exist or nothing in it, 
                //lets create one using our save call
                Save();
                //Now reload
                Load();
            }

            //Lets deserialize
            var xml = new XmlSerializer(typeof(GameSetting));
            if (xml != null)
                _defaultInstance = (GameSetting)xml.Deserialize(file);

            file.Close();
        }

        /// <summary>
        /// Creates or overrides our file first then serializes 
        /// our class to this file.
        /// </summary>
        public static void Save()
        {
            //No need to save if its up-to-date
            if (!save)
                return;

            save = false;

            FileStream file = FileHelper.SaveContentFile(SettingsFileName);

            //Save everything in this class using Xml Serializer
            new XmlSerializer(typeof(GameSetting)).Serialize(file, _defaultInstance);

            //Close it
            file.Close();
        }

        /// <summary>
        /// Sets the Graphics Detail Level for our game
        /// </summary>
        /// <param name="level"></param>
        public static void SetDetailLevel(int level)
        {
            SetDetailLevel((DetailLevel)level);
        }

        /// <summary>
        /// Set the Graphics Detail Level for our game
        /// </summary>
        /// <param name="level"></param>
        public static void SetDetailLevel(DetailLevel level)
        {
            switch (level)
            {
                case DetailLevel.Low:
                    GameSetting.Default.Reflections = false;
                    GameSetting.Default.Refractions = false;
                    GameSetting.Default.ShadowMapping = false;
                    GameSetting.Default.PostScreenEffects = false;
                    GameSetting.Default.DetailLvl = DetailLevel.Low;
                    break;

                case DetailLevel.Medium:
                    GameSetting.Default.Reflections = true;
                    GameSetting.Default.Refractions = true;
                    GameSetting.Default.ShadowMapping = false;
                    GameSetting.Default.PostScreenEffects = false;
                    GameSetting.Default.DetailLvl = DetailLevel.Medium;
                    break;

                case DetailLevel.High:
                    GameSetting.Default.Reflections = true;
                    GameSetting.Default.Refractions = true;
                    GameSetting.Default.ShadowMapping = true;
                    GameSetting.Default.PostScreenEffects = true;
                    GameSetting.Default.DetailLvl = DetailLevel.High;
                    break;

                default:
                    GameSetting.Default.Reflections = true;
                    GameSetting.Default.Refractions = true;
                    GameSetting.Default.ShadowMapping = true;
                    GameSetting.Default.PostScreenEffects = true;
                    GameSetting.Default.DetailLvl = DetailLevel.High;
                    break;

            }
            save = true;
            GameSetting.Save();
        }
        #endregion
    }
}
