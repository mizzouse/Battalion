using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_Items
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
            AssetInfo.items.Clear();

            count = msg.ReadInt32();
            Console.WriteLine("Receiving " + count + " Item assets");
            //Get the packet data
            for (int i = 0; i < count; i++)
            {
                ItemInfo ii = new ItemInfo();

                ii.Buyable = msg.ReadBoolean();
                ii.BuyPrice = msg.ReadInt32();
                ii.Category = msg.ReadString();
                ii.CloakAura = msg.ReadInt32();
                ii.CloakRadius = msg.ReadInt32();
                ii.Description = msg.ReadString();
                ii.DevOnly = msg.ReadBoolean();
                ii.Droppable = msg.ReadBoolean();                
                ii.ExpireTime = msg.ReadInt32();                
                ii.ItemID = msg.ReadInt32();
                ii.MaxAmount = msg.ReadInt32();
                ii.Name = msg.ReadString();
                ii.OneTimeUse = msg.ReadBoolean();
                ii.PruneChance = msg.ReadInt32();
                ii.Sellable = msg.ReadBoolean();
                ii.SellPrice = msg.ReadInt32();
                ii.StealthAura = msg.ReadInt32();
                ii.StealthRadius = msg.ReadInt32();                
                ii.Toggle = msg.ReadBoolean();
                ii.Visible = msg.ReadBoolean();
                ii.Weight = msg.ReadInt32();

                AssetInfo.items.Add(ii.ItemID, ii);

                /*
                ii.EnemyVehicleEffects = msg.ReadBoolean();
                ii.ItemEffects = msg.ReadBoolean();
                ii.SelfVehicleEffects = msg.ReadBoolean();
                ii.TeamVehicleEffects = msg.ReadBoolean();
                ii.WeaponEffects = msg.ReadBoolean();
                 */
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
