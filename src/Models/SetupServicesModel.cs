using System;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Util;

namespace ChristmasPi.Models {
    public class SetupServicesModel : SetupBase {
        public bool InitSystemSupported;
        public string InitSystemName;
        public string[] InstallSteps;

        public SetupServicesModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupServicesModel() {
            this.HasError = false;
            //this.InitSystemSupported = ServiceInstaller.CanInstallService();
            this.InitSystemSupported = true;
            this.InstallSteps = ServiceInstaller.GetSteps();
            this.InitSystemName = OSUtils.GetInitSystemType().ToString();
        }
    }
}
