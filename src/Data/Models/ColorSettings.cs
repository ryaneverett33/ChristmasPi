using System.Drawing;

namespace ChristmasPi.Data.Models {
    public class ColorSettings {

        /// <summary>
        /// The byte order for a color object
        /// </summary>
        public string colororder { get; set; }

        /// <summary>
        /// The default color if no other color is used
        /// </summary>
        public string @default { get; set; }
        private Color _defaultColor = Color.White;

        /// <summary>
        /// Gets the object representation of the default color
        /// </summary>
        /// <see cref="ColorSettings.@default"/>
        public Color DefaultColor {
            get {
                if (_defaultColor == Color.White)
                    _defaultColor = Util.ColorConverter.Convert(@default);
                return _defaultColor;
            }
            set { _defaultColor = value; }
        }

        /// <summary>
        /// Returns the default color settings
        /// </summary>
        public static ColorSettings DefaultSettings() {
            ColorSettings settings = new ColorSettings();
            settings.colororder = "RGB";
            settings.@default = "#fff";
            return settings;
        }
    }
}
