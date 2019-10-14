using System;
using System.Drawing;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Animation {
    public class SingleFrame : RenderFrame, IAnimationFrame {

        /// <summary>
        /// Creates a new SingleFrame with the specified action
        /// </summary>
        /// <param name="action">The render action</param>
        public SingleFrame(FrameAction action) : base(action, null) { }
        
        /// <summary>
        /// Creates a new Update SingleFrame with the specified colors
        /// </summary>
        /// <param name="colors">The colors to use during rendering</param>
        public SingleFrame(ColorList colors) : base(FrameAction.Update, colors) { }

        /// <summary>
        /// Creates a Single Frame where the resultant RenderFrame is not repeated
        /// </summary>
        /// <param name="action">The render action</param>
        /// <param name="colors">Colors to use during the render</param>
        public SingleFrame(FrameAction action, ColorList colors) : base(action, colors) { }

        public RenderFrame[] GetFrames(int fps) {
            return new RenderFrame[] { this };
        }
    }
}
