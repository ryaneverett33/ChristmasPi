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
        /// <returns>The equivalent color object</returns>
        public static Color Convert(string value) {
            if (Regex.IsMatch(value, Constants.REGEX_HEX_FORMAT))
                return ConvertFromHex(value.Trim());
            else if (Regex.IsMatch(value, Constants.REGEX_RGB_FORMAT))
                return ConvertFromRgb(value.Trim());
            else
                throw new InvalidColorFormatException();
        }
        /// <summary>
        /// Converts a Color from it's hexadecimal representation
        /// </summary>
        /// <param name="hex">hexidecimal representation of the color</param>
        /// <returns>The equivalent color object</returns>
        /// <example>'#fff' & '#ffffff' returns Color.FromARGB(255, 255, 255, 255)</example>
        /// <remarks>Follows the same functionality as the w3 color picker: https://www.w3schools.com/colors/colors_picker.asp </remarks>
        private static Color ConvertFromHex(string hex) {
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

        /// <summary>
        /// Converts a color from its rgb string representation
        /// </summary>
        /// <param name="rgb">the rgb(r,g,b) representation of the object</param>
        /// <returns>The equivalent color object</returns>
        /// <example>'rgb(255,255,255)' returns Color.FromARGB(255, 255, 255, 255)</example>
        private static Color ConvertFromRgb(string rgb) {
            string cleaned = rgb.Replace(" ", string.Empty).Substring(4);   // gets 1,2,3)
            cleaned = cleaned.Substring(0, cleaned.Length - 1); // gets 1,2,3
            string[] values = cleaned.Split(',');
            return Color.FromArgb(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
        }

        /// <summary>
        /// Converts from a HSV color to a RGB color
        /// </summary>
        /// <param name="h">The Hue property of the color (range: 0-360)</param>
        /// <param name="S">The Saturation property of the color (range: 0-1)</param>
        /// <param name="V">The Value property of the color (range: 0-1)</param>
        /// <source>https://stackoverflow.com/a/31626758</source>
        /// <returns>A RGB Color</returns>
        /// <remarks>If values are not within the proper range, they will be clamped</remarks>
        public static Color HsvToRgb(double hue, double saturation, double value) {
            hue = Math.Clamp(hue, 0, 360);
            saturation = Math.Clamp(saturation, 0, 1);
            value = Math.Clamp(value, 0, 1);

            int hi = System.Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = System.Convert.ToInt32(value);
            int p = System.Convert.ToInt32(value * (1 - saturation));
            int q = System.Convert.ToInt32(value * (1 - f * saturation));
            int t = System.Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, v, p);
            else if (hi == 2) return Color.FromArgb(255, p, v, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, v);
            else if (hi == 4) return Color.FromArgb(255, t, p, v);
            else return Color.FromArgb(255, v, p, q);
        }
    }
}
