using System;
using System.Drawing;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Animation {
    /// <summary>
    /// Allow the animation to create a frame with a list of colors
    /// </summary>
    public class SingleFrame : RenderFrame, IAnimationFrame {
        
        /// <summary>
        /// Creates a new Update SingleFrame with the specified colors
        /// </summary>
        /// <param name="colors">The colors to use during rendering</param>
        public SingleFrame(ColorList colors) : base(FrameAction.Update, colors) { }

        public RenderFrame[] GetFrames(int fps) {
            return new RenderFrame[] { this };
        }
    }
}
