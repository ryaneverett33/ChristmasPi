using System;
using System.Collections.Generic;

namespace ChristmasPi.Util {
    public class InstallationProgress {
        public List<string> Lines;
        public InstallationStatus Status;

        public InstallationProgress() {
            Lines = new List<string>();
            Status = InstallationStatus.Waiting;
        }
        
        public void WriteLine(string format, params object[] args) {
            WriteLine(String.Format(format, args));
        }
        public void WriteLine(string s) {
            Lines.Add(s);
        }
        
        public string[] GetLines => Lines == null ? new string[]{} : Lines.ToArray();
        public void StartInstall() => Status = InstallationStatus.Installing;
        public void FailedInstall() => Status = InstallationStatus.Failed;
        public void FinishInstall() => Status = InstallationStatus.Success;
        public bool IsInstalling() => Status == InstallationStatus.Installing;
        public bool HasFailed() => Status == InstallationStatus.Failed;
        public bool HasSucceeded() => Status == InstallationStatus.Success;
    }
    public enum InstallationStatus {
        Installing,         // Currently installing
        Failed,             // Failed to install
        Success,            // Successfully installed
        Waiting             // Waiting to start installation (waiting for StartInstall to be called)
    }
}