using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Managers;

namespace Infantry.Network
{
    public class CS_Projectile
    {
        Client client = NetworkManager.GetClient("ZoneServer");

        public CS_Projectile(int ItemID, Vector3 Position, float Yaw)
        {
            NetOutgoingMessage packet = new NetOutgoingMessage();

            packet.Write((byte)PacketHeaders.Packets.CS_Projectile);
            packet.Write(ItemID);
            packet.Write(Position);
            packet.Write(Yaw);

            client.SendMessage(packet, NetDeliveryMethod.ReliableSequenced);
        }
    }
}
