using System;
using System.Collections.Generic;

using Lidgren.Network;

using Infantry.Objects;
using Infantry.Managers;

namespace Infantry.Network
{
    public class CS_JoinTeam
    {
        #region Declarations
        Client client = NetworkManager.GetClient("ZoneServer");
        public int teamID;
        public string teamName;
        public string password;
        #endregion

        public void Send()
        {
            NetOutgoingMessage pkt = new NetOutgoingMessage();

            pkt.Write((byte)PacketHeaders.Packets.CS_JoinTeam);

            pkt.Write(teamID);
            pkt.Write(teamName);
            pkt.Write(password);

            client.SendMessage(pkt, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
