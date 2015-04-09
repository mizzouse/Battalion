using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;

namespace LinkServer.Network
{
    public class CS_Disconnect
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Disconnect() { }

        /// <summary>
        /// C2S - Reads in packet data, if this packet actually
        /// came from a zone server, we will remove it from our 
        /// connection list.
        /// </summary>
        /// <remarks>Note: this should only come from a zone server,
        /// any other connection will be ignored but logged.</remarks>
        public CS_Disconnect(NetIncomingMessage Incoming)
        {
            if (Incoming.SenderConnection != null)
            {
                NetConnection connection = Incoming.SenderConnection;
                if (connection.Peer.Configuration.AppIdentifier == "ZoneServer")
                {
                    foreach (KeyValuePair<int, NetConnection> pair in Receiver.Connections.ToArray())
                        if (pair.Value.RemoteUniqueIdentifier == connection.RemoteUniqueIdentifier)
                            Receiver.Connections.Remove(pair.Key);
                }
                else
                    Console.WriteLine("Incoming disconnection packet from unknown connection. - {0}", connection.RemoteUniqueIdentifier);
            }
        }
    }
}
