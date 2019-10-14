using System;
using ChristmasPi.Util;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Animation {
    public class SleepFrame : IAnimationFrame {
        /// <summary>
        /// The length of time to sleep for (in seconds)
        /// </summary>
        public float SleepTime;

        /// <summary>
        /// Creates a new Frame to sleep for x amount of seconds
        /// </summary>
        /// <param name="sleeptime">time to sleep (in seconds)</param>
        public SleepFrame(float sleeptime) {
            this.SleepTime = sleeptime;
        }

        public RenderFrame[] GetFrames(int fps) {
            int sleepframes = AnimationHelpers.SleepTime(SleepTime, fps);
            return FrameHelpers.repeat(new RenderFrame(FrameAction.Sleep), sleepframes);
        }
    }
}
