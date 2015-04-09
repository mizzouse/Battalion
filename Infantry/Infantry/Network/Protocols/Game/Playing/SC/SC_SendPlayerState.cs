using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

using Infantry.Objects;
using Infantry.Managers;

namespace Infantry
{
    public class SC_SendPlayerState
    {
        #region Declarations
        private NetIncomingMessage msg;
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

            Vehicle vechicleUpdate = GameManager.Vehicles[msg.ReadInt32()];
            vechicleUpdate.Position = msg.ReadVector3();
            vechicleUpdate.Velocity = msg.ReadVector3();
            vechicleUpdate.Acceleration = msg.ReadVector3();

            vechicleUpdate.Yaw = msg.ReadFloat();
        }

        internal void Execute()
        {
            //Act on the data received
          
        }

    }
}
