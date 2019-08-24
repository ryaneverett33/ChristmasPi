using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class TreeSettings {
        public string name { get; set; }
        public ColorSettings color { get; set; }

        public static TreeSettings DefaultSettings() {
            TreeSettings settings = new TreeSettings();
            settings.name = "Christmas Tree";
            settings.color = ColorSettings.DefaultSettings();
            return settings;
        }
    }
}
