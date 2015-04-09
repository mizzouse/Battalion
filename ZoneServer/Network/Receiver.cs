using System;
using System.Collections.Generic;

using PacketHeaders;
using Lidgren.Network;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    /// <summary>
    /// This is the Zone Servers Packet Receiving Object class.
    /// </summary>
    public class Receiver
    {
        /// <summary>
        /// Processes incoming data packets
        /// </summary>
        public void ProcessRequest(NetIncomingMessage Incoming)
        {
            switch (Incoming.MessageType)
            {
                case NetIncomingMessageType.Data:
                    {
                        switch ((Packets)Incoming.ReadByte())
                        {
                            #region Game Packets - High Frequency
                            case Packets.CS_Movement:
                                CS_Movement movement = new CS_Movement(Incoming);
                                break;

                            case Packets.CS_Projectile:
                                CS_Projectile projectile = new CS_Projectile(Incoming);
                                break;

                            case Packets.CS_Drop:
                                CS_Drop drop = new CS_Drop(Incoming);
                                break;

                            case Packets.CS_StoreBuy:
                                CS_StoreBuy buy = new CS_StoreBuy(Incoming);
                                break;
                            #endregion

                            #region Chat Packets - Medium Frequency
                            case Packets.CS_Chat:
                                CS_Chat chat = new CS_Chat(Incoming);
                                break;
                            #endregion

                            #region Connection - Low Frequency
                            case Packets.CS_Disconnect:
                                CS_Disconnect discon = new CS_Disconnect(Incoming);
                                break;

                            case Packets.SC_Disconnect:
                                //This packet only comes from other servers.
                                //If this packet is read in, that means a server is shutting down.
                                if (Incoming.SenderConnection != null)
                                {
                                    long Sender = Incoming.SenderConnection.RemoteUniqueIdentifier;
                                    NetPeerConfiguration Config = Incoming.SenderConnection.Peer.Configuration;
                                    Console.WriteLine("Server ({0}) disconnected. - {1}", Config.AppIdentifier, Sender);
                                }
                                break;
                            #endregion
                        }
                    }
                    break;

                case NetIncomingMessageType.ConnectionApproval:
                    //Client should be sending us a ticket id they received from
                    //logging in through the account server. Since the packet comes with
                    //a ticket id that changes each login, we can confirm or deny if this is a legit log in.
                    CS_JoinZone join = new CS_JoinZone(Incoming);
                    break;
            }

            //Finally recycle our message
            ZoneServer.Server.Recycle(Incoming);
        }
    }
}
