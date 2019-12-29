using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;

namespace ChristmasPi.Operations.Modes {
    public class OffMode : IOperationMode, IOffMode {
        #region Properties
        public string Name => "OffMode";
        public bool TurnedOff => _active;
        #endregion
        #region Fields
        private IRenderer renderer;
        private bool _active;           // Whether or not off mode is currently active
        #endregion
        public OffMode() {
            renderer = RenderFactory.GetRenderer();
            _active = false;
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            renderer.Start();
            TurnOff();
            _active = true;
            Console.WriteLine("Activated Off Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            _active = false;
            Console.WriteLine("Deactivated Off Mode");
        }
        public object Info() {
            return new {
                TurnedOff = true
            };
        }
        public object GetProperty(string property) {
            if (property.Equals(nameof(Name), StringComparison.CurrentCultureIgnoreCase))
                return Name;
            if (property.Equals(nameof(TurnedOff), StringComparison.CurrentCultureIgnoreCase))
                return Name;
            return null;
        }
        #endregion
        #region Methods
        public void TurnOff() {
            _active = true;
            renderer.SetAllLEDColors(Constants.COLOR_OFF);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }
        #endregion
    }
}