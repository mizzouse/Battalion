using System;
using System.Collections.Generic;

using Lidgren.Network;
using DatabaseServer;

using AccountServer.Network;

namespace AccountServer
{
    public class AccountServer
    {
        //Connections
        public static NetServer Server;
        public static NetClient DatabaseServer;

        //Connection Settings
        private NetPeerConfiguration Configuration;
        private const int Port = 14242;
        //private const int MaximumConnections = 500; dont use this unless you want restrictions
        private const string ServerHandle = "AccountServer";

        //Communication
        private Receiver Receive;

        //Database
        public static Database Database = new Database();

        public void Initiate()
        {
            //First check database connection
            if (!Database.DatabaseExists())
            {
                Console.WriteLine("Error in connecting to the database, Aborting!");
                return;
            }

            Configuration = new NetPeerConfiguration(ServerHandle);
            Configuration.Port = Port;
            //Configuration.MaximumConnections = MaximumConnections;
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(Configuration);
            
            //Start up our server network
            Server.Start();

            //Set up our packet receiver
            Receive = new Receiver();

            Console.WriteLine("Server started -- Now Listening");
            Console.WriteLine("Press Esc to quit.");

            //Poll communication
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
                ReadMessage();

            //If aborting, shut down all connections with a disconnection packet
            WriteMessage();
        }

        private void ReadMessage()
        {
            NetIncomingMessage msg;
            while ((msg = Server.ReadMessage()) != null)
                Receive.ProcessRequest(msg);
        }

        private void WriteMessage()
        {
            NetOutgoingMessage msg = Server.CreateMessage();

            msg.Write((byte)PacketHeaders.Packets.SC_Disconnect);
            Server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

            Server.Shutdown("Shutting down...");
        }
    }
}
