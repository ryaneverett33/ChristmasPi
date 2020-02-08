using ChristmasPi.Data.Models;

namespace ChristmasPi.Operations.Interfaces {
    public interface ISetupMode {
        string CurrentStep { get; }
        string GetNext(string current);
        void SetCurrentStep(string currentstep);
        bool SetHardware(RendererType rendererType, int datapin);
        bool SetLights(int lightcount, int fps, int brightness);
    }
}
