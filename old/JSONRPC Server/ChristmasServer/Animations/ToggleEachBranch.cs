using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class ToggleEachBranch : BaseAnimation, IAnimation {
        public ToggleEachBranch(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOff();
            for (int i = 0; i < gpio.pins.Length; i++) {
                gpio.turnOn(gpio.pins[i].gpioPin);
                Thread.Sleep(500);
                gpio.turnOff(gpio.pins[i].gpioPin);
            }
        }
        public int getBranchCount() {
            return 6;
        }
        public int getLength() {
            return 0;
        }
    }
}
