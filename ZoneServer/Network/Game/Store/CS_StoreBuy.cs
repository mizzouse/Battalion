using System;
using System.Collections.Generic;

using Lidgren.Network;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_StoreBuy
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_StoreBuy() { }

        /// <summary>
        /// Reads in packet data, sends outgoing information
        /// </summary>
        public CS_StoreBuy(NetIncomingMessage Incoming)
        {
            Player player = Zone.GetPlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
            if (player == null)
            {
                Console.WriteLine("StoreBuy: Player[" + Incoming.SenderConnection.RemoteUniqueIdentifier + "] doesn't exist.");
                return;
            }
            int cost;
            int itemID = Incoming.ReadInt32();
            int quantity = Incoming.ReadInt32();

            //Check to see if they exist
            if (!Zone.Players.ContainsKey(player.PlayerID))
                return;

            //Check to see if item exists
            if (itemID < 0)
            {
                if (!Assets.Items.ContainsKey(itemID) || !Assets.Items[itemID].Buyable)
                    return;

                cost = Assets.Items[itemID].BuyPrice;
            }
            else
            {
                if (!Assets.Weapons.ContainsKey(itemID) || !Assets.Weapons[itemID].Buyable)
                    return;

                cost = Assets.Weapons[itemID].BuyPrice;
                quantity = 1;
            }

            //Check to see if they can even buy it
            if (player.Cash < cost)
                return;

            //Check to see if they have the needed cash/requirements

            //Deduct the price of the item
            player.Cash -= Assets.Weapons[itemID].BuyPrice;

            //Add it to their inventory server-side
            player.AddInventory(itemID, quantity);

            //Send confirmation
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.SC_StoreBuy);
            packet.Write(itemID);
            packet.Write(quantity);
            packet.Write(cost);

            ZoneServer.Server.SendMessage(packet, Incoming.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Reads in data coming from the Command Parser, sends ougoing information
        /// </summary>
        public CS_StoreBuy(long PlayerID, int ItemID, string Quantity)
        {
            Player player = Zone.GetPlayer(PlayerID);
            if (player == null)
            {
                Console.WriteLine("StoreBuy: Player[" + PlayerID + "] doesn't exist.");
                return;
            }
            long playerID = player.Connection.RemoteUniqueIdentifier;
            int cost;
            int itemID = ItemID;
            int quantity = 0;

            //Check to see if the item exists
            if (itemID < 0)
            {
                if (!Assets.Items.ContainsKey(itemID) || !Assets.Items[itemID].Buyable)
                    return;

                cost = Assets.Items[itemID].BuyPrice;
            }
            else
            {
                if (!Assets.Weapons.ContainsKey(itemID) || !Assets.Weapons[itemID].Buyable)
                    return;

                cost = Assets.Weapons[itemID].BuyPrice;
                quantity = 1;
            }

            //Check to see if they can even buy it
            if (player.Cash < cost)
                return;

            //Check to see if they have the needed cash/requirement
            player.Cash -= cost;

            //Add it to their inventory(server-side)
            player.AddInventory(itemID, quantity);

            //If they used the # operator in the command, we then have to figure out the math
            //to make the new quantity total what they want instead of adding on
            if (Quantity.Contains("#"))
            {
                //Todo
                quantity = Convert.ToInt32(Quantity);
            }

            //Send confirmation
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.SC_StoreBuy);
            packet.Write(itemID);
            packet.Write(quantity);
            packet.Write(cost);

            ZoneServer.Server.SendMessage(packet, player.Connection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
