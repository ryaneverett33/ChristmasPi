using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace ChristmasPi.Models {
    public class SetupDefaultsModel : SetupBase {
        public string[] DefaultModes;
        public string[] DefaultAnimations;
        public Color DefaultColor;

        public string animation;
        public string mode;
        public string color;
        public SetupDefaultsModel(string ErrorMessage) : this() {
            this.ErrorMessage = ErrorMessage;
            this.HasError = true;
        }

        public SetupDefaultsModel() {
            this.HasError = false;
            DefaultAnimations = Animation.AnimationManager.Instance.GetAnimations();
            DefaultModes = Operations.OperationManager.Instance.GetModes(false);
            DefaultColor = Data.Constants.DEFAULT_COLOR;
        }
    }
}
