using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using ChristmasPi.Data.Models;
using ChristmasPi.Data;
using ChristmasPi.Util;
using Serilog;

namespace ChristmasPi.Util {
    public class ServiceInstaller : IDisposable {

        /*
            Installation Process
                1. Check if requested service is already installed
                    Yes: Uninstall service
                2. Copy service file to systemd directory
                3. Systemd enable service
                4. Systemd start service
        */
        
        public delegate void InstallSuccessHandler(ServiceInstallState state);
        public delegate void InstallFailureHandler(ServiceInstallState state);
        public delegate void InstallProgressHandler(ServiceInstallState state);

        // Executed after install succeeded
        public InstallSuccessHandler OnInstallSuccess;
        // Executed after install failed
        public InstallFailureHandler OnInstallFailure;
        // Executed after installation has made progress
        public InstallProgressHandler OnInstallProgress;

        private InstallationStatus status;
        private OutputWriter output;
        
        private object locker;
        private Thread installerThread;
        private string serviceName;
        private string servicePath;
        private bool isDisposed;
        public bool RebootRequired { get; private set;}
        public ServiceInstaller(string name, string path) {
            status = InstallationStatus.Waiting;
            locker = new object();
            output = new OutputWriter();
            serviceName = name;
            servicePath = path;
            isDisposed = false;
            RebootRequired = false;
        }

        // Starts the installation process with progress reported to Progress
        public void StartInstall() {
            installerThread = new Thread(installerMonitor);
            installerThread.Start();
        }

        // Safely gets the progress object
        public InstallationStatus GetStatus() {
            lock (locker) {
                return status;
            }
        }

        public OutputWriter GetWriter() {
            lock (locker) {
                return output;
            }
        }

        protected ServiceInstallState getState() => new ServiceInstallState() { ServiceName = serviceName, Status = status };

        private void installerMonitor() {
            try {
                startInstall();
                bool installResult = ConfigurationManager.Instance.RuntimeConfiguration.UseServiceInstallerStub ? installerStub() : installer();
                if (installResult) {
                    Log.ForContext("ClassName", "ServiceInstaller").Debug("Success");
                    finishInstall();
                    OnInstallSuccess.Invoke(getState());
                }
                else {
                    failedInstall();
                    OnInstallFailure.Invoke(getState());
                }
            }
            catch (Exception e) {
                writeline("Installer failed with exception {0}", e.Message);
                writeline(e.StackTrace);
                failedInstall();
                OnInstallFailure.Invoke(getState());
            }
        }

        private bool installerStub() {
            writeline("Starting installation process");
            writeline("Info\t\tname: {0}, path: {1}", this.serviceName, this.servicePath);
            OnInstallProgress.Invoke(getState());
            PIDFile.Save();
            writeline("Wrote PID file");
            Thread.Sleep(1500);
            for (int i = 0; i < 10; i++) {
                writeline("Step {0}", i);
                OnInstallProgress.Invoke(getState());
                Thread.Sleep(750);
            }
            writeline("Finished installation process");
            OnInstallProgress.Invoke(getState());
            RebootRequired = true;
            writeline("Reboot required");
            OnInstallProgress.Invoke(getState());
            return true;
        }

        private bool installer() {
            writeline("Starting installation process");
            writeline("Info\t\tname: {0}, path: {1}", this.serviceName, this.servicePath);
            OnInstallProgress.Invoke(getState());
            InitSystem initSystem = OSUtils.GetInitSystemType();
            if (initSystem != InitSystem.systemd) {
                writeline("Init System {0} is not supported.", initSystem);
                OnInstallProgress.Invoke(getState());
                return false;
            }
            if (isServiceInstalled()) {
                writeline("Service already installed, exiting");
                OnInstallProgress.Invoke(getState());
                return true;
            }
            writeline("Copying service files");
            if (!copyServiceFile()) {
                writeline("Failed to copy service files");
                OnInstallProgress.Invoke(getState());
                return false;
            }
            writeline("Enabling service");
            if (!enableService()) {
                writeline("Failed to enable service");
                OnInstallProgress.Invoke(getState());
                return false;
            }
            writeline("Starting {0} service", this.serviceName);
            if (!startService()) {
                writeline("Failed to start service");
                OnInstallProgress.Invoke(getState());
                return false;
            }
            return true;
        }

        private bool isServiceInstalled() {
            // systemctl list-units --full -all | grep "cron.service"
            /* Success
UNIT         LOAD   ACTIVE SUB     DESCRIPTION                                 
cron.service loaded active running Regular background program processing daemon

LOAD   = Reflects whether the unit definition was properly loaded.
ACTIVE = The high-level unit activation state, i.e. generalization of SUB.
SUB    = The low-level unit activation state, values depend on unit type.

1 loaded units listed.
To show all installed unit files use 'systemctl list-unit-files'.
            */
            /* Failure
Running command systemctl with list-units --full -all | grep "cron2.service"
0 loaded units listed.
To show all installed unit files use 'systemctl list-unit-files'.
            */
            return false;
        }
        private bool uninstallService() {
            // systemctl show -p FragmentPath cron.service
            // FragmentPath=/lib/systemd/system/cron.service
            // find servicefile
            // rm servicefile
            // disable service
            // daemon reload
            return false;
        }
        private bool copyServiceFile() {
            // copy service file to /etc/systemd/system/
            return false;
        }
        private bool enableService() {
            // systemctl enable service.service
            return false;
        }
        private bool startService() {
            // systemctl start service.service
            return false;
        }
        private bool daemonReload() {
            // systemctl daemon-reload
            // Success should be an empty output
            return false;
        }

        private void startInstall() {
            lock (locker) {
                status = InstallationStatus.Installing;
            }
        }
        private void failedInstall() {
            lock (locker) {
                status = InstallationStatus.Failed;
            }
        }
        private void finishInstall() {
            lock(locker) {
                status = InstallationStatus.Success;
            }
        }        
        private void writeline(string format, params object[] args) {
            lock (locker) {
                output.WriteLine(format, args);
            }
        }
        private void writeline(string line) {
            lock (locker) {
                output.WriteLine(line);
            }
        }
        private void write(string format, params object[] args) {
            lock (locker) {
                output.Write(format, args);
            }
        }
        private void write(string line) {
            lock (locker) {
                output.Write(line);
            }
        }

        /// TODO
        public void Dispose() {
            if (!isDisposed) {
                isDisposed = true;
            }
        }
    }
}