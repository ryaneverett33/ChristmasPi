using System;

namespace ChristmasPi.Util {
    public class AnimationHelpers {

        /// <summary>
        /// Calculates the runtime length of an animation at given fps
        /// </summary>
        /// <param name="frames">Total number of frames in a clip</param>
        /// <param name="fps">Frame rate of the animation</param>
        /// <returns>Time in (s) rounded to 0.1s</returns>
        public static float TimeOfAnimation(int frames, int fps) {
            float time = frames / (float)fps;
            return (float)Math.Round((decimal)time, 1);
        }

        /// <summary>
        /// Calculates the number of frames to sleep for a given time
        /// </summary>
        /// <param name="time">Total time (in s) to sleep</param>
        /// <param name="fps">Frame rate of the animation</param>
        /// <returns>Number of frames to sleep on</returns>
        public static int SleepTime(float time, int fps) {
            return FrameCount(time, fps);
        }

        /// <summary>
        /// Calculates the number of frames shown in a given time
        /// </summary>
        /// <param name="time">Total time of the clip in seconds</param>
        /// <param name="fps">Frame rate of the animation</param>
        /// <returns>Number of frames to be shown</returns>
        public static int FrameCount(float time, int fps) {
            float num = time * fps;
            return (int)num;
        }

        // the time in ms that a frame is visible
        public static float FrameTime(int fps) {
            return (float)(1 / (float)fps);
        }
    }
}