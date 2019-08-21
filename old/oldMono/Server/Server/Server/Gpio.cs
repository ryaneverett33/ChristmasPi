using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WiringPi;

namespace Server {
    class Gpio {
        int OUTPUT = (int)GPIO.GPIOpinmode.Output;
        int INPUT = (int)GPIO.GPIOpinmode.Input;
        int ON = 0;
        int OFF = 1;
        public List<pinInfo> pinData = new List<pinInfo>();
        int pins = 0;
        public void Setup() {
            Init.WiringPiSetup();
            GPIO.pinMode(7,OUTPUT);                     //Branch 0, Pin 7
            addPin(new pinInfo(0, 7, 7, 0));
            GPIO.pinMode(0, OUTPUT);                    //Branch 1, Pin 11
            addPin(new pinInfo(0, 11, 0, 1));
            GPIO.pinMode(2, OUTPUT);                    //Branch 2, Pin 13
            addPin(new pinInfo(0, 13, 2, 2));
            GPIO.pinMode(3, OUTPUT);                    //Branch 3, Pin 15
            addPin(new pinInfo(0, 15, 3, 3));
            GPIO.pinMode(1, OUTPUT);                    //Branch 4, Pin 12
            addPin(new pinInfo(0, 12, 1, 4));
            GPIO.pinMode(4, OUTPUT);                    //Branch 5, Pin 16
            addPin(new pinInfo(0, 16, 4, 5));
            //init pins
        }
        public void addPin(pinInfo info) {
            pinData.Add(info);
            pins++;
        }
        public void removePin(pinInfo info) {
            pinData.Remove(info);
            pins--;
        }
        public int togglePin(pinInfo info) {
            int status = -1;
            int pinNum = -1;
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == info.gpioPin) {
                    status = pin.status;
                    pinNum = pin.gpioPin;
                }
            }
            if (pinNum != -1) {
                GPIO.digitalWrite(pinNum, Math.Abs(1 - status));
                status = Math.Abs(1 - status);
            }
            return status;
        }
        public int togglePin(int gpio) {
            int status = -1;
            int pinNum = -1;
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == gpio) {
                    status = pin.status;
                    pinNum = pin.gpioPin;
                    break;
                }
            }
            if (pinNum != -1) {
                GPIO.digitalWrite(pinNum, Math.Abs(1 - status));
                status = Math.Abs(1 - status);
            }
            return status;
        }
        public void turnOff(pinInfo info) {
            GPIO.digitalWrite(info.gpioPin, OFF);
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == info.gpioPin) {
                    pin.status = OFF;
                    break;
                }
            }
        }
        public void turnOff(int gpio) {
            GPIO.digitalWrite(gpio, OFF);
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == gpio) {
                    pin.status = OFF;
                    break;
                }
            }
        }
        public void allOff() {
            foreach (pinInfo pin in pinData) {
                turnOff(pin);
            }
        }
        public void turnOn(int gpio) {
            GPIO.digitalWrite(gpio, ON);
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == gpio) {
                    pin.status = ON;
                    break;
                }
            }
        }
        public void turnOn(pinInfo info) {
            GPIO.digitalWrite(info.gpioPin, ON);
            foreach (pinInfo pin in pinData) {
                if (pin.gpioPin == info.gpioPin) {
                    pin.status = ON;
                    break;
                }
            }
        }
        public void allOn() {
            foreach (pinInfo pin in pinData) {
                turnOn(pin);
            }
        }
    }
    public class pinInfo {
        public pinInfo (int status, int headerPin, int gpioPin, int branch) {
            this.status = status;
            this.headerPin = headerPin;
            this.gpioPin = gpioPin;
            this.branch = branch;
        }
        public int status { get; set; }
        public int headerPin { get; set; }
        public int gpioPin { get; set; }
        public int branch { get; set; }
    }
}
