using System;
using Newtonsoft.Json;

namespace ChristmasPi.Data.Models {
    public class SetupProgress {
        public TreeConfiguration CurrentConfiguration;
        public SetupState CurrentState { get; private set; }
        public SetupStep[] CurrentStepProgress { get; private set;}
        public string CurrentStep { get; private set; }

        public void SetCurrentStep() {
            throw new NotImplementedException();
        }
        public string GetNextStep(string currentstep) {
            throw new NotImplementedException();
        }
        public void SetCurrentStep(string newstep) {
            throw new NotImplementedException();
        }
        public void CompleteStep() {
            throw new NotImplementedException();
        }
        public bool IsStepFinished(string step) {
            throw new NotImplementedException();
        }
        public void FinishSetup() {
            throw new NotImplementedException();
        }

        // TODO implement this class
    }
    public enum SetupState {
        NotStarted,
        Progressing,
        Finished
    };
    public class SetupStep {
        public string Name;
        public bool Completed;
        public SetupStep(string name) {
            Name = name;
        }
    }
}