using System;
using System.Collections.Generic;
using System.Text;

using PacketHeaders;
using Lidgren.Network;
using Lidgren.Network.Xna;

using Infantry.Managers;
using Infantry.Network;

namespace Infantry.Handlers
{
    public static class StringHandler
    {
        /// <summary>
        /// The position of the string for our text
        /// </summary>
        public enum StringPosition
        {
            /// <summary>
            /// Aligns from left to right by padding the right side
            /// if needed.
            /// </summary>
            Left = 0,
            /// <summary>
            /// Aligns from right to left by padding the left side
            /// if needed.
            /// </summary>
            Right = 1
        }

        /// <summary>
        /// Parses a message and sends it to our network
        /// <para>The 'server' is where its going to</para>
        /// </summary>
        public static void Parse(string message)
        {
            Client client = NetworkManager.GetClient("ZoneServer");
            if (client == null)
                return;

            NetOutgoingMessage msg = new NetOutgoingMessage();
            switch (message[0])
            {
                case '?':
                    msg.Write((int)UI.ChatMessage.Type.PersonalCommand);
                    msg.Write(GameManager.Player.ID);
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
                case '*':
                    msg.Write((int)UI.ChatMessage.Type.SystemCommand);
                    msg.Write(GameManager.Player.TicketID);
                    msg.Write(GameManager.Player.Vehicle.Position);
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
                case ':':
                    msg.Write((int)UI.ChatMessage.Type.Private);
                    msg.Write(ParsePrivate(message));
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
                case '/':
                case '\'':
                    msg.Write((int)UI.ChatMessage.Type.Team);
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
                case '#':
                    msg.Write((int)UI.ChatMessage.Type.Squad);
                    msg.Write(GameManager.Player.ID);
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
                case ';':
                    msg.Write((int)UI.ChatMessage.Type.Custom);
                    msg.Write(GameManager.Player.TicketID);
                    msg.Write(ParsePrivate(message));
                    msg.Write((long)Environment.TickCount);
                    break;
                default:
                    msg.Write((int)UI.ChatMessage.Type.Public);
                    msg.Write(message);
                    msg.Write((long)Environment.TickCount);
                    break;
            }

            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Wraps an alias around a specific width and alignment.
        /// </summary>
        /// <param name="alias">Alias</param>
        /// <param name="width">Max width</param>
        /// <param name="position">Alignment</param>
        public static string WrapAlias(string alias, int width, int position)
        {
            return WrapAlias(alias, width, "", position);
        }

        /// <summary>
        /// Wraps an alias around a specific width, alignment and set prefix.
        /// </summary>
        /// <param name="alias">Alias</param>
        /// <param name="width">Max width</param>
        public static string WrapAlias(string alias, int width, string prefix, int position)
        {
            string line = "";
                
            //Check for prefix, this takes priority first
            //Make sure it isnt our default prefix as well
            if (!String.IsNullOrEmpty(prefix) && prefix[0] != '>')
                line = prefix + alias;

            line = PadLength(line, width, position);
            return line;
        }

        /// <summary>
        /// Checks length of a string against max length, pads the left side if needed
        /// </summary>
        private static string PadLength(string original, int width, int position)
        {
            int index = original.Length;
            string line = "";

            foreach (char c in original)
            {
                line += c;
                if (line.Length == width)
                    return line;
            }

            //The line must be shorter than the width
            //Lets pad!
            if (position == (int)StringPosition.Right)
                return line.PadLeft(width);
            else
                return line.PadRight(width);
        }

        /// <summary>
        /// Parses the message sent from the chatbox.
        /// This function grabs the target player's name
        /// or grabs the private group chat number.
        /// </summary>
        static string ParsePrivate(string msg)
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
