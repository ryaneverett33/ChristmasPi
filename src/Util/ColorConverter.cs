using System;
using System.Text.RegularExpressions;
using System.Drawing;
using ChristmasPi.Data;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Util {
    public class ColorConverter {
        public static Color Convert(string value) {
            if (Regex.IsMatch(value, Constants.REGEX_HEX_FORMAT))
                return ConvertHex(value);
            else if (Regex.IsMatch(value, Constants.REGEX_RGB_FORMAT))
                return ConvertRgb(value);
            else
                throw new InvalidColorFormatException();
        }
        private static Color ConvertHex(string hex) {
            /// TODO implement 
            throw new NotImplementedException();
        }
        private static Color ConvertRgb(string rgb) {
            /// TODO implement
            throw new NotImplementedException();
        }
    }
}
