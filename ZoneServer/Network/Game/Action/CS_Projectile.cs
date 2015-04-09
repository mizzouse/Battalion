using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Lidgren.Network;
using Lidgren.Network.Xna;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_Projectile
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Projectile() { }

        /// <summary>
        /// Reads incoming packet data, sends outgoing information
        /// </summary>
        public CS_Projectile(NetIncomingMessage Incoming)
        {
            Player p = Zone.GetPlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
            if (p == null)
            {
                Console.WriteLine("Projectile: Player[" + Incoming.SenderConnection.RemoteUniqueIdentifier + "] doesn't exist.");
                return;
            }
            int playerID = p.PlayerID;
            int itemID = Incoming.ReadInt32();
            Vector3 position = Incoming.ReadVector3();
            float yaw = Incoming.ReadFloat();

            //Check if they have a weapon

            //Check if this ammo is for their weapon

            //Check if they have this ammo

            //Set up the packet
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.SC_Projectile);
            packet.Write(itemID);
            packet.Write(playerID);
            packet.Write(position);
            packet.Write(yaw);

            //Inform everyone that this projectile exists
            foreach (Vehicle v in p.Arena.GetHumanVehicles())
            {
                if (p == v.Owner)
                    continue;

                ZoneServer.Server.SendMessage(packet, v.Owner.Connection, NetDeliveryMethod.ReliableSequenced);
            }
        }
    }
}
