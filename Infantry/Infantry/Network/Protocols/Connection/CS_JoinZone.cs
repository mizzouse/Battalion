using System;
using System.Collections.Generic;
using System.Net;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.Network
{
    public class CS_JoinZone
    {
        Client client = NetworkManager.GetClient("ZoneServer");

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_JoinZone() { }

        /// <summary>
        /// Sends our packet data to our specified server.
        /// </summary>
        public bool Send(IPEndPoint endpoint)
        {
            if (client != null)
            {
                if (GameManager.GameState == State.ZoneSelect ||
                GameManager.GameState >= State.InGame)
                {
                    NetOutgoingMessage message;

                    message = client.CreateMessage();
                    message.Write(GameManager.Player.TicketID);
                    message.Write(GameManager.UserSettings.Username);
                    message.Write(GameManager.Player.Alias);

                    //Are we sending this while in game?
                    if (client.ConnectionStatus == NetConnectionStatus.Connected)
                    {
                        //First send our inventory update then dc
                        CS_InventoryUpdate iUpdate = new CS_InventoryUpdate();
                        iUpdate.client = client;
                        //iUpdate.inventory = GameManager.Player._inventory.GetInventory();
                        iUpdate.Send();

                        client.Destroy();
                    }

                    client.Connect(endpoint, message);

                    return true;
                }

                //Possible hacking attempt, dont allow them to transit
                return false;
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
