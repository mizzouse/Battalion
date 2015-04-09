using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

namespace Infantry.Network
{
    public class CS_InventoryUpdate
    {
        #region Declarations
        public Client client;
        public string inventory;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_Inventory);
            pkt.Write(inventory);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}