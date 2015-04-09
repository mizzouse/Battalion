using System;
using System.Collections.Generic;

namespace Infantry.Objects
{
    public class Weapon
    {
        /// <summary>
        /// The id of this weapon
        /// </summary>
        public int ID;
        /// <summary>
        /// Our current yaw position on the Y axis
        /// </summary>
        public float Yaw;
        /// <summary>
        /// Our current pitch position on the X axis
        /// </summary>
        public float Pitch;
        /// <summary>
        /// Is this weapon firing?
        /// </summary>
        public bool IsFiring;
        /// <summary>
        /// Player that owns this weapon if any
        /// </summary>
        private Vehicle Owner;
        /// <summary>
        /// Last fire time check
        /// </summary>
        private int LastFireTime;
        /// <summary>
        /// Last prefire check
        /// </summary>
        private int LastPreFire;
        /// <summary>
        /// Last time this was fired
        /// </summary>
        private int LastFire;
        /// <summary>
        /// Last time reloaded check
        /// </summary>
        private int LastReload;
        /// <summary>
        /// Is this an active gun?(handling it)
        /// </summary>
        private bool IsActive;
        /// <summary>
        /// Do we have to manually reload this?
        /// </summary>
        private bool ManualReload;
        /// <summary>
        /// How many bullets were shot
        /// </summary>
        private int TimesShot;
        /// <summary>
        /// Is this weapon loaded?
        /// </summary>
        private bool IsLoaded;
    }
}
