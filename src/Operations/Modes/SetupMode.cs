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
    public class SetupMode : IOperationMode, ISetupMode {
        #region Properties
        public string Name => "SetupMode";
        #endregion
        #region Fields
        private bool _active;           // Whether or not off mode is currently active
        #endregion
        public SetupMode() { }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
        }
        public void Deactivate() {
        }
        public object Info() {
            return new {};
        }
        public object GetProperty(string property) {
            return null;
        }
        #endregion
        #region Methods
        #endregion
    }
}