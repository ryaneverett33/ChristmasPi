using System;

namespace ChristmasPi.Models {
    public class SetupLightsModel : SetupBase {
        public int DefaultBrightness;

        // Form data
        public int lightcount;
        public int fps;
        public int brightness;
        public bool invert;

        public SetupLightsModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupLightsModel() {
            DefaultBrightness = 255;
            this.HasError = false;
        }
    }
}
