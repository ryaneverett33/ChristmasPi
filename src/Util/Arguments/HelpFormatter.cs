using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Data;
using System.Threading.Tasks;

namespace ChristmasPi.Util.Arguments {
    public class HelpFormatter {
        private ArgParser parser;
        private string dashstring;

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
            foreach (Section section in sections) {
                if (section.Name != "Reserved" && section.Name != "")
                    lines.Add($" {section.Name}");
                lines.Add(dashstring);
                lines.Add("");
                foreach (RuntimeArgument arg in section.args) {
                    string message = arg.HasMessage ? arg.Message : "";
                    lines.Add($"-{arg.ArgKey}/--{arg.ArgKey} \t\t\t {message}");
                }
                lines.Add("");
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
            if (section == null)
                section = new Section(argument.Section == null ? "" : argument.Section);
            sections.Add(section);
            return section;
        }

        private List<Section> sortSections(List<Section> sections) {
            List<Section> sorted = new List<Section>(sections.Count);
            Section reservedSection = sections.Where(s => s.Name == "Reserved").SingleOrDefault();
            Section emptySection = sections.Where(s => s.Name == "").First();
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
