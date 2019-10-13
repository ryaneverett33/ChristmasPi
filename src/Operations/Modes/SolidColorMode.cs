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
            SetColor(ConfigurationManager.Instance.TreeConfiguration.tree.color.DefaultColor);
            Console.WriteLine("Activated Solid Color Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            Console.WriteLine("Deactivated Solid Color Mode");
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
            catch (InvalidColorFormatException) {
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
