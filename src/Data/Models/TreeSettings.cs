using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class TreeSettings {

        /// <summary>
        /// The friendly name of the christmas tree
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Color settings for hardware and personal preferences
        /// </summary>
        public ColorSettings color { get; set; }

        /// <summary>
        /// Returns the default tree settings
        /// </summary>
        public static TreeSettings DefaultSettings() {
            TreeSettings settings = new TreeSettings();
            settings.name = "Christmas Tree";
            settings.color = ColorSettings.DefaultSettings();
            return settings;
        }
    }
}
