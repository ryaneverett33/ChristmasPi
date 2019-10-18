using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Models.Animation {
    public class ColorList : IDisposable {
        private bool evaluated;
        private List<ColorValue> list;
        private Color[] evaluatedColors;            // Current list of evaluated colors
        private bool disposed;

        public int Count => list.Count;

        /// <summary>
        /// Creates a new ColorList
        /// </summary>
        public ColorList() {
            list = new List<ColorValue>();
            disposed = false;
            evaluated = false;
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

        public ColorValue this[int index] {
            get {
                if (index > Count)
                    throw new IndexOutOfRangeException();
                return list[index];
            }
        }

        public void SetColor(int index, Color color) {
            SetColor(index, new ColorValue(color));
        }
        public void SetColor(int index, RandomColor color) {
            SetColor(index, new ColorValue(color));
        }
        public void SetColor(int index, ColorValue color) {
            if (index > Count)
                throw new IndexOutOfRangeException();
            list[index] = color;
        }

        /// <summary>
        /// Evaluates any randomcolor objects to primitive colors and stores the resultant array
        /// </summary>
        /// <remarks>If Evaluate is called multiple times, the stored array will be the result of the last call</remarks>
        public void Evaluate() {
            if (!evaluated)
                evaluatedColors = new Color[list.Count];
            for (int i = 0; i < list.Count; i++) {
                ColorValue value = list[i];
                if (!value.IsPrimitiveColor)
                    evaluatedColors[i] = value.RandomColor.Evaluate();
                else
                    evaluatedColors[i] = value.PrimitiveColor;
            }
            evaluated = true;
        }

        /// <summary>
        /// Gets the primitive color array.
        /// </summary>
        /// <returns>Array of primitive colors</returns>
        /// <remarks>If not previously evaluated, Evaluate() is called before returnning colors</remarks>
        /// <seealso cref="Evaluate"/>
        public Color[] GetColors() {
            if (!evaluated) {
                Evaluate();
            }
            return evaluatedColors;
        }

        // return a list of colors but don't evaluate them
        public ColorValue[] GetNonEvaluatedColors() {
            return list.ToArray();
        }

        public void Dispose() {
            if (!disposed) {
                list.Clear();
                disposed = true;
            }
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
