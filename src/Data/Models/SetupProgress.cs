using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using Serilog;

namespace ChristmasPi.Data.Models {
    [JsonConverter(typeof(SetupProgressConverter))]
    public class SetupProgress {
        public TreeConfiguration CurrentConfiguration;
        public SetupState CurrentState { get; private set; }
        public SetupStep[] CurrentStepProgress { get; private set; }
        public AuxiliaryStep[] AuxiliarySteps { get; private set; }
        public string CurrentStep { get; private set; }
        private bool currentStepIsAuxiliary; 

        public SetupProgress() {
            CurrentConfiguration = ConfigurationManager.Instance.CurrentTreeConfig;
            CurrentState = SetupState.NotStarted;
            setCurrentStep(null, false);
            CurrentStepProgress = null;
        }

        internal SetupProgress(TreeConfiguration configuration, string currentStep, SetupState state) {
            this.CurrentConfiguration = configuration;
            setCurrentStep(currentStep, false);
            this.CurrentState = state;
            CurrentStepProgress = null;
        }

        /// <summary>
        /// Gets the next step in the setup process
        /// </summary>
        /// <param name="currentpage">The current step in the setup process</param>
        /// <returns>The name of the next setup step, null if no step is next</returns>
        /// <remarks>If currentstep is provided, GetNextStep will return the step after currentstep</remarks>
        public string GetNextStep(string currentStep = null) {
            string searchStep = currentStep == null ? CurrentStep : currentStep;
            if (searchStep == null) {
                // If no steps are done, next step is the first step
                // If all steps are done, next step is null
                if (CurrentStepProgress.Any<SetupStep>(step => !step.Completed))
                    return CurrentStepProgress[0].Name;
                else
                    return null;
            }
            int stepIndex = -1;
            if (currentStepIsAuxiliary)
                stepIndex = Array.FindIndex<AuxiliaryStep>(AuxiliarySteps, step => step.Name == searchStep);
            else
                stepIndex = Array.FindIndex<SetupStep>(CurrentStepProgress, step => step.Name == searchStep);
            if (stepIndex == -1) {
                Log.ForContext<SetupProgress>().Debug("CurrentStepProgress does not contain currentstep: {currentstep}", currentStep);
                throw new ArgumentException("currentStep is not a valid setup step"); 
            }
            else {
                int lastPageIndex = currentStepIsAuxiliary ? AuxiliarySteps.Length - 1 : CurrentStepProgress.Length - 1;
                if (stepIndex == lastPageIndex)
                    return null;
                else
                    return currentStepIsAuxiliary ? AuxiliarySteps[stepIndex].Next : CurrentStepProgress[stepIndex + 1].Name;
            }
        }

        /// <summary>
        /// Sets the current step in the setup process
        /// </summary>
        /// <param name="newstep">The new setup step</param>
        /// <remarks>This should only be called by the SetupController</remarks>
        public void SetCurrentStep(string newstep) {
            //throw new NotImplementedException();
            return;
            // Don't implement this function


            /*
            Log.ForContext("ClassName", "AnimationMode").Debug("Setting current step: {newstep}", newstep);
            if (newstep == null || newstep.Length == 0 || newstep == "null")
                return;
            SetupStep step = steps.Where(step => step.Name.Equals(newstep)).Single();
            CurrentStepName = step.Name;
            */
        }

        /// <summary>
        /// Sets the current step as an auxiliary step
        /// </summary>
        /// <param name="auxstep">The name of the step without the 'aux/' prefix</param>
        /// <remarks>Is used to indicate a break in normal setup flow</remarks>
        /// <example>auxstep: reboot sets the current step to the auxiliary reboot step and GetNext will return services</example>
        public void SetCurrentAuxStep(string auxstep) {
            if (auxstep == null)
                throw new ArgumentNullException("auxstep");
            bool isValidStep = false;
            foreach (AuxiliaryStep step in AuxiliarySteps) {
                if (step.Name == auxstep) {
                    isValidStep = true;
                    break;
                }
            }
            if (!isValidStep)
                throw new InvalidSetupActionException($"Can't set current step to auxiliary {auxstep}, it doesn't exist");
            setCurrentStep($"aux/{auxstep}", true);
        }

        /// <summary>
        /// Marks the current setup step as complete
        /// </summary>
        public void CompleteStep() {
            if (CurrentStep == null) {
                Log.ForContext<SetupProgress>().Debug("CompleteStep() silently handling null CurrentStep");
                return;
            }
            if (currentStepIsAuxiliary) {
                // We don't care about the completion status of auxiliary steps
                setCurrentStep(GetNextStep(CurrentStep), false);
                return;
            }
            int currentStepIndex = Array.FindIndex<SetupStep>(CurrentStepProgress, step => step.Name == CurrentStep);
            CurrentStepProgress[currentStepIndex].Completed = true;
            setCurrentStep(GetNextStep(CurrentStep), false);
            //if (OperationManager.Instance.CurrentOperatingModeName == "SetupMode")
            //    (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SaveSetupProgress();
        }
        
        /// <summary>
        /// Check where a given step has been completed or not
        /// </summary>
        /// <param name="step">The name of the Setup Step to check</param>
        /// <returns>True if the step has been completed, False if it has not</returns>
        /// <remarks>If the step is an Auxiliary step, the function always returns true</remarks>
        public bool IsStepFinished(string step) {
            if (step == null)
                throw new ArgumentNullException("step");
            if (step.Contains("aux/")) {
                int stepIndex = Array.FindIndex<AuxiliaryStep>(AuxiliarySteps, s => s.Name == step);
                if (stepIndex != -1)
                    return true;
                else
                    throw new ArgumentException($"{step} is not a valid setup step");
            }
            return CurrentStepProgress.Where(s => s.Name == step).Single().Completed;
        }
        
