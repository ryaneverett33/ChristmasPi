using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using Microsoft.AspNetCore.Http;

namespace ChristmasPi.Operations.Modes {
    public class SolidColorMode : IOperationMode, ISolidColorMode {
        #region Properties
        public string Name => "SolidColorMode";
        public bool CanBeDefault => true;
        public Color CurrentColor => _currentColor;
        #endregion
        #region Fields
        private Color _currentColor;
        private IRenderer renderer;
        #endregion
        public SolidColorMode() {
            _currentColor = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor;
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            renderer = RenderFactory.GetRenderer();
            renderer.Start();
            SetColor(_currentColor);
            Console.WriteLine("Activated Solid Color Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            Console.WriteLine("Deactivated Solid Color Mode");
            renderer = null;
        }
        public object Info() {
            return new {
                currentcolor = CurrentColor
            };
        }
        public object GetProperty(string property) {
            if (property.Equals(nameof(Name), StringComparison.CurrentCultureIgnoreCase))
                return Name;
            else if (property.Equals(nameof(CurrentColor), StringComparison.CurrentCultureIgnoreCase))
                return CurrentColor;
            return null;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Sets the current color being shown
        /// </summary>
        /// <param name="newColor">The new color to show</param>
        public int SetColor(Color newColor) {
            try {
                _currentColor = newColor;
                renderer.SetAllLEDColors(newColor);
                if (!renderer.AutoRender)
                    renderer.Render(renderer);
                return 200;
            }
            catch (InvalidRendererException e) {
                Console.WriteLine("LOGTHIS SolidController::Update failed to get renderer");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
            catch (InvalidColorFormatException e) {
                Console.WriteLine("LOGTHIS SolidController invalid color format exception");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS SolidController::Update failed, an exception occured");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
        }
        #endregion
    }
}
