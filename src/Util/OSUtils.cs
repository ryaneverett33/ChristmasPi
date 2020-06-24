using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;

namespace ChristmasPi.Util {
    public static class OSUtils {

        [DllImport("libc")]
        private static extern uint getuid();

        /// <summary>
        /// Checks if the program is running with administrative priviledges
        /// </summary>
        /// <returns>True if running as admin, false else</returns>
        /// <see cref="https://stackoverflow.com/a/3600338"/>
        public static bool IsAdmin() {
            if (IsNix()) {
                return getuid() == 0;
            }
            else {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
        }

        /// <summary>
        /// Checks whether running on a nix system
        /// </summary>
        /// <returns>True if current OS is linux/unix, false if else</returns>
        public static bool IsNix() {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        /// <summary>
        /// Determines the current running init system
        /// </summary>
        /// <returns>An InitSystem enum of the proper system, NotSupported if else</returns>
        /// <seealso cref="InitSystem"/>
        /// <see cref="https://unix.stackexchange.com/q/196166"/>
        /// <remarks>This is a lazy, and not very accurate (see thread), way of determining systems. When multiple systems are detected
        /// certain systems are given higher priority.</remarks>
        public static InitSystem GetInitSystemType() {
            bool usesSystemD = Directory.Exists("/usr/lib/systemd");
            bool usesUpstart = Directory.Exists("/usr/share/upstart");
            bool usesInitD = Directory.Exists("/etc/init.d");
            if (usesSystemD && usesUpstart && usesInitD)
                return InitSystem.systemd;
            else if ((usesSystemD && usesUpstart) || (usesSystemD && usesInitD))
                return InitSystem.systemd;
            else if (usesUpstart && usesInitD)
                return InitSystem.SysV;
            else if (usesSystemD)
                return InitSystem.systemd;
            else if (usesUpstart)
                return InitSystem.upstart;
            else if (usesInitD)
                return InitSystem.SysV;
            return InitSystem.NotSupported;
        }
    }
    /// <summary>
    /// The daemon/process init system
    /// </summary>
    /// <see cref="https://freedesktop.org/wiki/Software/systemd/"/>
    /// <see cref="http://glennastory.net/boot/init.html"/>
    /// <see cref="http://upstart.ubuntu.com/"/>
    public enum InitSystem {
        systemd,
        upstart,
        SysV,
        NotSupported
    }
}
