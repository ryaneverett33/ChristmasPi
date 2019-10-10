using System;

namespace ChristmasPi.Util {
    public static class Floats {
        public static bool EqualsDelta(this float a, float b, float delta) {
            float result = 0;
            if (b > a)
                result = b - a;
            else if (a > b)
                result = a - b;
            if (Math.Abs(result) <= delta)
                return true;
            else
                return false;
        }
    }
}