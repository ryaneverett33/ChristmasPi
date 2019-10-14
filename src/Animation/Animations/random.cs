using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class random : IAnimation {
        private int lightcount;
        private int fps;
        private FrameList list;

        public string Name => "Flash";
        public int TotalFrames => list.Count;
        public float TotalTime => 1f;
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;

        public random() {
            list = new FrameList();
        }

        public RenderFrame[] GetFrames(int fps, int lightcount) {
            if (list.Count != 0 && this.fps == fps && this.lightcount == lightcount) {
                return list.ToFrames(fps);
            }
            else {
                construct(lightcount, fps);
                return list.ToFrames(fps);
            }
        }

        private void construct(int lightcount, int fps) {
            this.fps = fps;
            this.lightcount = lightcount;
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
        }
    }
}
