using System;
using System.Drawing;
using ChristmasPi.Data.Interfaces;
using ChristmasPi.Util;

namespace ChristmasPi.Data.Models.Animation {
    /// <summary>
    /// Allow the animation to repeat colors
    /// </summary>
    public class ColorFrame : RenderFrame, IAnimationFrame {
        /// <summary>
        /// Creates a new frame whose lights are all the same color
        /// </summary>
        /// <param name="color">The color of the lights</param>
        /// <param name="lightcount">The number of lights</param>
        public ColorFrame(Color color, int lightcount) : base(FrameAction.Update,
            new ColorList(FrameHelpers.repeat(color, lightcount))) { }

        public ColorFrame(RandomColor color, int lightcount) : base(FrameAction.Update,
            new ColorList(FrameHelpers.repeat(color, lightcount))) { }

        public RenderFrame[] GetFrames(int fps) {
            return new RenderFrame[] { this };
        }
    }
}
