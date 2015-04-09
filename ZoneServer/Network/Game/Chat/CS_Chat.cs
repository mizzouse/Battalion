using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;
using PacketHeaders;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    public class CS_Chat
    {
        private NetConnection Sender;

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Chat() { }

        /// <summary>
        /// C2S - Reads in packet data
        /// S2C - Sends outgoing packet data to either linkserver or client directly
        /// </summary>
        public CS_Chat(NetIncomingMessage Incoming)
        {
            if ((Sender = Incoming.SenderConnection) != null)
            {
                //Is this from the link server?
                if (Incoming.PeekByte() == (byte)Packets.Link)
                {
                    Incoming.ReadByte(); //Takes the link header out
                    ParseData(Incoming);
                    return;
                }

                //We only read the first packet data "type" to find
                //how the packet was constructed(which order).
                int ChatType = Incoming.ReadByte();

                //Finds our sender in the zone
                Player player = Zone.GetPlayer(Sender.RemoteUniqueIdentifier);
                if (player == null)
                {
                    Console.WriteLine("Player ({0}) not present in zone list.", Sender);
                    return;
                }

                NetOutgoingMessage Msg = ZoneServer.Server.CreateMessage();
                switch (ChatType)
                {
                    case 0:
                        //Public
                        //Prepare packet
                        Msg.Write((byte)ChatType);          //Public
                        Msg.Write(player.Alias);            //Sender's Alias
                        Msg.Write(Incoming.ReadString());   //Message
                        Msg.Write(Incoming.ReadInt64());    //Timestamp

                        //Find each player in the same arena and pass the chat to them
                        foreach (Player P in Zone.Players.Values)
                        {
                            if (P == null || P == player)
                                continue;

                            if (P.Arena == player.Arena)
                                ZoneServer.Server.SendMessage(Msg, P.Connection, NetDeliveryMethod.ReliableOrdered);
                        }
                        break;

                    case 1:
                        //Private(whisper)
                        //Grab the target player
                        //Note: We only peek at the packet to not move our pointer
                        Player Target = Zone.GetPlayer(Incoming.PeekString()); //Second byte in packet

                        //Set up the packet for sending
                        Msg.Write((byte)ChatType);        //Private
                        Msg.Write(player.Vehicle.Alias);  //Sender's Alias
                        Msg.Write(Incoming.ReadString()); //Target's Alias
                        Msg.Write(Incoming.ReadString()); //Message
                        Msg.Write(Incoming.ReadInt64());  //Timestamp

                        //Lets see if they are on the same server
                        if (Target == null)
                            //They arent, send it to our link server
                            ZoneServer.LinkServer.SendMessage(Msg, NetDeliveryMethod.ReliableOrdered);
                        else
                            //They are, send it directly to them
                            ZoneServer.Server.SendMessage(Msg, Target.Connection, NetDeliveryMethod.ReliableOrdered);
                        break;

                    case 2:
                        //Team
                        //Prepare Packet
                        Msg.Write((byte)ChatType);          //Team
                        Msg.Write(player.Alias);            //Sender's Alias
                        Msg.Write(Incoming.ReadString());   //Message
                        Msg.Write(Incoming.ReadInt64());    //Timestamp
                        //Find players on the same team and send
                        foreach (Player P in Zone.Players.Values)
                        {
                            if (P == null || P == player)
                                continue;

                            if (P.Vehicle == null)
                                continue;

                            if (P.Vehicle.Team == player.Vehicle.Team)
                                ZoneServer.Server.SendMessage(Msg, P.Connection, NetDeliveryMethod.ReliableOrdered);
                        }
                        break;

                    case 3:
                        //Squad
                        //Prepare Packet
                        Msg.Write((byte)ChatType);          //Squad
                        Msg.Write(player.Alias);            //Sender's Alias
                        Msg.Write(Incoming.ReadString());   //Message
                        Msg.Write(Incoming.ReadInt64());    //Timestamp
                        //Find players on the same squad and send
                        foreach (Player P in Zone.Players.Values)
                        {
                            if (P == null || P == player)
                                continue;

                            //Add chat checker here
                            ZoneServer.Server.SendMessage(Msg, P.Connection, NetDeliveryMethod.ReliableOrdered);
                        }
                        break;

                    case 4:
                        //Custom (!911)
                        //Prepare Packet
                        Msg.Write((byte)ChatType);          //Custom
                        Msg.Write(player.Alias);            //Sender's Alias
                        Msg.Write(Incoming.ReadString());   //Message
                        Msg.Write(Incoming.ReadInt64());    //Timestamp

                        //Send it to link server to pass along servers
                        ZoneServer.LinkServer.SendMessage(Msg, NetDeliveryMethod.ReliableOrdered);
                        break;

                    case 5:
                        //Personal Command
                        break;

                    case 6:
                        //System Command
                        break;

                    case 7:
                        //System Alert
                        break;
                }
            }
        }

        /// <summary>
        /// We internally parse any data coming from the link server
        /// to find our target player.
        /// </summary>
        internal void ParseData(NetIncomingMessage Incoming)
        {
            Player player;
            NetOutgoingMessage Msg;
            int ChatType = Incoming.ReadByte();

            //Get sender first then find what packet it is
            string Sender = Incoming.ReadString();
            switch (ChatType)
            {
                case 1:
                    //Private(whisper)
                    //Lets try finding the target player on our server
                    if ((player = Zone.GetPlayer(Incoming.ReadString())) == null)
                        //Player isnt here, disreguard packet
                        return;

                    Msg = ZoneServer.Server.CreateMessage();
                    Msg.Write((byte)Packets.SC_Chat);
                    Msg.Write(Sender);                      //Sender
                    Msg.Write(Incoming.ReadString());       //Message
                    Msg.Write(Incoming.ReadInt64());        //Timestamp
                    ZoneServer.Server.SendMessage(Msg, player.Connection, NetDeliveryMethod.ReliableOrdered);
                    break;

                case 4:
                    //Custom (!911)
                    Msg = ZoneServer.Server.CreateMessage();
                    Msg.Write((byte)Packets.SC_Chat);
                    Msg.Write(Sender);                      //Sender
                    //Msg.Write();                          //Chat Name
                    Msg.Write(Incoming.ReadString());       //Message
                    Msg.Write(Incoming.ReadInt64());        //Timestamp

                    //Lets find players that have the same chat
                    //TODO
                    break;

                case 5:
                    //Personal Command
                    break;

                case 6:
                    //System Command
                    break;

                case 7:
                    //System Alert
                    Msg = ZoneServer.Server.CreateMessage();
                    Msg.Write((byte)Packets.SC_Chat);
                    Msg.Write(Incoming.ReadString());       //Message
                    if (Incoming.PeekFloat() != null)
                        Msg.Write(Incoming.ReadVector4());  //Color

                    //Send to all players
                    foreach (Player P in Zone.Players.Values)
                    {
                        if (P == null)
                            continue;

                        ZoneServer.Server.SendMessage(Msg, P.Connection, NetDeliveryMethod.ReliableOrdered);
                    }
                    break;
            }
        }
    }
}
