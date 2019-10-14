using System;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Data.Interfaces {
    public interface IAnimationFrame {
        /// <summary>
        /// Evaluates an animation frame to its equivalent render frame
        /// </summary>
        /// <returns>An array of render frames</returns>
        RenderFrame[] GetFrames(int fps);
    }
}
