using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;
using Infantry.Objects;

namespace Infantry.Network
{
    public class SC_ArenaState
    {
        #region Declarations
        private NetIncomingMessage msg;

        private string alias;
        private long playerID;
        private int vehicleID;
        private int teamID;

        private Vector3 position;
        private Vector3 velocity;
        private Vector3 acceleration;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            playerID = msg.ReadInt64();
            alias = msg.ReadString();
            vehicleID = msg.ReadInt32();
            teamID = msg.ReadInt32();

            position = msg.ReadVector3();
            velocity = msg.ReadVector3();
            acceleration = msg.ReadVector3();

            if (GameManager.Vehicles.ContainsKey(playerID))
            {
                GameManager.Vehicles[playerID].Alias = alias;
                GameManager.Vehicles[playerID].PlayerID = playerID;
                GameManager.Vehicles[playerID].ID = vehicleID;
                //GameManager.Vehicles[playerID].Team = Infantry._arena.Teams[teamID];

                GameManager.Vehicles[playerID].Position = position;
                GameManager.Vehicles[playerID].Velocity = velocity;
                GameManager.Vehicles[playerID].Acceleration = acceleration;
            }
            else
            {
                /*
                Vehicle newVehicle = new Vehicle(AssetInfo.vehicles[vehicleID], game.GraphicsDevice, game.content);

                newVehicle.Alias = alias;
                newVehicle.PlayerID = playerID;
                newVehicle.ID = vehicleID;
                newVehicle.Team = Infantry._arena.Teams[teamID];

                newVehicle.Position = position;
                newVehicle.Velocity = velocity;
                newVehicle.Acceleration = acceleration;

                GameManager.Vehicles.Add(playerID, newVehicle);*/
            }

        }

        internal void Execute()
        {
            //Act on the data received

            //Set up store???
            /*
            game.store = new StoreScreen(game.device, game.currentFont, game.storeSizes, game.graphics.PreferredBackBufferHeight, game.graphics.PreferredBackBufferWidth, game.pixel, game.cmdHandler);
            game.store._us = game._player;

            game.findUs();

            game._player._client = game.zClient;
            //Tell game we got this data and loaded our vehicles
            game._receivedInitialState = true;*/
        }

    }
}
