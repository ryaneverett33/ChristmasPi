using System;
using System.Collections.Generic;
using ChristmasPi.Animations.Interfaces;
using ChristmasPi.Util;

namespace ChristmasPi.Animations.Animations {
    public class flash : IAnimation {
        public string Name => "Flash";
        public int TotalFrames => frames.Count;
        public float TotalTime => 1f;
        
        private List<IAnimationFrame> frames;

        public flash() {
            frames = new List<IAnimationFrame>();
        }

        public IAnimationFrame[] GetFrames(int fps) {
            if (frames.Count != 0 && TotalTime.EqualsDelta(AnimationHelpers.TimeOfAnimation(TotalFrames, fps), 0.2f)) {
                return frames.ToArray();
            }
            else {
                construct();
                return frames.ToArray();
            }
            throw new NotImplementedException();
        }

        private void construct() {
            /* Original animation
            allOn();
            Thread.Sleep(500);
            allOff();
            Thread.Sleep(500);
            */
        }
    }
}