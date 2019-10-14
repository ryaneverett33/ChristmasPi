using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BaseAnimation : IAnimation {
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;
        public string Name { get; set; }

        public int lightcount;
        public int fps;
        public FrameList list;

        public BaseAnimation() { }

        public virtual RenderFrame[] GetFrames(int fps, int lightcount) {
            if (list.Count != 0 && this.fps == fps && this.lightcount == lightcount) {
                return list.ToFrames(fps);
            }
            else {
                this.list = new FrameList();
                construct(lightcount, fps);
                return list.ToFrames(fps);
            }
        }

        public abstract void construct(int lightcount, int fps);
    }
}