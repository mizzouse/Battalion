using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class AttributeInfo
    {
        public int AttributeID;
        public string Name;
        public string Category;
        public string Description;

        public int MaxVelocity;
        public int FAcceleration;
        public int SAcceleration;
        public int BAcceleration;
        public int MaxAcceleration;

        public int NormalWeight;
        public int StopWeight;

        public int BaseEnergy;
        public int EnergyRate;
        public int HitPoints;

        public int BuyPrice;
        public int SellPrice;
        public bool UseExperience;          //Cash if false
        public bool Buyable;
        public bool Sellable;
        public bool Visible;
        public bool DevOnly;
        public int Multiplier;

        public AttributeInfo(int id)
        {
            AttributeID = id;
            Name = "Attribute Name";
            Category = "None";
            Description = "No Description";

            MaxVelocity = 0;
            FAcceleration = 0;
            SAcceleration = 0;
            BAcceleration = 0;
            MaxAcceleration = 0;

            NormalWeight = 0;
            StopWeight = 0;

            BaseEnergy = 0;
            EnergyRate = 0;
            HitPoints = 0;

            BuyPrice = 0;
            SellPrice = 0;
            UseExperience = true;          //Cash if false
            Buyable = true;
            Sellable = true;
            Visible = true;
            DevOnly = false;
            Multiplier = 0;
        }
    }
}
