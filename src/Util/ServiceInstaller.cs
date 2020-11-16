using System;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
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

        // Regexes
        private Regex LoadedUnitsRegex = new Regex(@"([0-9]+) loaded units listed\.");
        public ServiceInstaller(string service) {
            if (!File.Exists(service)) {
                throw new FileNotFoundException($"Unable to find {service} in the installation directory, cannot install");
            }
            status = InstallationStatus.Waiting;
            locker = new object();
            output = new OutputWriter();
            serviceName = Path.GetFileNameWithoutExtension(service);
            servicePath = service;
            isDisposed = false;
            RebootRequired = false;
        }

        // Starts the installation process with progress reported to Progress
        public void StartInstall() {
            if (isDisposed || status != InstallationStatus.Waiting) {
                Log.ForContext<ServiceInstaller>().Error("Can't start install for service {serviceName}, already ran", serviceName);
                Log.ForContext<ServiceInstaller>().Debug("ServiceInstaller status: {status}, disposed? {isDisposed}", status, isDisposed);
                throw new Exception("Can't install service, already installed");
            }
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
                    Log.ForContext<ServiceInstaller>().Debug("Success");
                    finishInstall();
                    OnInstallSuccess.Invoke(getState());
                }
                else {
                    Log.ForContext<ServiceInstaller>().Debug("Failure");
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
            writeline("Wrote PID file");
            Thread.Sleep(1500);
            for (int i = 0; i < 10; i++) {
                writeline("Step {0}", i);
                Thread.Sleep(750);
            }
            writeline("Finished installation process");
            RebootRequired = true;
            writeline("Reboot required");
            return true;
        }

        private bool installer() {
            writeline("Starting installation process");
            writeline("Info\t\tname: {0}, path: {1}", this.serviceName, this.servicePath);
            InitSystem initSystem = OSUtils.GetInitSystemType();
            if (initSystem != InitSystem.systemd) {
                writeline("Init System {0} is not supported.", initSystem);
                RebootRequired = false;
                return false;
            }
            if (isServiceInstalled()) {
                writeline("Service already installed, exiting");
                return true;
            }
            writeline("Copying service files");
            copyServiceFile();
            writeline("Enabling service");
            if (!enableService()) {
                writeline("Failed to enable service");
                return false;
            }
            writeline("Starting {0} service", this.serviceName);
            if (!startService()) {
                writeline("Failed to start service");
                return false;
            }
            RebootRequired = true;
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
            writeline($"systemctl list-units --full -all | grep \"{servicePath}\"");
            string output = ProcessRunner.Run("systemctl", $"list-units --full -all | grep \"{servicePath}\"");
            writeline(output);
            Match outputMatch = LoadedUnitsRegex.Match(output);
            if (outputMatch.Success) {
                int servicesInstalled = int.Parse(outputMatch.Groups[1].Value);
                return servicesInstalled > 0;
            }
            else
                throw new Exception("Unable to get program output");
        }
        private bool uninstallService() {
            // systemctl show -p FragmentPath cron.service
            // FragmentPath=/lib/systemd/system/cron.service
            // find servicefile
            // rm servicefile
            // disable service
            // daemon reload
            throw new NotImplementedException();
        }
        private void copyServiceFile() {
            // copy service file to /etc/systemd/system/
            if (File.Exists($"/etc/systemd/system/{servicePath}"))
                throw new Exception("Service file already exists, cannot install new service file");
            File.Copy(servicePath, $"/etc/systemd/system/{servicePath}");
            writeline($"Copying {servicePath}");
        }
        private bool enableService() {
            // systemctl enable service.service
            writeline($"systemctl enable {servicePath}");
            Process process = ProcessRunner.Popen("systemctl", $"enable {servicePath}");
            return process.ExitCode == 0;
        }
        private bool startService() {
            // systemctl start service.service
            writeline($"systemctl start {servicePath}");
            Process process = ProcessRunner.Popen("systemctl", $"start {servicePath}");
            return process.ExitCode == 0;
        }
        private bool daemonReload() {
            // systemctl daemon-reload
            writeline("systemctl daemon-reload");
            Process process = ProcessRunner.Popen("systemctl", "daemon-reload");
            return process.ExitCode == 0;
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
                Log.ForContext<ServiceInstaller>().Debug($"\t{String.Format(format, args)}");
                OnInstallProgress.Invoke(getState());
            }
        }
        private void writeline(string line) {
            lock (locker) {
                output.WriteLine(line);
                Log.ForContext<ServiceInstaller>().Debug($"\t{line}");
                OnInstallProgress.Invoke(getState());
            }
        }
        private void write(string format, params object[] args) {
            lock (locker) {
                output.Write(format, args);
                Log.ForContext<ServiceInstaller>().Debug($"\t{String.Format(format, args)}");
                OnInstallProgress.Invoke(getState());
            }
        }
        private void write(string line) {
            lock (locker) {
                output.Write(line);
                Log.ForContext<ServiceInstaller>().Debug($"\t{line}");
                OnInstallProgress.Invoke(getState());
            }
        }

        public void Dispose() {
            if (!isDisposed) {
                isDisposed = true;
                OnInstallFailure = null;
                OnInstallProgress = null;
                OnInstallSuccess = null;
                status = InstallationStatus.Success | InstallationStatus.Failed;
                output = null;
                if (installerThread.IsAlive)
                    installerThread.Abort();
            }
        }
    }
}