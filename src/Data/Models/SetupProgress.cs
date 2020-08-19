using System;
using Newtonsoft.Json;

namespace ChristmasPi.Data.Models {
    [JsonConverter(typeof(SetupProgress))]
    public class SetupProgress {
        public TreeConfiguration CurrentConfiguration;
        public SetupState CurrentState { get; private set; }
        public SetupStep[] CurrentStepProgress { get; private set;}
        public string CurrentStep { get; private set; }
        private bool loadedSteps;

        public SetupProgress() {
            throw new NotImplementedException();
        }
        public SetupProgress(TreeConfiguration configuration, string currentStep) {
            // set CurrentConfiguration
            // save current step
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the next step in the setup process
        /// </summary>
        /// <param name="currentpage">The current step in the setup process</param>
        /// <returns>The name of the next setup step</returns>
        public string GetNextStep(string currentstep) {
            throw new NotImplementedException();
            /*
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Name.Equals(currentpage, StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 >= steps.Length)
                        return null;
                    return steps[i+1].Name;
                }
            }
            return null;
            */
        }

        /// <summary>
        /// Sets the current step in the setup process
        /// </summary>
        /// <param name="newstep">The new setup step</param>
        /// <remarks>This should only be called by the SetupController</remarks>
        public void SetCurrentStep(string newstep) {
            throw new NotImplementedException();
            /*
            Log.ForContext("ClassName", "AnimationMode").Debug("Setting current step: {newstep}", newstep);
            if (newstep == null || newstep.Length == 0 || newstep == "null")
                return;
            SetupStep step = steps.Where(step => step.Name.Equals(newstep)).Single();
            CurrentStepName = step.Name;
            */
        }

        /// <summary>
        /// 
        /// </summary>
        public void CompleteStep() {
            throw new NotImplementedException();
            /*
            // completes the current step in a linear fashion
            if (CurrentStepName == null || CurrentStepName.Length == 0) {
                Log.ForContext("ClassName", "AnimationMode").Error("CompleteStep() CurrentStepName is null");
                return;
            }
            int currentStepIndex = -1;
            // get index of current step and check if previous steps are completed
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Name == CurrentStepName) {
                    currentStepIndex = i;
                    break;
                }
                else {
                    if (!steps[i].Completed)
                        throw new ApplicationException("Setup not completed in linear order");
                }
            }
            if (currentStepIndex == -1)
                throw new ApplicationException("Unable to find index for the current setup step");
            steps[currentStepIndex].Completed = true;
            */
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool IsStepFinished(string step) {
            throw new NotImplementedException();
            /*
            return steps.Where(s => s.Name == step).Single().Completed;
            */
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void StartSetup() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FinishSetup() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="steps"></param>
        public void LoadSteps(SetupStep[] steps) {
            // Set CurrentStepProgress, iterate through and update progress, set currentstate
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
    public class SetupProgressConverter : JsonConverter<SetupProgress> {
        // See HardwareTypeConverter in RendererType.cs
        public override SetupProgress ReadJson(JsonReader reader, Type objectType, SetupProgress existingValue, bool hasExistingValue, JsonSerializer serializer) {
            // load treeconfiguration and currentstep
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, SetupProgress value, JsonSerializer serializer) {
            // save treeconfiguration and currentstep
            throw new NotImplementedException();
        }
    }
}