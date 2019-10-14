using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Animation {
    public class FrameList {
        List<IAnimationFrame> animationFrames;

        public int Count => animationFrames.Count;

        /// <summary>
        /// Creates a new FrameList
        /// </summary>
        public FrameList() {
            animationFrames = new List<IAnimationFrame>();
        }

        /// <summary>
        /// Adds an animation frame to the list
        /// </summary>
        /// <param name="frame">Animation frame to add to list</param>
        public void Add(IAnimationFrame frame) {
            animationFrames.Add(frame);
        }

        /// <summary>
        /// Gets the evaluated RenderFrame list of all animation frames
        /// </summary>
        /// <param name="fps">The desired frame rate</param>
        /// <returns>List of render frames</returns>
        /// <remarks>Some Aniamtion frames are time sensitive, so calls with different fps values may return different size lists</remarks>
        public RenderFrame[] ToFrames(int fps) {
            List<RenderFrame> renderFrames = new List<RenderFrame>();
            foreach (IAnimationFrame frame in animationFrames) {
                RenderFrame[] frames = frame.GetFrames(fps);
                renderFrames.AddRange(frames);
            }
            return renderFrames.ToArray();
        }
    }
}
