using System;
using System.Collections.Generic;

using Lidgren.Network;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_PickUp
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_PickUp() { }

        /// <summary>
        /// Reads incoming packet data, sends outgoing information
        /// </summary>
        public CS_PickUp(NetIncomingMessage Incoming)
        {
            Player player = Zone.GetPlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
            if (player == null)
            {
                Console.WriteLine("PickUp: Player[" + Incoming.SenderConnection.RemoteUniqueIdentifier + "] doesn't exist.");
                return;
            }
            int uniqueID = Incoming.ReadInt32();
            int quantity = Incoming.ReadInt32();

            //Check to see if it exists
            if (!player.Arena.Drops.CheckDrop(uniqueID))
                return;

            //Check if they are close enough

            //Check if they are allowed to pick it up based on stats

            //Check if they are allowed to pick it up based on inventory

            player.Arena.Drops.ModifyDrop(uniqueID, -quantity);

            //Set up the packet
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.SC_PickUp);
            packet.Write(uniqueID);
            packet.Write(-quantity);

            foreach (Vehicle v in player.Arena.GetHumanVehicles())
                ZoneServer.Server.SendMessage(packet, v.Owner.Connection, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// /// <summary>
        /// Reads in data coming from the Command Parser, sends outgoing information
        /// </summary>
        public CS_PickUp(long PlayerID, int UniqueID, int Quantity)
        {
            Player player = Zone.GetPlayer(PlayerID);
            if (player == null)
            {
                Console.WriteLine("PickUp: Player[{0}] doesn't exist.", PlayerID);
                    return;
            }

            //Check if drop exists
            if (!player.Arena.Drops.CheckDrop(UniqueID))
                return;

            //Check if they are close enough

            //Check if they are allowed to pick it up based on stats

            //Check if they are allowed to pick it up based on inventory

            player.Arena.Drops.ModifyDrop(UniqueID, -Quantity);

            //Set up packet
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.SC_PickUp);
            packet.Write(UniqueID);
            packet.Write(-Quantity);

            //Send it
            foreach(Vehicle v in player.Arena.GetHumanVehicles())
                ZoneServer.Server.SendMessage(packet, v.Owner.Connection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
