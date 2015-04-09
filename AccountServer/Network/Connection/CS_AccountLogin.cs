using System;
using System.Collections.Generic;
using System.Linq;

using Lidgren.Network;
using PacketHeaders;

using Cryptokon;

namespace AccountServer.Network
{
    public class CS_AccountLogin
    {
        private NetConnection Target;
        private NetOutgoingMessage Outgoing;

        /// <summary>
        /// C2S - Reads in incoming packet data
        /// S2C - Will respond with the proper reason
        /// </summary>
        public CS_AccountLogin(NetIncomingMessage Incoming)
        {
            if ((Target = Incoming.SenderConnection) != null)
            {
                string username = Incoming.ReadString();
                string password = Incoming.ReadString();
                bool found = false;

                //Query the database
                foreach (var account in AccountServer.Database.Accounts)
                {
                    if (account.Name == username)
                    {
                        //Register checker
                        //if (!account.isRegistered)
                        //{
                        //    Outgoing = AccountServer.Server.CreateMessage();
                        //    Outgoing.Write((byte)Packets.Register);
                        //    AccountServer.Server.SendMessage(Outgoing, Target, NetDeliveryMethod.ReliableOrdered);
                        //    //Disconnect
                        //    Target.Deny();
                        //    return;
                        //}

                        if (Functions.CalculateMD5Hash(account.Password) == password)
                        {
                            Console.WriteLine("Account ID: {0} ({1}) Connected! {2}", account.ID, account.Name, Target.RemoteEndPoint);

                            account.LastLogin = DateTime.Now;
                            AccountServer.Database.SubmitChanges();

                            //Approve the connection with their account info
                            Outgoing = AccountServer.Server.CreateMessage();

                            //Calculate ticket id
                            account.Ticket = Functions.CalculateServer2ClientTicket(username, Target, Environment.TickCount);
                            Console.WriteLine("Ticket created for {0}:{1}", account.Name, account.Ticket);
                            //Save to db then send ticket
                            AccountServer.Database.SubmitChanges();
                            Outgoing.Write(account.Ticket);
                            //Approve
                            Target.Approve(Outgoing);

                            found = true;
                            break;
                        }
                        else
                        {
                            //Wrong password, send msg then disconnect
                            Outgoing = AccountServer.Server.CreateMessage();
                            Outgoing.Write((byte)Packets.SC_Disconnect);
                            Outgoing.Write((byte)1); //Wrong Password
                            AccountServer.Server.SendMessage(Outgoing, Target, NetDeliveryMethod.ReliableOrdered);
                            //Disconnect
                            Target.Deny();

                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    //Cannot find username, send denial message
                    Outgoing = AccountServer.Server.CreateMessage();
                    Outgoing.Write((byte)Packets.SC_Disconnect);
                    Outgoing.Write((byte)2); //Invalid username
                    AccountServer.Server.SendMessage(Outgoing, Target, NetDeliveryMethod.ReliableOrdered);
                    //Disconnect
                    Target.Deny();
                }
            }
        }
    }
}
