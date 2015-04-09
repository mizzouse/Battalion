using System;
using System.Collections.Generic;

using Lidgren.Network;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry
{
    public class SC_FlagPickup
    {
        #region Declarations
        private NetIncomingMessage msg;
        private int flagID;
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
            flagID = msg.ReadInt32();
            playerID = msg.ReadInt64();
        }

        internal void Execute()
        {
            //Act on the data received
            foreach (FlagInfo f in AssetInfo.zone.Flags)
            {
                if (f.ID == flagID)
                {
                    f._owner = GameManager.Vehicles[playerID];
                    return;
                }
            }
        }
    }
}
