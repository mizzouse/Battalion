using System;
using System.Collections.Generic;

using Lidgren.Network;

namespace AccountServer.Network
{
    public class CS_ZoneList
    {
        private class Zone
        {
            public long ID;
            public string Name;
            public string Description;
            public bool isPublic;
            public System.Net.IPEndPoint IP;
        }

        //Outgoing
        private NetConnection Target;
        private List<Zone> ZoneList = new List<Zone>();

        /// <summary>
        /// C2S - Reads a packet sent by a client
        /// S2C - Sends outgoing response
        /// </summary>
        /// <param name="Incoming">Incoming message/packet</param>
        public CS_ZoneList(NetIncomingMessage Incoming)
        {
            if ((Target = Incoming.SenderConnection) != null)
            {
                foreach (var p in AccountServer.Database.Zones)
                {
                    Zone zone = new Zone();
                    zone.Name = p.Name;
                    zone.ID = p.ID;
                    zone.isPublic = p.Active;
                    zone.Description = p.Description;
                    zone.IP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("24.7.247.119"), 64128);

                    ZoneList.Add(zone);
                }

                Respond();
            }
        }

        /// <summary>
        /// S2C - Reponds to the client with a packet
        /// </summary>
        private void Respond()
        {
            NetOutgoingMessage packet = AccountServer.Server.CreateMessage();
            packet.Write((byte)PacketHeaders.Packets.SC_ZoneList);
            packet.Write(ZoneList.Count);

            foreach (Zone z in ZoneList)
            {
                if (!z.isPublic)
                    continue;

                packet.Write(z.ID);
                packet.Write(z.Name);
                packet.Write(z.Description);
                packet.Write(z.IP);
            }

            AccountServer.Server.SendMessage(packet, Target, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
