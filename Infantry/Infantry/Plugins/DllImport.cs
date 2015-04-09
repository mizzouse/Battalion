using System;
using System.Runtime.InteropServices;

namespace Infantry.Plugins
{
    public static class DllImport
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Checks if our application window is active
        /// </summary>
        /// <param name="handle">Our application handler</param>
        /// <returns>Returns true if is in focus, false if not</returns>
        public static Boolean IsWindowActive(IntPtr handle)
        {
            IntPtr foreground = GetForegroundWindow();
            if (foreground.Equals(handle))
                return true;

            return false;
        }
    }
}
