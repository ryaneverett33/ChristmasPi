using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Serilog;

namespace ChristmasPi.Data.Models {
    public class RandomColor {
        // Delegate function that generates the entire color
        private Func<Color> generatorDelegate;
        private Func<object, Color> generatorDelegateWithState;
        private Func<int> rDelegate;       // Delegate function that generates the Red value of the color
        private Func<int> gDelegate;       // Delegate function that generates the Green value of the color
        private Func<int> bDelegate;       // Delegate function that generates the Blue value of the color
        private bool useGenerator;              // Whether or not to use the generator or component delegates
        private bool useState;                  // Whether or not to use generator supplied state info
        private object generatorStateObj;       // Generator specific state object to pass during generation
        private DelegateUsage usage;            // How to use the delegates

        /// <summary>
        /// Creates a new RandomColor object using a single delegate function
        /// </summary>
        /// <param name="generatorDelegate">The delegate function to create the random color</param>
        public RandomColor(Func<Color> generatorDelegate) {
            this.generatorDelegate = generatorDelegate;
            this.useGenerator = true;
            this.useState = false;
        }

        public RandomColor(Func<object, Color> generatorDelegateWithState, object stateObj) {
            this.generatorDelegateWithState = generatorDelegateWithState;
            this.generatorStateObj = stateObj;
            this.useGenerator = true;
            this.useState = true;
        }

        /// <summary>
        /// Creates a new RandomColor object using up to three delegate functions
        /// </summary>
        /// <param name="delegateR">The delegate function for the Red value</param>
        /// <param name="delegateG">The delegate function for the Green value</param>
        /// <param name="delegateB">The delegate function for the Blue value</param>
        /// <param name="usage">How to use the delegate function</param>
        /// <remarks>The DelegateUsage enum is a bitfield and can accept multiple values</remarks>
        public RandomColor(Func<int> delegateR, Func<int> delegateG, Func<int> delegateB, DelegateUsage usage) {
            if (delegateR == null && delegateG == null && delegateB == null)
                throw new ArgumentNullException();
            if (usage.HasFlag(DelegateUsage.RedDelegate) && delegateR == null)
                throw new ArgumentNullException("delegateA");
            if (usage.HasFlag(DelegateUsage.GreenDelegate) && delegateG == null)
                throw new ArgumentNullException("delegateB");
            if (usage.HasFlag(DelegateUsage.BlueDelegate) && delegateB == null)
                throw new ArgumentNullException("delegateC");
            this.rDelegate = delegateR;
            this.gDelegate = delegateG;
            this.bDelegate = delegateB;
            this.usage = usage;
            this.useGenerator = false;
            this.useState = false;
        }

        /// <summary>
        /// Creates a new RandomColor object using only one delegate function
        /// </summary>
        /// <param name="delegateFunc">The delegate function to use</param>
        /// <param name="usage">How to use the delegate function</param>
        /// <remarks>The DelegateUsage enum is a bitfield and can accept multiple values</remarks>
        public RandomColor(Func<int> delegateFunc, DelegateUsage usage) {
            if (delegateFunc == null)
                throw new ArgumentNullException("delegateFunc");
            if (usage.HasFlag(DelegateUsage.RedDelegate))
                rDelegate = delegateFunc;
            if (usage.HasFlag(DelegateUsage.GreenDelegate))
                gDelegate = delegateFunc;
            if (usage.HasFlag(DelegateUsage.BlueDelegate))
                bDelegate = delegateFunc;
            this.usage = usage;
            this.useGenerator = false;
            this.useState = false;
        }

        /// <summary>
        /// Generate a random table not used in a given table
        /// </summary>
        /// <param name="colors">List of previously used colors</param>
        /// <returns>A newly generated color</returns>
        public static Color RandomColorNotInTable(List<Color> colors, bool knownColor = false) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            Color color = knownColor ? RandomKnownColorGenerator() : MakeRandomColor();
            while (colors.Contains(color)) {
                color = knownColor ? RandomKnownColorGenerator() : MakeRandomColor();
            }
            return color;
        }

        /// <summary>
        /// Evaluates the current object and returns a primitive color
        /// </summary>
        /// <returns>A primitive color</returns>
        public Color Evaluate() {
            int r = -1, g = -1, b = -1;
            if (useGenerator) {
                if (useState)
                    return generatorDelegateWithState(generatorStateObj);
                else
                    return generatorDelegate();
            }
            else {
                if (usage.HasFlag(DelegateUsage.RedDelegate)) {
                    r = rDelegate();
                    if (r < Constants.RANDOM_MIN || r > Constants.RANDOM_MAX - 1)
                        Log.ForContext("ClassName", "RandomColor").Debug("Red delegate function returned values out of range and was clamped");
                    r = Math.Clamp(r, Constants.RANDOM_MIN, Constants.RANDOM_MAX - 1);
                }
                else if (usage.HasFlag(DelegateUsage.GreenDelegate)) {
                    g = gDelegate();
                    if (g < Constants.RANDOM_MIN || g > Constants.RANDOM_MAX - 1)
                        Log.ForContext("ClassName", "RandomColor").Debug("Green delegate function returned values out of range and was clamped");
                    g = Math.Clamp(g, Constants.RANDOM_MIN, Constants.RANDOM_MAX - 1);
                }
                else if (usage.HasFlag(DelegateUsage.BlueDelegate)) {
                    b = bDelegate();
                    if (b < Constants.RANDOM_MIN || g > Constants.RANDOM_MAX - 1)
                        Log.ForContext("ClassName", "RandomColor").Debug("Blue delegate function returned values out of range and was clamped");
                    b = Math.Clamp(b, Constants.RANDOM_MIN, Constants.RANDOM_MAX - 1);
                }
                r = r != -1 ? r : Constants.RANDOM_DEFAULT;
                g = g != -1 ? g : Constants.RANDOM_DEFAULT;
                b = b != -1 ? b : Constants.RANDOM_DEFAULT;
                return Color.FromArgb(255, r, g, b);
            }
        }

        /// <summary>
        /// Generates a new random primitive color
        /// </summary>
        /// <returns>A filled in color object</returns>
        public static Color MakeRandomColor() {
            return RandomColorGenerator();
        }

        /// <summary>
        /// Standard Function for generating random colors
        /// </summary>
        /// <seealso cref="MakeRandomColor"/>
        public static Func<Color> RandomColorGenerator = () => {
            int r = RandomGenerator.Instance.Number();
            int g = RandomGenerator.Instance.Number();
            int b = RandomGenerator.Instance.Number();
            return Color.FromArgb(255, r, g, b);
        };

        public static Func<Color> RandomKnownColorGenerator = () => {
            int num = RandomGenerator.Instance.Number(0, ColorTable.KnownColorTable.Count);
            return ColorTable.KnownColorTable[num];
        };

        public static Func<object, Color> RandomColorFromColorTable = (object stateObj) => {
            ColorTable table = (ColorTable)stateObj;
            return table[RandomGenerator.Instance.Number(0, table.Count)];
        };
    }
    /// <summary>
    /// Specifies how delegate functions should be used
    /// </summary>
    /// <remarks>This enum is a bitfield and thus can take multiple values</remarks>
    /// <example>DelegateUsage value = (DelegateUsage.RedDelegate | DelegateUsage.GreenDelegate)</example>
    [Flags]
    public enum DelegateUsage {
        RedDelegate = 1,
        GreenDelegate = 2,
        BlueDelegate = 4,
    }
}
