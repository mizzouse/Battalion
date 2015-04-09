using System;
using System.Collections.Generic;

using Lidgren.Network;
using Microsoft.Xna.Framework;

using Infantry.Objects;
using Infantry.Managers;

namespace Infantry
{
    public class SC_SpecMode
    {
        #region Declarations
        private NetIncomingMessage msg;
        public long ID;
        public int spectate;
        public int teamID;
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
            ID = msg.ReadInt64();
            spectate = msg.ReadInt32();
            teamID = msg.ReadInt32();
        }

        internal void Execute()
        {/*
            if (spectate == 1)
            {
                GameManager.Vehicles[ID]._team = _arena.Teams[teamID];
                Infantry._vehicles[ID].isSpec = false;
            }

            else if (spectate == 2)
            {
                Infantry._vehicles[ID]._team = Infantry._arena.Teams[teamID];
                Infantry._vehicles[ID].isSpec = true;
            }
          */
        }

    }
}
