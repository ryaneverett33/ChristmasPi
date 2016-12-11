using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WiringPi;

namespace GpioTest {
    class Animations {
        public static int[] pins = { 3, 2, 0, 7, 6, 4 };
        public void Setup() {
            Console.WriteLine("Setting up");
            Init.WiringPiSetup();
            GPIO.pinMode(3, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(7, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(2, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(0, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(6, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(4, (int)GPIO.GPIOpinmode.Output);
        }
        public void twinkle() {
            Random rand = new Random();
            int r = rand.Next(6);
            //Console.WriteLine("Twinkle on branch: " + r);
            for (int i = 0; i < 6; i++) {
                if (i == r) {
                    turnOff(pins[i]);
                }
                else {
                    turnOn(pins[i]);
                }
            }
            Thread.Sleep(1500);
        }
        public void toggleeachbranch() {
            allOff();
            for (int i = 0; i < pins.Length; i++) {
                turnOn(pins[i]);
                Thread.Sleep(500);
                turnOff(pins[i]);
            }
        }
        public void flash() {
            allOn();
            Thread.Sleep(500);
            allOff();
            Thread.Sleep(500);
        }
        public void staircase() {
            allOff();
            bool isEven = (pins.Length % 2 == 0);
            int middle = pins.Length / 2;   //Don't add one
            for (int i = 0; i < pins.Length; i++) {
                int modI = Math.Abs(i - (pins.Length - 1));
                if (i == middle && !isEven) {
                    turnOn(pins[middle]);
                    Thread.Sleep(750);
                    turnOff(pins[middle]);
                }
                else {
                    turnOn(pins[i]);
                    turnOn(pins[modI]);
                    Thread.Sleep(750);
                    turnOff(pins[i]);
                    turnOff(pins[modI]);
                }
            }
        }
        public void mirage() {
            allOff();
            for (int i = 0; i < pins.Length; i++) {
                turnOn(pins[i]);
                //Console.WriteLine("pin: " + pins[i]);
                Thread.Sleep(750);
                turnOff(pins[i]);
            }
            for (int i = (pins.Length - 1); i > -1; i--) {
                turnOn(pins[i]);
                //Console.WriteLine("pin: " + pins[i]);
                Thread.Sleep(750);
                turnOff(pins[i]);
            }
        }
        public void binary() {
            allOff();
            int max = (int)Math.Pow(2,pins.Length);
            for (int i = 0; i < max; i++) {
                char[] dat = decimalToBinary(i);
                Console.WriteLine("Length: " + dat.Length);
                for (int c = 0; c < dat.Length; c++) {
                    //Dangerously not checking if dat.length and pin.length are the same
                    Console.Write(dat[c]);
                    if (dat[c] == '1') {
                        turnOn(pins[c]);
                    }
                    else {
                        turnOff(pins[c]);
                    }
                }
                Console.WriteLine();
                Thread.Sleep(2500);
            }
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
            int needed = pins.Length - binary.Length;
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
        public static void turnOn(int pin) {
            GPIO.digitalWrite(pin, 0);
        }
        public static void turnOff(int pin) {
            GPIO.digitalWrite(pin, 1);
        }
        public static void allOn() {
            for (int i = 0; i < pins.Length; i++) {
                turnOn(pins[i]);
            }
        }
        public static void allOff() {
            for (int i = 0; i < pins.Length; i++) {
                turnOff(pins[i]);
            }
        }
    }
}
