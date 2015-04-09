using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class TerrainInfo
    {
        public string Name;
        public string Message;

        public bool StoreEnabled;
        public bool SkillStoreEnabled;
        public bool AllowTeamChange;
        public bool AllowSpectate;
        public bool AllowQuit;
        public bool AllowFiring;
        public bool AllowItemUse;
        public bool AllowProjectiles;
        public bool AllowAFK;

        public int QuitDelay;
        public int SpectateDelay;
        public int ChangeArenaDelay;

        public int HealthRate;
        public int EnergyRate;
        public int BountyRate;
    }
}
