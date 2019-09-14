﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Modes;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;

namespace ChristmasPi.Operations {
    public class OperationManager {
        #region Singleton Methods
        private static readonly OperationManager _instance = new OperationManager();
        public static OperationManager Instance { get { return _instance; } }
        #endregion
        private Dictionary<string, IOperationMode> operatingModes;
        private string _currentOperatingMode = null;
        public string CurrentOperatingMode => _currentOperatingMode;

        public void Init() {
            operatingModes = new Dictionary<string, IOperationMode>();
            string[] classes = getClasses();
            foreach (string classname in classes) {
                object obj = Activator.CreateInstance(Type.GetType(classname));
                if (obj == null) {

                } else {
                    IOperationMode operationMode = (IOperationMode)obj;
                    operatingModes.Add(operationMode.Name, operationMode);
                }
            }
            // Set Current Operating mode
            setCurrentMode(Constants.DEFAULT_OPERATING_MODE);
        }
        public string[] GetModes() {
            ICollection<string> keys = operatingModes.Keys;
            return keys.ToArray<string>();
        }
        /// <summary>
        /// Gets a list of all classes in the executing assembly that implement the IOperationMode interface
        /// </summary>
        /// <returns>List of classes [simple name, full name]</returns>
        private string[] getClasses() {
            /// TODO use reflection to get classes instead of a hardcoded list
            return new string[] {
                typeof(SolidColorMode).FullName
            };
        }
        /// <summary>
        /// Switches the current operating mode to the newly requested one
        /// </summary>
        /// <param name="newMode">The mode being switched to</param>
        public void SwitchModes(string newMode) {

        }
        /// <summary>
        /// Deactivates the current operating mode and activates the new mode
        /// </summary>
        /// <param name="newModeName">The mode being switched to</param>
        private void setCurrentMode(string newModeName) {
            if (!operatingModes.ContainsKey(newModeName))
                throw new InvalidOperatingModeException($"{newModeName} is not a valid operating mode");
            Task task;
            if (_currentOperatingMode != null) {
                var currentMode = operatingModes[_currentOperatingMode];
                task = Task.Run(() => currentMode.Deactivate());
                if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                    Console.WriteLine("LOGHIS Operating Mode deactivation timed out.");
                    Console.WriteLine($"Current mode: {_currentOperatingMode}");
                }
            }
            var newMode = operatingModes[newModeName];
            task = Task.Run(() => newMode.Activate());
            if (!task.Wait(Constants.ACTIVATION_TIMEOUT)) {
                Console.WriteLine("LOGHIS Operating Mode activation timed out.");
                Console.WriteLine($"new mode: {newModeName}");
            }
            _currentOperatingMode = newModeName;
        }
    }
}