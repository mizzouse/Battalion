using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

using Infantry.Objects;
using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_FireWeapon
    {
        #region Declarations
        private NetIncomingMessage msg;
        private int weaponID;
        private Vector3[] vectors = { Vector3.Zero, Vector3.Zero, Vector3.Zero };
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
            weaponID = msg.ReadInt32();
            
            vectors[0] = msg.ReadVector3();
            vectors[1] = msg.ReadVector3();
            vectors[2] = msg.ReadVector3();

            yaw = msg.ReadFloat();
        }

        internal void Execute()
        {
            //Act on the data received
            ItemInfo bullet = AssetInfo.GetItemByID(0);
            //GameManager.BulletList.Add(bullet, vectors, yaw);
        }
    }
}
