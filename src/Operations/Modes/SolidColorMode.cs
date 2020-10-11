using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Operations.Utils;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using Microsoft.AspNetCore.Http;
using Serilog;

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
            Log.ForContext<SetupMode>().Information("Activated Solid Color Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            Log.ForContext<SetupMode>().Information("Deactivated Solid Color Mode");
            renderer = null;
        }
        public object Info() {
            return new {
                currentcolor = CurrentColor
            };
        }
        public object GetProperty(string property) {
            return PropertyHelper.ResolveProperty(property, this, typeof(SolidColorMode));
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
                Log.ForContext<SetupMode>().Error(e, "Update() failed to get renderer");
                return StatusCodes.Status500InternalServerError;
            }
            catch (InvalidColorFormatException e) {
                Log.ForContext<SetupMode>().Error(e, "Update() invalid color format exception");
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Log.ForContext<SetupMode>().Error(e, "Update() failed");
                return StatusCodes.Status500InternalServerError;
            }
        }
        #endregion
    }
}
