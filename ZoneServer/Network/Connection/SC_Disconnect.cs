using System;
using System.Collections.Generic;

using Lidgren.Network;

namespace ZoneServer.Network
{
    public class SC_Disconnect
    {
        /// <summary>
        /// Sets the disconnection reason
        /// </summary>
        public enum Flags
        {
            Register,
            WrongPassword,
            InvalidUsername,
            AccountBanned,
            FullZone,
            FullArena,
            Gracefully,
            Unknown
        }

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public SC_Disconnect() { }

        /// <summary>
        /// S2C - Reads in packet data, sends a disconnected gracefully tag
        /// Note: This can also be called by the Command Parser
        /// </summary>
        public SC_Disconnect(NetIncomingMessage Incoming)
        {
            Disconnect(Incoming, Flags.Gracefully);
        }

        /// <summary>
        /// S2C - Reads in packet data, sends a specified disconnection tag
        /// Note: This can also be called by the Command Parser
        /// </summary>
        public SC_Disconnect(NetIncomingMessage Incoming, Flags reason)
        {
            Disconnect(Incoming, reason);
        }

        /// <summary>
        /// Internal packet sender
        /// </summary>
        private void Disconnect(NetIncomingMessage Incoming, Flags reason)
        {
            NetOutgoingMessage packet = new NetOutgoingMessage();
            packet.Write((byte)PacketHeaders.Packets.SC_Disconnect);
            packet.Write((byte)reason);

            //Send it
            ZoneServer.Server.SendMessage(packet, Incoming.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
