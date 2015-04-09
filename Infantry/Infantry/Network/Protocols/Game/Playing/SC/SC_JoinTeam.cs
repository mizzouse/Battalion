using System;
using System.Collections.Generic;

using Lidgren.Network;
using Microsoft.Xna.Framework;

using Infantry.Objects;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_JoinTeam
    {
        #region Declarations
        private NetIncomingMessage msg;
        public long ID;
        public int teamID;
        public string teamName;

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
            teamID = msg.ReadInt32();
            teamName = msg.ReadString();
        }

        internal void Execute()
        {/*
            if (!Infantry._arena.Teams.ContainsKey(teamID))
            {
                //Create the team
                Infantry._arena.Teams.Add(teamID, teamName);

                Infantry._vehicles[ID]._team = teamName;
            }
            else
            {
                Infantry._vehicles[ID]._team = Infantry._arena.Teams[teamID];
            }
           */ 
        }

    }
}
