﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class random : BaseAnimation {

        public new string Name => "Flash";

        public override void construct(int lightcount, int fps) {
            this.fps = fps;
            this.lightcount = lightcount;
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
        }
    }
}
