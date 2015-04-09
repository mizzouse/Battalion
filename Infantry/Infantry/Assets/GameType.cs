using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class GameType
    {
        public int DelayBetweenGames;
        public int GameLength;
        public int MinPlayers;
        public int ChanceToScramble;

        public int WinnerCashReward;
        public int WinnerExperienceReward;
        public int LoserCashReward;
        public int LoserExperienceReward;
        public int Multipler;
        public int Timer;

        public bool Enabled;
        public bool ResetFlags;
        public bool RespawnPlayers;
    }
}
