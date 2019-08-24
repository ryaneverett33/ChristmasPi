using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class HardwareSettings {
        public int lightcount { get; set; }
        public int fps { get; set; }
        public int datapin { get; set; }    // Uses broadcom mapping
        public HardwareType type { get; set; }

        public static HardwareSettings DefaultSettings() {
            HardwareSettings settings = new HardwareSettings();
            settings.lightcount = 0;
            settings.fps = 30;
            settings.datapin = 18;      // first PWM pin
            settings.type = HardwareType.UNKNOWN;
            return settings;
        }
    }
}
