using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class SkillInfo
    {
        public int SkillID;
        public string Name;
        public string Category;
        public string Description;
        public int VehicleID;

        public int BuyPrice;
        public int SellPrice;
        public bool UseExperience;          //Cash if false
        public bool Buyable;
        public bool Sellable;
        public bool Visible;
        public bool DevOnly;

        public SkillInfo(int id)
        {
            SkillID = id;
            Name = "Class Name";
            Category = "None";
            Description = "No Description";
            
            BuyPrice = 0;
            SellPrice = 0;
            UseExperience = true;          //Cash if false
            Buyable = true;
            Sellable = true;
            Visible = true;
            DevOnly = false;
        }
    }
}
