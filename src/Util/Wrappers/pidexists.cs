using System;
using System.Runtime.InteropServices;

namespace ChristmasPi.Util.Wrappers {
    public sealed class pidexistswrapper {
        [DllImport("libpidexists.so")]
		private static extern int pidexists(int pid);

        /// <summary>
        /// Checks if a PID exists
        /// </summary>
        /// <param name="pid">The PID to check</param>
        /// <returns>True if the PID exists, False otherwise</returns>
        /// <remarks>If a Program is running/exiting/alive, a PID will exist for it.
        /// Use this method to check if the program is still running.</remarks>
        /// <seealso="/helpers/wrappers/pidexists.c" />
        public static bool PidExists(int pid) {
            if (pid < 0)
                throw new ArgumentOutOfRangeException("pid");
            int returnValue = pidexists(pid);
            switch (returnValue) {
                case 0:
                    // 0 if it exists
                    return true;
                case 1:
                    // 1 if it doesn't exist
                    return false;
                default:
                    // -1 if error
                    throw new Exception("An error occurred while executing native call");
            }
        }
    }
}