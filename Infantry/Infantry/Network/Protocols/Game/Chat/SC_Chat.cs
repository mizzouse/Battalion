using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Lidgren.Network;
using Lidgren.Network.Xna;
using PacketHeaders;

using Infantry.UI;

namespace Infantry.Network
{
    public class SC_Chat
    {
        /// <summary>
        /// Reads in packet data and sends the message to our chat box
        /// </summary>
        static public void Read(NetIncomingMessage Incoming)
        {
            ChatBox Box = Managers.ChatManager.GetChat("Chat");
            byte ChatType;

            //Are we past entering a zone?
            if (Managers.GameManager.GameState < State.EnteringZone)
                return;

            //Is our chatbox even initialized?
            if (!Box.Initialized)
                return;

            //Does this have a proper header?
            if (Incoming.ReadByte(out ChatType))
            {
                ChatMessage Message = new ChatMessage();
                switch (ChatType)
                {
                    case (byte)ChatMessage.Type.Private:
                        Message.TypeChat = ChatType;
                        Message.Alias = Incoming.ReadString();
                        Message.Message = Incoming.ReadString();
                        Message.Time = DateTime.Now.AddTicks(-Incoming.ReadInt64());
                        //Send it to our chatbox
                        Box.WriteLine(Message);
                        break;

                    case (byte)ChatMessage.Type.Team:
                        Message.TypeChat = ChatType;
                        Message.Alias = Incoming.ReadString();
                        Message.Message = Incoming.ReadString();
                        Message.Time = DateTime.Now.AddTicks(-Incoming.ReadInt64());
                        //Send it to our chatbox
                        Box.WriteLine(Message);
                        break;

                    case (byte)ChatMessage.Type.Squad:
                        Message.TypeChat = ChatType;
                        Message.Alias = Incoming.ReadString();
                        Message.Message = Incoming.ReadString();
                        Message.Time = DateTime.Now.AddTicks(-Incoming.ReadInt64());
                        //Send it to our chatbox
                        Box.WriteLine(Message);
                        break;

                    case (byte)ChatMessage.Type.Custom:
                        Message.TypeChat = ChatType;
                        Message.Alias = Incoming.ReadString();
                        Message.Message = Incoming.ReadString();
                        Message.Time = DateTime.Now.AddTicks(-Incoming.ReadInt64());
                        //Sent it to our chatbox
                        Box.WriteLine(Message);
                        break;

                    case (byte)ChatMessage.Type.PersonalCommand:
                        break;

                    case (byte)ChatMessage.Type.SystemCommand:
                        break;

                    case (byte)ChatMessage.Type.SystemAlert:
                        Message.TypeChat = ChatType;
                        Message.Message = Incoming.ReadString();
                        //Check if we have a specific color to use
                        //Note: Writing a vector4 = writing 4 floats
                        if (Incoming.PeekFloat() > 0)
                            Message.Color = new Color(Incoming.ReadVector4());
                        //Send it to our message display-TODO
                        
                        break;

                    case (byte)ChatMessage.Type.Public:
                    default:
                        Message.TypeChat = ChatType;
                        Message.Alias = Incoming.ReadString();
                        Message.Message = Incoming.ReadString();
                        Message.Time = DateTime.Now.AddTicks(-Incoming.ReadInt64());
                        if (Incoming.PeekFloat() > 0)
                            //Sent a special color
                            Message.Color = new Color(Incoming.ReadVector4());

                        //Send it to our chatbox
                        Box.WriteLine(Message);
                        break;
                }
            }
        }
    }
}
