using System;

namespace ChristmasPi.Data.Models.Hardware {
    // Provides information about the hardware requirements of a renderer
    public class RendererHardwareInfo {
        public Func<string> GetImageUrl;
        public Func<string> GetValidationString;
        public Func<string> GetPlaceholderValue;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="GetImageUrlFunc">The function to retrieve the image URL</param>
        /// <param name="GetValidationStringFunc">The function to retrieve the validation string</param>
        /// <param name="GetPlaceholderValueFunc">The function to retrieve the placeholer string</param>
        /// <notes>Delegate functions are responsible for exception handling and state management</notes>
        public RendererHardwareInfo(Func<string> GetImageUrlFunc, Func<string> GetValidationStringFunc, Func<string> GetPlaceholderValueFunc) {
            GetImageUrl = GetImageUrlFunc;
            GetValidationString = GetValidationStringFunc;
            GetPlaceholderValue = GetPlaceholderValueFunc;
        }

        /// <summary>
        /// Default Constructor to represent unknown hardware
        /// </summary>
        public RendererHardwareInfo() {
            GetImageUrl = () => "/img/hardware/unknown.png";
            GetValidationString = () => "";
            GetPlaceholderValue = () => "0";
        }
    }
}