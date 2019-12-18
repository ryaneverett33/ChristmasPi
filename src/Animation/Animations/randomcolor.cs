using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class randomcolor : BasicAnimation {

        public override string Name => "Random Color";

        public override void construct(int lightcount, int fps) {
            base.construct(lightcount, fps);
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomKnownColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomKnownColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
        }
    }
}
