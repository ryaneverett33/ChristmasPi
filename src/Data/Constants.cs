using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data {
    public class Constants {
        public static readonly int FPS_MAX = 100;
        public static readonly int FPS_DEFAULT = 30;
        public static readonly string REGEX_RGB_FORMAT = @"rgb\((\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *)\)";
        public static readonly string REGEX_HEX_FORMAT = @"#[0-9a-fA-F]{3,6}";
    }
}
