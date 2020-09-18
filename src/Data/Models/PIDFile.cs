using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Serilog;
using ChristmasPi.Data;

namespace ChristmasPi.Data.Models {
    public static class PIDFile {
        /// <summary>
        /// Saves the current pid to a file
        /// </summary>
        public static void Save() {
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
                        Log.ForContext("ClassName", "PIDFile").Error(writeerr, "Failed writing pidfile");
                        throw;
                    }
                }
            }
            catch (UnauthorizedAccessException autherr) {
                Log.ForContext("ClassName", "PIDFile").Error(autherr, "Unable to modify pidfile, unauthorized exception");
                throw;
            }
        }
        /// <summary>
        /// Loads the PID from a pid file
        /// </summary>
        /// <returns>A null value if the pid file doesn't exist, else the pid stored in the file</returns>
        /// <seealso cref="Consume" />
        public static int? Load() {
            if (File.Exists(Constants.PID_FILE)) {
                string fileContents = null;
                try {
                    using (FileStream stream = File.OpenRead(Constants.PID_FILE)) {
                        using (StreamReader reader = new StreamReader(stream)) {
                            fileContents = reader.ReadLine();
                        }
                    }
                }
                catch (UnauthorizedAccessException autherr) {
                    Log.ForContext("ClassName", "PIDFile").Error(autherr, "Unable to access pidfile, unauthorized exception");
                    throw;
                }
                catch (IOException ioerr) {
                    Log.ForContext("ClassName", "PIDFile").Error(ioerr, "IO error reading from pidfile");
                    throw;
                }
                if (fileContents == null) {
                    Log.ForContext("ClassName", "PIDFile").Error("Pidfile was empty, read no data from it");
                    return null;
                }
                if (!Regex.IsMatch(fileContents, Constants.REGEX_PID_FORMAT)) {
                    Log.ForContext("ClassName", "PIDFile").Error("Pidfile is corrupted, unable to parse contents");
                    return null;
                }
                string[] keyValue = fileContents.Split(':');
                return int.Parse(keyValue[1]);
            }
            return null;
        }
        /// <summary>
        /// Destroys the current pid file after it's been loaded
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no pidfile to consume or it has already been consumed</exception>
        public static void Consume() {
            if (File.Exists(Constants.PID_FILE)) {
                try {
                    File.Delete(Constants.PID_FILE);
                }
                catch (IOException ioerr) {
                    Log.ForContext("ClassName", "PIDFile").Error(ioerr, "Unable to consume pidfile, an IO error occurred");
                    throw;
                }
                catch (UnauthorizedAccessException autherr) {
                    Log.ForContext("ClassName", "PIDFile").Error(autherr, "Unable to consume pidfile, an unauthorized error occurred");
                    throw;
                }
            }
            else
                throw new InvalidOperationException("There is no pidfile to consume");
        }
    }
}