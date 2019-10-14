using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Models.Animation {
    public class ColorList {
        private bool evaluated;
        private List<ColorValue> list;
        private Color[] evaluatedColors;

        public int Count => list.Count;

        /// <summary>
        /// Creates a new ColorList
        /// </summary>
        public ColorList() {
            list = new List<ColorValue>();
        }

        /// <summary>
        /// Creates a new ColorList given an array of colors
        /// </summary>
        /// <param name="colors">Array to convert to a colorlist</param>
        public ColorList(Color[] colors) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            list = new List<ColorValue>(colors.Length);
            list.AddRange(colors.Select(color => {
                return new ColorValue(color);
            }));
        }

        /// <summary>
        /// Creates a new ColorList given an array of random colors
        /// </summary>
        /// <param name="colors">Array to convert to a colorlist</param>
        public ColorList(RandomColor[] colors) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            list = new List<ColorValue>(colors.Length);
            list.AddRange(colors.Select(color => {
                return new ColorValue(color);
            }));
        }

        /// <summary>
        /// Adds a PrimitiveColor to the list
        /// </summary>
        /// <param name="color">primitive color to add</param>
        public void Add(Color color) {
            Add(new ColorValue(color));
        }

        /// <summary>
        /// Adds a RandomColor to the list
        /// </summary>
        /// <param name="color">randomcolor to add</param>
        public void Add(RandomColor color) {
            Add(new ColorValue(color));
        }

        /// <summary>
        /// Adds a colorvalue to the list
        /// </summary>
        /// <param name="value">colorvalue to add</param>
        public void Add(ColorValue value) {
            list.Add(value);
        }


        /// <summary>
        /// Evaluates any randomcolor objects to primitive colors and stores the resultant array
        /// </summary>
        public void Evaluate(int randomA, int randomB, int randomC) {
            if (!evaluated) {
                evaluatedColors = new Color[list.Count];
                for (int i = 0; i < list.Count; i++) {
                    ColorValue value = list[i];
                    if (!value.IsPrimitiveColor)
                        evaluatedColors[i] = value.RandomColor.Evaluate();
                    else
                        evaluatedColors[i] = value.PrimitiveColor;
                }
            }
        }

        /// <summary>
        /// Gets the primitive color array.
        /// </summary>
        /// <returns>Array of primitive colors</returns>
        /// <exception cref="InvalidOperationException">Thrown if Evaluate() has been called prior</exception>
        /// <seealso cref="Evaluate"/>
        public Color[] GetColors() {
            if (!evaluated) {
                throw new InvalidOperationException("ColorList must be evaluated before returning colors");
            }
            return evaluatedColors;
        }
    }
    public struct ColorValue {
        public bool IsPrimitiveColor;
        public Color PrimitiveColor;
        public RandomColor RandomColor;

        public ColorValue(Color PrimitiveColor) {
            this.PrimitiveColor = PrimitiveColor;
            IsPrimitiveColor = true;
            RandomColor = null;
        }

        public ColorValue(RandomColor randomColor) {
            this.RandomColor = randomColor;
            IsPrimitiveColor = false;
            PrimitiveColor = Color.Empty;
        }
    }
}
