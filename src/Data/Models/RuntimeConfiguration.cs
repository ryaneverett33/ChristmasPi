using System;
using ChristmasPi.Util.Arguments;

namespace ChristmasPi.Data.Models {
    public class RuntimeConfiguration {
        [HelpSection("Debug")]
        [Argument("tr", "Allows the test renderer to be selected during setup", true)]
        public bool AllowTestRenderer;

        [HelpSection("Debug")]
        [Argument("ignorepriv", "Ignore admin privilege requirement", true)]
        public bool IgnorePrivileges;

        [HelpSection("Debug")]
        [Argument("debug", "Sets the logging level to debug", true)]
        public bool DebugLogging;

        [HelpSection("Debug")]
        [Argument("no-asp-logging", "Don't log ASP.NET data", true)]
        public bool NoASPLogging;

        [HelpSection("Debug")]
        [Argument("no-redirect-logging", "Don't log RedirectHandler data", true)]
        public bool NoRedirectLogging;

        [HelpSection("Debug")]
        [Argument("ignore-restart", "Ignore any restart attempts", true)]
        public bool IgnoreRestarts;

        [HelpSection("Debug")]
        [Argument("daemon-log-console", "Allow console logging in daemon mode", true)]
        public bool DaemonLogToConsole;

        [HelpSection("Debug")]
        [Argument("allow-debug-animations", "Allow viewing and playback of debug animations", true)]
        public bool AllowDebugAnimations;

        [HelpSection("Debug")]
        [Argument("use-service-installer-stub", "Use a stub for the service installer", true)]
        public bool UseServiceInstallerStub;

        [Argument("daemon", "Starts the server in a daemon configuration", true)]
        public bool DaemonMode;

        public RuntimeConfiguration() {}
    }
}
