using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class WeaponInfo
    {
        #region Declarations
        public class AmmoUsage
        {
            public int AmmoID;
            public int AmmoUsed;
            public bool DefaultAmmo;

            public int MuzzleAcceleration;
            public int FireDelay;
            public int ShotsBeforeDelay;
            public int PreFireDelay;//
            public int ReloadDelay;//
            public int EnergyCost;//
            public int AmmoCapacity;//

            public int StartOffsetX;
            public int StartOffsetY;
            public int StartOffsetZ;

            public int YawOffset;

            public AmmoUsage(int id)
            {
                AmmoID = id;
                AmmoUsed = 0;
                DefaultAmmo = false;
                MuzzleAcceleration = 0;
                FireDelay = 0;
                ShotsBeforeDelay = 0;
                StartOffsetX = 0;
                StartOffsetY = 0;
                StartOffsetZ = 0;
                YawOffset = 0;
                PreFireDelay = 0;
                ReloadDelay = 0;
                EnergyCost = 0; //Grab this value from the corresponding item effectonselfvehicle energy value here
                AmmoCapacity = 0; 
            }
        }

        public int WeaponID;
        public string Name;
        public string Category;
        public string Description;

        public int Weight;
        public int ExpireTime;

        public int BuyPrice;
        public int SellPrice;
        public bool Buyable;
        public bool Sellable;
        public bool Visible;
        public bool DevOnly;

        public bool OneTimeUse;

        public Dictionary<int, AmmoUsage> AmmoTypes;
        #endregion

        #region Constructors

        #endregion

        #region Member Functions
        public string parseClassSkills()
        {
            return String.Empty;
        }

        public string[] GetWeaponInfo()
        {
            string[] info = new string[5];

            info[0] = String.Format(
                "{0} : {1}"
                , Category, Name);

            info[1] = parseClassSkills();

            info[2] = "";

            //damage type
            return info;
        }
        #endregion
    }
}
