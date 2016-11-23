using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server {
    class Animations {
        //Contains the animations and their respective info
        class anim {
            //Describes an individual animation
            //Same properties as the current structure
            public anim(int id, string name, int len) {
                animID = id;
                animName = name;
                animLength = len;
            }
            public int animID;
            public string animName;
            public int animLength;          //-1 represents No Length, and is ignored
        }
        public struct current {
            public int animID;          //The identifier for the animation
            public string animName;     //The name of the animation, properly formatted
            public int animLength;      //The time it takes to play an animation
        }
        List<anim> anims = new List<anim>();
        //Objects
        Gpio gpio;
        public current curr = new current();

        public Animations(Gpio gpio) {
            this.gpio = gpio;
            //Set Default anims
            anims.Add(new anim(1, "Twinkle", -1));
            anims.Add(new anim(2, "Toggle Each Branch", -1));
            anims.Add(new anim(3, "Flash", -1));
            anims.Add(new anim(4, "Staircase", -1));
            anims.Add(new anim(5, "Mirage", -1));
            anims.Add(new anim(6, "Binary", -1));
            gpio.allOn();
        }
        public string[] getAnimations() {
            List<string> names = new List<string>();
            foreach (anim a in anims) {
                names.Add(a.animName);
            }
            return names.ToArray();
        }
        public void twinkle() {
            setCurrent(0);
            //pick random branch, turn it off
            Random rand = new Random();
            int r = rand.Next(6);       //Exclusive random [0,5], Assuming 6 branches
            for (int b = 0; b < 6; b++) {
                if (b == r) {
                    gpio.turnOff(gpio.pinData[b]);
                }
                else {
                    gpio.turnOn(gpio.pinData[b]);
                }
            }
            Thread.Sleep(1500);
        }
        public void toggleeachbranch() {
            setCurrent(1);
            gpio.allOff();
            for (int t = 1; t < 6; t++) {
                gpio.turnOn(gpio.pinData[t]);
                Thread.Sleep(500);
                gpio.turnOff(gpio.pinData[t]);
            }
        }

        public void flash() {
            setCurrent(2);
            gpio.allOff();
            for (int t = 0; t < 6; t++) {
                gpio.turnOn(gpio.pinData[t]);
            }
            Thread.Sleep(500);
            for (int t = 0; t < 6; t++) {
                gpio.turnOff(gpio.pinData[t]);
            }
            Thread.Sleep(500);
        }
        public void staircase() {
            setCurrent(3);
            gpio.allOff();
            for (int i = 0; i < 3; i++) {
                int modI = 5 - i;
                gpio.turnOn(gpio.pinData[i]);
                gpio.turnOn(gpio.pinData[modI]);
                Thread.Sleep(750);
                gpio.turnOff(gpio.pinData[i]);
                gpio.turnOff(gpio.pinData[modI]);
            }
            for (int i = 2; i > -1; i--) {
                int modI = 5 - i;
                gpio.turnOn(gpio.pinData[i]);
                gpio.turnOn(gpio.pinData[modI]);
                Thread.Sleep(750);
                gpio.turnOff(gpio.pinData[i]);
                gpio.turnOff(gpio.pinData[modI]);
            }
        }
        public void mirage() {
            setCurrent(4);
            gpio.allOff();
            for (int i = 0; i < 6; i++) {
                gpio.turnOn(gpio.pinData[i]);
                Thread.Sleep(750);
                gpio.turnOff(gpio.pinData[i]);
            }
            for (int a = 5; a > -1; a--) {
                gpio.turnOn(gpio.pinData[a]);
                Thread.Sleep(750);
                gpio.turnOn(gpio.pinData[a]);
            }
        }
        public void binary() {
            setCurrent(5);
            gpio.allOff();
            int max = 64;   //2^(number of branches)
            for (int i = 0; i < max; i++) {
                char[] dat = decimalToBinary(i);
                for (int t = 0; t < 6; t++) {
                    if (dat[t] == null || (t+1) > dat.Length) {
                        gpio.turnOff(gpio.pinData[t]);
                    }
                    else {
                        if (dat[t] == '1') {
                            gpio.turnOn(gpio.pinData[t]);
                        }
                        else {
                            gpio.turnOff(gpio.pinData[t]);
                        }
                    }
                }
                Thread.Sleep(500);
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
                    char[] tmp = { '0' };
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
            return binary.ToCharArray();
        }
        private void setCurrent(int num) {
            if (num > -1 && num < anims.Count) {
                curr.animID = anims[num].animID;
                curr.animLength = anims[num].animLength;
                curr.animName = anims[num].animName;
            }
        }
        public void resetCurrent() {
            //Reset's the current struct to default values
            curr.animID = -1;
            curr.animLength = -1;
            curr.animName = null;
        }
    }
    class AnimationManager {
        //Objects
        Animations animations;
        Thread workerThread;
        string[] names;
        Animations.current curr;
        bool play;
        bool workerAlive;
        public AnimationManager(Animations animations) {
            this.animations = animations;
            names = animations.getAnimations();
            curr = animations.curr;
            workerAlive = true;
            /*workerThread = new Thread(new ThreadStart(() => worker()));
            workerThread.Start();*/
        }
        public string[] getAnimations() {
            return names;
        }
        private bool animExists(string name) {
            for (int i = 0; i < names.Length; i++) {
                if (names[i].Equals(name)) {
                    return true;
                }
            }
            return false;
        }
        public bool playAnimation(string name) {
            if (!animExists(name)) return false;
            curr.animName = name;
            if (!workerAlive) {
                return false;
            }
            return true;
        }
        public bool stopAnimation() {
            if (!workerAlive) return false;
            play = false;
            return true;
        }
        private void worker() {
            Thread animThread = null;
            int id = -1;    //current playing animation id
            while (workerAlive) {
                if (play) {
                    if (curr.animName.Equals("Twinkle")) {
                        if (id != 0 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.twinkle()));
                            animThread.Start();
                            id = 0;
                        }
                        Thread.Sleep(1000);
                    }
                    else if (curr.animName.Equals("Toggle Each Branch")) {
                        if (id != 1 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.toggleeachbranch()));
                            animThread.Start();
                            id = 1;
                        }
                        Thread.Sleep(1000);
                    }
                    else if (curr.animName.Equals("Flash")) {
                        if (id != 2 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.flash()));
                            animThread.Start();
                            id = 2;
                        }
                        Thread.Sleep(1000);
                    }
                    else if (curr.animName.Equals("Staircase")) {
                        if (id != 3 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.staircase()));
                            animThread.Start();
                            id = 3;
                        }
                        Thread.Sleep(1000);
                    }
                    else if (curr.animName.Equals("Mirage")) {
                        if (id != 4 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.mirage()));
                            animThread.Start();
                            id = 4;
                        }
                        Thread.Sleep(1000);
                    }
                    else if (curr.animName.Equals("Binary")) {
                        if (id != 5 && id != -1) { animThread.Abort(); }            //Another animation is playing, kill it
                        if (animThread == null || !animThread.IsAlive) {
                            animThread = new Thread(new ThreadStart(() => animations.binary()));
                            animThread.Start();
                            id = 5;
                        }
                        Thread.Sleep(1000);
                    }
                }
                else {
                    if (animThread != null) {
                        if (animThread.IsAlive) {
                            animThread.Abort();
                        }
                        Thread.Sleep(1000);
                    }
                    
                }
            }
        }
    }
}
