using System;

namespace ChristmasPi.Data {
    public class RandomGenerator {
        #region Singleton Methods
        private static readonly RandomGenerator _instance = new RandomGenerator();
        public static RandomGenerator Instance { get { return _instance; } }
        #endregion
        Random random;
        public RandomGenerator() {
            random = new Random();
        }
        public int Number() {
            return random.Next(Constants.RANDOM_MIN, Constants.RANDOM_MAX);
        }

        public int Number(int min, int max) {
            return random.Next(min, max);
        }
    }
}
