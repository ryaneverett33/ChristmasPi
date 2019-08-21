using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Mirage : BaseAnimation, IAnimation {
        public Mirage(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOff();
            for (int i = 0; i < gpio.pins.Length; i++) {
                gpio.turnOn(gpio.pins[i].gpioPin);
                //Console.WriteLine("pin: " + pins[i]);
                Thread.Sleep(750);
                gpio.turnOff(gpio.pins[i].gpioPin);
            }
            for (int i = (gpio.pins.Length - 1); i > -1; i--) {
                gpio.turnOn(gpio.pins[i].gpioPin);
                //Console.WriteLine("pin: " + pins[i]);
                Thread.Sleep(750);
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
