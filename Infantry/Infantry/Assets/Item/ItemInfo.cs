using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class ItemInfo
    {
        #region Member Classes
        public class ItemOnVehicleEffects
        {
            public bool Enabled;                //Is this enabled
            public bool IgnorePhysics;          //Does this just ignore physics and allow moving through anything
            public int AuraRadius;              //Radius of the sphere of influence for this effect

            public int OCashCost;               //One time use -- cash cost
            public int OEnergyCost;             //                energy cost
            public int OHealthCost;             //                health cost
            public int OBountyCost;             //                bounty cost

            public int MCashCost;               //Ongoing use  -- cash cost
            public int MEnergyCost;             //                energy cost
            public int MHealthCost;             //                health cost
            public int MBountyCost;             //                bounty cost

            public ItemOnVehicleEffects()
            {
                Enabled = false;
                IgnorePhysics = false;
                AuraRadius = 0;
                OCashCost = 0;
                OEnergyCost = 0;
                OHealthCost = 0;
                OBountyCost = 0;
                MCashCost = 0;
                MEnergyCost = 0;
                MHealthCost = 0;
                MBountyCost = 0;
            }
        }

        public class ItemOnItemEffects
        {
            public int AuraRadius;              //Radius of the sphere of influence for this effect
            public bool Enabled;
            public int ItemID;                  //ID of item we are going to manipulate
            public bool Disable;                //Do we disable the item
            public bool TurnOff;                //Do we turn it off once

            public ItemOnItemEffects()
            {
                AuraRadius = 0;
                Enabled = false;
                ItemID = 0;
                Disable = false;
                TurnOff = false;
            }
        }

        public class ItemOnWeaponEffects
        {
        }
        #endregion

        #region Member Declarations
        public int ItemID;                      //This items ID
        public string Name;                     //Name of the item
        public string Category;                 //Category of the item
        public string Description;              //Description of item
        public int Weight;                      //Weight of the item
        public int MaxAmount;                  //Max amount of this item the player can have
        public int ExpireTime;                  //Allows item to disappear after a certain amount of time from it started to be held

        public int ModelID = 2;

        public int BuyPrice;                    //Cost of this item
        public int SellPrice;                   //Price this item sells for
        public bool Droppable;                  //Can this item be dropped?
        public bool Toggle;                     //Can this item be toggled off?
        public bool OneTimeUse;                 //Can this item be used only once?
        public bool Buyable;                    //Can this item be bought from store?
        public bool Sellable;                   //Can this item be sold to store?
        public bool Visible;                    //Can this item be visible in the store?
        public bool DevOnly;                    //Can this item be only be used by Developers/Admins?

        public int CloakRadius;                 //How far are we cloaked
        public int StealthRadius;               //How far are we stealthed
        public int CloakAura;                   //If greater than 0 this will apply the same cloak effects to teammates within radius
        public int StealthAura;                 //If greater than 0 this will apply the same stealth effects to teammates within radius

        public ItemOnVehicleEffects EnemyVehicleEffects;
        public ItemOnVehicleEffects TeamVehicleEffects;
        public ItemOnVehicleEffects SelfVehicleEffects;

        public List<ItemOnItemEffects> ItemEffects;
        public List<ItemOnWeaponEffects> WeaponEffects;

        public int PruneChance;                 //Chance of this item disappearing when on the ground
        #endregion

        #region Constructors

        #endregion

        #region Member Functions
        public string[] getItemInfo()
        {

            string[] info = new string[5];

            info[0] = String.Format(
                "{0} : {1}"
                , Category, Name);

            info[1] = "";

            info[2] = String.Format(
            "Weight: {0,20} {1,13} {2,15}\n"
                , Weight, "Max Allowed:", MaxAmount);

            //damage type
            return info;

        }
        #endregion
    }
}
