using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Objects;
using Infantry.Managers;

namespace Infantry
{
    public class SC_PickUp
    {
        #region Declarations
        private NetIncomingMessage msg;
        public int ID;
        public int quantity;
        public Vector3 position;
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
            ID = msg.ReadInt32();
            quantity = msg.ReadInt32();
            position = msg.ReadVector3();

        }

        internal void Execute()
        {
            //Act on the data received
            if (ID > 0)
            {//Weapon
            }
            else
            {//Item             
                /*
                //Remove from drop lists
                InventoryItem item = new InventoryItem(AssetInfo.GetItemByID(ID), position);

                item.Inventory = GameManager.Player._inventory;
                item._amount = quantity;

                //GameManager.Drops.removeItem(item);

                //Add to our inventory
                GameManager.Player._inventory.AddItem(item);
                 */
            }
        }

    }
}
