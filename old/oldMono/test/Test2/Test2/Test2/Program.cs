using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WiringPi;

namespace Test2 {
    class Program {
        static void Main(string[] args) {
            Init.WiringPiSetup();
            GPIO.pinMode(7, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(0, (int)GPIO.GPIOpinmode.Output);
            Thread.Sleep(1000);
            Console.WriteLine("Attempting to turn on GPIO");
            GPIO.digitalWrite(7, 1);
            Thread.Sleep(1000);
            Console.WriteLine("Attempting to turn off GPIO");
            GPIO.digitalWrite(7, 0);
            Thread.Sleep(500);
            Console.WriteLine("Blinking Pin 0");
            for (int i = 0; i < 5; i++) {
                GPIO.digitalWrite(0, 1);
                Thread.Sleep(500);
                GPIO.digitalWrite(0, 0);
                Thread.Sleep(500);
            }
            Console.WriteLine("Finished");
        }
    }
}
