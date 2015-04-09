using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Lidgren.Network;
using Lidgren.Network.Xna;

using PacketHeaders;
using Microsoft.Xna.Framework;

using Infantry.Managers;

namespace Infantry.Network
{
    public class NetworkClient : NetClient
    {
        #region Declarations
        private Client client;
        private static NetPeerConfiguration NetConfig;
        private Thread networkThread;
        private int _lastConnectionAttempt = System.Environment.TickCount;

        /// <summary>
        /// Our reverse pointer lookup
        /// </summary>
        public Client Client
        {
            get { return client; }
            set
            {
                if (client == null)
                    client = value;
            }
        }

        /// <summary>
        /// Returns the current threading process for our network client
        /// </summary>
        public Thread NetworkThread
        {
            get { return networkThread; }
        }
        #endregion

        /// <summary>
        /// Constructor base for our client connection
        /// </summary>
        /// <param name="name">Name of our connection</param>
        public NetworkClient(string name)
            : base(NetConfig = new NetPeerConfiguration(name)) 
        {
            // setup network thread
            networkThread = new Thread(new ThreadStart(NetworkLoop));
            networkThread.Name = name + " NetworkThread";
            networkThread.IsBackground = true;
        }

        /// <summary>
        /// Processes all the incoming data for our game client
        /// </summary>
        private void NetworkLoop()
        {
            NetIncomingMessage incoming;

            while (networkThread.IsAlive)
            {
                if ((incoming = ReadMessage()) != null)
                {
                    switch (incoming.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            SC_DiscoveryPing.Read(incoming.SenderConnection);
                            break;

                        case NetIncomingMessageType.Data:
                            //Add to the list
                            HandleCommunication(incoming);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = incoming.SenderConnection.Status;
                            //Read in the status change, send any messages to the user if needed
                            SC_Status.Read(status, incoming, this.Configuration.AppIdentifier);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates our packet handlers
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Handles all incoming data messages
        /// </summary>
        private void HandleCommunication(NetIncomingMessage message)
        {
            switch ((Packets)message.ReadByte())
            {
                #region Game Packets
                case Packets.SC_Chat:
                    SC_Chat.Read(message);
                    break;

                #endregion

                #region Connection Denied
                case Packets.SC_Disconnect:
                    SC_Disconnect.Read(message);
                    break;

                #endregion

                #region Initial and In-Game

                #region Zonelist Receive
                case Packets.SC_ZoneList:
                    SC_ZoneList.Read(message);
                    break;

                #endregion

                #region Assets
                case Packets.SC_TotalAssets:
                    Assets.Checksum.TotalChecksumValue = message.ReadString();
                    break;

                
                #endregion
                #endregion
            }
        }
    }
}
