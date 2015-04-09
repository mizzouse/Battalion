using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using Infantry.Assets;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_Weapons
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
            AssetInfo.weapons.Clear();

            count = msg.ReadInt32();
            Console.WriteLine("Receiving " + count + " Weapon assets");
            //Get the packet data
            for (int i = 0; i < count; i++)
            {
                WeaponInfo wi = new WeaponInfo();

                wi.Buyable = msg.ReadBoolean();
                wi.BuyPrice = msg.ReadInt32();
                wi.Category = msg.ReadString();
                wi.Description = msg.ReadString();
                wi.DevOnly = msg.ReadBoolean();
                wi.ExpireTime = msg.ReadInt32();
                wi.Name = msg.ReadString();
                wi.OneTimeUse = msg.ReadBoolean();
                wi.Sellable = msg.ReadBoolean();
                wi.SellPrice = msg.ReadInt32();
                wi.Visible = msg.ReadBoolean();
                wi.WeaponID = msg.ReadInt32();
                wi.Weight = msg.ReadInt32();

              //  wi.AmmoTypes = msg.ReadInt32();

                AssetInfo.weapons.Add(wi.WeaponID, wi);
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
