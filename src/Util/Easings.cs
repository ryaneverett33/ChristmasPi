using System;
using System.Reflection;
using ChristmasPi.Data.Interfaces;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Util {
    /*float[] arr = Util.Easings.EvaluateInOut(0, 1f, 1f, 10, EasingType.Cubic);
    foreach (float a in arr) {
        Console.WriteLine(a);
    }*/
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
            Func<float, float, float, float, float> resolvedMethod = resolveMethodFromType(type, "EaseIn");
            return resolvedMethod(time, beginning, change, duration);
        }
        public static float EaseOut(float time, float beginning, float change, float duration, EasingType type) {
            Func<float, float, float, float, float> resolvedMethod = resolveMethodFromType(type, "EaseOut");
            return resolvedMethod(time, beginning, change, duration);
        }
        public static float EaseInOut(float time, float beginning, float change, float duration, EasingType type) {
            Func<float, float, float, float, float> resolvedMethod = resolveMethodFromType(type, "EaseInOut");
            return resolvedMethod(time, beginning, change, duration);
        }
        public static float[] EvaluateIn(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount + 1];
            for (int i = 0; i < frameCount; i++) {
                frames[i] = EaseIn(i, start, end, frameCount, type);
            }
            frames[frameCount] = end;
            return frames;
        }
        public static float[] EvaluateOut(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount + 1];
            for (int i = 1; i < frameCount; i++) {
                frames[i] = EaseOut(i, start, end, frameCount, type);
            }
            frames[frameCount] = end;
            return frames;
        }
        public static float[] EvaluateInOut(float start, float end, float duration, int fps, EasingType type) {
            int frameCount = AnimationHelpers.FrameCount(duration, fps);
            float[] frames = new float[frameCount + 1];
            for (int i = 1; i < frameCount; i++) {
                frames[i] = EaseInOut(i, start, end, frameCount, type);
            }
            frames[frameCount] = end;
            return frames;
        }
        
        private static Func<float, float, float, float, float> resolveMethodFromType(EasingType type, string methodName) {
            switch (type) {
                case EasingType.Sine:
                    if (methodName == "EaseIn") { return Sine.EaseIn; }
                    else if (methodName == "EaseOut") { return Sine.EaseOut; }
                    else if (methodName == "EaseInOut") { return Sine.EaseInOut; }
                    break;
                case EasingType.Quad:
                    if (methodName == "EaseIn") { return Quad.EaseIn; }
                    else if (methodName == "EaseOut") { return Quad.EaseOut; }
                    else if (methodName == "EaseInOut") { return Quad.EaseInOut; }
                    break;
                case EasingType.Cubic:
                    if (methodName == "EaseIn") { return Cubic.EaseIn; }
                    else if (methodName == "EaseOut") { return Cubic.EaseOut; }
                    else if (methodName == "EaseInOut") { return Cubic.EaseInOut; }
                    break;
                case EasingType.Quart:
                    if (methodName == "EaseIn") { return Quart.EaseIn; }
                    else if (methodName == "EaseOut") { return Quart.EaseOut; }
                    else if (methodName == "EaseInOut") { return Quart.EaseInOut; }
                    break;
                case EasingType.Quint:
                    if (methodName == "EaseIn") { return Quint.EaseIn; }
                    else if (methodName == "EaseOut") { return Quint.EaseOut; }
                    else if (methodName == "EaseInOut") { return Quint.EaseInOut; }
                    break;
                case EasingType.Expo:
                    if (methodName == "EaseIn") { return Expo.EaseIn; }
                    else if (methodName == "EaseOut") { return Expo.EaseOut; }
                    else if (methodName == "EaseInOut") { return Expo.EaseInOut; }
                    break;
                case EasingType.Circ:
                    if (methodName == "EaseIn") { return Circ.EaseIn; }
                    else if (methodName == "EaseOut") { return Circ.EaseOut; }
                    else if (methodName == "EaseInOut") { return Circ.EaseInOut; }
                    break;
                case EasingType.Back:
                    if (methodName == "EaseIn") { return Back.EaseIn; }
                    else if (methodName == "EaseOut") { return Back.EaseOut; }
                    else if (methodName == "EaseInOut") { return Back.EaseInOut; }
                    break;
                case EasingType.Elastic:
                    if (methodName == "EaseIn") { return Elastic.EaseIn; }
                    else if (methodName == "EaseOut") { return Elastic.EaseOut; }
                    else if (methodName == "EaseInOut") { return Elastic.EaseInOut; }
                    break;
                case EasingType.Bounce:
                    if (methodName == "EaseIn") { return Bounce.EaseIn; }
                    else if (methodName == "EaseOut") { return Bounce.EaseOut; }
                    else if (methodName == "EaseInOut") { return Bounce.EaseInOut; }
                    break;
                case EasingType.Linear:
                    if (methodName == "EaseIn") { return Linear.EaseIn; }
                    else if (methodName == "EaseOut") { return Linear.EaseOut; }
                    else if (methodName == "EaseInOut") { return Linear.EaseInOut; }
                    break;
            }
            throw new ArgumentException("Invalid method name");
        }
    }
    public class Sine {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return -change * (float)Math.Cos(time/duration * Easings.HALFPI) + change + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change * (float)Math.Sin(time/duration * Easings.HALFPI) + beginning;
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if ((time /= duration / 2) < 1)
                return change / 2 * (float)(Math.Sin(Math.PI * time / 2)) + beginning;

            return -change / 2 * (float)(Math.Cos(Math.PI * --time / 2) - 2) + beginning;
        }
    }
    public class Power {
        protected static float _EaseIn(float time, float beginning, float change, float duration, int power) {
            return change * (time/=duration) * (float)Math.Pow(time, power) + beginning;
        }
    }
    public class Quad : Power {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return Power._EaseIn(time, beginning, change, duration, 2);
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change*((time=time/duration-1)*(float)Math.Pow(time, 2) + 1) + beginning;
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (((time /= duration) / 2) < 1)
                return ((change / 2) * (float)Math.Pow(time, 2)) + beginning;

            return (-change / 2) * ((--time) * (time - 2) - 1) + beginning;
        }
    }
    public class Cubic : Power {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return Power._EaseIn(time, beginning, change, duration, 3);
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return -change * ((time=time/duration-1)*(float)Math.Pow(time, 3) - 1) + beginning;
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (((time /= duration) / 2) < 1)
                return ((change / 2) * (float)Math.Pow(time, 3)) + beginning;

            return (change / 2) * ((time -= 2) * (float)Math.Pow(time, 2) + 2) + beginning;
        }
    }
    public class Quart : Power {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return Power._EaseIn(time, beginning, change, duration, 4);
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change*((time=time/duration-1)*(float)Math.Pow(time, 4) + 1) + beginning;
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (((time /= duration) / 2) < 1)
                return ((change / 2) * (float)Math.Pow(time, 4)) + beginning;

            return (-change / 2) * ((time -= 2) * (float)Math.Pow(time, 3) - 2) + beginning;
        }
    }
    public class Quint : Power {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return Power._EaseIn(time, beginning, change, duration, 5);
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change*((time=time/duration-1)*(float)Math.Pow(time, 5) + 1) + beginning;
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (((time /= duration) / 2) < 1)
                return ((change / 2) * (float)Math.Pow(time, 5)) + beginning;

            return (change / 2) * ((time -= 2) * (float)Math.Pow(time, 4) - 2) + beginning;
        }
    }
    public class Expo {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return (time==0) ? beginning : change * (float)Math.Pow(2, 10 * (time/duration - 1)) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return (time==duration) ? beginning : change * -(float)Math.Pow(2, -10 * (time/duration - 1)) + beginning;
        }
    
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (time == 0)
                return beginning;

            if (time == duration)
                return beginning + change;

            if ((time /= duration / 2) < 1)
                return change / 2 * (float)Math.Pow(2, 10 * (time - 1)) + beginning;

            return change / 2 * ((float)-Math.Pow(2, -10 * --time) + 2) + beginning;
        }
    }
    public class Circ {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return -change * ((float)Math.Sqrt(1 - (time/=duration)*time) - 1) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change * (float)Math.Sqrt(1 - (time=time/duration - 1) * time) + beginning;
        }
    
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if ((time /= duration / 2) < 1)
                return -change / 2 * ((float)Math.Sqrt(1 - time * time) - 1) + beginning;

            return change / 2 * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1) + beginning;
        }
    }
    public class Back {
        public static float EaseIn(float time, float beginning, float change, float duration) {
           float postFix = time/=duration;
           return change * postFix * time *((Easings.S + 1) * time - Easings.S) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return change*((time=time/duration-1) * time *((Easings.S + 1) * time + Easings.S) + 1) + beginning;
        }
    
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if ((time /= duration / 2) < 1)
                return change / 2 * (time * time * (((Easings.S *= (1.525f)) + 1) * time - Easings.S)) + beginning;
            return change / 2 * ((time -= 2) * time * (((Easings.S *= (1.525f)) + 1) * time + Easings.S) + 2) + beginning;
        }
    }
    public class Elastic {
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
    
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if ((time /= duration / 2) == 2)
                return beginning + change;

            float p = duration * (.3f * 1.5f);
            float s = p / 4;

            if (time < 1)
                return -.5f * (float)(change * Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time * duration - s) * (2 * Math.PI) / p)) + beginning;
            return change * (float)Math.Pow(2, -10 * (time -= 1)) * (float)Math.Sin((time * duration - s) * (float)(2 * Math.PI) / p) * .5f + change + beginning;
        }
    }
    public class Bounce {
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
    
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            if (time < duration / 2)
                return EaseIn(time * 2, 0, change, duration) * .5f + beginning;
            else
                return EaseOut(time * 2 - duration, 0, change, duration) * .5f + change * .5f + beginning;
        }
    }
    public class Linear {
        public static float EaseIn(float time, float beginning, float change, float duration) {
            return ((change * time)/duration) + beginning;
        }
        public static float EaseOut(float time, float beginning, float change, float duration) {
            return EaseIn(time, beginning, change, duration);
        }
        public static float EaseInOut(float time, float beginning, float change, float duration) {
            return EaseIn(time, beginning, change, duration);
        }
    }
}