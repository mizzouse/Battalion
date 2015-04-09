using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Lidgren.Network;
using ZoneServer.Objects;

namespace ZoneServer.Network.Routing
{
    public class GameWorld
    {
        private Dictionary<int, Cube> Cubes = new Dictionary<int, Cube>();

        /// <summary>
        /// Calculates the size of the Game World
        /// </summary>
        public void CalculateGameWorld(int width, int height)
        {
            int CurrentCube = 0;

            for (int i = 0; i < width; i += 10)
            {
                for (int j = 0; j < height; j += 10)
                    Cubes.Add(CurrentCube++, new Cube());
            }
        }

        /// <summary>
        /// Gets a specific cube using a key
        /// </summary>
        /// <returns>Returns the cube, null if not found</returns>
        public Cube GetCube(int key)
        {
            if (Cubes.Keys.Contains(key))
                return Cubes[key];

            return null;
        }

        /// <summary>
        /// Moves a member from one Game Cube to another 
        /// </summary>
        /// <param name="ID">The Id of the player</param>
        /// <param name="Destination">The destination cube's id</param>
        /// <param name="Initial"></param>
        public void MoveMember(long ID, int Destination, int Initial)
        {
            Cubes[Destination].AddMember(Cubes[Initial].RemoveMember(ID));
        }
    }

    public class Cube
    {
        private Dictionary<long, NetConnection> Members;

        /// <summary>
        /// Generic Cube Constructor
        /// </summary>
        public Cube()
        {
            Members = new Dictionary<long, NetConnection>();
        }

        /// <summary>
        /// Adds a member(Net Connection) to our cube
        /// </summary>
        public void AddMember(NetConnection Member)
        {
            Members.Add(Member.RemoteUniqueIdentifier, Member);
        }

        /// <summary>
        /// Removes a member based on ID from our cube
        /// </summary>
        public NetConnection RemoveMember(long ID)
        {
            NetConnection connection = Members[ID];
            Members.Remove(ID);

            return connection;
        }

        /// <summary>
        /// Returns a list of all the members in a cube
        /// </summary>
        public List<NetConnection> GetMembers()
        {
            return Members.Values.ToList();
        }
    }

    public class Functions
    {
        public GameWorld World = new GameWorld();

        /// <summary>
        /// Gets the current position based on map coordinates
        /// </summary>
        public int GetPosition(Vector3 MapPosition)
        {
            float x = Math.Abs((MapPosition.X) / (500));
            float y = Math.Abs((MapPosition.Y) / (500));

            return (int)(x + y);
        }

        /// <summary>
        /// Moves a player across the game world
        /// </summary>
        /// <param name="ID">Id of the player</param>
        /// <param name="Destination">The destination cube's id</param>
        /// <param name="Initial"></param>
        public void MovePlayer(long ID, int Destination, int Initial)
        {
            World.MoveMember(ID, Destination, Initial);
        }

        /// <summary>
        /// Adds a player to a game world cube
        /// </summary>
        /// <param name="Destination">The destination cube</param>
        /// <param name="Connection">The players connection</param>
        public void AddPlayer(int Destination, NetConnection Connection)
        {
            if ((World.GetCube(Destination)) != null)
                World.GetCube(Destination).AddMember(Connection);
        }

        /// <summary>
        /// Returns a list of members within a cube, null if none exist
        /// </summary>
        /// <param name="ID">The ID of the cube</param>
        public List<NetConnection> GetPlayersInCube(int ID)
        {
            if ((World.GetCube(ID)) != null)
                return World.GetCube(ID).GetMembers();

            return null;
        }

        /// <summary>
        /// Gets a list of players within a specific range of a map coordinate
        /// </summary>
        public List<NetConnection> GetPlayersWithinRange(int Range, Vector3 MapPosition)
        {
            float x1 = MapPosition.X;
            float x2 = MapPosition.X + Range;
            float x3 = MapPosition.X - Range;

            float y1 = MapPosition.Z;
            float y2 = MapPosition.Z + Range;
            float y3 = MapPosition.Z - Range;

            List<NetConnection> output = new List<NetConnection>();

            output.AddRange(GetPlayersInCube((int)(x1 + y1)));
            output.AddRange(GetPlayersInCube((int)(x2 + y2)));
            output.AddRange(GetPlayersInCube((int)(x3 + y3)));

            return output;
        }
    }
}
