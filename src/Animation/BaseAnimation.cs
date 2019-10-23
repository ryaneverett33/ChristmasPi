using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BaseAnimation : IAnimation {
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;
        public virtual string Name { get { throw new NotImplementedException(); } }
        public AnimationProperty[] Properties => _properties.ToAnimationArray();

        private int lightcount;
        private int fps;
        private PropertyList _properties;
        public FrameList list;

        public BaseAnimation() { list = new FrameList(); }

        public void AddProperties(AnimationProperty[] properties) {
            this._properties = new PropertyList();
            foreach (AnimationProperty property in properties) {
                this._properties.Add(property.Name, property.Value);
            }
        }

        public virtual RenderFrame[] GetFrames(int fps, int lightcount) {
            if (list.Count != 0 && this.fps == fps && this.lightcount == lightcount) {
                return list.ToFrames(fps);
            }
            else {
                // this.list = new FrameList();
                this.construct(lightcount, fps);
                return list.ToFrames(fps);
            }
        }

        public virtual void construct(int lightcount, int fps) {
            this.fps = fps;
            this.lightcount = lightcount;
        }
    }
}