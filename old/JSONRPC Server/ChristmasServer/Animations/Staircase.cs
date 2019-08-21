using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Staircase : BaseAnimation, IAnimation {
        public Staircase(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOff();
            bool isEven = (gpio.pins.Length % 2 == 0);
            int middle = gpio.pins.Length / 2;   //Don't add one
            for (int i = 0; i < gpio.pins.Length; i++) {
                int modI = Math.Abs(i - (gpio.pins.Length - 1));
                if (i == middle && !isEven) {
                    gpio.turnOn(gpio.pins[middle].gpioPin);
                    Thread.Sleep(750);
                    gpio.turnOff(gpio.pins[middle].gpioPin);
                }
                else {
                    gpio.turnOn(gpio.pins[i].gpioPin);
                    gpio.turnOn(gpio.pins[modI].gpioPin);
                    Thread.Sleep(750);
                    gpio.turnOff(gpio.pins[i].gpioPin);
                    gpio.turnOff(gpio.pins[modI].gpioPin);
                }
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
