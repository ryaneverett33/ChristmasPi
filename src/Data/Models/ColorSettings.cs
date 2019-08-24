using System.Drawing;

namespace ChristmasPi.Data.Models {
    public class ColorSettings {
        public bool flipRG { get; set; }
        public bool flipGB { get; set; }
        public bool flipRB { get; set; }
        public string @default { get; set; }
        private Color _defaultColor = Color.Empty;
        public Color DefaultColor {
            get {
                if (_defaultColor == Color.Empty)
                    _defaultColor = Util.ColorConverter.Convert(@default);
                return _defaultColor;
            }
            set { _defaultColor = value; }
        }

        public static ColorSettings DefaultSettings() {
            ColorSettings settings = new ColorSettings();
            settings.flipRG = false;
            settings.flipGB = false;
            settings.flipRB = false;
            settings.@default = "#fff";
            return settings;
        }
    }
}
