using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Assets;
using Infantry.Objects;

namespace Infantry.Network
{
    public class SC_SkillUpdate
    {
        #region Declarations
        private NetIncomingMessage msg;
        private Player player;
        #endregion

        public SC_SkillUpdate(Player player)
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
            int id = msg.ReadInt32();
            //Get the packet data
            player.Skill = AssetInfo.skills[id];
            player.VehicleID = player.Skill.VehicleID;
        }

        internal void Execute()
        {
            //Act on the data received
            //player._vehicle.ApplyInfo();
        }
    }
}
