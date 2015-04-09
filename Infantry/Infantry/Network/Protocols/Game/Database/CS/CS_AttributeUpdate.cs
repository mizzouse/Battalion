using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

namespace Infantry.Network
{
    public class CS_AttributeUpdate
    {
        #region Declarations
        public Client client;
        public string attributes;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_Attribute);
            pkt.Write(attributes);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}