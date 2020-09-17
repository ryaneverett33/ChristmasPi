using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using ChristmasPi.Data;
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
        public ServiceInstaller(string name, string path) {
            status = InstallationStatus.Waiting;
            locker = new object();
            output = new OutputWriter();
            serviceName = name;
            servicePath = path;
            isDisposed = false;
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
                if (installer()) {
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

        private bool installer() {
            writeline("Starting installation process");
            writeline("Info\t\tname: {0}, path: {1}", this.serviceName, this.servicePath);
            OnInstallProgress.Invoke(getState());
            writePIDFile();
            writeline("Wrote PID file");
            Thread.Sleep(1500);
            for (int i = 0; i < 10; i++) {
                writeline("Step {0}", i);
                OnInstallProgress.Invoke(getState());
                Thread.Sleep(750);
            }
            writeline("Finised installation process");
            OnInstallProgress.Invoke(getState());
            return true;
        }

        private bool isServiceInstalled() {
            // systemctl list-units --full -all | grep "cron.service"
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
            return false;
        }
        // Save current process PID to a file
        private void writePIDFile() {
            int pid = Process.GetCurrentProcess().Id;
            string fileContents = String.Format("pid:{0}", pid);
            try {
                if (File.Exists(Constants.PID_FILE))
                    File.Delete(Constants.PID_FILE);
                using (StreamWriter writer = File.CreateText(Constants.PID_FILE)) {
                    try {
                        writer.WriteLine(fileContents);
                        writer.Flush();
                    }
                    catch (IOException writeerr) {
                        Log.ForContext<ServiceInstaller>().Error(writeerr, "Failed writing pid file");
                        throw;
                    }
                }
            }
            catch (UnauthorizedAccessException autherr) {
                Log.ForContext<ServiceInstaller>().Error(autherr, "Unable to modify file, unauthorized exception");
                throw;
            }
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