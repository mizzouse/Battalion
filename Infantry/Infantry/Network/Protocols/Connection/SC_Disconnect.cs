using System;
using System.Collections.Generic;

using Lidgren.Network;
using Infantry.Managers;
using Infantry.Screens;

namespace Infantry.Network
{
    public class SC_Disconnect
    {
        /// <summary>
        /// Sets the disconnection reason.
        /// Most of them are only from the account server
        /// except Gracefully and Unknown. These 2 can be used
        /// at any time. Gracefully can be a server's shutdown
        /// or a client quit zone/game.
        /// </summary>
        public enum Flags
        {
            Register,
            WrongPassword,
            InvalidUsername,
            AccountBanned,
            FullZone,
            FullArena,
            Gracefully,
            Unknown
        }

        /// <summary>
        /// Since these flags are added in with our disconnection reasoning,
        /// we only need to read it.
        /// </summary>
        private enum BanType
        {
            Arena,
            Zone,
            IP,
            Global
        }

        /// <summary>
        /// Reads in packet data, shows the appropriate message box
        /// </summary>
        static public void Read(NetIncomingMessage Incoming)
        {
            byte reason;
            MessageBox MsgBox = new MessageBox(true);

            if (Incoming.ReadByte(out reason))
            {
                switch (reason)
                {
                    case (byte)Flags.Register:
                        GameManager.GameState = State.LogInScreen;
                        MsgBox.Message = "You are not registered yet.";
                        Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                        break;

                    case (byte)Flags.WrongPassword:
                        GameManager.GameState = State.LogInScreen;
                        MsgBox.Message = "Your password doesn't match.\nPlease try again.";
                        Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                        break;

                    case (byte)Flags.InvalidUsername:
                        GameManager.GameState = State.LogInScreen;
                        MsgBox.Message = "Your username is invalid.";
                        Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                        break;

                    case (byte)Flags.AccountBanned:
                        {
                            byte banType;
                            //Do we have more bytes in this packet?
                            if (Incoming.ReadByte(out banType))
                            {
                                //Yes.
                                switch (banType)
                                {
                                    case (byte)BanType.Arena:
                                        MsgBox.Message = "You are banned from the arena.";
                                        //Kick them back to whatever we are doing later
                                        break;

                                    case (byte)BanType.Zone:
                                        MsgBox.Message = "You are banned from this zone.";
                                        GameManager.GameState = State.ZoneSelect;
                                        Screens.LoadingScreen.Load(false, new Screens.ZoneListScreen(), MsgBox);
                                        break;

                                    case (byte)BanType.IP:
                                        MsgBox.Message = "You are banned from all zones.";
                                        GameManager.GameState = State.ZoneSelect;
                                        Screens.LoadingScreen.Load(false, new Screens.ZoneListScreen(), MsgBox);
                                        break;

                                    case (byte)BanType.Global:
                                        MsgBox.Message = "You are banned.";
                                        GameManager.GameState = State.LogInScreen;
                                        Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                                        break;
                                }
                            }
                            else
                            {
                                GameManager.GameState = State.LogInScreen;
                                MsgBox.Message = "Your account is banned.";
                                Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                            }
                        }
                        break;

                    case (byte)Flags.FullZone:
                        GameManager.GameState = State.ZoneSelect;
                        MsgBox.Message = "Server is full.";
                        Screens.LoadingScreen.Load(false, new Screens.ZoneListScreen(), MsgBox);
                        break;

                    case (byte)Flags.FullArena:
                        //TODO: Will change as game(not coding) structure progresses
                        break;

                    case (byte)Flags.Gracefully:
                    case (byte)Flags.Unknown:
                        MsgBox.Message = "Lost connection.";
                        if (GameManager.GameState >= State.EnteringZone)
                            //Lets kick them back to zone select
                            Screens.LoadingScreen.Load(false, new Screens.ZoneListScreen(), MsgBox);
                        else if (GameManager.GameState > State.LogInScreen)
                            //Lets kick them back to login
                            Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
                        break;
                }
            }
            else
            {
                //If a packet was sent without a reason, give
                //this as a default.
                MsgBox.Message = "Lost connection.";
                if (GameManager.GameState > State.EnteringZone)
                    //Lets kick them back to zone select
                    Screens.LoadingScreen.Load(false, new Screens.ZoneListScreen(), MsgBox);
                else if (GameManager.GameState > State.LogInScreen
                    && GameManager.GameState <= State.ZoneSelect)
                    //Lets kick them back to login
                    Screens.LoadingScreen.Load(false, new Screens.LoginScreen(), MsgBox);
            }
        }
    }
}
