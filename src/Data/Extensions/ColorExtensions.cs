using System.Drawing;

namespace ChristmasPi.Data.Extensions {
    public static class ColorExtensions {
        /// <summary>
        /// Flips the red and green values for a given color
        /// </summary>
        /// <param name="color">The color struct to be flipped</param>
        /// <returns></returns>
        public static Color FlipRG(this Color color) {
            return Color.FromArgb(color.G, color.R, color.B);
        }
        /// <summary>
        /// Flips the green and blue values for a given color
        /// </summary>
        /// <param name="color">The color struct to be flipped</param>
        /// <returns></returns>
        public static Color FlipGB(this Color color) {
            return Color.FromArgb(color.R, color.B, color.G);
        }
        /// <summary>
        /// Flips the red and blue values for a given color
        /// </summary>
        /// <param name="color">The color struct to be flipped</param>
        /// <returns></returns>
        public static Color FlipRB(this Color color) {
            return Color.FromArgb(color.B, color.G, color.R);
        }
    }
}
