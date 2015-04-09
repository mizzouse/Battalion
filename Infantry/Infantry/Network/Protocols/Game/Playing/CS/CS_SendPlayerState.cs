using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Lidgren.Network;
using Lidgren.Network.Xna;

using PacketHeaders;

namespace Infantry.Network
{
    public class CS_SendPlayerState
    {
        #region Declarations
        public Client client;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float yaw;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            //pkt.Write((byte)Packets.CS_PlayerState);

            pkt.Write(position);
            pkt.Write(velocity);
            pkt.Write(acceleration);
            pkt.Write(yaw);
            
            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
