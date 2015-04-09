using System;
using System.Text;

namespace Infantry.Assets
{
    public class Checksum
    {
        public static string TotalChecksumValue;

        /// <summary>
        /// This returns the checksum value of assets to make sure the
        /// user got everything before moving onto arena state.
        /// </summary>
        public static string GetMD5()
        {
            string output = "";

            foreach (WeaponInfo wInfo in AssetInfo.weapons.Values)
                output += wInfo.Name;

            foreach (ItemInfo item in AssetInfo.items.Values)
                output += item.Name;

            foreach (VehicleInfo veh in AssetInfo.vehicles.Values)
                output += veh.Name;

            return CalculateMD5Hash(output);
        }

        /// <summary>
        /// Calculates an MD5 Hash string
        /// </summary>
        public static string CalculateMD5Hash(string input)
        {
            //Step 1, calculate md5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            //Step 2, convert byte array to a hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
