using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Lidgren.Network;
using Lidgren.Network.Xna;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_Drop
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Drop() { }

        /// <summary>
        /// Reads in packet data, sends outgoing information
        /// </summary>
        public CS_Drop(NetIncomingMessage Incoming)
        {
            Player p = Zone.GetPlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
            if (p == null)
            {
                Console.WriteLine("Drop: Player[" + Incoming.SenderConnection.RemoteUniqueIdentifier + "] doesn't exist.");
                return;
            }
            int itemID = Incoming.ReadInt32();
            int quantity = Incoming.ReadInt32();
            Vector3 position = p.Vehicle.Position;

            //Check if the item exits
            if (itemID < 0)
            {
                if (!Assets.Items.ContainsKey(itemID))
                    return;
            }
            else
            {
                if (!Assets.Weapons.ContainsKey(itemID))
                    return;
            }

            //Check if they have it

            //Add it
            int uniqueID = p.Arena.Drops.AddDrop(itemID, quantity, position);

            //Set up the packet
            NetOutgoingMessage Outgoing = new NetOutgoingMessage();

            Outgoing.Write((byte)PacketHeaders.Packets.SC_Drop);
            Outgoing.Write(uniqueID);
            Outgoing.Write(itemID);
            Outgoing.Write(quantity);
            Outgoing.Write(position);

            //Update everyone
            foreach (Vehicle v in p.Arena.GetHumanVehicles())
                ZoneServer.Server.SendMessage(Outgoing, v.Owner.Connection, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Reads in data coming from the Command Parser, sends ougoing information
        /// </summary>
        public CS_Drop(long PlayerID, int ItemID, int Quantity)
        {
            Player player = Zone.GetPlayer(PlayerID);
            if (player == null)
            {
                Console.WriteLine("Drop: Player[" + PlayerID + "] doesn't exist.");
                return;
            }
            Vector3 position = player.Vehicle.Position;

            //Check if the item exists
            if (ItemID < 0)
            {
                if (!Assets.Items.ContainsKey(ItemID))
                    return;
            }
            else
            {
                if (!Assets.Weapons.ContainsKey(ItemID))
                    return;
            }

            //Check if they have it

            //Add it
            int uniqueID = player.Arena.Drops.AddDrop(ItemID, Quantity, position);

            //Set up the packet
            NetOutgoingMessage Outgoing = new NetOutgoingMessage();

            Outgoing.Write((byte)PacketHeaders.Packets.SC_Drop);
            Outgoing.Write(uniqueID);
            Outgoing.Write(ItemID);
            Outgoing.Write(Quantity);
            Outgoing.Write(position);

            //Update everyone
            foreach (Vehicle v in player.Arena.GetHumanVehicles())
                ZoneServer.Server.SendMessage(Outgoing, v.Owner.Connection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
