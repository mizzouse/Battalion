using System;
using System.Collections.Generic;

using Lidgren.Network;

using PacketHeaders;

namespace AccountServer.Network
{
    class Receiver
    {
        /// <summary>
        /// Processes our incoming packet data
        /// </summary>
        public void ProcessRequest(NetIncomingMessage Incoming)
        {
            switch (Incoming.MessageType)
            {
                case NetIncomingMessageType.Data:
                    {
                        switch ((Packets)Incoming.ReadByte())
                        {
                            case Packets.CS_ZoneList:
                                CS_ZoneList Zonelist = new CS_ZoneList(Incoming);
                                break;
                        }
                    }
                    break;
                    
                case NetIncomingMessageType.ConnectionApproval:
                    //A client is attempting to log into the account server
                    CS_AccountLogin login = new CS_AccountLogin(Incoming);
                    break;

                case NetIncomingMessageType.StatusChanged:
                    if (Incoming.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        Console.WriteLine("Client: " + "({0}) Disconnected!", Incoming.SenderEndPoint);
                    break;

                case NetIncomingMessageType.DiscoveryRequest:
                    //Send the discovery response back to the client
                    AccountServer.Server.SendDiscoveryResponse(null, Incoming.SenderEndPoint);
                    break;
            }
        }
    }
}
