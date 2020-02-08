using System;

namespace ChristmasPi.Models {
    public class SetupBranchesModel : SetupBase {

        public SetupBranchesModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupBranchesModel() {
            this.HasError = false;
        }
    }
}
