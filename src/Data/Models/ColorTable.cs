using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace ChristmasPi.Data.Models {
    public class ColorTable {
        private Dictionary<string, Color> colors;

        /// <summary>
        /// Number of colors defined in this table
        /// </summary>
        public int Count => colors.Count;

        /// <summary>
        /// Creates a new ColorTable object with the given color array
        /// </summary>
        /// <param name="colors">Array of colors and their names</param>
        public ColorTable(Tuple<string, Color>[] colors) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            this.colors = new Dictionary<string, Color>(colors.Length);
            foreach (Tuple<string, Color> colorpair in colors) {
                this.colors.Add(colorpair.Item1, colorpair.Item2);
            }
        }

        /// <summary>
        /// Creates a new ColorTable object and assigns each color a new name
        /// </summary>
        /// <param name="colors">Array of colors</param>
        public ColorTable(Color[] colors) {
            if (colors == null)
                throw new ArgumentNullException("colors");
            this.colors = new Dictionary<string, Color>(colors.Length);
            for (int i = 0; i < colors.Length; i++) {
                this.colors.Add($"Color-{i + 1}", colors[i]);
            }
        }

        /// <summary>
        /// Gets a color by its name
        /// </summary>
        /// <param name="name">Name of the color to get</param>
        /// <returns>The requested color</returns>
        public Color this[string name] {
            get {
                if (!colors.ContainsKey(name))
                    throw new KeyNotFoundException("Color not in table");
                return colors[name];
            }
        }

        /// <summary>
        /// Gets a color at the specified index, used for getting random colors
        /// </summary>
        /// <param name="index">Index at which to get the color from</param>
        /// <returns>Color at the given index</returns>
        public Color this[int index] {
            get {
                if (index >= Count)
                    throw new IndexOutOfRangeException("index is out of range for the given color table");
                return colors.ElementAt(index).Value;
            }
        }

        #region Premade ColorTables
        private static ColorTable _knownColorTable;
        private static ColorTable _christmasColors1;

        /// <summary>
        /// Creates a new ColorTable with the defined colors in the Color struct
        /// </summary>
        /// <remarks>Filters out colors like Name and IsSystemColor</remarks>
        /// <source>https://stackoverflow.com/a/3821197</source>
        public static ColorTable KnownColorTable {
            get {
                if (_knownColorTable == null) {
                    List<Tuple<string, Color>> colors = new List<Tuple<string, Color>>();
                    Type colorType = typeof(Color);
                    PropertyInfo[] propertyInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
                    foreach (PropertyInfo info in propertyInfos) {
                        colors.Add(new Tuple<string, Color>(info.Name, Color.FromName(info.Name)));
                    }
                    _knownColorTable = new ColorTable(colors.ToArray());
                }
                return _knownColorTable;
            }
        }

        /// <summary>
        /// A Collection of Christmas Colors
        /// </summary>
        /// <source>https://blog.psprint.com/designing/30-cool-christmas-color-palettes/</source>
        public static ColorTable ChristmasColors1 {
            get {
                if (_christmasColors1 == null) {
                    List<Color> colors = new List<Color>();
                    colors.Add(Util.ColorConverter.Convert("#213496"));
                    colors.Add(Util.ColorConverter.Convert("#6f89fc"));
                    colors.Add(Util.ColorConverter.Convert("#9eaefd"));
                    colors.Add(Util.ColorConverter.Convert("#9eaefd"));
                    colors.Add(Util.ColorConverter.Convert("#6ac5fd"));
                    colors.Add(Util.ColorConverter.Convert("#bae3fe"));
                    colors.Add(Util.ColorConverter.Convert("#a16dfc"));
                    colors.Add(Util.ColorConverter.Convert("#c09dfd"));
                    colors.Add(Util.ColorConverter.Convert("#feb9fd"));
                    colors.Add(Util.ColorConverter.Convert("#fd9dfd"));
                    _christmasColors1 = new ColorTable(colors.ToArray());
                }
                return _christmasColors1;
            }
        }
        #endregion
    }
}