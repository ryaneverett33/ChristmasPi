using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class HardwareSettings {
        /// <summary>
        /// The total number of lights/LEDs on the tree 
        /// </summary>
        public int lightcount { get; set; }

        /// <summary>
        /// How many "frames" should be rendered each second
        /// </summary>
        public int fps { get; set; }

        /// <summary>
        /// The GPIO Pin for the WS281x renderer
        /// </summary>
        /// <remarks>Uses broadcom mapping</remarks>
        public int datapin { get; set; }

        /// <summary>
        /// The type of hardware being used to select a renderer
        /// </summary>
        public RendererType type { get; set; }

        /// <summary>
        /// The brightness of the lights
        /// </summary>
        public int brightness { get; set; }

        /// <summary>
        /// Whether or not to invert the invert the colors
        /// </summary>
        public bool invert { get; set; }

        /// <summary>
        /// Returns the default hardware settings
        /// </summary>
        public static HardwareSettings DefaultSettings() {
            HardwareSettings settings = new HardwareSettings();
            settings.lightcount = 0;
            settings.fps = 30;
            settings.datapin = 18;      // first PWM pin
            settings.brightness = 255;
            settings.invert = false;
            settings.type = RendererType.UNKNOWN;
            return settings;
        }
    }
}
