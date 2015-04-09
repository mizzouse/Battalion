using System;
using System.Collections.Generic;

using Lidgren.Network;

using Infantry.Screens;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_Status
    {
        /// <summary>
        /// Reads in status messages, parses any information and/or sends a message to our user.
        /// </summary>
        static public void Read(NetConnectionStatus Status, NetIncomingMessage Incoming, string Server)
        {
            switch (Server)
            {
                case "AccountServer":
                    if (GameManager.GameState > State.LogInScreen 
                        && GameManager.GameState < State.EnteringZone)
                    {
                        switch (Status)
                        {
                            case NetConnectionStatus.Connected:
                                //First time login
                                if (GameManager.GameState == State.LoggingIn)
                                {
                                    //Fully connected, lets parse info
                                    GameManager.AccountTicket = Incoming.SenderConnection.RemoteHailMessage.ReadString();
                                    GameManager.GameState = State.ZoneSelect;

                                    BusyScreen busy = ScreenManager.GetScreen("busyscreen") as BusyScreen;
                                    if (busy != null)
                                        //Raise our completion event
                                        busy.OnOperationCompleted(true, null);

                                    break;
                                }

                                //DC/Quit from being in game
                                if (GameManager.GameState == State.ZoneSelect)
                                    GameManager.AccountTicket = Incoming.SenderConnection.RemoteHailMessage.ReadString();
                                break;

                            case NetConnectionStatus.Disconnected:
                                if (GameManager.GameState == State.LoggingIn)
                                {
                                    //Switch state to previous
                                    GameManager.GameState = State.LogInScreen;

                                    BusyScreen busy = ScreenManager.GetScreen("busyscreen") as BusyScreen;
                                    if (busy != null)
                                        //Lets flag our event completion
                                        busy.OnOperationCompleted(false, Incoming.ReadString());
                                    else
                                        //No busy screen, use default method
                                        LoadingScreen.Load(false, new LoginScreen(),
                                                        new MessageBox(Incoming.ReadString(), true));

                                    break;
                                }
                                break;
                        }
                    }
                    break;

                case "ZoneServer":
                    {
                        switch (Status)
                        {
                            case NetConnectionStatus.Connected:
                                if (GameManager.GameState == State.EnteringZone)
                                {
                                    BusyScreen busy = ScreenManager.GetScreen("busyscreen") as BusyScreen;
                                    if (busy != null)
                                        //Raise our completion event
                                        busy.OnOperationCompleted(true, null);
                                }
                                break;

                            case NetConnectionStatus.Disconnected:
                                if (GameManager.GameState == State.EnteringZone)
                                {
                                    //Switch state to previous
                                    GameManager.GameState = State.ZoneSelect;

                                    BusyScreen busy = ScreenManager.GetScreen("busyscreen") as BusyScreen;
                                    if (busy != null)
                                        //Lets flag our event completion
                                        busy.OnOperationCompleted(false, Incoming.ReadString());
                                    else
                                        //No busy screen, use default method
                                        LoadingScreen.Load(false, new ZoneListScreen(),
                                                        new MessageBox(Incoming.ReadString(), true));

                                    break;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
