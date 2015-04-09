using System;
using System.Collections.Generic;
using System.ComponentModel;

using Lidgren.Network;

using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.Network
{
    public class CS_DiscoveryPing
    {
        Client client;

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_DiscoveryPing() { }

        /// <summary>
        /// Object Constructor using a specified client name
        /// </summary>
        /// <param name="Name">Client name</param>
        public CS_DiscoveryPing(string Name)
        {
            if (!String.IsNullOrEmpty(Name))
                return;

            if ((client = NetworkManager.GetClient(Name)) == null)
            {
                MessageBox msg = new MessageBox("Internal network error. Please relog.", true);
                ScreenManager.AddScreen(msg);
            }

            if (!client.DiscoverKnownPeer("127.0.0.1", 14242))
            {
                MessageBox msg = new MessageBox("Server host not found.", true);
                ScreenManager.AddScreen(msg);
            }
        }
    }
}
