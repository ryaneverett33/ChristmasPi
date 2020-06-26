using System;
using System.Diagnostics;
using System.Threading;

namespace ChristmasPi.Util {
    public class ServiceInstaller {

        /*
            Installation Process
                1. Check if requested service is already installed
                    Yes: Uninstall service
                2. Copy service file to systemd directory
                3. Systemd enable service
                4. Systemd start service
        */
        private InstallationProgress Progress;
        private object locker;
        private Thread installerThread;
        private string serviceName;
        private string servicePath;
        public ServiceInstaller(string name, string path) {
            Progress = new InstallationProgress();
            locker = new object();
        }

        // Starts the installation process with progress reported to Progress
        public void StartInstall() {
            installerThread = new Thread(installerMonitor);
            installerThread.Start();
        }

        // Safely gets the progress object
        public InstallationProgress GetProgress() {
            InstallationProgress progressHolder;
            lock (locker) {
                progressHolder = Progress;
            }
            return progressHolder;
        }

        private void writeline(string format, params object[] args) {
            lock (locker) {
                Progress.WriteLine(format, args);
            }
        }
        private void writeline(string line) {
            lock (locker) {
                Progress.WriteLine(line);
            }
        }

        private void installerMonitor() {
            try {
                lock (locker) {
                    Progress.StartInstall();
                }
                installer();
                lock (locker) {
                    Progress.FinishInstall();
                }
            }
            catch (Exception e) {
                writeline("Installer failed with exception {0}", e.Message);
                writeline(e.StackTrace);
                lock (locker) {
                    Progress.FailedInstall();
                }
            }
        }

        private void installer() {
            writeline("Starting installation process");
            writeline("Info\t\tname: {0}, path: {1}", serviceName, servicePath);
            Thread.Sleep(1500);
            for (int i = 0; i < 10; i++) {
                writeline("Step {0}", i);
                Thread.Sleep(750);
            }
            writeline("Finised installation process");
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
    }
}