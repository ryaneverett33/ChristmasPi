using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class SetupSettings {
        
        /// <summary>
        /// Whether or not the setup process has ran before
        /// </summary>
        public bool firstrun { get; set; }

        /// <summary>
        /// Returns the default settings for the setup process
        /// </summary>
        public static SetupSettings DefaultSettings() {
            SetupSettings settings = new SetupSettings();
            settings.firstrun = true;
            return settings;
        }
    }
}
