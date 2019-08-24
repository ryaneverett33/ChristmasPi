using System;
using System.Collections.Generic;
using System.Linq;
namespace ChristmasPi.Data.Models {
    public class TreeConfiguration {
        public TreeSettings tree { get; set; }
        public SetupSettings setup { get; set; }
        public HardwareSettings hardware { get; set; }

        public static TreeConfiguration DefaultSettings() {
            TreeConfiguration config = new TreeConfiguration();
            config.tree = TreeSettings.DefaultSettings();
            config.setup = SetupSettings.DefaultSettings();
            config.hardware = HardwareSettings.DefaultSettings();
            return config;
        }
    }
}
