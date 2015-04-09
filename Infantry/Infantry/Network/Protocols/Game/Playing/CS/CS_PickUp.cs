using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Lidgren.Network;
using Lidgren.Network.Xna;
using PacketHeaders;

using Infantry.Objects;

namespace Infantry.Network
{
    public class CS_PickUp
    {
        #region Declarations
        public Client client;
        public int id;
        public int quantity;
        public Vector3 position;
        
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_PickUp);

            pkt.Write(id);
            pkt.Write(quantity);
            pkt.Write(position);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
