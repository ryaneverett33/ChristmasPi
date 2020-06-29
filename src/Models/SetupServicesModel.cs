using System;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Util;

namespace ChristmasPi.Models {
    public class SetupServicesModel : SetupBase {
        public bool alreadyInstalled;

        public SetupServicesModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupServicesModel() {
            this.HasError = false;
        }
    }
}
