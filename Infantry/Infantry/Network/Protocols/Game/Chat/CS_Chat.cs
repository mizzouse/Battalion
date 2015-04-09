using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;
using PacketHeaders;

using Infantry.Managers;

namespace Infantry.Network
{
    public class CS_Chat
    {
        Client client = NetworkManager.GetClient("ZoneServer");

        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Chat() { }

        /// <summary>
        /// Constructor using a chat type and the message
        /// </summary>
        public CS_Chat(int Type, string message)
        {
            if (client != null)
            {
                NetOutgoingMessage packet = client.CreateMessage();

                //First write our header then our object type
                packet.Write((byte)Packets.CS_Chat);
                packet.Write(Type);

                //Now write our info
                switch (Type)
                {
                    case (int)UI.ChatMessage.Type.Private:
                        packet.Write(ParsePrivate(message));
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    case (int)UI.ChatMessage.Type.Team:
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    case (int)UI.ChatMessage.Type.Squad:
                        packet.Write(GameManager.Player.ID);
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    case (int)UI.ChatMessage.Type.Custom:
                        packet.Write(GameManager.Player.TicketID);
                        packet.Write(ParsePrivate(message));
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    case (int)UI.ChatMessage.Type.PersonalCommand:
                        packet.Write(GameManager.Player.ID);
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    case (int)UI.ChatMessage.Type.SystemCommand:
                        packet.Write(GameManager.Player.TicketID);
                        packet.Write(GameManager.Player.Vehicle.Position);
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                    default:
                        packet.Write(message);
                        packet.Write((long)Environment.TickCount);
                        break;
                }

                client.SendMessage(packet, NetDeliveryMethod.ReliableOrdered);
            }
        }

        /// <summary>
        /// Parses the message sent from the chatbox.
        /// This function grabs the target player's name
        /// or grabs the number of a private group chat.
        /// </summary>
        private static string ParsePrivate(string msg)
        {
            int i = 0;
            string output = String.Empty;

            foreach (char c in msg)
            {
                if ((c == ':' || c == ';') && i <= 2)
                {
                    i++;
                    continue;
                }

                if (i == 1)
                    output += c;
                if (i > 2)
                    break;
            }

            return output;
        }
    }
}
