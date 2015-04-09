using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;

namespace LinkServer.Network
{
    public class CS_ZoneLogin
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_ZoneLogin() { }

        /// <summary>
        /// C2S - Reads in packet data, checks if its already linked or not.
        /// S2C - Sends back approve or deny based on check.
        /// </summary>
        /// <param name="Incoming"></param>
        public CS_ZoneLogin(NetIncomingMessage Incoming)
        {
            if (Incoming.SenderConnection != null)
            {
                Console.WriteLine("Incoming Connection...");
                //Are we in the list already? Possible fake zoneserver attempt
                if (Receiver.Connections.ContainsValue(Incoming.SenderConnection))
                {
                    Console.WriteLine("Connection already in list.. denying it.");
                    Incoming.SenderConnection.Deny();
                }
                //Are we already maxed for active connections?
                else if (Receiver.Connections.Values.Count() >= LinkServer.Server.Configuration.MaximumConnections)
                {
                    Console.WriteLine("Max connection limit reached({0}), denying the connection.", LinkServer.Server.Configuration.MaximumConnections);
                    Incoming.SenderConnection.Deny();
                }
                //Passed
                else
                {
                    Receiver.Connections.Add((Receiver.Connections.Keys.Count()), Incoming.SenderConnection);
                    Incoming.SenderConnection.Approve();
                    Console.WriteLine("Server({0}) has connected!", Incoming.SenderConnection.RemoteUniqueIdentifier);
                }
            }
        }
    }
}
