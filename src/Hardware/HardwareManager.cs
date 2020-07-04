using System;
using System.Linq;
using System.Collections.Generic;
using ChristmasPi.Data.Models.Hardware;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data.Models;
using ChristmasPi.Data;
using ChristmasPi.Hardware.Factories;

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
        /// Gets list of renderers and necessary info about the renderer for setup
        /// </summary>
        /// <returns>An array of HardwareInfoResult's</returns>
        public HardwareInfoResult[] GetRendererInfo() {
            RendererType[] rendererTypes = (RendererType[])Enum.GetValues(typeof(RendererType));
            // filter out unknown
            rendererTypes = rendererTypes.Where(rt => rt != RendererType.UNKNOWN).ToArray();
            List<RendererType> supportedRendererTypes = new List<RendererType>();
            foreach (RendererType type in rendererTypes) {
                if (RenderFactory.GetSupportedHardwareForRenderer(type).HasFlag(hardwareType))
                    supportedRendererTypes.Add(type);
            }
            List<HardwareInfoResult> results = new List<HardwareInfoResult>();
            foreach (RendererType type in supportedRendererTypes) {
                RendererHardwareInfo info = RenderFactory.GetRendererHardwareInfoForRenderer(type);
                results.Add(new HardwareInfoResult() {
                    Name = Enum.GetName(typeof(RendererType), type),
                    Placeholder = info.GetPlaceholderValue(),
                    Image = info.GetImageUrl(),
                    ValidationString = info.GetValidationString()
                });
            }
            return results.ToArray();
        }
    }
    public class HardwareInfoResult {
        /// <summary>
        /// The name of the renderer
        /// </summary>
        public string Name;
        /// <summary>
        /// The default pin or port needed to setup the renderer
        /// </summary>
        public string Placeholder;
        /// <summary>
        /// Image URL for any hardware diagrams
        /// </summary>
        public string Image;
        /// <summary>
        /// The validation string
        /// </summary>
        /// <notes>This is either a csv or an empty string. 
        /// The csv denotes acceptable values while an empty string allows all values</notes>
        public string ValidationString;
    }
}
