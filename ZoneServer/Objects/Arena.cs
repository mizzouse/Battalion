using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ZoneServer.Objects
{
    public class Arena
    {
        //Information
        private int ArenaID;
        public string Title;
        public bool bIsPublic;
        public string Password;

        //Objects in the arena
        private Dictionary<int, Vehicle> Vehicles;
        private List<Team> Teams;

        //World Communication
        public Network.Routing.Functions Routing;

        #region Constructors
        /// <summary>
        /// Constructor for a public arena
        /// </summary>
        /// <param name="ID">ID of the arena</param>
        public Arena(int ID, string title)
        {
            ArenaID = ID;
            Title = title;
            bIsPublic = true;

            Vehicles = new Dictionary<int, Vehicle>();
            Teams = new List<Team>();
            AddTeam("Spectator");

            Routing = new Network.Routing.Functions();
            Routing.World.CalculateGameWorld(500, 500);
        }

        /// <summary>
        /// Constructor for a private arena
        /// </summary>
        /// <param name="ID">ID of the arena</param>
        /// <param name="password">Password for the arena</param>
        public Arena(int ID, string title, string password)
        {
            ArenaID = ID;
            Title = title;
            bIsPublic = false;
            Password = password;

            Vehicles = new Dictionary<int, Vehicle>();
            Teams = new List<Team>();
            AddTeam("Spectator");

            Routing = new Network.Routing.Functions();
            Routing.World.CalculateGameWorld(500, 500);
        }
        #endregion

        #region Polling
        /// <summary>
        /// Updates the arena
        /// </summary>
        public void Poll()
        {
            foreach (Vehicle vehicle in Vehicles.Values)
                vehicle.Poll();
        }
        #endregion

        #region Dropped Item Class
        /// <summary>
        /// Our dropped item
        /// </summary>
        public class DroppedItem
        {
            public int ItemID;
            public int Quantity;
            public Vector3 Position;
        }
        #endregion

        #region Dropped Items Add/Remove/Get
        #endregion

        #region Team Add/Remove/Get
        /// <summary>
        /// Adds a public team
        /// </summary>
        public void AddTeam(string Name)
        {
            Team team = new Team();
            team.bPublic = true;
            team.Name = Name;
            team.Password = "";

            Teams.Add(team);
        }

        /// <summary>
        /// Adds a private team
        /// </summary>
        /// <param name="Password">Sets a password for the team if desired</param>
        public void AddTeam(string Name, string Password)
        {
            Team team = new Team();
            team.bPublic = false;
            team.Name = Name;
            team.Password = (Password == null || Password == "") ? "" : Password;
        }

        /// <summary>
        /// Removes a team by name
        /// </summary>
        public void RemoveTeam(string Name)
        {
            foreach (Team t in Teams)
                if (t.Name == Name)
                {
                    Teams.Remove(t);
                    return;
                }
        }

        /// <summary>
        /// Gets a team object by name
        /// </summary>
        /// <returns>Returns team if found, null if not</returns>
        public Team GetTeam(string Name)
        {
            foreach (Team t in Teams)
            {
                if (t.Name == Name)
                    return t;
            }

            return null;
        }
        #endregion

        #region Vehicle Add/Remove/Get
        /// <summary>
        /// Adds a vehicle to the arena[Automated]
        /// </summary>
        public void AddVehicle(Vehicle veh)
        {
            //Get them a player ID [key to the dictionary]
            Vehicles.Add(ProducePlayerID(), veh);

            //Make an announcement
        }

        /// <summary>
        /// Removes a vehicle from the arena
        /// </summary>
        public void RemoveVehicle(Vehicle veh)
        {
            foreach(KeyValuePair<int, Vehicle> pair in Vehicles)
                if (pair.Value == veh)
                {
                    Vehicles.Remove(pair.Key);
                    //Make an announcement
                    return;
                }
        }

        /// <summary>
        /// Gets a vehicle class from the dictionary
        /// </summary>
        public Vehicle GetVehicle(Vehicle veh)
        {
            foreach (KeyValuePair<int, Vehicle> pair in Vehicles)
            {
                if (pair.Value == veh)
                    return pair.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of vehicles within the arena
        /// </summary>
        public List<Vehicle> GetVehicles()
        {
            return Vehicles.Values.ToList();
        }

        /// <summary>
        /// Gets a list of actual players(Humans) in the arena
        /// </summary>
        public List<Vehicle> GetHumanVehicles()
        {
            List<Vehicle> Output = new List<Vehicle>();

            foreach (Vehicle v in Vehicles.Values)
            {
                if (v.bIsHuman)
                    Output.Add(v);
            }

            return Output;
        }

        /// <summary>
        /// Gets a list of all the vehicles within the arena
        /// </summary>
        public List<Vehicle> GetAllVehicles()
        {
            return Vehicles.Values.ToList();
        }

        /// <summary>
        /// Gets a list of vehicles within a specific range
        /// </summary>
        public List<Vehicle> GetVehiclesInRange(Vector3 origin, int range)
        {
            List<Vehicle> output = new List<Vehicle>();

            return output;
        }
        #endregion

        #region Player Add/Remove/Get
        /// <summary>
        /// Adds a player vehicle to the arena
        /// </summary>
        public void AddPlayer(Vehicle veh)
        {
            //Get them a player ID [key to the dictionary]
            int ID = ProducePlayerID();

            //Add them to our list
            Vehicles.Add(ID, veh);

            //Either assign it here or use ID in this function
            veh.Owner.PlayerID = ID;

            //Make announcements
        }

        /// <summary>
        /// Removes a player vehicle from the arena
        /// </summary>
        public void RemovePlayer(Vehicle veh)
        {
        }
        #endregion

        /// <summary>
        /// Gets an available player ID
        /// This is used to distinguish vehicles from others, its still called a players ID
        /// </summary>
        /// <returns>Player ID / Vehicles Key</returns>
        private int ProducePlayerID()
        {
            int i = 0;

            //Loops through the list till either an ID is found or available
            while (Vehicles.ContainsKey(i))
                i++;

            return i;
        }
    }
}
