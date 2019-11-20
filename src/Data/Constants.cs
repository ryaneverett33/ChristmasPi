using System;
using System.Drawing;

namespace ChristmasPi.Data {
    public class Constants {
        // Hardware
        public static readonly int FPS_MAX = 100;
        public static readonly int FPS_DEFAULT = 30;

        // Regexs
        public static readonly string REGEX_RGB_FORMAT = @"rgb\((\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *)\)";
        public static readonly string REGEX_HEX_FORMAT = @"#[0-9a-fA-F]{3,6}";
        public static readonly string REGEX_HEX_WEB_FORMAT = @"[0-9a-fA-F]{3,6}";
        public static readonly string REGEX_AMPM_FORMAT = @"(0|1)[0-9]:[0-5][0-9] *((AM)|(PM))";
        public static readonly string REGEX_24HR_FORMAT = @"(0|1|2)[0-9]:[0-5][0-9]";

        // Operations
        public static readonly string DEFAULT_OPERATING_MODE = "SolidColorMode";
        public static readonly int ACTIVATION_TIMEOUT = 1000;           // in ms

        // Colors
        public static readonly int RANDOM_DEFAULT = 255;
        public static readonly int RANDOM_MAX = 255;                    // exclusive range
        public static readonly int RANDOM_MIN = 0;
        public static readonly Color COLOR_OFF = Color.Black;
    }
}
