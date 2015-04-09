using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Objects;

namespace Infantry.Network
{
    public class CS_FireWeapon
    {
        #region Declarations
        public Client client;
        public int weaponID;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)Packets.CS_Projectile);
            pkt.Write(weaponID);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
