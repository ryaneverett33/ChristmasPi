using System;
using System.Drawing;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Util;
using ChristmasPi.Data;
using System.Linq;

namespace ChristmasPi.Animation.Animations {
    public class flash : BaseAnimation {
        private Color color;
        
        public override string Name => "Flash";

        public flash() : base() {
            color = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor;
        }

        public override void construct(int lightcount, int fps) {
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