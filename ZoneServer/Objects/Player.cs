using System;
using System.Collections.Generic;

using Lidgren.Network;

namespace ZoneServer.Objects
{
    /// <summary>
    /// Unlike the client, the player class is an extension of a vehicle
    /// </summary>
    public class Player
    {
        //Connection
        public string Alias;                //Players alias
        public NetConnection Connection;    //Contains their private UID
        public int PlayerID;                //Their public arena ID

        //Information
        public Arena Arena;
        public int VehicleID;
        public Vehicle Vehicle;
        public bool bIsSpectator;

        //Inventory
        private Dictionary<int, int> Inventory;     //[id, quantity]
        public int Cash;
        public int Experience;

        #region Constructor
        /// <summary>
        /// Constructor for creating a player
        /// </summary>
        /// <param name="connection">Their connection to the server</param>
        /// <param name="arena">The arena they are joining</param>
        public Player(NetConnection connection, Arena arena)
        {
            Connection = connection;
            Arena = arena;
        }
        #endregion

        #region Connection
        /// <summary>
        /// This can only be called if the player has begun entering an arena
        /// </summary>
        /// <param name="ID">The Skill ID</param>
        public void SetUpVehicle(int ID)
        {
            //Make this player's vehicle base
            Vehicle = new Vehicle(Assets.Skills[ID].VehicleID);

            Vehicle.Alias = Alias;
            Vehicle.bIsHuman = true;
            this.bIsSpectator = true;
            Vehicle.Owner = this;

            Vehicle.Arena = Arena;

            //Add their vehicle to the arena
            Arena.AddPlayer(Vehicle);
        }

        /// <summary>
        /// Sends all assets to the player
        /// </summary>
        public void SendAssets()
        {
            //Lidgren will take care of making sure all of these are sent
            //Once the client receives all of the assets it will signal this
            //server to start sending all the other info needed to get in game
            //We do this by just sending a hash and making sure client can calculate
            //the same one. This hash has nothing in common with our security techniques
            //to make sure the client is not changing the memory
            Assets.SendTotalChecksum(Connection);
            Assets.SendItems(Connection);
            Assets.SendVehicles(Connection);
            Assets.SendWeapons(Connection);
        }
        #endregion
    }
}
