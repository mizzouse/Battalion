using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

namespace Infantry.Network
{
    public class CS_LeftArena
    {
        #region Declarations
        public Client client;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_LeftArena);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
