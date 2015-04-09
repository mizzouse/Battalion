using System;
using System.Collections.Generic;

namespace Infantry.Assets
{
    public static class AssetInfo
    {
        public static Dictionary<int, WeaponInfo> weapons = new Dictionary<int, WeaponInfo>();
        public static Dictionary<int, VehicleInfo> vehicles = new Dictionary<int, VehicleInfo>();
        public static Dictionary<int, ItemInfo> items = new Dictionary<int, ItemInfo>();
        public static Dictionary<int, SkillInfo> skills = new Dictionary<int, SkillInfo>();
        public static Dictionary<int, AttributeInfo> attributes = new Dictionary<int, AttributeInfo>();
        public static ZoneInfo zone = new ZoneInfo();

        public static string[] GetWeaponInfo(int id)
        {
            foreach (WeaponInfo wi in weapons.Values)
            {
                if (wi.WeaponID == id)
                {
                    return wi.GetWeaponInfo();
                }
            }
            return null;
        }

        public static string[] GetItemInfo(int id)
        {
            foreach (ItemInfo ii in items.Values)
            {
                if (ii.ItemID == id)
                {
                    return ii.getItemInfo();
                }
            }
            return null;
        }

        public static WeaponInfo GetWeaponByID(int id)
        {
            foreach (WeaponInfo wi in weapons.Values)
            {
                if (id == wi.WeaponID)
                    return wi;
            }
            return null;
        }

        public static WeaponInfo GetWeaponByName(string s)
        {
            foreach (WeaponInfo wi in weapons.Values)
            {
                if (s.ToLower() == wi.Name.ToLower())
                    return wi;
            }
            return null;
        }

        public static ItemInfo GetItemByID(int id)
        {
            foreach (ItemInfo ii in items.Values)
            {
                if (id == ii.ItemID)
                    return ii;
            }
            return null;
        }

        public static ItemInfo GetItemByName(string s)
        {
            foreach (ItemInfo ii in items.Values)
            {
                if (s.ToLower() == ii.Name.ToLower())
                    return ii;
            }
            return null;
        }
    }
}
