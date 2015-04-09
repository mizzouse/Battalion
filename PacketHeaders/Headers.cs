using System;

namespace PacketHeaders
{
    public enum Packets
    {
        #region Player Updates
        /// <summary>
        /// These packets are player related in game packets.
        /// </summary>
        SC_PlayerUpdate,
        CS_Projectile,
        SC_Projectile,
        CS_Movement,
        CS_SpecMode,
        SC_SpecMode,
        SC_NewPlayer,

        CS_Inventory,
        SC_Inventory,

        CS_FlagPickup,
        SC_FlagPickup,
        CS_Skill,
        SC_Skill,
        CS_Attribute,
        SC_Attribute,

        #endregion

        #region Account Packets
        /// <summary>
        /// These packets handle a connection to the account server
        /// along with other functions like send a list of zones.
        /// </summary>
        CS_ZoneList,
        SC_ZoneList,
        #endregion

        #region Initial Connection
        /// <summary>
        /// These packets handles an incoming connection to a zone/arena
        /// </summary>
        CS_Initial,
        SC_Initial,
        CS_ArenaState,
        SC_ArenaState,
        SC_DropState,
        SC_FlagState,
        SC_ArenaBots,
        #endregion

        #region Assets
        /// <summary>
        /// These are the packets that the server uses to give a client assets
        /// it needs to run a zone.
        /// </summary>
        CS_RequestAssets,
        SC_TotalAssets,
        SC_ItemInfo,
        SC_VehicleInfo,
        SC_WeaponInfo,
        #endregion

        #region In Game
        /// <summary>
        /// These various packets are used while in game, from seeing a list of arena's to dropping an item
        /// </summary>
        SC_FullArena,

        CS_ArenaList,
        SC_ArenaList,
        CS_LeftArena,
        SC_LeftArena,
        CS_Drop,
        SC_Drop,
        CS_Warp,
        SC_Warp,
        CS_PickUp,
        SC_PickUp,
        CS_JoinTeam,
        SC_JoinTeam,
        #endregion

        #region Store Packets
        /// <summary>
        /// These packets are in game store packets, buying/selling.
        /// </summary>
        CS_StoreBuy,
        SC_StoreBuy,
        CS_StoreSell,
        SC_StoreSell,
        #endregion

        #region Chat Packets
        CS_Chat,
        SC_Chat,
        #endregion

        #region Commands and Alerts
        /// <summary>
        /// These packets are command related and system alerts.
        /// </summary>
        CS_PersonalCommand,
        SC_PersonalCommand, //???? not used
        CS_OperatorCommand,
        SC_OperatorCommand, //????
        CS_SystemCommand,
        SC_SystemCommand,
        CS_SystemAlert,
        SC_SystemAlert,
        #endregion

        #region Server/Client Misc Packets
        CS_Disconnect,
        SC_Disconnect,
        //Designates that this packet is coming from the link server
        Link
        #endregion
    }

    public enum PersonalCommands
    {
        NULL,               //Keep 00 as NULL

        accountinfo,
        arena,
        away,
        alarm,

        briefing,
        buy,
        breakdown,

        chat,
        chatadd,
        chatdrop,
        chatchart,
        crown,

        drop,

        fakealpha,
        find,
        flags,

        go,

        help,
        hide,

        ignore,
        ignoresave,
        ignoresummon,
        info,

        lag,
        loadmacro,              // ****Specific to user -- not server controlled
        log,                    // ****

        mark,                   // ****
        marksave,               // ****

        //...
        sell
    }

    public enum BanType
    {
        Arena,
        Zone,
        IP,
        Global
    }
}