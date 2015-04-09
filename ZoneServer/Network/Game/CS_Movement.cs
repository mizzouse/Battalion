using System;
using System.Collections.Generic;

using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

using ZoneServer.Objects;

namespace ZoneServer.Network
{
    /// <summary>
    /// CS_Movement Object class
    /// </summary>
    public class CS_Movement
    {
        /// <summary>
        /// Generic Object Constructor
        /// </summary>
        public CS_Movement() { }

        /// <summary>
        /// Reads in packet data, sends outgoing information
        /// </summary>
        public CS_Movement(NetIncomingMessage packet)
        {
            int tick = packet.ReadInt32();
            Vector3 pos = packet.ReadVector3();
            Vector3 vel = packet.ReadVector3();
            Vector3 acc = packet.ReadVector3();
            float yaw = packet.ReadFloat();

            //Process Info
            Player p = Zone.Players[packet.SenderConnection.RemoteUniqueIdentifier];

            p.Vehicle.Position = pos;
            p.Vehicle.Velocity = vel;
            p.Vehicle.Acceleration = acc;
            p.Vehicle.Yaw = yaw;

            //Do some predictions quickly before routing the update

            //Set up packet
            Vehicle update = Zone.Players[p.Connection.RemoteUniqueIdentifier].Vehicle;
            NetOutgoingMessage Outgoing = new NetOutgoingMessage();

            Outgoing.Write((byte)PacketHeaders.Packets.SC_PlayerUpdate);
            Outgoing.Write(update.Owner.PlayerID);
            Outgoing.Write(update.Position);
            Outgoing.Write(update.Velocity);
            Outgoing.Write(update.Acceleration);
            Outgoing.Write(update.Yaw);

            //Route update based on cubed world
            foreach (NetConnection connection in p.Arena.Routing.GetPlayersInCube(p.Vehicle.CurrentCube))
            {
                if (connection == p.Connection)
                    continue;

                ZoneServer.Server.SendMessage(Outgoing, connection, NetDeliveryMethod.UnreliableSequenced);
            }
        }
    }
}
