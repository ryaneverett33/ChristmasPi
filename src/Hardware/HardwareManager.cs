using System;
using System.Collections.Generic;
using ChristmasPi.Data.Models.Hardware;

namespace ChristmasPi.Hardware {
    public class HardwareManager {
        #region Singleton Methods
        private static readonly HardwareManager _instance = new HardwareManager();
        public static HardwareManager Instance { get { return _instance; } }
        #endregion

        public Dictionary<int, RPI_Type> RPITypeDict { get; private set; }

        public HardwareManager() {
            RPITypeDict = RPIType.GetTypeDictionary();
        }
    }
}
