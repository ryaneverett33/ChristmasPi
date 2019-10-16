using System;
using ChristmasPi.Util;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Animation {
    /// <summary>
    /// Allow the animation to wait for other frames to run
    /// </summary>
    public class WaitFrame : IAnimationFrame {
        /// <summary>
        /// The number of frames to wait for
        /// </summary>
        public int FrameCount;

        /// <summary>
        /// Creates a new Frame to sleep for x frames
        /// </summary>
        /// <param name="framecount">number of frames</param>
        public WaitFrame(int framecount) {
            this.FrameCount = framecount;
        }

        public RenderFrame[] GetFrames(int fps) {
            return FrameHelpers.repeat(new RenderFrame(FrameAction.Blank), FrameCount);
        }
    }
}
