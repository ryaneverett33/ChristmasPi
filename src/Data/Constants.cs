using System;
using System.Drawing;

namespace ChristmasPi.Data {
    public class Constants {
        // Hardware
        public static readonly int FPS_MAX = 100;
        public static readonly int FPS_DEFAULT = 30;
        public static readonly int LIGHTS_MAX = 1000;
        public static readonly Color INDICATION_COLOR = Color.Lime;

        // Regexs
        public static readonly string REGEX_RGB_FORMAT = @"rgb\((\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *),(\ *[0-9]{1,3}\ *)\)";
        public static readonly string REGEX_HEX_FORMAT = @"#[0-9a-fA-F]{3,6}";
        public static readonly string REGEX_HEX_WEB_FORMAT = @"[0-9a-fA-F]{3,6}";
        public static readonly string REGEX_AMPM_FORMAT = @"(0|1){0,1}[0-9]:[0-5][0-9] *((AM)|(PM))";
        public static readonly string REGEX_24HR_FORMAT = @"(0|1|2){0,1}[0-9]:[0-5][0-9]";
        public static readonly string REGEX_PID_FORMAT = @"pid:[0-9]+";

        // Operations
        public static readonly string DEFAULT_OPERATING_MODE = "SolidColorMode";
        public static readonly string DEFAULT_ANIMATION = "Twinkle";
        public static readonly int ACTIVATION_TIMEOUT = 1000;           // in ms
        public static readonly int SCHEDULER_MAX_ATTEMPTS = 5;
        public static readonly int REBOOT_MAX_ATTEMPTS = 30 * 10;      // 30s at 10 polls a second
        public static readonly TimeSpan SCHEDULER_ERR_SLEEP = new TimeSpan(0, 30, 0);
        public static readonly TimeSpan SCHEDULER_LONG_SLEEP = new TimeSpan(1, 0, 0);
        public static readonly TimeSpan REBOOT_POLL_SLEEP = new TimeSpan(0, 0, 0, 0, 100);      // 100 ms
        public static readonly TimeSpan REBOOT_DELAY_SLEEP = new TimeSpan(0, 0, 1);

        // Colors
        public static readonly int RANDOM_DEFAULT = 255;
        public static readonly int RANDOM_MAX = 255;                    // exclusive range
        public static readonly int RANDOM_MIN = 0;
        public static readonly Color COLOR_OFF = Color.Black;
        public static readonly Color DEFAULT_COLOR = Util.ColorConverter.Convert("#db7b26");

        // Files
        public static readonly string CONFIGURATION_FILE = "configuration.json";
        public static readonly string CONFIGURATION_FILE_OLD = "configuration.old.json";
        public static readonly string SCHEDULE_FILE = "schedule.json";
        public static readonly string SCHEDULE_FILE_OLD = "schedule.old.json";
        public static readonly string LOG_FILE = "ChristmasPi.log";
        public static readonly string ASP_LOG_FILE = "aspnet.log";
        public static readonly string SETUP_PROGRESS_FILE = "Setup.Progress.json";
        public static readonly string PID_FILE = "ChristmasPi.pid";

        // Networking
        public static readonly int PORT = 50808;                        // set in asp.net configuration

        // Misc
        public static readonly string USAGE_STRING = "Usage: dotnet ChristmasPi.dll [OPTION]";
        public static readonly string LOG_FORMAT = "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}";
        public static readonly int EXIT_REBOOT = 10;
    }
}
