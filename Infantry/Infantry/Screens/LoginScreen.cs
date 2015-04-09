using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;

using Infantry.Managers;

namespace Infantry.Screens
{
    class LoginScreen : MenuScreen
    {
        BoxEntry username;
        BoxEntry password;
        CheckBoxEntry CheckBox;
        BusyScreen Busy;

        /// <summary>
        /// The Login Screen is the first screen you see when the app starts
        /// </summary>
        public LoginScreen()
            : base("Login")
        {
            //Inherits our login background
            ScreenManager.AddScreen(new BackgroundScreen());
            
            Settings.UserSetting Settings = GameManager.UserSettings;
            
            ButtonEntry play = new ButtonEntry("Play", true);
            ButtonEntry options = new ButtonEntry("Options", true);
            ButtonEntry exit = new ButtonEntry("Exit", true);

            play.Selected += PlaySelected;
            options.Selected += OptionsSelected;
            exit.Selected += OnCancel;

            ButtonEntries.Add(play);
            ButtonEntries.Add(options);
            ButtonEntries.Add(exit);

            CheckBox = new CheckBoxEntry();
            CheckBox.Text = "Remember Password";
            CheckBox.TextPosition = CheckBoxEntry.TextLocation.Right;
            CheckBoxEntries.Add(CheckBox);

            username = new BoxEntry((!String.IsNullOrEmpty(Settings.Username) ? Settings.Username : "Username"));
            if (Settings.PasswordSave && !String.IsNullOrEmpty(Settings.Password))
            {
                //Because our password is already hashed,
                //lets just make an object show "*"
                string str = "";
                for (int i = 0; i < Settings.PassLength; i++)
                    str += "*";

                password = new BoxEntry(str);
            }
            else
                password = new BoxEntry("Password");

            username.Selected += UserSelected;
            password.Selected += PassSelected;

            username.InputEnabled = true;
            password.InputEnabled = true;
            password.ScrambleInput = true;

            BoxEntries.Add(username);
            BoxEntries.Add(password);

            //Dont draw the login title
            DrawTitle = false;
        }

        /// <summary>
        /// Loads any content, updates box sizes
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            int sizeX = GameManager.ViewingSize.Width / 4;
            int sizeY = GameManager.ViewingSize.Height / 3;

            username.Size = new Rectangle(sizeX + 120, sizeY + 64, 200, ScreenManager.Font.LineSpacing);
            password.Size = new Rectangle(sizeX + 120, sizeY + 164, 200, ScreenManager.Font.LineSpacing);

            //Initialize first
            CheckBox.Initialize();

            CheckBox.BoxPosition = new Vector2(username.Size.X,
                GameManager.ViewingSize.Bottom - (GameManager.ViewingSize.Height / 4));
        }

        /// <summary>
        /// Unloads any content, removes the boxes
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            username.Size = new Rectangle(0, 0, 0, 0);
            password.Size = new Rectangle(0, 0, 0, 0);
            CheckBox.Text = "";
            CheckBox.BoxPosition = new Vector2(0, 0);
        }

        /// <summary>
        /// Event handler raised when this box is selected
        /// </summary>
        void UserSelected(object sender, EventArgs e)
        {
            if (username.Text == "Username")
                username.Text = "";
        }

        /// <summary>
        /// Event handler raised when this box is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PassSelected(object sender, EventArgs e)
        {
            if (password.Text == "Password")
                password.Text = "";
        }

        /// <summary>
        /// Event handler raised when the Play entry is selected
        /// </summary>
        void PlaySelected(object sender, EventArgs e)
        {
            MessageBox box;
            if (username.Text == "Username" || password.Text == "Password"
                || String.IsNullOrWhiteSpace(username.Text) || String.IsNullOrWhiteSpace(password.Text))
            {
                box = new MessageBox("You must enter a valid username and password.", true);
                ScreenManager.AddScreen(box);
                return;
            }

            if (ScreenManager.Initialized)
            {
                GameManager.UserSettings.Username = username.Text;
                GameManager.UserSettings.Password = password.Text;

                //Save their data
                Settings.UserSetting.Save();

                Network.CS_AccountLogin login = new Network.CS_AccountLogin();
                //If this doesnt pass, an error box within cs_account will be shown
                if (login.Send())
                {
                    //Game State is only initially started here
                    //every other game state is controlled by our network client
                    GameManager.GameState = State.LoggingIn;

                    //Wait for a response while showing a message
                    Busy = new BusyScreen(Messages.Connecting);
                    Busy.OperationCompleted += ConnectAccountServer;
                    ScreenManager.AddScreen(Busy);
                }
            }
        }

        /// <summary>
        /// Event handler raised when the Options entry is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OptionsSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MainOptionsScreen());
        }

        /// <summary>
        /// When the user cancels, ask if they want to exit
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit?";

            MessageBox box = new MessageBox(message);
            box.Accepted += ExitAccepted;
            ScreenManager.AddScreen(box);
        }

        /// <summary>
        /// Event handler raised when the user selects ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExitAccepted(object sender, EventArgs e)
        {
            GameManager.Game.Exit();
        }

        /// <summary>
        /// Our internal account server connection attempt event.
        /// </summary>
        void ConnectAccountServer(object sender, OperationCompletedEvent e)
        {
            //Since this was fired, check connection
            if (e.Result)
                //True = connected approved
                Screens.LoadingScreen.Load(false, new ZoneListScreen());
            else
            {
                //Denied, show reason
                ScreenManager.AddScreen(new MessageBox(e.Reason, true));
                Busy.ExitScreen();
            }
        }

        /// <summary>
        /// Allows us to switch between saving a pass or not.
        /// </summary>
        void RememberPassword(object sender, EventArgs e)
        {
            GameManager.UserSettings.PasswordSave = !GameManager.UserSettings.PasswordSave;
        }
    }
}
