using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Cryptokon;

using Infantry.Helpers;

namespace Infantry.Settings
{
    /// <summary>
    /// Our Users class object for our user settings.
    /// </summary>
    [Serializable, XmlRoot("User")]
    public class UserSetting
    {
        #region Xml Settings
        private string password;

        /// <summary>
        /// The players Username
        /// </summary>
        [XmlElement(ElementName = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// The players Password(hashed only)
        /// </summary>
        [XmlElement(ElementName = "Password")]
        public string Password
        {
            get { return password; }
            set { password = Functions.CalculateMD5Hash(value); }
        }

        /// <summary>
        /// Are we distinguishing left and right?
        /// </summary>
        [XmlElement(ElementName = "DistinguishLeftRight")]
        public bool DistinguishLeftRight { get; set; }

        /// <summary>
        /// Do we want save our password to auto-load it later?
        /// </summary>
        [XmlElement(ElementName = "SavePass")]
        public bool PasswordSave { get; set; }

        /// <summary>
        /// Shows how long the password is
        /// </summary>
        [XmlElement(ElementName = "Length")]
        public int PassLength { get; set; }
        #endregion

        [NonSerialized]
        const string Filename = "UserSetting.ass";
        private static UserSetting usersetting = null;

        /// <summary>
        /// Gets or creates a user setting structure
        /// </summary>
        public static UserSetting GetUserSettings()
        {
            if (usersetting == null)
            {
                usersetting = new UserSetting();
                Load();
            }

            return usersetting;
        }

        /// <summary>
        /// Our Generic User Settings Constructor
        /// </summary>
        private UserSetting() { }

        /// <summary>
        /// Creates the UserSettings. This constructor will only 
        /// load the user settings once.
        /// </summary>
        public static void Initialize()
        {
            usersetting = new UserSetting();
            Load();
        }

        /// <summary>
        /// Loads a user setting xml file for our game
        /// </summary>
        public static void Load()
        {
            FileStream file = FileHelper.LoadContentFile(Filename);
            if (file == null || file.Length == 0)
            {
                //File doesnt exist or nothing in it,
                //lets create one using our save call.
                Save();
                //Now reload
                Load();
            }

            //Found, load everything
            var load = new XmlSerializer(typeof(UserSetting));
            if (load != null)
                usersetting = (UserSetting)load.Deserialize(file);

            file.Close();
        }

        /// <summary>
        /// Creates or overrides our file first then serializes 
        /// our class to this file.
        /// </summary>
        public static void Save()
        {
            FileStream file = FileHelper.SaveContentFile(Filename);
            new XmlSerializer(typeof(UserSetting)).Serialize(file, usersetting);

            file.Close();
        }
    }
}
