using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server {
    class anim {
        // Describes an animation
        public anim(int[][] steps, int timeStep, int ID, string name) {
            this.steps = steps;
            this.timeStep = timeStep;
            this.ID = ID;
            this.name = name;
            branches = steps[0].Length;
            stepCount = steps.Length;
        }
        public int[][] steps;           //The animation data itself
        public int stepCount;
        public int branches;
        public int timeStep;            //The amount of ms a step is active
        public int ID;
        public string name;             //The formatted name of the animation
    }
    class AnimationManager {
        List<anim> animations = new List<anim>();
        public struct current {
            public int animID;          //The identifier for the animation
            public string animName;     //The name of the animation, properly formatted
            public int animLength;      //The time it takes to play an animation
            public int animStep;        //Number of steps in animation
        }
        public bool isPlaying;

        //Objects
        Logger log;
        Gpio gpio;

        public AnimationManager(Logger log, Gpio gpio) {
            this.log = log;
            this.gpio = gpio;
            isPlaying = false;
        }
        public bool playAnim(string name) {
            int index = -1;
            for (int i = 0; i < animations.Count; i++) {
                anim foo;
                try {
                    foo = animations.ElementAt(i);
                }
                catch (Exception e) {
                    log.logError("Could not get animation at index: " + i);
                    continue;
                }
                if (foo.name.Equals(name, StringComparison.CurrentCultureIgnoreCase) {
                    index = i;
                    break;
                }
            }
            //if (index == -1)
            //TODO
        }
        protected void play(int index) {
            //Only callable by playAnim() method
            //Plays an animation given an index
            if (index > animations.Count || index < 0) {
                play(0);    //Revert to default animation
            }
            else {
                isPlaying = true;
                current cur = new current();
                anim play = animations.ElementAt(index);
                cur.animID = play.ID;
                cur.animName = play.name;
                cur.animStep = play.stepCount;
                cur.animLength = play.stepCount * play.timeStep;
                for (int i = 0; i < play.stepCount; i++) {
                    for (int j = 0; j < play.branches; j++) {
                        int value = -1;
                        try {
                            value = play.steps[i][j]
                        }
                        catch (Exception e) {
                            log.logError("Step invalid, could not play step. Trying next step");
                            continue;
                        }
                        if (value == 1) {
                            // Turn on branch at j
                            gpio.turnOn(gpio.pinData.ElementAt(j));
                        }
                        else if (value == 0) {
                            // Turn off branch at j
                            gpio.turnOff(gpio.pinData.ElementAt(j));
                        }
                    }
                    Thread.Sleep(play.timeStep);
                }
            }
        } 
    }
}
