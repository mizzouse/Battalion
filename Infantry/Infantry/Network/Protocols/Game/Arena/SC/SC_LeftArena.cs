using System;
using System.Collections.Generic;

using Lidgren.Network;
using Microsoft.Xna.Framework;

using Infantry.UI;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_LeftArena
    {
        #region Declarations
        private NetIncomingMessage msg;

        private string alias;
        private long playerID;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            //Get the packet data
            playerID = msg.ReadInt64();
            alias = msg.ReadString();
        }

        internal void Execute()
        {
            //Act on the data received
            if (GameManager.Vehicles.ContainsKey(playerID))
                GameManager.Vehicles.Remove(playerID);

            ChatMessage chatMsg = new ChatMessage(alias, "");
            chatMsg.Message = " has left the arena.";
            chatMsg.TypeChat = (byte)ChatMessage.Type.Custom;
            chatMsg.Color = Color.Red;
            chatMsg.Time = DateTime.Now;
            //game.chatBox.writeLine(chatMsg);
        }
    }
}