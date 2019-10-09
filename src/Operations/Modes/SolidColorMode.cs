using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;

namespace ChristmasPi.Operations.Modes {
    public class SolidColorMode : IOperationMode, ISolidColorMode {
        #region Properties
        public string Name => "SolidColorMode";
        public Color CurrentColor => _currentColor;
        #endregion
        #region Fields
        private Color _currentColor;
        private IRenderer renderer;
        #endregion
        public SolidColorMode() {
            renderer = RenderFactory.GetRenderer();
        }
        #region IOperationMode Methods
        public void Activate() {
            renderer.Start();
            Console.WriteLine("Activated Solid Color Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            Console.WriteLine("Deactivate Solid Color Mode");
        }
        public object Info() {
            return new {
                currentcolor = CurrentColor
            };
        }
        #endregion
        #region Methods
        /// <summary>
        /// Sets the current color being shown
        /// </summary>
        /// <param name="newColor">The new color to show</param>
        public void SetColor(Color newColor) {
            _currentColor = newColor;
            renderer.SetAllLEDColors(_currentColor);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }
        #endregion
    }
}
