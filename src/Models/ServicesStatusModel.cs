using System;
using ChristmasPi.Util;

namespace ChristmasPi.Models {
    public class ServiceStatusModel {
        public string Output;
        public string Status;

        public ServiceStatusModel() {}

        public ServiceStatusModel(OutputWriter writer, InstallationStatus status) {
            if (writer == null)
                Output = "";
            else
                Output = writer.ToString();
            Status = Enum.GetName(typeof(InstallationStatus), status);
        }

        public static ServiceStatusModel Stale() {
            ServiceStatusModel model = new ServiceStatusModel();
            model.Status = "Stale";
            model.Output = "";
            return model;
        }

        public static ServiceStatusModel Finished() {
            return new ServiceStatusModel(null, InstallationStatus.Finished);
        }

        public static ServiceStatusModel Rebooting() {
            return new ServiceStatusModel(null, InstallationStatus.Rebooting);
        }
    }
}