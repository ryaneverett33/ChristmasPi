using System;
using ChristmasPi.Operations.Interfaces;
using Serilog;

namespace ChristmasPi.Operations.Modes {

    /// <summary>
    /// An operating mode where nothing is happening
    /// </summary>
    /// <remarks>This mode is used when a controller exclusively switches to a certain mode, see SetupController</remarks>
    public class NothingMode : IOperationMode {
        public string Name => "NothingMode";
        public bool CanBeDefault => false;

        public void Activate(bool defaultmode) {
            Log.ForContext("ClassName", "AnimationMode").Information("Activated nothing mode");
        }
        public void Deactivate() {
            Log.ForContext("ClassName", "AnimationMode").Information("Deactivated nothing mode");
        }
        public object Info() { return null; }
        public object GetProperty(string property) { return null; }
    }
}
