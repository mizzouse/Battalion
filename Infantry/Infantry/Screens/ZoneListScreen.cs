using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Managers;
using Infantry.Handlers;

namespace Infantry.Screens
{
    class ZoneListScreen : MenuScreen
    {
        BusyScreen Busy;
        const string texture = "zonelist";
        Rectangle _size = GameManager.ViewingSize;
        Dictionary<string, ZoneSettings> Zones = new Dictionary<string, ZoneSettings>();
        List<string> oldZones = new List<string>();
        ZoneList _zonelist;

        const int TicksBetweenRequests = 5000;
        int tickLastZoneListRequest;
        int now;

        /// <summary>
        /// Gets or sets the size of our zonelist screen
        /// Default is our window size
        /// </summary>
        public Rectangle Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Zone List Screen Constructor
        /// </summary>
        public ZoneListScreen()
            : base("ZoneList")
        {
            TransOnTime = TimeSpan.FromSeconds(0.5f);
            TransOffTime = TimeSpan.FromSeconds(0.5f);

            //Create our button entries
            ButtonEntry Back = new ButtonEntry("Back", true);
            ButtonEntry Exit = new ButtonEntry("Exit", true);

            //Set the hooks
            Back.Selected += Back_Selected;
            Exit.Selected += OnExit;

            //Add the entries to our main list
            ButtonEntries.Add(Back);
            ButtonEntries.Add(Exit);

            //Dont draw the login title
            DrawTitle = false;

            //Make our object linker
            _zonelist = new ZoneList();
            _zonelist.Screen = this;
        }

        /// <summary>
        /// Event handler for when a zone is selected
        /// </summary>
        void EnterZone_Selected(object sender, EventArgs e)
        {
            if (ScreenManager.Initialized)
            {
                if (Zones.Keys.Contains(LastButtonClicked.Text))
                {
                    Network.CS_JoinZone join = new Network.CS_JoinZone();
                    //If this doesnt pass, an error box within cs_join will be shown
                    if (join.Send(Zones[LastButtonClicked.Text].Address))
                    {
                        //Wait for a response while showing a message
                        Busy = new BusyScreen(Messages.Connecting);
                        Busy.OperationCompleted += ConnectZoneServer;
                        ScreenManager.AddScreen(Busy);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the back button is selected
        /// </summary>
        void Back_Selected(object sender, EventArgs e)
        {
            if (ScreenManager.Initialized)
                Screens.LoadingScreen.Load(false, new LoginScreen());
        }

        /// <summary>
        /// Event Handler for when the exit button is selected
        /// </summary>
        void OnExit(object sender, EventArgs e)
        {
            GameManager.Game.Exit();
        }

        /// <summary>
        /// Our internal zone server connection attempt event.
        /// </summary>
        void ConnectZoneServer(object sender, OperationCompletedEvent e)
        {
            //Since this was fired, check connection
            if (e.Result)
            {
                //True = connection approved
                //Set up our message display first then load
                LoadingScreen.Load(true, new GameScreen());
            }
            else
            {
                //Denied, show reason
                ScreenManager.AddScreen(new MessageBox(e.Reason, true));
                Busy.ExitScreen();
            }
        }

        /// <summary>
        /// Loads our texture content
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(texture, "Textures/ZoneList");
        }

        /// <summary>
        /// Unloads our texture
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            TextureManager.RemoveTexture(texture);
        }

        /// <summary>
        /// Updates the background screen of the zonelist. Because we are
        /// setting covered to false, the screen will not transition away when another layer
        /// is placed on top of this layer.
        /// </summary>
        public override void Update(GameTime gameTime, bool screenFocus, bool covered)
        {
            base.Update(gameTime, screenFocus, false);

            int now = Environment.TickCount;
            if (now - tickLastZoneListRequest > TicksBetweenRequests)
            {
                Network.CS_ZoneList list = new Network.CS_ZoneList();
                list.Send();

                tickLastZoneListRequest = now;
            }
        }

        /// <summary>
        /// Draws the main zonelist screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            byte fade = Alpha;
            ScreenManager.SpriteBatch.Begin();

            if (TextureManager.RenderReady(texture))
                ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture), Size, new Color(fade, fade, fade));

            ScreenManager.SpriteBatch.End();

            //We still want to draw our buttons after
            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates our zonelist screen with required info
        /// </summary>
        /// <param name="msg">Incoming message</param>
        public void UpdateList(Lidgren.Network.NetIncomingMessage msg)
        {
            if (Zones.Count > 0)
            {
                foreach (string zname in Zones.Keys)
                    oldZones.Add(zname);
            }
            Zones.Clear();

            int count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ZoneSettings _zone = new ZoneSettings();
                _zone.ID = msg.ReadInt64();
                _zone.Name = msg.ReadString();
                _zone.Description = msg.ReadString();
                _zone.Address = msg.ReadIPEndPoint();

                //Add it to our dictionary
                Zones.Add(_zone.Name, _zone);

                //Remove it from our old list
                if (oldZones.Contains(_zone.Name))
                    oldZones.Remove(_zone.Name);
            }

            UpdateButtons();
        }

        /// <summary>
        /// Updates our buttons
        /// </summary>
        private void UpdateButtons()
        {
            //Add our zone buttons to the main list
            foreach (string zoneName in Zones.Keys)
            {
                ButtonEntry entry = new ButtonEntry(zoneName, true);
                entry.Selected += EnterZone_Selected;
                if (!ButtonEntries.Contains(entry))
                    ButtonEntries.Add(entry);
            }

            //Remove any buttons that arent active in our zone list
            foreach (ButtonEntry button in ButtonEntries.ToList())
                if (oldZones.Contains(button.Text))
                    ButtonEntries.Remove(button);

            //Clear old list
            oldZones.Clear();
        }

        /// <summary>
        /// This class is just an object linker used to static call our
        /// zonelist within the screen itself.
        /// </summary>
        public class ZoneList
        {
            static ZoneListScreen zonelist;
            /// <summary>
            /// Our parent screen
            /// </summary>
            public ZoneListScreen Screen
            {
                get { return zonelist; }
                set { zonelist = value; }
            }

            public static void UpdateZoneList(Lidgren.Network.NetIncomingMessage msg)
            {
                if (zonelist == null)
                    return;

                zonelist.UpdateList(msg);
            }
        };

        /// <summary>
        /// Our zone info class that holds all read-in data
        /// </summary>
        class ZoneSettings
        {
            public long ID;
            public string Name;
            public string Description;
            public IPEndPoint Address;
        }
    }
}
