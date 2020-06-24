using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Util {
    public class ServiceInstaller {
        #region Singleton Methods
        private static readonly ServiceInstaller _instance = new ServiceInstaller();
        private static ServiceInstaller Instance { get { return _instance; } }
        #endregion
        private InitSystem? initSystem;
        private SystemDInstaller systemDInstaller;

        private ServiceInstaller() {
            systemDInstaller = new SystemDInstaller();
        }
        
        public static bool CanInstallService() {
            if (Instance.initSystem == null)
                Instance.initSystem = OSUtils.GetInitSystemType();
            switch (Instance.initSystem) {
                case InitSystem.systemd:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Install(int step) {
            switch (Instance.initSystem) {
                case InitSystem.systemd:
                    if (step >= Instance.systemDInstaller.Steps.Length)
                        return false;
                    return Instance.systemDInstaller.Steps[step].Item2();
                default:
                    return false;
            }
        }

        public static string[] GetSteps() {
            return Instance.systemDInstaller.Steps.Select(tup => tup.Item1).ToArray();
            switch (Instance.initSystem) {
                case InitSystem.systemd:
                    return Instance.systemDInstaller.Steps.Select(tup => tup.Item1).ToArray();
                default:
                    return null;
            }
        }
    }
    class SystemDInstaller {
        public Tuple<string, Func<bool>>[] Steps { get; private set; }

        public SystemDInstaller() {
            Steps = new Tuple<string, Func<bool>>[] {
                new Tuple<string, Func<bool>>("Copy Service Files", copyFiles),
                new Tuple<string, Func<bool>>("Example step 1", () => {return true; }),
                new Tuple<string, Func<bool>>("Example step 2", () => {return true; }),
                new Tuple<string, Func<bool>>("Example step 3", () => {return true; }),
            };
        }

        private bool copyFiles() {
            return true;
        }
    }
}
