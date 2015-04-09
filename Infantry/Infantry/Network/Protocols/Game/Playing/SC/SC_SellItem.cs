using System;
using System.Collections.Generic;

using Lidgren.Network;
using Microsoft.Xna.Framework;

using Infantry.Objects;
using Infantry.Assets;
using Infantry.Managers;

namespace Infantry
{
    public class SC_SellItem
    {
        #region Declarations
        private NetIncomingMessage msg;
        private int itemID;
        private int quantity;
        private ItemInfo itemInfo;
        private WeaponInfo weaponInfo;
        private Weapon weapon;
        private InventoryItem item;
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
            itemID = msg.ReadInt32();                  //Item ID
            quantity = msg.ReadInt32();                //Quantity of item
        }

        internal void Execute()
        {/*
            //Act on the data received
            if (itemID > 0)
            {//Weapon
                weaponInfo = AssetInfo.GetWeaponByID(itemID);
                weapon = new Weapon(GameManager.Player, GameManager.Player._inventory, weaponInfo);
                GameManager.Player._inventory.RemoveWeapon(weapon);
            }
            else
            {//Item
                itemInfo = AssetInfo.GetItemByID(itemID);
                item = new InventoryItem(GameManager.Player._vehicle, GameManager.Player._inventory, itemInfo);
                GameManager.Player._inventory.RemoveItem(item, quantity);
            }
          */
        }

    }
}
