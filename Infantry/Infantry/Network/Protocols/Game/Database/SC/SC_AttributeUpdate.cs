using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Assets;
using Infantry.Objects;

namespace Infantry.Network
{
    public class SC_AttributeUpdate
    {
        #region Declarations
        private NetIncomingMessage msg;        
        private Player player;
        #endregion

        public SC_AttributeUpdate(Player player)
        {
            this.player = player;
        }

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            //Get the packet data
            string att = msg.ReadString();

            if (String.IsNullOrEmpty(att))
                return;

            string[] values = att.Split(',');
            int id = 0;
            bool GotID = false;

            foreach (string s in values)
            {
                if (String.IsNullOrEmpty(s))
                    return;

                if (GotID)
                {
                    int quantity = Convert.ToInt32(s);
                    player.Attributes.Add(new AttributeInfo(id), quantity);
                    GotID = false;
                }
                else
                {
                    id = Convert.ToInt32(s);
                    GotID = true;
                }
            }
        }

        internal void Execute()
        {
            //Act on the data received
            //player._vehicle.ApplyInfo();
        }
    }
}
