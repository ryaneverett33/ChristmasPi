using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BasicAnimation : BaseAnimation, IAnimation {
        public int LightCount { get; private set; }
        public int FPS { get; private set; }
        public override bool isBranchAnimation => false;

        public FrameList list;

        public BasicAnimation() { 
            list = new FrameList();
            
        }

        public virtual RenderFrame[] GetFrames(int fps, int lightcount) {
            if (list.Count != 0 && this.FPS == fps && this.LightCount == lightcount) {
                return list.ToFrames(fps);
            }
            else {
                // this.list = new FrameList();
                this.construct(lightcount, fps);
                return list.ToFrames(fps);
            }
        }

        public virtual void construct(int lightcount, int fps) {
            this.FPS = fps;
            this.LightCount = lightcount;
        }
    }
}