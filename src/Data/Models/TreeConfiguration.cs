using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace ChristmasPi.Data.Models {
    public class ColorSettings {
        public bool flipRG { get; set; }
        public bool flipGB { get; set; }
        public bool flipRB { get; set; }
        public string @default { get; set; }
        public Color DefaultColor {
            get {
                if (DefaultColor == null)
                    DefaultColor = Util.ColorConverter.Convert(@default);
                return DefaultColor;
            }
            set { DefaultColor = value; }
        }
    }
    public class TreeSettings {
        public string name { get; set; }
        public ColorSettings color { get; set; }
    }
    public class SetupSettings {
        public bool firstrun { get; set; }
    }
    public class HardwareSettings {
        public int lightcount { get; set; }
        public int fps { get; set; }
        public int datapin { get; set; }    // Uses broadcom mapping
        public HardwareType type { get; set; }
    }

    public class TreeConfiguration {
        public TreeSettings tree { get; set; }
        public SetupSettings setup { get; set; }
        public HardwareSettings hardware { get; set; }
    }
}
