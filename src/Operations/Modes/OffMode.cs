using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Operations.Utils;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using Serilog;

namespace ChristmasPi.Operations.Modes {
    public class OffMode : IOperationMode, IOffMode {
        #region Properties
        public string Name => "OffMode";
        public bool CanBeDefault => false;
        public bool TurnedOff => _active;
        #endregion
        #region Fields
        private IRenderer renderer;
        private bool _active;           // Whether or not off mode is currently active
        #endregion
        public OffMode() {
            _active = false;
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            renderer = RenderFactory.GetRenderer();
            renderer.Start();
            TurnOff();
            _active = true;
            Log.ForContext("ClassName", "AnimationMode").Information("Activated Off Mode");
        }
        public void Deactivate() {
            renderer.Stop();
            _active = false;
            Log.ForContext("ClassName", "AnimationMode").Information("Deactivated Off Mode");
            renderer = null;
        }
        public object Info() {
            return new {
                TurnedOff = _active
            };
        }
        public object GetProperty(string property) {
            return PropertyHelper.ResolveProperty(property, this, typeof(OffMode));
            /*if (property.Equals(nameof(Name), StringComparison.CurrentCultureIgnoreCase))
                return Name;
            if (property.Equals(nameof(TurnedOff), StringComparison.CurrentCultureIgnoreCase))
                return Name;
            return null;*/
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