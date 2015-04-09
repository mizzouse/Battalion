using System;
using System.Collections.Generic;

using Lidgren.Network;

using LinkServer.Network;

namespace LinkServer
{
    public class LinkServer
    {
        //Connections
        public static NetServer Server;

        //Connection Settings
        private NetPeerConfiguration Configuration;
        private const int Port = 12424;
        private const int MaximumConnections = 10;
        private const string ServerHandle = "LinkServer";

        //Communication
        private Receiver Receive;

        public void Initiate()
        {
            Configuration = new NetPeerConfiguration(ServerHandle);
            Configuration.Port = Port;
            Configuration.MaximumConnections = MaximumConnections;
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(Configuration);

            //Start up our network
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

        internal void ReadMessage()
        {
            NetIncomingMessage msg;
            while ((msg = Server.ReadMessage()) != null)
                Receive.ProcessRequest(msg);
        }

        internal void WriteMessage()
        {
            NetOutgoingMessage msg = Server.CreateMessage();

            msg.Write((byte)PacketHeaders.Packets.SC_Disconnect);
            msg.Write((byte)7); //Gracefully
            Server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

            Server.Shutdown("Shutting down...");
        }
    }
}
