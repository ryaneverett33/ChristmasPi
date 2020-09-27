namespace ChristmasPi.Util {
    public enum InstallationStatus {
        Installing,         // Currently installing
        Failed,             // Failed to install
        Success,            // Successfully installed
        Waiting,            // Waiting to start installation (waiting for StartInstall to be called)
        Rebooting           // The server is rebooting
    }
}