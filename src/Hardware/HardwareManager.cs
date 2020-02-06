using System;
using System.Collections.Generic;
using ChristmasPi.Data.Models.Hardware;
using ChristmasPi.Data.Exceptions;

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

        public int GetPlaceHolderPin() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.PlaceHolderPin;
            throw new UnsupportedHardwareException();
        }

        public string GetValidationOptions() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.GetValidationString();
            throw new UnsupportedHardwareException();
        }

        public string GetPinImageUrl() {
            if (hardwareType == Hardware_Type.RPI)
                return RPIType.GetImageUrl();
            throw new UnsupportedHardwareException();
        }
    }
}
