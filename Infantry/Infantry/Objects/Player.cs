using System;
using System.Collections.Generic;
using System.Linq;

using Infantry.Assets;
using Infantry.Network;

namespace Infantry.Objects
{
    public class Player
    {
        #region Declarations
        /// <summary>
        /// Our account ticket id
        /// </summary>
        public string TicketID;

        /// <summary>
        /// Our players id after joining an arena
        /// </summary>
        public long ID;

        /// <summary>
        /// Our current player alias
        /// </summary>
        public string Alias;

        /// <summary>
        /// Our inventory class
        /// </summary>
        public Inventory Inventory;

        /// <summary>
        /// Our current attributes
        /// </summary>
        public Dictionary<AttributeInfo, int> Attributes;

        /// <summary>
        /// Our skill class
        /// </summary>
        public SkillInfo Skill;

        /// <summary>
        /// Our base vehicle class
        /// </summary>
        public Vehicle Vehicle;

        /// <summary>
        /// Our vehicle class id
        /// </summary>
        public int VehicleID;

        /// <summary>
        /// Our current experience
        /// </summary>
        public int Experience;

        /// <summary>
        /// Our overall experience
        /// </summary>
        public int TotalExperience;
        #endregion

        /// <summary>
        /// Generic Player Constructor
        /// </summary>
        public Player()
        {
            Inventory = new Inventory();
            Attributes = new Dictionary<AttributeInfo, int>();
            Skill = new SkillInfo(0);
        }

        /// <summary>
        /// Switches you between in game and out of game
        /// </summary>
        public void Spectate()
        {
            if (!Vehicle.IsSpec)
            {
                if (Vehicle.Energy < AssetInfo.zone.Teams.EnergyRequiredToSwitch)
                    return;

                //Subtract the energy required to switch teams from them
                Vehicle.Energy -= AssetInfo.zone.Teams.EnergyRequiredToSwitch;
            }

            CS_SpecMode spectate = new CS_SpecMode();
            spectate.Send();
        }

        public string GetAttributes()
        {
            string output = "";

            foreach (KeyValuePair<AttributeInfo, int> pair in Attributes)
            {
                output += pair.Key.AttributeID.ToString() + ",";
                output += pair.Value.ToString() + ",";
            }

            return output;
        }
    }
}
