using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class TeamInfo
    {
        public class Team
        {
            public string Name;
            public string Color;
        }

        public List<Team> PublicTeams;

        public bool AllowTeamSwitch;
        public bool AllowTeamKills;
        public bool ForceEvenTeams;
        public bool AllowPrivateTeams;

        public int EnergyRequiredToSwitch;

        public int MaxPublicPlayers;
        public int MaxPrivatePlayers;

        public TeamInfo()
        {
            PublicTeams = new List<Team>();
        }
    }
}
