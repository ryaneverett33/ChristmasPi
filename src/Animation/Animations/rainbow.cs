using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Util;

namespace ChristmasPi.Animation.Animations {
    public class rainbow : BasicAnimation {
        public override string Name => "Rainbow";
        //private float SLEEP_TIME = 0.01f;

        public override void construct(int lightcount, int fps) {
            base.construct(lightcount, fps);
            for (int i = 0; i < 360; i++) {
                System.Drawing.Color c = ColorConverter.HsvToRgb(i, 1, 1);
                list.Add(new ColorFrame(c, lightcount));
                //list.Add(new SleepFrame(SLEEP_TIME));
            }
        }
    }
}