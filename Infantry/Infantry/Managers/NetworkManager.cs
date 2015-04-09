using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Infantry.Network;

namespace Infantry.Managers
{
    public sealed class NetworkManager : GameComponent
    {
        private static Dictionary<string, Client> _clients = new Dictionary<string, Client>();
        private static bool _initialized = false;

        /// <summary>
        /// Are we initialized yet?
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// Gets a client from our manager's list
        /// </summary>
        /// <param name="name">Name of our client</param>
        /// <returns>Returns client if found, null if not</returns>
        public static Client GetClient(string name)
        {
            if (!_clients.ContainsKey(name))
                return null;
            return _clients[name];
        }

        /// <summary>
        /// The Network Manager Constructor
        /// </summary>
        public NetworkManager(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Initialized the Network Manager
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Client account = new Client("AccountServer");
            AddClient(account);
            //Since this is a first startup of the game, auto start
            //our network
            account.StartUp();

            AddClient(new Client("ZoneServer"));

            _initialized = true;
        }

        /// <summary>
        /// Updates the network connections
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            foreach (Client client in _clients.Values)
                client.Poll(gameTime);
        }

        /// <summary>
        /// Adds a client with a specified name to our list
        /// </summary>
        /// <param name="name">Name of our client</param>
        /// <param name="client">Actual client</param>
        public static void AddClient(string name, Client client)
        {
            if (!_clients.ContainsKey(name))
                _clients.Add(name, client);
        }

        /// <summary>
        /// Adds a client connection to our list
        /// </summary>
        public static void AddClient(Client client)
        {
            if (!_clients.ContainsValue(client))
                _clients.Add(client.Name, client);
        }

        /// <summary>
        /// Removes a client from our connection list
        /// </summary>
        /// <param name="name">Our Clients name</param>
        public static void RemoveClient(string name)
        {
            if (_clients.ContainsKey(name))
                _clients.Remove(name);
        }

        /// <summary>
        /// Removes a client from our connection list
        /// </summary>
        /// <param name="client">Our client</param>
        public static void RemoveClient(Client client)
        {
            foreach (Client clients in _clients.Values)
                if (clients == client)
                {
                    _clients.Remove(client.Name);
                    break;
                }
        }

        /// <summary>
        /// Closes all active connection when disposing(exiting game)
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            foreach (Client client in _clients.Values)
            {
                client.NetworkThread.Abort();
                client.Shutdown("Exiting");
            }

            base.Dispose(disposing);
        }
    }
}
