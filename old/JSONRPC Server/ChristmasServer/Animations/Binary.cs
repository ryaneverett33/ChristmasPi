using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class Binary : BaseAnimation, IAnimation {
        public Binary(Gpio gpio) {
            this.gpio = gpio;
        }
        public void playAnimation() {
            gpio.AllOff();
            int max = (int)Math.Pow(2, gpio.pins.Length);
            for (int i = 0; i < max; i++) {
                char[] dat = decimalToBinary(i);
                //Console.WriteLine("Length: " + dat.Length);
                for (int c = 0; c < dat.Length; c++) {
                    //Dangerously not checking if dat.length and pin.length are the same
                    //Console.Write(dat[c]);
                    if (dat[c] == '1') {
                        gpio.turnOn(gpio.pins[c].gpioPin);
                    }
                    else {
                        gpio.turnOff(gpio.pins[c].gpioPin);
                    }
                }
                //Console.WriteLine();
                Thread.Sleep(2500);
            }
        }
        public int getBranchCount() {
            return 6;
        }
        public int getLength() {
            return 0;
        }
        private char[] decimalToBinary(int dec) {
            //Basic function that converts decimals (integers) to binary strings and returns them as character arrays
            string binary = "";
            string currentBinary = "";
            int currentInt = dec;
            int currentRemainder = 0;

            for (int i = 0; i < 1000; i++) {
                if (dec == 0) {
                    char[] tmp = { '0', '0', '0', '0', '0', '0' };
                    return tmp;
                }
                if (dec == 1) {
                    char[] tmp = { '0', '0', '0', '0', '0', '1' };
                    return tmp;
                }
                if (i == 0) {
                    currentInt = dec / 2;
                    currentRemainder = dec % 2;
                }
                else {
                    int tmpVar = currentInt;
                    currentInt = tmpVar / 2;
                    currentRemainder = tmpVar % 2;
                }
                currentBinary = currentRemainder.ToString();

                binary = currentBinary + binary;
                if (currentInt == 1) {
                    binary = "1" + binary;
                    break;
                }
            }
            //Check if binary is the right length
            int needed = gpio.pins.Length - binary.Length;
            if (needed == 0) {
                return binary.ToCharArray();
            }
            else {
                char[] leading = new char[needed];
                for (int i = 0; i < leading.Length; i++) {
                    leading[i] = '0';   //add leading zeros;
                }
                string tmp = new string(leading);
                binary = tmp + binary;      //Lazy way of adding two char[]'s
                return binary.ToCharArray();
            }
        }
    }
}
