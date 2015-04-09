using System;
using System.Collections.Generic;

using Lidgren.Network;

using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.Network
{
    public class SC_DiscoveryPing
    {
        /// <summary>
        /// Reads in our connection packet
        /// </summary>
        static public void Read(NetConnection Connection)
        {
            if (Connection == null)
                return;

            if (!String.IsNullOrEmpty(Connection.Peer.Configuration.AppIdentifier))
                return;

            ScreenLayer screen;
            switch (Connection.Peer.Configuration.AppIdentifier)
            {
                case "AccountServer":
                    if (GameManager.GameState < State.ZoneSelect)
                    {
                        //Did we use this with our busy screen?
                        if ((screen = ScreenManager.GetScreen("busyscreen")) != null)
                        {   //Yes
                            BusyScreen busy = screen as BusyScreen;
                            //Signal yes we got a response
                            busy.OnOperationCompleted(true, null);
                        }
                        else
                        {
                            //No, pop up a message instead
                            MessageBox box = new MessageBox("Server responded to ping.", true);
                            ScreenManager.AddScreen(box);
                        }
                    }
                    break;

                case "ZoneServer":
                    if (GameManager.GameState >= State.ZoneSelect
                        && GameManager.GameState < State.InGame)
                    {
                        //Did we use this with our busy screen?
                        if ((screen = ScreenManager.GetScreen("busyscreen")) != null)
                        {   //Yes
                            BusyScreen busy = screen as BusyScreen;
                            //Signal yes we got a response
                            busy.OnOperationCompleted(true, null);
                        }
                        else
                        {
                            //No, pop up a message instead
                            MessageBox box = new MessageBox("Server responded to ping.", true);
                            ScreenManager.AddScreen(box);
                        }
                    }
                    break;
            }
        }
    }
}
