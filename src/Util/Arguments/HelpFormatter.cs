using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Data;
using System.Threading.Tasks;
using Serilog;

namespace ChristmasPi.Util.Arguments {
    public class HelpFormatter {
        private ArgParser parser;
        private string dashstring;
        private readonly int argumentColumnWidth = 56;

        public HelpFormatter() {
            this.dashstring = new string('-', 15);
        }
        public void GiveParser(ArgParser parser) {
            this.parser = parser;
        }
        /// <summary>
        /// Gets the help message ready for printing
        /// </summary>
        /// <returns></returns>
        public string[] GetHelpLines() {
            List<string> lines = new List<string>();
            RuntimeArgument[] arguments = parser.Arguments;
            List<Section> sections = new List<Section>();
            // assign sections
            foreach (RuntimeArgument argument in arguments) {
                Section section = getSection(argument, ref sections);
                section.Add(argument);
            }
            sections = sortSections(sections);
            lines.Add(Constants.USAGE_STRING);
            for (int i = 0; i < sections.Count; i++) {
                Section section = sections[i];
                if (section.Name != "Reserved" && section.Name != "")
                    lines.Add($" {section.Name}");
                lines.Add(dashstring);
                lines.AddRange(formatSection(section));
                if (i != sections.Count - 1)
                    lines.Add(String.Empty);     // Add new line at the end of the section
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Prints the help message and exits
        /// </summary>
        public void PrintHelp() {
            foreach (string line in GetHelpLines()) {
                Console.WriteLine(line);
            }
            Environment.Exit(0);
        }

        private Section getSection(RuntimeArgument argument, ref List<Section> sections) {
            // search for section, if exists (return) else create new section
            Section section;
            if (argument.Section == null)
                section = sections.Where(s => s.Name == "").SingleOrDefault();
            else
                section = sections.Where(s => s.Name == argument.Section).SingleOrDefault();
            if (section == null) {
                section = new Section(argument.Section == null ? "" : argument.Section);
                sections.Add(section);
            }          
            return section;
        }

        private List<Section> sortSections(List<Section> sections) {
            List<Section> sorted = new List<Section>(sections.Count);
            Section reservedSection = sections.Where(s => s.Name == "Reserved").SingleOrDefault();
            Section emptySection = sections.Where(s => s.Name == "").FirstOrDefault();
            List<Section> rest = sections.Where(s => s != reservedSection && s != emptySection).ToList();
            if (rest != null)
                rest.Sort(delegate(Section a, Section b) {
                    if (b == null)
                        return a.Name.CompareTo(null);
                    return a.Name.CompareTo(b.Name);
                });
            sorted.Add(reservedSection);
            if (emptySection != null)
                sorted.Add(emptySection);
            if (rest != null)
                sorted.AddRange(rest);
            return sorted;
        }
        private string[] formatSection(Section section) {
            List<string> lines = new List<string>();
            foreach (RuntimeArgument arg in section.args) {
                string message = arg.HasMessage ? arg.Message : "";
                // Format argument column first
                string argument = $"-{arg.ArgKey}/--{arg.ArgKey}";
                int argColumnPaddingCount = argumentColumnWidth - argument.Length;
                if (argColumnPaddingCount > 0)
                    argument += new string(' ', argColumnPaddingCount);
                // Format Message column
                // " | " between columns
                string[] messageLines = fitMessageToLines(message);
                lines.Add($"{argument} | {messageLines[0]}");
                if (messageLines.Length > 1) {
                    for (int i = 1; i < messageLines.Length; i++) {
                        string line = messageLines[i];
                        lines.Add(String.Format("{0}| {1}", 
                                                new string(' ', argumentColumnWidth + 1), 
                                                line));
                    }
                }
            }
            return lines.ToArray();
        }
        private string[] fitMessageToLines(string message) {
            if (Console.WindowWidth < argumentColumnWidth + 10)
                return new string[] { message };
            List<string> lines = new List<string>(10);      // Max lines is 10
            int maxWidth = Console.WindowWidth + (argumentColumnWidth + 3);
            string[] wordStack = message.Split(' ');
            int stackPosition = 0;
            for (int i = 0; i < 10; i++) {
                string line = "";
                // go through wordStack until more words can't be fit into maxWidth
                for (; stackPosition < wordStack.Length; stackPosition++) {
                    string nextWord = wordStack[stackPosition];
                    if ((line + nextWord + 1).Length > maxWidth)
                        break;
                    line += nextWord + ' ';
                }
                if (line.Length != 0)
                    lines.Add(line);
                if (stackPosition == wordStack.Length - 1)
                    break;
            }
            return lines.ToArray();
        }
    }
    class Section {
        public List<RuntimeArgument> args;
        public string Name;

        public Section(string name) {
            Name = name;
            args = new List<RuntimeArgument>();
        }

        public void Add(RuntimeArgument arg) => args.Add(arg);
        public RuntimeArgument[] ToArray() => args.ToArray();
    }
}
