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
        public object CurrentOperatingInfo => operatingModes[_currentOperatingMode].Info();

        public void Init() {
            operatingModes = new Dictionary<string, IOperationMode>();
            string[] classes = getClasses();
            foreach (string classname in classes) {
                IOperationMode operationMode = null;
                try {
                    operationMode = (IOperationMode)Activator.CreateInstance(Type.GetType(classname));
                }
                catch (Exception e) {
                    Log.ForContext<OperationManager>().Error(e, "An exception ocurred creating an instance of class {classname}", classname);
                    Environment.Exit(Constants.EXIT_INIT_FAILURE);
                }
                operatingModes.Add(operationMode.Name, operationMode);
            }
            // Set Current Operating mode
            try {
                if (ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                    setCurrentMode("SetupMode", true);
                else {
                    setCurrentMode(ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode, true);
                }
            }
            catch (Exception e) {
                Log.ForContext<OperationManager>().Error(e, "An error occurred activating {defaultmode}, activating {constantmode} instead",
                                                        ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode,
                                                        Constants.DEFAULT_OPERATING_MODE);
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
            // https://docs.microsoft.com/en-us/dotnet/api/system.type.isassignablefrom?view=netcore-3.1
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
                Log.ForContext<OperationManager>().Debug("Attempting to deactivate {currentMode}", currentMode);
                task = Task.Run(() => currentMode.Deactivate());
                if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                    Log.ForContext<OperationManager>().Error("Operating Mode deactivation timed out");
                    Log.ForContext<OperationManager>().Error("Current mode: {_currentOperatingMode}", _currentOperatingMode);
                }
                else
                    Log.ForContext<OperationManager>().Debug("Successfully deactivated {currentMode}", currentMode);
            }
            var newMode = operatingModes[newModeName];
            Log.ForContext<OperationManager>().Debug("Attempting to activate {newModeName}, default? {default}", newMode, defaultmode);
            task = Task.Run(() => newMode.Activate(defaultmode));
            if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                Log.ForContext<OperationManager>().Error("Operating Mode activation timed out");
                Log.ForContext<OperationManager>().Error("New mode: {newModeName}", newModeName);
            }
            else
                Log.ForContext<OperationManager>().Debug("Successfully activated {newMode}", newModeName);
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
