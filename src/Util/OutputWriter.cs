using System;
using System.Collections.Generic;
using ChristmasPi.Data.Extensions;

namespace ChristmasPi.Util {
    public class OutputWriter {
        // Format specifiers
        private readonly int TAB_SPACES = 4;
        private List<string> lines;
        public OutputWriter() {
            lines = new List<string>();
        }

        public void Write(string format, params object[] args) {
            Write(String.Format(format, args));
        }
        public void Write(string message) {
            string[] newLines;
            if (lines.Contains(Environment.NewLine))
                newLines = message.Split(Environment.NewLine);
            else
                newLines = new string[1] { message };
            for (int i = 0; i < newLines.Length; i++) {
                if (newLines[i].Contains('\t'))
                    newLines[i] = newLines[i].Replace("\t", new string(' ', TAB_SPACES));
            }
            if (newLines.Length == 1) {
                if (lines.Count == 0)
                    lines.Add(newLines[0]);
                else
                    lines[lines.Count - 1] = lines[lines.Count - 1] + newLines[0];
            }
            else {
                if (lines.Count == 0)
                    lines.AddRange(newLines);
                else {
                    string[] newLinesSubset = (string[])newLines.Subset(1, newLines.Length - 1);
                    lines[lines.Count - 1] = lines[lines.Count - 1] + newLines[0];
                    lines.AddRange(newLinesSubset);
                }
            }
        }
        
        public void WriteLine(string format, params object[] args) {
            WriteLine(String.Format(format, args));
        }
        public void WriteLine(string line) {
            string[] newLines;
            if (line.Contains(Environment.NewLine))
                newLines = line.Split(Environment.NewLine);
            else
                newLines = new string[1] { line };
            for (int i = 0; i < newLines.Length; i++) {
                if (newLines[i].Contains('\t'))
                    newLines[i] = newLines[i].Replace("\t", new string(' ', TAB_SPACES));
            }
            lines.AddRange(newLines);
        }

        public override string ToString() {
            string message = "";
            for (int i = 0; i < lines.Count; i++) {
                message += lines[i];
                if (i != lines.Count - 1)
                    message += '\n';
            }
            return message;
        }
    }
}