using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Modes;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using Serilog;

namespace ChristmasPi.Operations {
    public class OperationManager {
        #region Singleton Methods
        private static readonly OperationManager _instance = new OperationManager();
        public static OperationManager Instance { get { return _instance; } }
        #endregion
        private Dictionary<string, IOperationMode> operatingModes;
        private string _currentOperatingMode = null;
        public IOperationMode CurrentOperatingMode => operatingModes[_currentOperatingMode];
        public string CurrentOperatingModeName => _currentOperatingMode;
        public string DefaultOperatingMode { get; private set; }
        public object CurrentOperatingInfo => operatingModes[_currentOperatingMode].Info();

        public void Init() {
            operatingModes = new Dictionary<string, IOperationMode>();
            string[] classes = getClasses();
            foreach (string classname in classes) {
                /// TODO Handle if exception occurs when creating instance or casting
                IOperationMode operationMode = (IOperationMode)Activator.CreateInstance(Type.GetType(classname));
                operatingModes.Add(operationMode.Name, operationMode);
            }
            // Set Current Operating mode
            try {
                DefaultOperatingMode = ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode;
                if (ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                    setCurrentMode("SetupMode", true);
                else {
                    setCurrentMode(ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode, true);
                }
            }
            catch (Exception) {
                DefaultOperatingMode = Constants.DEFAULT_OPERATING_MODE;
                setCurrentMode(Constants.DEFAULT_OPERATING_MODE, true);
            }
        }
        public string[] GetModes(bool includeNonDefault = true) {
            List<string> modes = new List<string>();
            foreach (string key in operatingModes.Keys) {
                IOperationMode mode = operatingModes[key];
                if (!mode.CanBeDefault && includeNonDefault)
                    modes.Add(key);
                else if (mode.CanBeDefault)
                    modes.Add(key);
            }

            return modes.ToArray<string>();
        }

        /// <summary>
        /// Gets a list of all classes in the executing assembly that implement the IOperationMode interface
        /// </summary>
        /// <returns>List of classes</returns>
        private string[] getClasses() {
            /// TODO use reflection to get classes instead of a hardcoded list
            return new string[] {
                typeof(SolidColorMode).FullName,
                typeof(AnimationMode).FullName,
                typeof(OffMode).FullName,
                typeof(SetupMode).FullName,
                typeof(NothingMode).FullName
            };
        }

        /// <summary>
        /// Switches the current operating mode to the newly requested one
        /// </summary>
        /// <param name="newMode">The mode being switched to</param>
        public void SwitchModes(string newMode) {
            setCurrentMode(newMode);
        }

        /// <summary>
        /// Deactivates the current operating mode and activates the new mode
        /// </summary>
        /// <param name="newModeName">The mode being switched to</param>
        private void setCurrentMode(string newModeName, bool defaultmode = false) {
            if (!operatingModes.ContainsKey(newModeName))
                throw new InvalidOperatingModeException($"{newModeName} is not a valid operating mode");
            Task task;
            if (_currentOperatingMode != null) {
                var currentMode = operatingModes[_currentOperatingMode];
                task = Task.Run(() => currentMode.Deactivate());
                if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                    Log.ForContext("ClassName", "OperationManager").Error("Operating Mode deactivation timed out");
                    Log.ForContext("ClassName", "OperationManager").Error("Current mode: {_currentOperatingMode}", _currentOperatingMode);
                }
            }
            var newMode = operatingModes[newModeName];
            task = Task.Run(() => newMode.Activate(defaultmode));
            if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                Log.ForContext("ClassName", "OperationManager").Error("Operating Mode activation timed out");
                Log.ForContext("ClassName", "OperationManager").Error("New mode: {newModeName}", newModeName);
            }
            _currentOperatingMode = newModeName;
        }

        /// <summary>
        /// Requests a property from an operating mode
        /// </summary>
        /// <param name="mode">The operating mode to request from</param>
        /// <param name="property">The name of property to retrieve</param>
        /// <returns>The value of the property on success, null on failure</returns>
        public object GetProperty(string mode, string property) {
            if (!operatingModes.ContainsKey(mode))
                throw new InvalidOperatingModeException($"{mode} is not a valid operating mode");
            else {
                IOperationMode modeObj = operatingModes[mode];
                return modeObj.GetProperty(property);
            }
        }

        /// <summary>
        /// Gets the list of operating modes that can be defaulted to
        /// </summary>
        /// <returns>Array of operation mode names</returns>
        public string[] GetDefaultableModes() {
            IEnumerable<KeyValuePair<string, IOperationMode>> pairs =  operatingModes.Where(nameModePair => nameModePair.Value.CanBeDefault);
            return pairs.Select(pair => pair.Key).ToArray();
        }
    }
}
