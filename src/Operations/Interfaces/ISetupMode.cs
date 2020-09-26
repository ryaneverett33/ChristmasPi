using ChristmasPi.Data.Models;
using ChristmasPi.Util;
using ChristmasPi.Models;
using System.Drawing;

namespace ChristmasPi.Operations.Interfaces {
    public interface ISetupMode {
        string CurrentStepName { get; }
        TreeConfiguration Configuration { get; }
        SetupProgress CurrentProgress { get; }
        bool IsSettingUpBranches { get; }
        bool IsInstallingAService { get; }
        string GetNext(string current);
        void SetCurrentStep(string currentstep);
        void CompleteStep();
        bool IsStepFinished(string step);
        void Start();
        void Finish();
        bool SetHardware(RendererType rendererType, int datapin);
        bool SetLights(int lightcount, int fps, int brightness);
        bool SetBranches(Branch[] branches);
        bool SetDefaults(string animation, string mode, string color);
        void StartSettingUpBranches();
        Color? NewBranch();
        bool RemoveBranch();
        bool NewLight();
        bool RemoveLight();
        bool StartServicesInstall(bool installSchedulerService);
        ServiceStatusModel GetServicesInstallProgress();
        string SaveSetupProgress();
        bool IsRebootRequired();
        bool Reboot();
    }
}
