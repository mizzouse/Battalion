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
    public class SC_DropItem
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
                //InventoryItem newItem = 
                // arena._arenaItems.addItem(arena._arenaItems.getWeaponByID(ID));
            }
            else
            {//Item
                /*
                //Find out if one already exists of same id, if so just add to quantity of that one                
                foreach (InventoryItem i in GameManager.Drops.GetAllItems())
                {
                    float dx = Math.Abs(i._position.X - position.X);
                    float dy = Math.Abs(i._position.Y - position.Y);

                    if (dx < 30 && dy < 30)
                    {
                        //Close enough to bundle
                        i._Count += quantity;
                        return;
                    }
                }

                //None exist just add it
                InventoryItem item = new InventoryItem(AssetInfo.GetItemByID(ID), position);
                item._amount = quantity;
                GameManager.Drops.addItem(item);

                //Remove from our inventory
                GameManager.Player._inventory.RemoveItem(item, quantity);
                 */
            }
        }

    }
}
