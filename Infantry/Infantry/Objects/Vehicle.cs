using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Infantry.Objects
{
    public class Vehicle
    {
        #region Declarations
        /// <summary>
        /// The vehicle id
        /// </summary>
        public int ID;
        /// <summary>
        /// The player id of the owner for this vehicle
        /// </summary>
        public long PlayerID;
        /// <summary>
        /// The vehicle's alias
        /// </summary>
        public string Alias;
        /// <summary>
        /// The player that owns this vehicle if possible
        /// </summary>
        public Player Owner;
        /// <summary>
        /// The team name we are on 
        /// </summary>
        public string Team;
        /// <summary>
        /// If this is a player, are they in spec?
        /// </summary>
        public bool IsSpec;
        /// <summary>
        /// How much points this player has
        /// </summary>
        public int Bounty;
        /// <summary>
        /// The position this vehicle is
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// The velocity this vehicle is travelling
        /// </summary>
        public Vector3 Velocity = Vector3.Zero;
        /// <summary>
        /// The current acceleration this vehicle has
        /// </summary>
        public Vector3 Acceleration = Vector3.Zero;
        /// <summary>
        /// Our max velocity this vehicle can go
        /// </summary>
        public int MaxVelocity;
        /// <summary>
        /// Our max acceleration this vehicle has
        /// </summary>
        public Vector3 MaxAcceleration = Vector3.Zero;
        /// <summary>
        /// Our rotation along the Y axis(yaw)
        /// </summary>
        public float Yaw;
        /// <summary>
        /// Our current energy amount if this is a player
        /// </summary>
        public int Energy;
        /// <summary>
        /// How fast our energy recharges
        /// </summary>
        public int EnergyRate;
        /// <summary>
        /// Our health points
        /// </summary>
        public int HitPoints;
        /// <summary>
        /// Our normal weight standing
        /// </summary>
        public int NormalWeight;
        /// <summary>
        /// Our stopping weight
        /// </summary>
        public int StopWeight;
        /// <summary>
        /// How much we weigh
        /// </summary>
        public int Weight;
        /// <summary>
        /// 
        /// </summary>
        //private VehicleController Controller;
        #endregion
    }
}
