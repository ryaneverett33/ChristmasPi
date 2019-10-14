using System;
using System.Drawing;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Util;
using ChristmasPi.Data;
using System.Linq;

namespace ChristmasPi.Animation.Animations {
    public class flash : IAnimation {
        private int lightcount;
        private int fps;
        private FrameList list;
        private Color color;
        
        public string Name => "Flash";
        public int TotalFrames => list.Count;
        public float TotalTime => 1f;
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;

        

        public flash() {
            list = new FrameList();
            color = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor;
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
            /* Original animation
                allOn();
                Thread.Sleep(500);
                allOff();
                Thread.Sleep(500);
            */
            this.fps = fps;
            this.lightcount = lightcount;
            list.Add(new ColorFrame(color, lightcount));
            list.Add(new SleepFrame(0.5f));
            list.Add(new ColorFrame(Color.Black, lightcount));
            list.Add(new SleepFrame(0.5f));
        }
    }
}