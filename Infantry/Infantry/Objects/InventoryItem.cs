using System;
using System.Collections.Generic;

namespace Infantry.Objects
{
    public class InventoryItem
    {
        /// <summary>
        /// The id of this inventory item
        /// </summary>
        public int ID;
        /// <summary>
        /// How many we have
        /// </summary>
        public int Amount = 1;
        /// <summary>
        /// What inventory we are tied to
        /// </summary>
        public Inventory Inventory;
    }
}
