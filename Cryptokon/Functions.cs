using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Lidgren.Network;
using System.Security.Cryptography;

namespace Cryptokon
{
    public static class Functions
    {
        public const int Key1 = 13;
        public const int Key2 = 13;

        public static string CalculateServer2ClientTicket(string Alias, NetConnection Conn, int Tick)
        {
            //We are going to use their alias as the int powers to each value of the ip used by client,
            //repeating until the end of their IP without the decimal points
            //then we turn their large number results and multiply each by our one prime number key and the
            //product of our date-time, their connection port, and a different prime number key.
            //Using these sets of numbers we form an MD5 checksum
            //
            //DateTime [Tick] is used to make it harder for a reverser to figure out how the checksum changes
            //on each login and how we know if the ticket is expired. We can later add their processor speed
            //if we somehow can mask a message like that from the client [getting their processor specs][[almost impossible if there is a genius]]
            //
            //We use a prime number because it is not divisible by anything but itself
            //making it impossible for someone to simplify our keys to one number unless
            //they figure out the key itself. It is recommended these numbers do not get too high
            //but not too low to prevent people from guessing them easily by altering between high and low
            //numbers when trying to generate the same ticket
            //
            //The prime number key changes once every day[7 different numbers] and the 7 different numbers
            //change by month. After a year the number shifts again to another set determined by the modulus
            //of our datetime and origin date so that no year contains the same number combinations per day.
            //Every 12th year this will repeat. If learned then we can change many factors or rewrite entire method
            //
            //We then shift the entire checksum by three bytes to the most significant digit and pad it
            //with our identifier for our current version of the client to both double check our original login
            //to accounts and to provide proof of the original unaltered packet[before shifting]
            //This is done by multiplying our server variable CurrentClientVersion by the last true digits of
            //the original checksum value.
            //
            //This will throw off anyone suspecting their IP or DateTime is somehow used in the hash they are sent at
            //the end of this method. Also by shifting the checksum we make it near to impossible for someone to reverse the real
            //method to generating it. They can probably fake it if they see something we overlook.
            //
            //We then send the shifted and altered checksum as their ticket which will be used to login to a zone server
            //
            //Cracking this will cause a headache, but if compromised we can simply shift the alias or scramble it
            //before using it as a key to mask the IP address.

            string output = "";
            int counter = 0;

            IPAddress address = Conn.RemoteEndPoint.Address;

            foreach (byte i in address.GetAddressBytes())
            {
                output += (Convert.ToInt32(i.ToString()) ^ Alias.ElementAt(counter % Alias.Length) * Key1).ToString();
                counter++;
            }

            output = CalculateMD5Hash(output);
            return output;
        }

        public static string CalculateMD5Hash(string input)
        {
            //Step 1, calculate the hash from our input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            //Step 2, convert the byte array to a hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
