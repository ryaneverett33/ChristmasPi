using System;
using System.Drawing;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Util;
using ChristmasPi.Data;
using System.Linq;

namespace ChristmasPi.Animation.Animations {
    public class flash : IAnimation {
        private int lightcount;
        private int fps;
        
        public string Name => "Flash";
        public int TotalFrames => frames.Count;
        public float TotalTime => 1f;
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;
        public bool isLegacyAnimation => false;

        
        private List<AnimationFrame> frames;

        public flash() {
            frames = new List<AnimationFrame>();
        }

        public AnimationFrame[] GetFrames(int fps, int lightcount) {
            if (frames.Count != 0 && this.fps == fps && this.lightcount == lightcount) {
                return frames.ToArray();
            }
            else {
                construct(lightcount, fps);
                return frames.ToArray();
            }
        }

        private void construct(int lightcount, int fps) {
            /* Original animation
            allOn();
            Thread.Sleep(500);
            allOff();
            Thread.Sleep(500);
            */
            this.lightcount = lightcount;
            this.fps = fps;
            int currentFrameCount = 0;
            frames.Clear();
            // Turn On
            AnimationFrame frame = new AnimationFrame(FrameAction.Update,
                Enumerable.Repeat(ConfigurationManager.Instance.TreeConfiguration.tree.color.DefaultColor, lightcount).ToArray());
            frames.Add(frame);
            currentFrameCount++;
            // Sleep 0.5s
            AnimationFrame[] sleepframes = Enumerable.Repeat(new AnimationFrame(FrameAction.Sleep), AnimationHelpers.SleepTime(0.5f, fps)).ToArray();
            frames.AddRange(sleepframes);
            currentFrameCount += sleepframes.Length;
            // turn off
            frame = new AnimationFrame(FrameAction.Update,
                Enumerable.Repeat(Color.Black, lightcount).ToArray());
            frames.Add(frame);
            currentFrameCount++;
            // sleep 0.5s
            frames.AddRange(sleepframes);
            currentFrameCount += sleepframes.Length;
            int blankFrameCount = AnimationHelpers.FrameCount(TotalTime, fps) - currentFrameCount;
            if (blankFrameCount > 0)
                frames.AddRange(Enumerable.Repeat(new AnimationFrame(FrameAction.Blank), blankFrameCount));
        }
    }
}