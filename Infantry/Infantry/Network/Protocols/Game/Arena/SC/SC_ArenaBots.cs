using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Objects;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_ArenaBots
    {
        #region Declarations
        private NetIncomingMessage msg;

        private int botID;
        private int vehicleID;

        private Vector3 position;
        private Vector3 velocity;
        private Vector3 acceleration;

        private float yaw;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            //Get the packet data
            botID = msg.ReadInt32();
            vehicleID = msg.ReadInt32();

            position = msg.ReadVector3();
            velocity = msg.ReadVector3();
            acceleration = msg.ReadVector3();

            yaw = msg.ReadFloat();
        }

        internal void Execute()
        {
            //Act on the data received
            if (GameManager.Vehicles.ContainsKey(botID))
            {
                GameManager.Vehicles[botID].Position = position;
                GameManager.Vehicles[botID].Velocity = velocity;
                GameManager.Vehicles[botID].Acceleration = acceleration;

                GameManager.Vehicles[botID].Yaw = yaw;
            }
            else
            {/*
                Vehicle bot = new Vehicle(AssetInfo.vehicles[vehicleID], GameManager.Device, GameManager.Contents);

                bot.PlayerID = botID;
                bot.Position = position;
                bot.Velocity = velocity;
                bot.Acceleration = acceleration;

                bot.Yaw = yaw;

                bot.Alias = "Bot 1";

                GameManager.Vehicles.Add(botID, bot);*/
            }
        }
    }
}