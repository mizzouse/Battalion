using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Managers;

namespace Infantry.Network
{
    public class CS_ZoneList
    {
        public void Send()
        {
            Client client = NetworkManager.GetClient("AccountServer");
            if (client != null)
            {
                NetOutgoingMessage msg = client.CreateMessage();
                msg.Write((byte)Packets.CS_ZoneList);
                client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}
