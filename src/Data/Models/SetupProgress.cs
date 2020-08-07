using System;

namespace ChristmasPi.Data.Models {
    public class SetupProgress {
        public TreeConfiguration CurrentConfiguration;
        public SetupState CurrentState;

        // TODO implement this class
    }
    public enum SetupState {
        NotStarted,
        Progressing,
        Finished
    };
}