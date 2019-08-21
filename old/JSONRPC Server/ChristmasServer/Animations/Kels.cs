using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Kels : BaseAnimation, IAnimation {
        public Kels(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOff();
            gpio.turnOn(gpio.pins[0].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOn(gpio.pins[5].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOn(gpio.pins[1].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOn(gpio.pins[4].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOn(gpio.pins[2].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOn(gpio.pins[3].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[2].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[3].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[1].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[4].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[0].gpioPin);
            Thread.Sleep(1000);
            gpio.turnOff(gpio.pins[5].gpioPin);
            Thread.Sleep(1000);
        }
        public int getBranchCount() {
            return 6;
        }
        public int getLength() {
            return 0;
        }
    }
}
