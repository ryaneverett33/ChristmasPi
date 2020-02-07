using System;
using ChristmasPi.Hardware;

namespace ChristmasPi.Models {
    public class SetupHardwareModel : SetupBase {
        public string Placeholder { get; set; }
        public string Image { get; set; }
        public string ValidationString { get; set; }
        public string[] Renderers { get; set; }

        // Form data
        public string datapin { get; set; }
        public string renderer { get; set; }

        public SetupHardwareModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupHardwareModel() {
            Placeholder = HardwareManager.Instance.GetPlaceHolderPin().ToString();
            Image = HardwareManager.Instance.GetPinImageUrl();
            ValidationString = HardwareManager.Instance.GetValidationOptions();
            Renderers = HardwareManager.Instance.GetRenderers();
            this.HasError = false;
        }
    }
}
