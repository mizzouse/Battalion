using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.Network
{
    public class CS_AccountLogin
    {
        Client client = NetworkManager.GetClient("AccountServer");

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_AccountLogin() { }

        /// <summary>
        /// Sends our message, sends an error message if not connected
        /// </summary>
        public bool Send()
        {
            //Try creating a client message
            if (client != null)
            {
                //Are we already connected?
                if (client.ConnectionStatus == NetConnectionStatus.Connected)
                    //We are, disreguard trying to reconnect
                    return true;

                NetOutgoingMessage message;

                message = client.CreateMessage();
                message.Write(GameManager.UserSettings.Username);
                message.Write(GameManager.UserSettings.Password);
                client.Connect("127.0.0.1", 14242, message);

                return true;
            }
            else
            {
                MessageBox msg = new MessageBox("Internal network error. Please relog.", true);
                ScreenManager.AddScreen(msg);
            }

            return false;
        }
    }
}
