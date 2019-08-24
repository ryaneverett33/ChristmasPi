using System;
using System.Text.RegularExpressions;
using System.Drawing;
using ChristmasPi.Data;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Util {
    public class ColorConverter {
        /// <summary>
        /// Converts a color parameter of form rgb() or #{} to a Color struct
        /// </summary>
        /// <param name="value">Color string to be converted</param>
        /// <returns></returns>
        public static Color Convert(string value) {
            if (Regex.IsMatch(value, Constants.REGEX_HEX_FORMAT))
                return ConvertHex(value.Trim());
            else if (Regex.IsMatch(value, Constants.REGEX_RGB_FORMAT))
                return ConvertRgb(value.Trim());
            else
                throw new InvalidColorFormatException();
        }
        private static Color ConvertHex(string hex) {
            // reference: https://www.w3schools.com/colors/colors_picker.asp
            string cleaned = hex.Substring(1).ToUpper(); // get values after #
            if (cleaned.Length != 3 && cleaned.Length != 5 && cleaned.Length != 6)
                throw new InvalidColorFormatException("Hexidecimal color is an incorrect length");
            string a, b, c = String.Empty;
            switch (cleaned.Length) {
                case 3:
                    a = new string(cleaned[0], 2);
                    b = new string(cleaned[1], 2);
                    c = new string(cleaned[2], 2);
                    break;
                case 5:
                    cleaned = cleaned.Substring(0, 4) + '0' + cleaned[4];           // Pad the last value
                    goto default;   // fallthrough
                default:
                    a = cleaned.Substring(0, 2);
                    b = cleaned.Substring(2, 2);
                    c = cleaned.Substring(4, 2);
                    break;
            }
            return Color.FromArgb(System.Convert.ToInt32(a, 16), System.Convert.ToInt32(b, 16), System.Convert.ToInt32(c, 16));
        }
        private static Color ConvertRgb(string rgb) {
            string cleaned = rgb.Replace(" ", string.Empty).Substring(4);   // gets 1,2,3)
            cleaned = cleaned.Substring(0, cleaned.Length - 1); // gets 1,2,3
            string[] values = cleaned.Split(',');
            return Color.FromArgb(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
        }
    }
}
