using System;

#if !XBOX || !XBOX360
using System.Windows.Forms;
#endif

namespace Infantry.Plugins
{
    /// <summary>
    /// Our winform key plugin class. This class only returns checks and
    /// does not need nor have an object instance.
    /// </summary>
    public static class WinFormKeys
    {
        /// <summary>
        /// Checks to see if capslock is in a locked state
        /// </summary>
        /// <returns>Returns true if it is, false if not</returns>
        public static Boolean IsCapsLocked()
        {
            return Control.IsKeyLocked(Keys.CapsLock);
        }

        /// <summary>
        /// Checks to see if numlock is in a locked state
        /// </summary>
        /// <returns>Returns true if it is, false if not</returns>
        public static Boolean IsNumLocked()
        {
            return Control.IsKeyLocked(Keys.NumLock);
        }

        /// <summary>
        /// Checks to see if scroll lock is in a locked state
        /// </summary>
        /// <returns>Returns true if it is, false if not</returns>
        public static Boolean IsScrollLocked()
        {
            return Control.IsKeyLocked(Keys.Scroll);
        }
    }
}
