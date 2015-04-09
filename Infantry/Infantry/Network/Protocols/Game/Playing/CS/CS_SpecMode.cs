using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Managers;

namespace Infantry.Network
{
    public class CS_SpecMode
    {
        Client client = NetworkManager.GetClient("ZoneServer");

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_SpecMode);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
