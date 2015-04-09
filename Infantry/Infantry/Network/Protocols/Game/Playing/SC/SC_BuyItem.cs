using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Assets;
using Infantry.Objects;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_BuyItem
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
            quantity = msg.ReadInt32();             //Quantity of item
        }

        internal void Execute()
        {
            //Act on the data received
            //Have inventory handle the buy now
            if (itemID > 0)
            {//Weapon
                weaponInfo = AssetInfo.GetWeaponByID(itemID);
                /*
                weapon = new Weapon(GameManager.Player, GameManager.Player._inventory, weaponInfo);
                GameManager.Player._inventory.AddWeapon(weapon);
            }
            else
            {//Item
                itemInfo = AssetInfo.getItemByID(itemID);
                item = new InventoryItem(GameManager.Player._vehicle, GameManager.Player._inventory, itemInfo);
                GameManager.Player._inventory.AddItem(item, quantity);
                */
            }
        }

    }
}
