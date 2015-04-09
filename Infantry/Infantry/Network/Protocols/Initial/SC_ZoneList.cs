using System;
using System.Collections.Generic;

using Lidgren.Network;
using Infantry.Managers;

namespace Infantry.Network
{
    public class SC_ZoneList
    {
        /// <summary>
        /// Reads in packet data then sends data to our zonelist screen
        /// </summary>
        /// <param name="Incoming">Incoming Data</param>
        static public void Read(NetIncomingMessage Incoming)
        {
            //Is our screen even ready to get the data?
            if (!ScreenManager.ScreenExists("zonelistscreen"))
                return;

            //Are we in the correct states?
            if (GameManager.GameState != State.ZoneSelect
                || GameManager.GameState != State.InGame)
                return;

            //Parse Data
            Screens.ZoneListScreen.ZoneList.UpdateZoneList(Incoming);
        }
    }
}
