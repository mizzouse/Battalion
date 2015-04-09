using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

namespace Infantry.Network
{
    public class CS_DropItem
    {
        #region Declarations
        public Client client;
        public int id;
        public int quantity;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_Drop);

            pkt.Write(id);
            pkt.Write(quantity);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
