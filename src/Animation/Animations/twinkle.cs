using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class twinkle : BaseAnimation {

        public override string Name => "Twinkle";
        private float sleeptime = 0.5f;

        public override void construct(int lightcount, int fps) {
            base.construct(lightcount, fps);
            list.Add(new ColorFrame(new RandomColor(() => {
                if (RandomGenerator.Instance.Number(0,100) > 75)
                    return Constants.COLOR_OFF;
                else
                    return ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor;
            }), lightcount));
            list.Add(new SleepFrame(sleeptime));
        }
    }
}
