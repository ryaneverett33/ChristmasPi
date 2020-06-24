using System;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;

namespace ChristmasPi.Models {
    public class SetupBranchesModel : SetupBase {
        public int LightCount;

        public SetupBranchesModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupBranchesModel() {
            this.HasError = false;
            LightCount = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).Configuration.hardware.lightcount;
        }
    }
}
