using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Models {
    public class SetupDefaultsModel : SetupBase {
        public string[] DefaultModes;
        public string[] DefaultAnimations;

        public string animation;
        public string mode;
        public SetupDefaultsModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }

        public SetupDefaultsModel() {
            this.HasError = false;
            DefaultAnimations = Animation.AnimationManager.Instance.GetAnimations();
            DefaultModes = Operations.OperationManager.Instance.GetModes(false);
        }
    }
}
