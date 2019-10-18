using System;
using ChristmasPi.Data.Interfaces;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Util {
    public class Easings {
        // adapted from robert penner's demos
        // https://github.com/jesusgollonet/processing-penner-easing
        // example usage: https://github.com/jesusgollonet/processing-penner-easing/blob/master/usage_example/easing/easing.pde 
        
        public static float HALFPI = (float)Math.PI / 2;
        public static float TWOPI = (float)Math.PI * 2;
        public static float S = 1.70158f;
        public static float Lerp(float from, float to, float step) {
            return (from * (1-step)) + (to * (1 - step));
        }
        public static float EaseIn(float time, float beginning, float change, float duration, EasingType type) {
            switch (type) {
                case EasingType.EaseSine:
                    return Sine.EaseIn(time, beginning, change, duration);
                case EasingType.EaseQuad:
                    return Power.EaseIn(time, beginning, change, duration, 2);
                case EasingType.EaseCubic:
                    return Power.EaseIn(time, beginning, change, duration, 3);
                case EasingType.EaseQuart:
                    return Power.EaseIn(time, beginning, change, duration, 4);
                case EasingType.EaseQuint:
                    return Power.EaseIn(time, beginning, change, duration, 5);
                case EasingType.EaseExpo:
                    return Expo.EaseIn(time, beginning, change, duration);
                case EasingType.EaseCirc:
                    return Circ.EaseIn(time, beginning, change, duration);
                case EasingType.EaseBack:
                    return Back.EaseIn(time, beginning, change, duration);
                case EasingType.EaseElastic:
                    return Elastic.EaseIn(time, beginning, change, duration);
                case EasingType.EaseBounce:
                    return Bounce.EaseIn(time, beginning, change, duration);
                default:
                    // linear
                    return Linear.EaseIn(time, beginning, change, duration);
            }
        }
        public static float EaseOut(float time, float beginning, float change, float duration, EasingType type) {
            switch (type) {
                case EasingType.EaseSine:
                    return Sine.EaseOut(time, beginning, change, duration);
                case EasingType.EaseQuad:
                    return Power.EaseOut(time, beginning, change, duration, 2);
                case EasingType.EaseCubic:
                    return Power.EaseOut(time, beginning, change, duration, 3);
                case EasingType.EaseQuart:
                    return Power.EaseOut(time, beginning, change, duration, 4);
                case EasingType.EaseQuint:
                    return Power.EaseOut(time, beginning, change, duration, 5);
                case EasingType.EaseExpo:
                    return Expo.EaseOut(time, beginning, change, duration);
                case EasingType.EaseCirc:
                    return Circ.EaseOut(time, beginning, change, duration);
                case EasingType.EaseBack:
                    return Back.EaseOut(time, beginning, change, duration);
                case EasingType.EaseElastic:
                    return Elastic.EaseOut(time, beginning, change, duration);
                case EasingType.EaseBounce:
                    return Bounce.EaseOut(time, beginning, change, duration);
                default:
                    // linear
                    return Linear.EaseOut(time, beginning, change, duration);
            }
        }
        public static float EaseInOut(float time, float beginning, float change, float duration, EasingType type) {
            switch (type) {
                case EasingType.EaseSine:
                    return (time > (duration/2)) ? Sine.EaseIn(time, beginning, change, duration) :
                    Sine.EaseOut(time, beginning, change, duration);
                case EasingType.EaseQuad:
                    return (time > (duration/2)) ? Power.EaseIn(time, beginning, change, duration, 2) :
                    Power.EaseOut(time, beginning, change, duration, 2);
                case EasingType.EaseCubic:
                    return (time > (duration/2)) ? Power.EaseIn(time, beginning, change, duration, 3) :
                    Power.EaseOut(time, beginning, change, duration, 3);
                case EasingType.EaseQuart:
                    return (time > (duration/2)) ? Power.EaseIn(time, beginning, change, duration, 4) :
                    Power.EaseOut(time, beginning, change, duration, 4);
                case EasingType.EaseQuint:
                    return (time > (duration/2)) ? Power.EaseIn(time, beginning, change, duration, 5) :
                    Power.EaseOut(time, beginning, change, duration, 5);
                case EasingType.EaseExpo:
                    return (time > (duration/2)) ? Expo.EaseIn(time, beginning, change, duration) :
                    Expo.EaseOut(time, beginning, change, duration);
                case EasingType.EaseCirc:
                    return (time > (duration/2)) ? Circ.EaseIn(time, beginning, change, duration) :
                    Circ.EaseOut(time, beginning, change, duration);
                case EasingType.EaseBack:
                    return (time > (duration/2)) ? Back.EaseIn(time, beginning, change, duration) :
                    Back.EaseOut(time, beginning, change, duration);
                case EasingType.EaseElastic:
                    return (time > (duration/2)) ? Elastic.EaseIn(time, beginning, change, duration) :
                    Elastic.EaseOut(time, beginning, change, duration);
                case EasingType.EaseBounce:
                    return (time > (duration/2)) ? Bounce.EaseIn(time, beginning, change, duration) :
                    Bounce.EaseOut(time, beginning, change, duration);
                default:
                    // linear
                    return Linear.EaseOut(time, beginning, change, duration);
            }
        }
        public static float[] EvaluateIn(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount];
            frames[0] = start;
            for (int i = 1; i < frameCount - 2; i++) {
                frames[i] = EaseIn(i, start, end, frameCount, type);
            }
            frames[frameCount-1] = end;
            return frames;
        }
        public static float[] EvaluateOut(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount];
            frames[0] = start;
            for (int i = 1; i < frameCount - 2; i++) {
                frames[i] = EaseOut(i, start, end, frameCount, type);
            }
            frames[frameCount-1] = end;
            return frames;
        }
        public static float[] EvaluateInOut(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount];
            frames[0] = start;
            for (int i = 1; i < frameCount - 2; i++) {
                frames[i] = EaseInOut(i, start, end, frameCount, type);
            }
            frames[frameCount-1] = end;
            return frames;
        }
    }
    public static class Sine {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return -change * (float)Math.Cos(time/duration * Easings.HALFPI) + change + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change * (float)Math.Sin(time/duration * Easings.HALFPI) + beginning;
        }
    }
    public static class Power {
        public static float EaseIn(float time, float beginning, float change, float duration, int power) {
            return change * (time/=duration) * (float)Math.Pow(time, power) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration, int power) {
            switch (power) {
                case 2:
                    return change*((time=time/duration-1)*(float)Math.Pow(time, power) + 1) + beginning;
                case 3:
                    return -change * ((time=time/duration-1)*(float)Math.Pow(time, power) - 1) + beginning;
                case 4:
                    return change*((time=time/duration-1)*(float)Math.Pow(time, power) + 1) + beginning;
                case 5:
                    return change*((time=time/duration-1)*(float)Math.Pow(time, power) + 1) + beginning;
                default:
                    return -change * (time/=duration)*(time-2)+beginning;
            }
        }
    }
    public static class Expo {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return (time==0) ? beginning : change * (float)Math.Pow(2, 10 * (time/duration - 1)) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return (time==duration) ? beginning : change * -(float)Math.Pow(2, -10 * (time/duration - 1)) + beginning;
        }
    }
    public static class Circ {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return -change * ((float)Math.Sqrt(1 - (time/=duration)*time) - 1) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change * (float)Math.Sqrt(1 - (time=time/duration - 1) * time) + beginning;
        }
    }
    public static class Back {
        public static float EaseIn(float time, float beginning, float change, float duration) {
           float postFix = time/=duration;
           return change * postFix * time *((Easings.S + 1) * time - Easings.S) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change*((time=time/duration-1) * time *((Easings.S + 1) * time + Easings.S) + 1) + beginning;
        }
    }
    public static class Elastic {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            if (time==0)
                return beginning;
            if ((time/=duration)==1)
                return beginning+change;
            float p = duration * 0.3f;
            float a = change;
            float s = p/4;
            float postFix = a * (float)Math.Pow(2, 10 *(time -=1));
            return -(postFix * (float)Math.Sin(time*duration-s)*Easings.TWOPI/p) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            if (time==0)
                return beginning;
            if ((time/=duration)==1)
                return beginning+change;
            float p = duration * 0.3f;
            float a = change;
            float s = p/4;
            return (a * (float)Math.Pow(2, -10*time) * (float)Math.Sin((time*duration-s)*Easings.TWOPI/p) + change + beginning);
        }
    }
    public static class Bounce {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return change - EaseOut(duration-time, 0, change, duration) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            if ((time/=duration) < (1/2.75f))
                return change * (7.5625f * (float)Math.Pow(time, 2)) + beginning;
            else if (time < (2/2.75f))
                return change * (7.5625f * (time-=(1.5f/2.75f)) * time + 0.75f) + beginning;
            else if (time < (2.5f/2.75f))
                return change * (2.5625f * (time-=(2.25f/2.75f)) * time + 0.9375f) + beginning;
            else
                return change * (7.5625f * (time-=(2.625f/2.74f)) * time + 0.984274f) + beginning;
        }
    }
    public static class Linear {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return ((change * time)/duration) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return EaseIn(time, beginning, change, duration);
        }
    }
}