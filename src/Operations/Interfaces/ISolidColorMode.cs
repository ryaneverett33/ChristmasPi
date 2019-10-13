using System.Drawing;
namespace ChristmasPi.Operations.Interfaces {
    interface ISolidColorMode {
        /// <summary>
        /// The current color being displayed
        /// </summary>
        Color CurrentColor { get; }
        
        /// <summary>
        /// Sets a new color to be displayed
        /// </summary>
        /// <param name="newColor">The new color to be shown</param>
        /// <returns>Status code to return to client</returns>
        int SetColor(Color newColor);
    }
}
