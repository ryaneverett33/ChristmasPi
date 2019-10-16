using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChristmasPi.Data.Models {
    public class ColorTable {
        private Dictionary<string, Color> colors;

        public int Count => colors.Count;
        public ColorTable(Tuple<string, Color>[] colors) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            this.colors = new Dictionary<string, Color>();
            foreach (Tuple<string, Color> colorpair in colors) {
                this.colors.Add(colorpair.Item1, colorpair.Item2);
            }
        }

        public Color this[string color] {
            get {
                if (!colors.ContainsKey(color))
                    throw new KeyNotFoundException("Color not in table");
                return colors[color];
            }
        }

        public Color this[int index] {
            get {
                if (index >= Count)
                    throw new IndexOutOfRangeException("index is out of range for the given color table");
                return colors.ElementAt(index).Value;
            }
        }

        private static ColorTable _knownColorTable;

        public static ColorTable KnownColorTable {
            get {
                if (_knownColorTable == null) {
                    List<Tuple<string, Color>> colors = new List<Tuple<string, Color>>();
                    string[] names = Enum.GetNames(typeof(KnownColor));
                    Color[] knownValues = (Color[])Enum.GetValues(typeof(KnownColor));
                    for (int i = 0; i < names.Length; i++) {
                        colors.Add(new Tuple<string, Color>(names[i], knownValues[i]));
                    }
                    _knownColorTable = new ColorTable(colors.ToArray());
                }
                return _knownColorTable;
            }
        }
    }
}