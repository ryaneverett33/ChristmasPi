using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Server;
using WiringPi;

namespace GpioTest {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Beginning Test");
            Program p = new Program();
            Animations a = new Animations();
            a.Setup();
            Console.WriteLine("Twinkling");
            a.twinkle();
            Console.WriteLine("Toggle Each Branch");
            a.toggleeachbranch();
            Console.WriteLine("Staircase");
            a.staircase();
            Console.WriteLine("Mirage");
            a.mirage();
            Console.WriteLine("Binary");
            a.binary();
            Console.WriteLine("Finished Test");
            /*Gpio gpio = new Gpio();
            Animations animations = new Animations(gpio);
            Console.WriteLine("Flash Animation");
            animations.flash();
            Console.WriteLine("Twinkle Animation");
            animations.twinkle();
            Console.WriteLine("Twinkle Animation");
            animations.twinkle();
            Console.WriteLine("Twinkle Animation");
            animations.twinkle();
            Console.WriteLine("Staircase Animation");
            animations.staircase();*/
        }
    }
}
