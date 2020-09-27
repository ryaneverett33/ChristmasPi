using System;
using ChristmasPi.Util;

namespace ChristmasPi.Models {
    public class ServiceStatusModel {
        public string Output;
        public string Status;

        public ServiceStatusModel() {}

        public ServiceStatusModel(OutputWriter writer, InstallationStatus status) {
            Output = writer.ToString();
            Status = Enum.GetName(typeof(InstallationStatus), status);
        }

        public static ServiceStatusModel Stale() {
            ServiceStatusModel model = new ServiceStatusModel();
            model.Status = "Stale";
            model.Output = "";
            return model;
        }

        public ServiceStatusModel AllDone() {
            this.Status = "AllDone";
            return this;
        }

        public ServiceStatusModel Reboot() {
            this.Status = "Reboot";
            return this;
        }
    }
}