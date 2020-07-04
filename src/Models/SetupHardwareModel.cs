using System;
using System.Collections.Generic;
using ChristmasPi.Hardware;

namespace ChristmasPi.Models {
    public class SetupHardwareModel : SetupBase {
        public string[] Placeholders { get; set; }
        public string[] Images { get; set; }
        public string[] ValidationStrings { get; set; }
        public string[] Renderers { get; set; }

        // Form data
        public string datapin { get; set; }
        public string renderer { get; set; }

        public SetupHardwareModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }
        public SetupHardwareModel() {
            var infoArr = HardwareManager.Instance.GetRendererInfo();
            List<string> placeholders = new List<string>();
            List<string> images = new List<string>();
            List<string> validations = new List<string>();
            List<string> renderers = new List<string>();
            foreach (var info in infoArr) {
                placeholders.Add(info.Placeholder);
                images.Add(info.Image);
                validations.Add(info.ValidationString);
                renderers.Add(info.Name);
            }
            Placeholders = placeholders.ToArray();
            Images = images.ToArray();
            ValidationStrings = validations.ToArray();
            Renderers = renderers.ToArray();
            this.HasError = false;
        }
    }
}
