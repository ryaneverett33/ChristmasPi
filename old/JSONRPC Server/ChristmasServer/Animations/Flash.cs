using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Flash : BaseAnimation, IAnimation {
        public Flash(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOn();
            Thread.Sleep(500);
            gpio.AllOff();
            Thread.Sleep(500);
        }
        public int getBranchCount() {
            return 6;
        }
        public int getLength() {
            return 0;
        }
    }
}
