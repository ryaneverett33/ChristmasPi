using System;

namespace ChristmasPi.Models {
    public class SetupHardwareModel : SetupBase {
        public string Placeholder { get; set; }
        public string Image { get; set; }
        public string ValidationString { get; set; }
        public string datapin {get; set;}

        public SetupHardwareModel(string ErrorMessage) {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupHardwareModel() { }
    }
}
