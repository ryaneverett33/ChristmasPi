using ChristmasPi.Data.Models;
using System.Drawing;

namespace ChristmasPi.Operations.Interfaces {
    public interface ISetupMode {
        string CurrentStep { get; }
        TreeConfiguration Configuration { get; }
        bool IsSettingUpBranches { get; }
        string GetNext(string current);
        void SetCurrentStep(string currentstep);
        bool SetHardware(RendererType rendererType, int datapin);
        bool SetLights(int lightcount, int fps, int brightness);
        bool SetBranches(Branch[] branches);
        bool SetDefaults(string animation, string mode);
        void StartSettingUpBranches();
        Color? NewBranch();
        bool RemoveBranch();
        bool NewLight();
        bool RemoveLight();
        bool InstallServiceStep(int step);
    }
}
