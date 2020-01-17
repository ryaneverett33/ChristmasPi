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
        private string[] steps;
        public string CurrentStep { get; private set; }
        #endregion
        public SetupMode() {
            steps = new string[] {
                "start",
                "hardware",
                "lights",
                "branches",
                "defaults",
                "finished"
            };
            SetCurrentStep(null);
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            SetCurrentStep("start");
        }
        public void Deactivate() {
            SetCurrentStep("null");
        }
        public object Info() {
            return new {};
        }
        public object GetProperty(string property) {
            return null;
        }
        #endregion
        #region Methods
        public string GetNext(string currentpage) {
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Equals(currentpage, StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 >= steps.Length)
                        return null;
                    return steps[i+1];
                }
            }
            return null;
        }
        public void SetCurrentStep(string currentstep) {
            CurrentStep = currentstep;
        }
        #endregion
    }
}