using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace ZoneServer.Objects
{
    /// <summary>
    /// This can be a bot, player, or an empty vessel
    /// </summary>
    public class Vehicle
    {
        //Human variables
        public int UniqueID;    //For clients
        public Player Owner;
        public string Alias;

        //Stats
        public bool bIsHuman;
        public int VehicleID;
        public Team Team;

        //State
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Yaw;

        //Location
        public Arena Arena;
        public int CurrentCube;

        //Timing
        private int TickLastCubeUpdate;
        private const int CubeUpdateInterval = 100;

        #region Constructors
        /// <summary>
        /// Constructor for our Vehicle object
        /// </summary>
        public Vehicle(int ID)
        {
            VehicleID = ID;
        }
        #endregion

        #region Polling
        /// <summary>
        /// Updates our vehicles(bots, players, etc)
        /// </summary>
        public void Poll()
        {
            int now = Environment.TickCount;

            if (now - TickLastCubeUpdate > CubeUpdateInterval)
            {
                //Update their current cube
                int pos = Arena.Routing.GetPosition(Position);
                if (CurrentCube != pos)
                {
                    if (Owner != null)
                    {
                        Arena.Routing.World.MoveMember(Owner.Connection.RemoteUniqueIdentifier, pos, CurrentCube);
                        CurrentCube = pos;
                    }
                }

                TickLastCubeUpdate = now;
            }
        }
        #endregion
    }
}
