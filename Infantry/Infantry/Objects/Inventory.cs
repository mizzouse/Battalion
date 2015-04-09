using System;
using System.Collections.Generic;

using Infantry.Assets;

namespace Infantry.Objects
{
    public class Inventory
    {
        public Dictionary<int, Weapon> Weapons = new Dictionary<int, Weapon>();
        public Dictionary<int, InventoryItem> Items = new Dictionary<int, InventoryItem>();

        /// <summary>
        /// Generic Constructor
        /// </summary>
        public Inventory() { }

        /// <summary>
        /// Adds an item or amount to inventory
        /// </summary>
        public void AddItem(InventoryItem item)
        {
            //Do we need to update quantity?
            if (Items.ContainsValue(item))
                Items[item.ID].Amount += item.Amount;
            else
                //Doesnt have it, lets add it
                Items.Add(item.ID, item);
        }

        public void AddItem(InventoryItem item, int quantity)
        {
            if (!Items.ContainsValue(item))
            {
                Items.Add(item.ID, item);
                if (quantity == 1)
                    //Our inventory item starts at 1
                    return;
            }

            Items[item.ID].Amount += quantity;
        }

        /// <summary>
        /// Removes an item from inventory by ID
        /// </summary>
        public void RemoveItem(int id)
        {
            if (!Items.ContainsKey(id))
                return;

            Items.Remove(id);
        }

        /// <summary>
        /// Removes an item from inventory by object
        /// </summary>
        public void RemoveItem(InventoryItem item)
        {
            if (!Items.ContainsKey(item.ID))
                return;

            Items.Remove(item.ID);
        }

        /// <summary>
        /// Removes a quantity amount of an item 
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <param name="quantity">Amount to remove</param>
        public void RemoveItem(int id, int quantity)
        {
            if (!Items.ContainsKey(id))
                return;

            RemoveItem(Items[id], quantity);
        }

        /// <summary>
        /// Removes a quantity of an item, if more than what we have
        /// we remove the item as well
        /// </summary>
        public void RemoveItem(InventoryItem item, int quantity)
        {
            if (!Items.ContainsKey(item.ID))
                return;

            if (quantity >= Items[item.ID].Amount)
                Items.Remove(item.ID);
            else
                Items[item.ID].Amount -= quantity;
        }

        /// <summary>
        /// Adds a weapon by object to inventory
        /// </summary>
        public void AddWeapon(Weapon weapon)
        {
            if (Weapons.ContainsValue(weapon))
                return;

            Weapons.Add(weapon.ID, weapon);
        }

        /// <summary>
        /// Removes a weapon from inventory by id
        /// </summary>
        public void RemoveWeapon(int id)
        {
            if (!Weapons.ContainsKey(id))
                return;

            Weapons.Remove(id);
        }

        /// <summary>
        /// Removes a weapon from inventory by object
        /// </summary>
        public void RemoveWeapon(Weapon weapon)
        {
            if (!Weapons.ContainsValue(weapon))
                return;

            Weapons.Remove(weapon.ID);
        }
        /*
        /// <summary>
        /// Gets a weapon by a slot number
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>Returns the weapon, null if not found </returns>
        public int GetWeaponBySlot(int slot)
        {
            foreach (Weapon weapon in _weapons.Values)
            {
                if (weapon.GetSlotNumber() == slot)
                    return weapon;
            }

            return null;
        }*/

        /// <summary>
        /// Gets an item from our inventory
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>Returns the item, null if not found</returns>
        public InventoryItem GetItemByID(int id)
        {
            foreach (InventoryItem item in Items.Values)
            {
                if (item.ID == id)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Gets a string output of everything in our inventory
        /// </summary>
        public string GetInventory()
        {
            string output = "";

            foreach (Weapon weapon in Weapons.Values)
                output += Convert.ToString(weapon.ID + ",");

            foreach (InventoryItem item in Items.Values)
            {
                output += Convert.ToString(item.ID + ",");
                output += Convert.ToString(item.Amount + ",");
            }

            return output;
        }
    }
}
