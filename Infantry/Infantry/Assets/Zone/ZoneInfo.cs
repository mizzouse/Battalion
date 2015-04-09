using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class ZoneInfo
    {
        public List<GameType> GameTypes;            //Game types supported by this zone
        public List<TerrainInfo> Terrains;
        public List<FlagInfo> Flags;
        public List<WarpInfo> Warps;
        public TeamInfo Teams;

        #region Zone-Wide variables
        //Zone options
        public int MaxPlayers;
        public int MaxArenas;
        public bool AllowPrivateArena;

        //Player Options
        public bool playerShowEnergy;
        public bool playerShowHealth;
        public bool playerShowAlias;
        public bool playerShowBounty;

        //Spectator Options
        public bool spectatorShowEnergy;
        public bool spectatorShowEnergyShowHealth;
        public bool spectatorShowEnergyShowAlias;
        public bool spectatorShowEnergyShowBounty;

        //Enemy Options
        public bool enemyShowEnergy;
        public bool enemyShowHealth;
        public bool enemyShowAlias;
        public bool enemyShowBounty;

        //Bounty
        public int PercentBountyToKiller;
        public int PercentBountyToAssit;
        public int FixedBounty;

        //Cash
        public int PercentCashToKiller;
        public int PercentCashToAssit;
        public int FixedCash;
        public int CashShareRadius;

        //Experience
        public int PercentExperienceToKiller;
        public int PercentExperienceToAssit;
        public int FixedExperience;
        public int ExperienceShareRadius;

        //Points
        public int PercentPointsToKiller;
        public int PercentPointsToAssit;
        public int FixedPoints;
        public int PointsShareRadius;

        //Line of sight
        public int LOSAngle;
        public int LOSDistance;
        public bool LOSShare;
        #endregion

        public ZoneInfo()
        {
            GameTypes = new List<GameType>();
            Terrains = new List<TerrainInfo>();
            Flags = new List<FlagInfo>();
            Warps = new List<WarpInfo>();

            Teams = new TeamInfo();
        }
    }
}
