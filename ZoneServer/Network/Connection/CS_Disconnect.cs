using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_Disconnect
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Disconnect() { }

        /// <summary>
        /// C2S - Reads in packet data only, does server side functions and 
        /// sends responses back to other clients if needed.
        /// </summary>
        /// <remarks>This is mainly used when a client has either quit
        /// a zone or quit their client.</remarks>
        public CS_Disconnect(NetIncomingMessage Incoming)
        {
            NetConnection Target;

            if ((Target = Incoming.SenderConnection) != null)
            {
                Player player = Zone.GetPlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
                if (player == null)
                    //Since they arent in a list, no further computation needed
                    return;

                //Check if the player is leaving an arena
                if (player.Arena != null)
                {
                    //Remove them from arena

                    //Inform everyone they left the arena

                    //Remove them from zone
                    Zone.RemovePlayer(Incoming.SenderConnection.RemoteUniqueIdentifier);
                }
            }
        }
    }
}
