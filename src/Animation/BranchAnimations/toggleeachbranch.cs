using System;
using System.Drawing;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.BranchAnimations {
    public class toggleeachbranch : BaseBranchAnimation {
        public override string Name => "Toggle Each Branch";
        private Color color;
        private float SLEEP_TIME = 0.25f;
        private readonly int WAIT_FRAMES = 3;

        public toggleeachbranch() : base() {
            color = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor;
        }

        public override void RegisterProperties() {
            Ref<object> refone = new Ref<object>(
                () => SLEEP_TIME,
                v => { SLEEP_TIME = (float)v; });
            base.RegisterProperty(refone, "SLEEP_TIME", 0.25f, PrimType.Float);
            base.ResolveProperties();
        }

        public override void constructbranch(int fps, ref BranchData branch) {
            base.constructbranch(fps, ref branch);
            if (branch.index != 0) {
                int sleepMultiple = branch.index;
                for (int i = 0; i < sleepMultiple; i++) {
                    branch.Add(new SleepFrame(SLEEP_TIME));
                }
                // wait for other branchs to execute
                branch.Add(new WaitFrame(WAIT_FRAMES));
            }
            branch.Add(new ColorFrame(color, branch.LightCount));
            branch.Add(new SleepFrame(SLEEP_TIME));
            branch.Add(new ColorFrame(Constants.COLOR_OFF, branch.LightCount));
        }
    }
}