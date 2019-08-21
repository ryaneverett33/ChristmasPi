﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Server {
    class Logger {
        FileStream log;
        bool created;
        string fileName;
        DateTime dt = DateTime.Now;
        public Logger() {
            log = null;
            created = false;
            fileName = null;
        }
        public void createNewLog(string name) {
            DateTime dt = DateTime.Now.Date;
            try {
                log = File.Create(name + ".log");
                fileName = name + ".log";
                created = true;
            }
            catch (Exception e) {
                created = false;
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to create log: " + name);
            }
            if (created) {
                string head = "Created log file at " + dt.ToLongTimeString() + "\n";
                byte[] data = Encoding.UTF8.GetBytes(head);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
        public void createNewLog() {
            string date = dt.Month + "-" + dt.Day + "-" + dt.Year + "-" + dt.Hour + "-" + dt.Minute;
            try {
                log = File.Create(date + ".log");
                fileName = date + ".log";
                created = true;
            }
            catch (Exception e) {
                created = false;
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to create New Log");
            }
            if (created) {
                string head = "Created log file at " + dt.ToLongTimeString() + Environment.NewLine;
                byte[] data = Encoding.UTF8.GetBytes(head);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
        public void writeLine(string message) {
            // Generic Info Line
            // No Date included
            if (created) {
                byte[] data = Encoding.UTF8.GetBytes(message + Environment.NewLine);
                log.Write(data, 0, data.Count());
                log.Flush();
                //File.AppendAllText(fileName, message + Environment.NewLine);
            }            
        }
        public void write(string message) {
            // Generic Info Write
            // No Date or new line included
            if (created) {
                byte[] data = Encoding.UTF8.GetBytes(message);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
        public void logWarning(string message) {
            // Warning message
            // format - [WARNING] TIME - <message>
            // This is a non-fatal warning
            if (created) {
                string date = dt.Hour + "-" + dt.Minute + "-" + dt.Second;
                byte[] data = Encoding.UTF8.GetBytes("[WARNING] " + date + " " + message + Environment.NewLine);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
        public void logError(string message) {
            // Error message
            // format - [ERROR] TIME - <message>
            // This is a non-fatal warning
            if (created) {
                string date = dt.Hour + "-" + dt.Minute + "-" + dt.Second;
                byte[] data = Encoding.UTF8.GetBytes("[ERROR] " + date + " " + message + Environment.NewLine);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
        public void logFatal(string message) {
            // Fatal message
            // format - [FATAL] TIME - <message>
            // This message must be followed by an application exit.
            // Logging will cease after this message
            if (created) {
                string date = dt.Hour + "-" + dt.Minute + "-" + dt.Second;
                byte[] data = Encoding.UTF8.GetBytes("[FATAL] " + date + " " + message + Environment.NewLine);
                log.Write(data, 0, data.Count());
                log.Flush();
                created = false;
            }
        }
        public void logOK(string message) {
            // Message indicating something has succeeded, a good indication
            // format - [OK] TIME - <message>
            // This is a non-fatal warning
            if (created) {
                string date = dt.Hour + "-" + dt.Minute + "-" + dt.Second;
                byte[] data = Encoding.UTF8.GetBytes("[OK] " + date + " " + message + Environment.NewLine);
                log.Write(data, 0, data.Count());
                log.Flush();
            }
        }
    }
}
