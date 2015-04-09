using System;
using System.Collections.Generic;

using Lidgren.Network;
using PacketHeaders;

using Infantry.Assets;
using Infantry.Objects;

namespace Infantry.Network
{
    public class SC_InventoryUpdate
    {
        #region Declarations
        private NetIncomingMessage msg;
        private Player player;
        #endregion

        public SC_InventoryUpdate(Player player)
        {
            this.player = player;
        }

        public void Receive(NetIncomingMessage msg)
        {
            this.msg = msg;

            Process();
            Execute();
        }

        internal void Process()
        {
            //Get the packet data
            string inven = msg.ReadString();

            if (String.IsNullOrEmpty(inven))
                return;

            string[] values = inven.Split(',');
            bool item = false;
            int id = 0;

            foreach (string s in values)
            {
                if (String.IsNullOrEmpty(s))
                    return;

                if (item)
                {
                    int quantity = Convert.ToInt32(s);
                    //player._inventory.AddItem(new InventoryItem(player._vehicle, player._inventory, Assets.getItemByID(id)), quantity);
                    item = false;
                    continue;
                }
                else
                {
                    id = Convert.ToInt32(s);

                    if (id > 0)
                    {//Weapon
                        //player._inventory.AddWeapon(new Weapon(player, player._inventory, AssetInfo.GetWeaponByID(id)));
                    }
                    else
                    {//Item
                        item = true;
                    }
                }
            }
        }

        internal void Execute()
        {
            //Act on the data received
        }
    }
}
