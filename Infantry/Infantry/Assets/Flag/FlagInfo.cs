using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Infantry.Objects;
using Infantry.Managers;
using Infantry.Network;

namespace Infantry.Assets
{
    public class FlagInfo
    {
        #region General variables
        public int ID;
        public string Name;
        public Vector3 Position;

        //For Interactions between player and flag
        public Vehicle _owner;                                  //Player who owns this
        public bool _carried;                                   //Is this being carried
        #endregion

       
        public FlagInfo(int ID)
        {
            this.ID = ID;
            Name = "";
            Position = Vector3.Zero;
        }

        #region Communication
        public void PickUp()
        {
            if (_owner == null)
            {
                return;
            }

            CS_FlagPickup pkt = new CS_FlagPickup();
            pkt.Send();
        }
        #endregion
    }
}
