using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_Vehicles
    {
        #region Declarations
        private NetIncomingMessage msg;

        private int count;
        #endregion

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            AssetInfo.vehicles.Clear();

            count = msg.ReadInt32();
            Console.WriteLine("Receiving " + count + " Vehicle assets");
            //Get the packet data
            for (int i = 0; i < count; i++)
            {
                VehicleInfo vehicle = new VehicleInfo();

                vehicle.VehicleID = msg.ReadInt32();
                vehicle.ModelID = msg.ReadInt32();
                vehicle.Name = msg.ReadString();
                vehicle.Description = msg.ReadString();
                vehicle.HitPoints = msg.ReadInt32();
                vehicle.NormalWeight = msg.ReadInt32();
                vehicle.StopWeight = msg.ReadInt32();
                vehicle.ShowEnergy = msg.ReadInt32();
                vehicle.ShowHealth = msg.ReadInt32();
                vehicle.BaseEnergy = msg.ReadInt32();
                vehicle.EnergyRate = msg.ReadInt32();
                vehicle.StartingEnergy = msg.ReadInt32();
                vehicle.Controllable = msg.ReadBoolean();
                vehicle.Warpable = msg.ReadBoolean();
                vehicle.NmeRadarColor = msg.ReadString();
                vehicle.DisplayOnNmeRader = msg.ReadBoolean();
                vehicle.TeamRadarColor = msg.ReadString();
                vehicle.DisplayOnFriendlyRadar = msg.ReadBoolean();
                vehicle.MaxVelocity = msg.ReadInt32();
                vehicle.forwardAcceleration = msg.ReadInt32();
                vehicle.backwardAcceleration = msg.ReadInt32();
                vehicle.sideAcceleration = msg.ReadInt32();

                int c = msg.ReadInt32();
                int counter = 0;
                while (counter < c)
                {
                    int key = msg.ReadInt32();
                    int value = msg.ReadInt32();

                    vehicle.Drops.Add(key, value);
                    counter++;
                }

                c = msg.ReadInt32();
                counter = 0;
                while (counter < c)
                {
                    int key = msg.ReadInt32();
                    int value = msg.ReadInt32();

                    vehicle.Inventory.Add(key, value);
                    counter++;
                }

                //armors

                AssetInfo.vehicles.Add(vehicle.VehicleID, vehicle);
            }
        }

        internal void Execute()
        {
            //Act on the data received
            //Last packet received for our game state loading, we are in game
            GameManager.GameState = State.InGame;
        }
    }
}
