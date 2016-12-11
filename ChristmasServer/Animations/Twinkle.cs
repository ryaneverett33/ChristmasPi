using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Twinkle : BaseAnimation, IAnimation {
        public Twinkle(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            Random rand = new Random();
            int r = rand.Next(6);
            //Console.WriteLine("Twinkle on branch: " + r);
            for (int i = 0; i < 6; i++) {
                if (i == r) {
                    gpio.turnOff(gpio.pins[i].gpioPin);
                }
                else {
                    gpio.turnOn(gpio.pins[i].gpioPin);
                }
            }
            Thread.Sleep(1500);
        }
        public int getBranchCount() {
            return 6;
        }
        public int getLength() {
            return 0;
        }
    }
}
