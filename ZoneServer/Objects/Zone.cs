using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;
using ZoneServer.Network;

namespace ZoneServer.Objects
{
    /// <summary>
    /// This is our main Zone Object class
    /// </summary>
    public static class Zone
    {
        //Zone Settings
        private const string PublicArenaTitle = "Public ";
        private const string PrivateArenaTitle = "Private ";

        //Arena's within the zone
        public static List<Arena> Arenas = new List<Arena>();
        public static Dictionary<long, Player> Players = new Dictionary<long, Player>();

        //Statistics
        private static int PlayersCount;
        private static int lastPlayerCheck;

        #region Polling
        /// <summary>
        /// Updates our arena's within this zone
        /// </summary>
        public static void Poll()
        {
            foreach (Arena arena in Arenas.ToList())
                arena.Poll();

            //Update player count
            //15 seconds = 15000
            if (Environment.TickCount - lastPlayerCheck > 15000)
            {
                PlayersCount = Players.Values.Count;
                lastPlayerCheck = Environment.TickCount;
            }
        }
        #endregion

        #region Initial
        /// <summary>
        /// Called when our zone is initially created
        /// </summary>
        public static void Initiate()
        {
            //Start up our first arena
            Arena first = new Arena(0, PublicArenaTitle + "0");

            //Add it to our list
            Arenas.Add(first);
        }
        #endregion

        /// <summary>
        /// Returns a count all the active players under this zone
        /// </summary>
        public static int PlayerCount
        {
            get { return PlayersCount; }
        }

        #region Player Add/Remove/Get
        /// <summary>
        /// Adds a new player to the zone
        /// </summary>
        /// <returns>Returns either a new player structure or a current one</returns>
        public static Player NewPlayer(NetConnection Connection, string Alias)
        {
            Player newPlayer = new Player(Connection, Arenas.First());
            newPlayer.Alias = Alias;

            foreach (var a in ZoneServer.Database.Aliases)
            {
                if (a == null)
                    continue;

                if (a.Name == Alias)
                {
                    foreach (var b in ZoneServer.Database.Players)
                    {
                        if (b.AliasID == a.ID)
                        {
                            //Add them to our list of connections in this zone
                            Players.Add(Connection.RemoteUniqueIdentifier, newPlayer);

                            //Set their Player ID

                            //Set their vehicle based on Skill ID
                            newPlayer.SetUpVehicle(b.SkillID);

                            //Fill their inventory

                            //Fill their cash and stats

                            return newPlayer;
                        }
                    }
                }
            }

            //If we get here, means they dont have a player entry
            return newPlayer;
        }

        /// <summary>
        /// Removes a player from the zone.
        /// </summary>
        public static void RemovePlayer(long connection)
        {
            if (Players.ContainsKey(connection))
                Players.Remove(connection);
        }

        /// <summary>
        /// Finds a player within the Zone and returns their player structure,
        /// null if its not found.
        /// </summary>
        /// <param name="ID">Players UID</param>
        public static Player GetPlayer(long ID)
        {
            if (Zone.Players.ContainsKey(ID))
                return Zone.Players[ID];

            return null;
        }

        /// <summary>
        /// Finds a player within the Zone and returns their player structure,
        /// null if its not found.
        /// </summary>
        /// <param name="Alias">Players alias</param>
        public static Player GetPlayer(string Alias)
        {
            if (String.IsNullOrEmpty(Alias))
                return null;

            foreach (Player P in Zone.Players.Values)
            {
                if (P == null)
                    continue;

                if (P.Alias == Alias)
                    return P;
            }

            return null;
        }
        #endregion
    }
}
