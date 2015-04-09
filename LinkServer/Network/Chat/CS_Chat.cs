using System;
using System.Collections.Generic;

using Lidgren.Network;

namespace LinkServer.Network
{
    public class CS_Chat
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Chat() { }

        /// <summary>
        /// C2S - Gathers or reads incoming data
        /// S2C - Sends outgoing packet to all zoneservers
        /// </summary>
        public CS_Chat(NetIncomingMessage Incoming)
        {
            if (Incoming.SenderConnection != null)
            {
                NetOutgoingMessage Msg = LinkServer.Server.CreateMessage();

                switch (Incoming.PeekByte())
                {
                    //Re-Add header back to our packet then pass it along
                    case 1: //Private
                    case 3: //Squad
                    case 4: //Custom Chats(Grouped type like !911)
                    case 5: //Personal Command
                    case 6: //System Command
                    case 7: //System Alert(Like server shutdown or message from a mod)
                        Msg.Write((byte)PacketHeaders.Packets.CS_Chat);
                        Msg.Write((byte)PacketHeaders.Packets.Link);
                        Msg.Write(Incoming);
                        LinkServer.Server.SendToAll(Msg, NetDeliveryMethod.ReliableOrdered);
                        break;
                }
            }
        }
    }
}
