using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_ArenaList
    {
        #region Declarations
        private NetIncomingMessage msg;
        private int arenaCount;
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
            /*
            game._arenas.Clear();
            arenaCount = msg.ReadInt32();

            for (int i = 0; i < arenaCount; i++)
            {
                Arena a = new Arena();

                a._arenaID = msg.ReadInt32();
                a._arenaName = msg.ReadString();
                a._isPublic = msg.ReadBoolean();

                int j = 0;

                foreach (TeamInfo.Team team in AssetInfo.zone.Teams.PublicTeams)
                {
                    a.Teams.Add(j++, team.Name);
                }

                game._arenas.Add(a);
            } */
        }

        internal void Execute()
        {
            //Act on the data received
            if (arenaCount > 0)
                GameManager.GameState = State.InGame;

            //Set the first arena to our current one if the player is joining the zone and not switching arenas
            //Infantry._arena = game._arenas.First();
        }
    }
}
