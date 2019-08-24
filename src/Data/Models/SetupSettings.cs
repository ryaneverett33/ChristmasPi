using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class SetupSettings {
        public bool firstrun { get; set; }

        public static SetupSettings DefaultSettings() {
            SetupSettings settings = new SetupSettings();
            settings.firstrun = true;
            return settings;
        }
    }
}
