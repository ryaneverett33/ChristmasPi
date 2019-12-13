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

        public List<Branch> branches { get; set;}

        /// <summary>
        /// The operation mode to run at startup
        /// </summary>
        public string defaultmode { get; set; }

        /// <summary>
        /// The animation to play if the default mode is Animation
        /// </summary>
        public string defaultanimation { get; set; }

        /// <summary>
        /// Returns the default tree settings
        /// </summary>
        public static TreeSettings DefaultSettings() {
            TreeSettings settings = new TreeSettings();
            settings.name = "Christmas Tree";
            settings.defaultmode = "SolidColorMode";
            settings.defaultanimation = "Twinkle";
            settings.color = ColorSettings.DefaultSettings();
            settings.branches = new List<Branch>();
            return settings;
        }
    }
}
