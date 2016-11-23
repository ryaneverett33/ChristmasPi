using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server {
    class Manager {
        //Objects
        Logger log;
        Animations animations;
        Thread workerThread;
        string[] names;
        Animations.current curr;
        bool play;
        bool workerAlive;
        public Manager(Logger log, Animations animations) {
            this.log = log;
            this.animations = animations;
            names = animations.getAnimations();
            curr = animations.curr;
            workerAlive = true;
            workerThread = new Thread(new ThreadStart(() => worker()));
            workerThread.Start();
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
            if (!animExists(name)) {
                Console.WriteLine("Animation Doesn't exist");
                return false;
            }
            if (!workerThread.IsAlive) {
                Console.WriteLine("WorkerThread is not alive");
                return false;
            }
            int id = animations.getAnimID(name);
            if (id == int.MinValue) {
                Console.WriteLine("Unable to play animation");
                return false;
            }
            curr.animID = id;
            curr.animName = name;
            Console.WriteLine("Manager: " + name + " is now playing");
            play = true;
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
            int id = 0;    //current playing animation id
            while (workerAlive) {
                //{"Twinkle","Toggle Each Branch","Random Branches","Staircase","Mirage","Binary","All Off"};
                if (play) {
                    switch (curr.animID) {
                        case 1:
                            //Console.WriteLine("Twinkle playing");
                            animations.twinkle();
                            break;
                        case 2:
                            //Console.WriteLine("Toggle Each Branch Playing");
                            animations.toggleeachbranch();
                            break;
                        case 3:
                            animations.flash();
                            break;
                        case 4:
                            //Console.WriteLine("Staircase Playing");
                            animations.staircase();
                            break;
                        case 5:
                            //Console.WriteLine("Mirage Playing");
                            animations.mirage();
                            break;
                        case 6:
                            //Console.WriteLine("Binary Playing");
                            animations.binary();
                            break;
                        case -1:
                            //Console.WriteLine("AllOff");
                            Animations.allOff();
                            Thread.Sleep(5000);
                            break;
                        case 0:
                            //Console.WriteLine("AllOn");
                            Animations.allOn();
                            Thread.Sleep(5000);
                            break;
                        case 10:
                            animations.kels();
                            break;
                        default:
                            //Console.WriteLine("Nothing Playing");
                            Thread.Sleep(5000);
                            break;
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
