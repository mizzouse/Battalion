using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;
using PacketHeaders;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_JoinZone
    {
        private NetConnection Target;
        private NetOutgoingMessage Outgoing;

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_JoinZone() { }

        /// <summary>
        /// C2S - Reads in packet data
        /// S2C - Sends out responses to the client
        /// </summary>
        /// <param name="Incoming"></param>
        public CS_JoinZone(NetIncomingMessage Incoming)
        {
            if ((Target = Incoming.SenderConnection) != null)
            {
                //Before anything, check to see if we've reached our zone limit
                if (ZoneServer.Server.ConnectionsCount > ZoneServer.Server.Configuration.MaximumConnections)
                {
                    Outgoing = ZoneServer.Server.CreateMessage();
                    Outgoing.Write((byte)Packets.SC_Disconnect);
                    Outgoing.Write((byte)4); //Full Zone
                    //Send it
                    ZoneServer.Server.SendMessage(Outgoing, Target, NetDeliveryMethod.ReliableOrdered);

                    //Deny them
                    Target.Deny();
                    return;
                }

                string ticket = Incoming.ReadString();
                string username = Incoming.ReadString();
                string alias = Incoming.ReadString();

                Console.WriteLine("Player ({0}) is requesting connection approval.", alias);

                foreach (var a in ZoneServer.Database.Aliases)
                {
                    //Find their alias
                    if (a.Name == alias)
                    {
                        foreach (var b in ZoneServer.Database.Accounts)
                        {
                            //Find their account ID
                            if (b.ID == a.AccountID)
                            {
                                if (b.Ticket == ticket)
                                {
                                    //Approved!
                                    Target.Approve();

                                    //Make their player structure
                                    Player newPlayer = Zone.NewPlayer(Target, alias);

                                    //Send their assets

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
