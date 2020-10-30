using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class random : BasicAnimation {
        public override string Name => "Random";
        public override bool isDebugAnimation => true; 

        private float sleeptime;

        public override void RegisterProperties() {
            Ref<object> reference = new Ref<object>(
                ()=>sleeptime, 
                v=>{sleeptime=(float)v;});
            base.RegisterProperty(reference, "SLEEP_TIME", 0.2f, PrimType.Float);
            base.ResolveProperties();
        }

        public override void construct(int lightcount, int fps) {
            base.construct(lightcount, fps);
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            //list.Add(new SleepFrame(0.2f));
            list.Add(new SleepFrame(sleeptime));
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            //list.Add(new SleepFrame(0.2f));
            list.Add(new SleepFrame(sleeptime));
        }
    }
}
