using System;
using System.Collections.Generic;

using Lidgren.Network;
using DatabaseServer;

using ZoneServer.Objects;
using ZoneServer.Network;

namespace ZoneServer
{
    /// <summary>
    /// This is our Zone Server class
    /// </summary>
    class ZoneServer
    {
        //Connections
        public static NetServer Server;
        public static NetClient LinkServer;

        //Connection Settings
        private NetPeerConfiguration Configuration;
        private const int Port = 64128;
        private const int MaximumConnections = 500; //Switch this to an xml reader
        private const string ServerHandle = "ZoneServer1";

        private NetPeerConfiguration LinkConfig;
        private System.Net.IPAddress IP = System.Net.IPAddress.Parse("127.0.0.1");
        private const int LinkPort = 64000;

        //Communication
        private Receiver Receive;
        private int _lastConnectionAttempt;

        //Database
        public static Database Database = new Database();

        public void Initiate()
        {
            //Load all assets
            Assets.Load();
            Console.WriteLine("Assets Loaded Successfully");

            //Load our zone
            Zone.Initiate();

            Configuration = new NetPeerConfiguration(ServerHandle);
            Configuration.Port = Port;
            Configuration.MaximumConnections = MaximumConnections;
            Configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(Configuration);

            Server.Start();

            LinkConfig = new NetPeerConfiguration("LinkServer");
            LinkServer = new NetClient(LinkConfig);
            LinkServer.Start();

            Receive = new Receiver();
            Console.WriteLine("Server started -- Now Listening..");

            //Poll Communcation
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Discovery();
                ReadMessage();
                Zone.Poll();
            }

            //Sends a disconnection message and shuts down server
            WriteMessage();
        }

        internal void Discovery()
        {
            int now = Environment.TickCount;
            if (now - _lastConnectionAttempt > 15000) //15 Seconds
            {
                if (LinkServer.ConnectionStatus == NetConnectionStatus.Disconnected)
                    //Note: Change this later to "discover known peer" if we use a different server for linkserver
                    LinkServer.DiscoverLocalPeers(LinkPort);

                _lastConnectionAttempt = now;
            }
        }

        internal void ReadMessage()
        {
            NetIncomingMessage msg;

            while ((msg = Server.ReadMessage()) != null || (msg = LinkServer.ReadMessage()) != null)
                Receive.ProcessRequest(msg);
        }

        internal void WriteMessage()
        {
            //Send to all clients/servers
            NetOutgoingMessage msg = new NetOutgoingMessage();

            msg.Write((byte)PacketHeaders.Packets.SC_Disconnect);
            Server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

            LinkServer.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            Server.Shutdown("Shutting down...");
            LinkServer.Shutdown("Shutting down...");
        }
    }
}
