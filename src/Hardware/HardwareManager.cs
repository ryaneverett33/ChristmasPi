using System;
using System.Collections.Generic;
using ChristmasPi.Data.Models.Hardware;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data.Models;
using ChristmasPi.Data;

namespace ChristmasPi.Hardware {
    public class HardwareManager {
        #region Singleton Methods
        private static readonly HardwareManager _instance = new HardwareManager();
        public static HardwareManager Instance { get { return _instance; } }
        #endregion

        public Dictionary<int, RPI_Type> RPITypeDict { get; private set; }
        private Hardware_Type hardwareType;

        public void Init(bool defaultToRpi = false) {
            RPITypeDict = RPIType.GetTypeDictionary();
            hardwareType = HardwareType.GetHardwareType(defaultToRpi);
            if (defaultToRpi)
                RPIType.fallback = true;
        }

        /// <summary>
        /// Gets the default data pin to act as a placeholder for the Hardware Setup View
        /// </summary>
        /// <returns>the default data pin for the current hardware</returns>
        public int GetPlaceHolderPin() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.PlaceHolderPin;
            throw new UnsupportedHardwareException();
        }

        /// <summary>
        /// Gets the acceptable values for data pin entry
        /// </summary>
        /// <returns>A comma seperated string containing acceptable values for the data pin</returns>
        public string GetValidationOptions() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.GetValidationString();
            throw new UnsupportedHardwareException();
        }

        /// <summary>
        /// Gets the hardware diagram for specifying the hardware pin
        /// </summary>
        /// <returns>The image url for the given diagram</returns>
        public string GetPinImageUrl() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.GetImageUrl();
            throw new UnsupportedHardwareException();
        }

        /// <summary>
        /// Gets the list of supported renderers
        /// </summary>
        /// <returns>An array of renderer names</returns>
        public string[] GetRenderers() {
            string[] keys = Enum.GetNames(typeof(RendererType));
            List<string> renderers = new List<string>(keys.Length);
            foreach (string key in keys) {
                if (key == Enum.GetName(typeof(RendererType), RendererType.UNKNOWN))
                    continue;
                if (key == Enum.GetName(typeof(RendererType), RendererType.TEST_RENDER) &&
                    !ConfigurationManager.Instance.DebugConfiguration.AllowTestRenderer)
                    continue;
                renderers.Add(key);
            }
            return renderers.ToArray();
        }
    }
}
