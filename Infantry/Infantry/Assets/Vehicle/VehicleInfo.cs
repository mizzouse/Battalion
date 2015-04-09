using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public class VehicleInfo
    {
        #region Declarations
        public int VehicleID;                   //The ID of the vehicle used to distinguish this vehicle from other vehicles
        public int ModelID;                     //The ID of the model used for this vehicle. Generated in ModelManager.cs
        public string Name;                     //The name of this vehicle that is shown to players
        public string Description;              //The description of this vehicle that is shown to players

        public int HitPoints;                   //The amount of health this vehicle has. At zero the vehicle is considered dead
        public int NormalWeight;              //The weight at which this vehicle begins slowing down because of being overweight [1%-99%]
        public int StopWeight;                //The weight at which this vehicle is no longer able to move    [100% overweight]

        //The following variables decide whether or not to show others this vehicles health or energy
        //0 = No one, 1 = Team mates, 2 = Everyone
        public int ShowEnergy;
        public int ShowHealth;

        public int BaseEnergy;                   //The maximum amount of energy this vehicle has initially -- this can change with attributes or items
        public int EnergyRate;                   //Recharge rate for the vehicle's energy
        public int StartingEnergy;               //The starting amount of energy a vehicle has       

        public bool Controllable;               //Can a human player controll this vehicle?
        public bool Warpable;                   //Can this vehicle be warped to another location through the use of skills/items/weapons

        public string NmeRadarColor;             //Color of the blip on the minimap 
        public bool DisplayOnNmeRader;          //Does this vehicle show up on an enemie's mini-map
        public string TeamRadarColor;            //Color of the blip on the minimap
        public bool DisplayOnFriendlyRadar;     //Does this vehicle show up on a teammates mini-map     

        public Dictionary<int, int> Drops;       //Ids of the items or weapons this vehicle will drop when it dies and the quantity dropped
        public Dictionary<int, int> Inventory;   //Ids of the items or weapons this vehicle will have in their inventory
    //    public Dictionary<int, Assets.ArmorType> InnateArmor;


        public int MaxVelocity;
        public int forwardAcceleration;
        public int backwardAcceleration;
        public int sideAcceleration;
        #endregion

        #region Constructors
        public VehicleInfo()
        {
            Drops = new Dictionary<int, int>();
            Inventory = new Dictionary<int, int>();
        }
        #endregion

        #region Member Functions
        
        #endregion
    }
}
