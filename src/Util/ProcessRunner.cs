using System;
using System.Diagnostics;

namespace ChristmasPi.Util {
    public static class ProcessRunner {
        public static Process Popen(string command, string arguments) {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.Start();
            process.WaitForExit();
            return process;
        }
        public static string Run(string command, string arguments) {
            return Popen(command, arguments).StandardOutput.ReadToEnd();
        }
    }
}