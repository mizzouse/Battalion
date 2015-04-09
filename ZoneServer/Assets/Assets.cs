using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoneServer
{
    public static class Assets
    {
        //Basic security checksums
        public static long TotalAssetChecksum;
        public static long ItemChecksum;
        public static long VehicleChecksum;
        public static long WeaponChecksum;

        //Wildcard checksums to throw off reversers
        //and checksums of the client's end of computing the checksums
        //incase calculation method is comprimised to allow for more time in between
        //security compiles.
        public static long Crypt1Checksum;
        public static long Crypt1RoutineChecksum;
        public static long Crypt2Checksum;
        public static long Crypt2RoutineChecksum;
        public static long Crypt3Checksum;
        public static long Crypt3RoutineChecksum;
        public static long Crypt4Checksum;
        public static long Crypt4RoutineChecksum;
        public static long Crypt5Checksum;
        public static long Crypt5RoutineChecksum;

        //Dictionarys containing our assets
        //TODO: Change dictionaries to List<T> incase they never exceed 100 total values
        /*
        public static List<ArmorType> armorTypes = new List<Asset.ArmorType>();
        public static Dictionary<int, ItemInfo> Items = new Dictionary<int, ItemInfo>();
        public static Dictionary<int, VehicleInfo> Vehicles = new Dictionary<int, VehicleInfo>();
        public static Dictionary<int, WeaponInfo> Weapons = new Dictionary<int, WeaponInfo>();
        public static Dictionary<int, SkillInfo> Skills = new Dictionary<int, Asset.SkillInfo>();
        public static Dictionary<int, AttributeInfo> Attributes = new Dictionary<int, AttributeInfo>();
        public static ZoneInfo zoneInfo;
         */

        /// <summary>
        /// Loads all assets
        /// This is called when the zoneserver is starting before it accepts connections
        /// </summary>
        public static void Load()
        {
        }
    }
}