        /// <summary>
        /// Start the setup process
        /// </summary>
        public void StartSetup() {
            if (CurrentState != SetupState.NotStarted) {
                throw new InvalidSetupActionException("Unable to Start Setup, setup already started");
            }
            CurrentState = SetupState.Progressing;
            CompleteStep();
        }

        /// <summary>
        /// Finish the setup process
        /// </summary>
        public void FinishSetup() {
            if (CurrentStepProgress[CurrentStepProgress.Length - 1].Name == CurrentStep
                && !IsStepFinished(CurrentStep))
                CompleteStep();
            if (CurrentStepProgress.Any(s => !s.Completed)) {
                throw new InvalidSetupActionException("Unable to Finish Setup, some steps aren't complete");
            }
            setCurrentStep(null, false);
            CurrentState = SetupState.Finished;
            // Delete any setup files
            if (File.Exists(Constants.SETUP_PROGRESS_FILE))
                File.Delete(Constants.SETUP_PROGRESS_FILE);
        }

        /// <summary>
        /// Load the CurrentStepProgress array and updates progress with respect to CurrentStep
        /// </summary>
        /// <param name="steps">Names of each setup step</param>
        public void LoadSteps(string[] steps) {
            // Set CurrentStepProgress, iterate through and update progress, set currentstate
            if (steps == null)
                throw new ArgumentNullException("steps");
            if (CurrentStepProgress != null)
                throw new InvalidOperationException("Already called LoadSteps, duplicate call would overwrite state");
            CurrentStepProgress = new SetupStep[steps.Length];
            int currentStepIndex = -1;
            // assign step names
            for (int i = 0; i < CurrentStepProgress.Length; i++) {
                CurrentStepProgress[i] = new SetupStep(steps[i]);
                if (CurrentStepProgress[i].Name.Equals(CurrentStep))
                    currentStepIndex = i;
            }
            // assign progress
            if (currentStepIndex != -1) {
                for (int i = 0; i < currentStepIndex; i++) {
                    CurrentStepProgress[i].Completed = true;
                }
            }
            if (CurrentStep == null) {
                setCurrentStep(GetNextStep(), false);
            }
        }
        
        /// <summary>
        /// Loads the possible auxiliary steps
        /// </summary>
        /// <param name="steps">Array of Tuples<Step Name, Next Step, Previous Step> for each step</param>
        /// <remarks>Next/Previous should correspond to the step's relative url.
        /// If a next/prev step is an auxiliary function, it should be prefixed with 'aux/'.</remarks>
        /// <example>Name: DoReboot, Next: aux/postreboot, Prev: services</example>
        public void LoadAuxiliarySteps(Tuple<string, string, string>[] steps) {
            if (steps == null)
                throw new ArgumentNullException("steps");
            if (AuxiliarySteps != null)
                throw new InvalidOperationException("Already called LoadAuxiliarySteps, duplicate call would overwrite state");
            AuxiliarySteps = new AuxiliaryStep[steps.Length];
            for (int i = 0; i < steps.Length; i++) {
                AuxiliarySteps[i] = new AuxiliaryStep(steps[i].Item1, steps[i].Item2, steps[i].Item3);
            }
        }

        // Handles updating CurrentStep while keeping currentStepIsAuxiliary in sync
        private void setCurrentStep(string stepname, bool isAuxiliaryStep) {
            CurrentStep = stepname;
            currentStepIsAuxiliary = isAuxiliaryStep;
        }
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
            Completed = false;
        }
    }
    public struct AuxiliaryStep {
        public string Name;
        /// <summary>
        /// The name of the next setup step after this step
        /// </summary>
        /// <remarks>We currently don't support the next step being an auxiliary step</remarks>
        public string Next;
        public string Previous;
        public AuxiliaryStep(string name, string next, string previous) {
            Name = name;
            Next = next;
            Previous = previous;
        }
    }
    public class SetupProgressConverter : JsonConverter<SetupProgress> {
        // See HardwareTypeConverter in RendererType.cs
        public override SetupProgress ReadJson(JsonReader reader, Type objectType, SetupProgress existingValue, bool hasExistingValue, JsonSerializer serializer) {
            // load treeconfiguration, currentstep, and setupstate
            JObject jo = JObject.Load(reader);
            JToken configurationObj = jo["Configuration"];
            TreeConfiguration configuration = configurationObj.ToObject<TreeConfiguration>(serializer);
            string step = jo["Step"].Value<string>();
            string stateRaw = jo["State"].Value<string>();
            SetupState state = (SetupState)Enum.Parse(typeof(SetupState), stateRaw);
            return new SetupProgress(configuration, step, state);
        }

        public override void WriteJson(JsonWriter writer, SetupProgress value, JsonSerializer serializer) {
            // save treeconfiguration, currentstep, and setupstate
            writer.WriteStartObject();
            writer.WritePropertyName("Configuration");
            serializer.Serialize(writer, value.CurrentConfiguration);
            //might have to use serializer.Serialize(writer, value.CurrentConfiguration)
            writer.WritePropertyName("State");
            writer.WriteValue(value.CurrentState.ToString());
            writer.WritePropertyName("Step");
            writer.WriteValue(value.CurrentStep);
            writer.WriteEndObject();
        }
    }
}