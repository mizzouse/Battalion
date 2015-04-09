using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

namespace LinkServer.Network
{
    class Receiver
    {
        /// <summary>
        /// This is our main list for each zone thats currently connected to the link server
        /// </summary>
        /// <remarks>int = position in list, connection = zoneserver</remarks>
        public static Dictionary<int, NetConnection> Connections = new Dictionary<int, NetConnection>();

        public void ProcessRequest(NetIncomingMessage Incoming)
        {
            switch (Incoming.MessageType)
            {
                case NetIncomingMessageType.Data:
                    {
                        switch ((Packets)Incoming.ReadByte())
                        {
                            case Packets.SC_Disconnect:
                            case Packets.CS_Disconnect:
                                CS_Disconnect discon = new CS_Disconnect(Incoming);
                                break;

                            case Packets.CS_Chat:
                                CS_Chat chat = new CS_Chat(Incoming);
                                break;
                            //Write packets that can pass through the link server
                        }
                    }
                    break;

                case NetIncomingMessageType.ConnectionApproval:
                    //A zone server is attempting a connection
                    CS_ZoneLogin login = new CS_ZoneLogin(Incoming);
                    break;

                case NetIncomingMessageType.StatusChanged:
                    if (Incoming.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        //Pass this to our CS_Disconnect packet and let it handle it there
                        new CS_Disconnect(Incoming);
                    break;

                case NetIncomingMessageType.DiscoveryRequest:
                    //A connection is requesting if we are running.
                    //Lets send a response back, no need to do checks here
                    //since our packet readers do this for us.
                    LinkServer.Server.SendDiscoveryResponse(null, Incoming.SenderEndPoint);
                    break;
            }
        }
    }
}
