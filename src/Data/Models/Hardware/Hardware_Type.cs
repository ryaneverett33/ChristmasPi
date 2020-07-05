using System;
using System.IO;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Models.Hardware {
    /// <summary>
    /// The types of hardware/systems recognized by ChristmasPi
    /// </summary>
    [Flags]
    public enum Hardware_Type {
        All = 0 | 1 | 2,        // Referring to all hardware (even unkown) in this enumeration
        RPI = 1,                // A Raspberry Pi
        UNKNOWN = 2             // Unknown hardware, could be a PC or a new board
    }
    public class HardwareType {
        /// <summary>
        /// Detects what hardware the system is running on
        /// </summary>
        /// <returns>Current detected type</returns>
        public static Hardware_Type GetHardwareType(bool defaultToRpi = false) {
            // check if raspberry pi
            try {
                RPI_Type type = RPIType.GetHardwareType(defaultToRpi);
                return Hardware_Type.RPI;
            }
            catch (Exception) {
                if (defaultToRpi)
                    return Hardware_Type.RPI;
            }
            return Hardware_Type.UNKNOWN;
        }
    }
}
