using System;
using System.Collections.Generic;
using System.Net;

using Microsoft.Xna.Framework;

using Infantry.Managers;

namespace Infantry.Network
{
    public class Client : NetworkClient
    {
        /// <summary>
        /// Returns the string identifier used for this client
        /// </summary>
        public string Name
        {
            get { return this.Configuration.AppIdentifier; }
        }

        /// <summary>
        /// Gets the mac address, ip and port used
        /// </summary>
        public long GetIdentifier
        {
            get { return this.UniqueIdentifier; }
        }

        /// <summary>
        /// Returns the IP address of this client
        /// </summary>
        public IPAddress IP
        {
            get { return this.Configuration.LocalAddress; }
        }

        /// <summary>
        /// Our Connection Constructor
        /// </summary>
        /// <param name="name">Name of our connection</param>
        public Client(string name)
            : base(name)
        {
            base.Client = this;
        }

        /// <summary>
        /// Starts our connection by binding our sockets and port,
        /// uses our base client(NetClient)
        /// </summary>
        public void StartUp()
        {
            //Start network client's receiving loop
            base.NetworkThread.Start();
            //Start lidgren network
            this.Start();

            //add to the manager's list if needed
            if (NetworkManager.GetClient(this.Name) == null)
                NetworkManager.AddClient(this);
        }

        /// <summary>
        /// Sends a disconnection packet then disconnects our connection from a server
        /// </summary>
        public void Destroy()
        { 
            Lidgren.Network.NetOutgoingMessage msg = new Lidgren.Network.NetOutgoingMessage();
            msg.Write((byte)PacketHeaders.Packets.CS_Disconnect);
            this.SendMessage(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

            this.Disconnect("Quit");
        }

        /// <summary>
        /// Updates our network client
        /// </summary>
        public void Poll(GameTime gameTime) 
        {
            this.Update(gameTime);
        }

        /// <summary>
        /// Returns a string that represents our information
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + this.Configuration.AppIdentifier + "] "
                + this.UniqueIdentifier;
        }
    }
}
