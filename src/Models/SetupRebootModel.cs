using System;

namespace ChristmasPi.Models {
    public class SetupRebootModel : SetupBase {
        public bool RebootRequired;

        public SetupRebootModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupRebootModel() {
            this.RebootRequired = false;
            this.HasError = false;
        }
    }
}
