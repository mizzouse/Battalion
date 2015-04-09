using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_ZoneInfo
    {
        #region Declarations
        private NetIncomingMessage msg;

        private int count;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            Console.WriteLine("Receiving Zone assets");
            //Get the packet data

            ZoneInfo info = new ZoneInfo();

            info.AllowPrivateArena = msg.ReadBoolean();
            info.CashShareRadius = msg.ReadInt32();
            info.enemyShowAlias = msg.ReadBoolean();
            info.enemyShowBounty = msg.ReadBoolean();
            info.enemyShowEnergy = msg.ReadBoolean();
            info.enemyShowHealth = msg.ReadBoolean();
            info.ExperienceShareRadius = msg.ReadInt32();
            info.FixedBounty = msg.ReadInt32();
            info.FixedCash = msg.ReadInt32();
            info.FixedExperience = msg.ReadInt32();
            info.FixedPoints = msg.ReadInt32();
           
            info.LOSAngle = msg.ReadInt32();
            info.LOSDistance = msg.ReadInt32();
            info.LOSShare = msg.ReadBoolean();
            info.MaxArenas = msg.ReadInt32();
            info.MaxPlayers = msg.ReadInt32();
            info.PercentBountyToAssit = msg.ReadInt32();
            info.PercentBountyToKiller = msg.ReadInt32();
            info.PercentCashToAssit = msg.ReadInt32();
            info.PercentCashToKiller = msg.ReadInt32();
            info.PercentExperienceToAssit = msg.ReadInt32();
            info.PercentExperienceToKiller = msg.ReadInt32();
            info.PercentPointsToAssit = msg.ReadInt32();
            info.PercentPointsToKiller = msg.ReadInt32();
            info.playerShowAlias = msg.ReadBoolean();
            info.playerShowBounty = msg.ReadBoolean();
            info.playerShowEnergy = msg.ReadBoolean();            
            info.playerShowHealth = msg.ReadBoolean();
            info.PointsShareRadius = msg.ReadInt32();
            info.spectatorShowEnergy = msg.ReadBoolean();
            info.spectatorShowEnergyShowAlias = msg.ReadBoolean();
            info.spectatorShowEnergyShowBounty = msg.ReadBoolean();
            info.spectatorShowEnergyShowHealth = msg.ReadBoolean();

            info.Teams.AllowPrivateTeams = msg.ReadBoolean();
            info.Teams.AllowTeamKills = msg.ReadBoolean();
            info.Teams.AllowTeamSwitch = msg.ReadBoolean();
            info.Teams.EnergyRequiredToSwitch = msg.ReadInt32();
            info.Teams.ForceEvenTeams = msg.ReadBoolean();
            info.Teams.MaxPrivatePlayers = msg.ReadInt32();
            info.Teams.MaxPublicPlayers = msg.ReadInt32();

            count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                TeamInfo.Team t = new TeamInfo.Team();

                t.Color = msg.ReadString();
                t.Name = msg.ReadString();

                info.Teams.PublicTeams.Add(t);
            }

            count = msg.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                FlagInfo f = new FlagInfo(0);
                //f.DrawScale = msg.ReadInt32();
                f.ID = msg.ReadInt32();
                //f.ModelID = msg.ReadInt32();
                f.Name = msg.ReadString();
                f.Position.X = msg.ReadInt32();
                f.Position.Y = msg.ReadInt32();
                f.Position.Z = msg.ReadInt32();

                info.Flags.Add(f);
            }
        /*    info.Flags = msg.ReadBoolean();
            info.GameTypes = msg.ReadBoolean();
            info.Teams = msg.ReadBoolean();
            info.Terrains = msg.ReadBoolean();
            info.Warps = msg.ReadBoolean();
         */

            AssetInfo.zone = info;
        }

        internal void Execute()
        {
            //Act on the data received
            //Last packet received for our game state loading, we are in game
            GameManager.GameState = State.InGame;
        }
    }
}
