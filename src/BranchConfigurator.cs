using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Data;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Models;

namespace ChristmasPi.BranchConfigurator {
    class Program {
        static void Main(string[] args) {
            BranchConfigurator b = new BranchConfigurator(args);
            b.Run();
        }
    }
    public class BranchConfigurator {
        public string ConfigurationLoc { get; private set; }
        private IRenderer renderer;
        private List<Color> usedColors;
        private List<ActiveBranchData> data;
        private Color indicationColor = Color.Lime;

        public BranchConfigurator(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("--config", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-config", StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 < args.Length) {
                        ConfigurationLoc = args[i + 1];
                        i++;
                        continue;
                    }
                    Console.WriteLine("Invalid config argument");
                    printHelp();
                }
                else if (args[i].Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("help", StringComparison.CurrentCultureIgnoreCase))
                    printHelp();
            }
            if (ConfigurationLoc == null)
                ConfigurationLoc = Constants.CONFIGURATION_FILE;
            ConfigurationLoc = Path.GetFullPath(ConfigurationLoc);
            ConfigurationManager.Instance.LoadConfiguration(ConfigurationLoc);
            renderer = RenderFactory.GetRenderer();
            usedColors = new List<Color>();
            data = new List<ActiveBranchData>();
        }
        public void Run() {
            renderer.Start();
            data.Add(new ActiveBranchData(0, getColor()));
            int currentBranch = 0;
            drawTree();
            for (int i = 1; i < renderer.LightCount; i++) {
                bool validInput = false;
                while (!validInput) {
                    // prompt for user input
                    Console.WriteLine($"Is Light {i + 1} on a new branch? [yes/no]");
                    string response = Console.ReadLine();
                    if (response.Equals("yes", StringComparison.CurrentCultureIgnoreCase) || response.Equals("y", StringComparison.CurrentCultureIgnoreCase)) {
                        // add new branch
                        ActiveBranchData newBranch = new ActiveBranchData(i, getColor());
                        data.Add(newBranch);
                        currentBranch++;
                        validInput = true;
                    }
                    else if (response.Equals("no", StringComparison.CurrentCultureIgnoreCase) || response.Equals("n", StringComparison.CurrentCultureIgnoreCase)) {
                        // add to current branch
                        ActiveBranchData current = data[currentBranch];
                        current.Add();
                        data[currentBranch] = current;
                        validInput = true;
                    }
                    else {
                        Console.WriteLine("Acceptable input: 'yes' or 'no'");
                        validInput = false;
                    }
                }
                drawTree();
            }
            storeData();
            Console.WriteLine($"Saving {data.Count} branches");
            ConfigurationManager.Instance.SaveConfiguration(ConfigurationLoc);
        }

        /// <seealso cref="ChristmasPi.Models.ScheduleModel.getNewColor()"/>
        private Color getColor() {
            bool contains = true;
            Color color = Color.Aquamarine;
            while (contains) {
                color = RandomColor.RandomKnownColorGenerator();
                contains = usedColors.Contains(color) || color == Color.White || color == Color.Black;
            }
            usedColors.Add(color);
            return color;
        }

        // renders the tree
        private void drawTree() {
            ActiveBranchData currentBranch = data[0];
            int currentBranchNumber = 0;
            bool outOfBranches = false;
            for (int i = 0; i < renderer.LightCount; i++) {
                if (outOfBranches) {
                    renderer.SetLEDColor(i, Color.Black);
                }
                else if (currentBranch.Contains(i)) {
                    renderer.SetLEDColor(i, currentBranch.Color);
                }
                else {
                    // get next branch and check
                    if (currentBranchNumber + 1 < data.Count) {
                        currentBranchNumber++;
                        currentBranch = data[currentBranchNumber];
                        renderer.SetLEDColor(i, currentBranch.Color);
                    }
                    else {
                        outOfBranches = true;
                        // indicate the current led
                        renderer.SetLEDColor(i, indicationColor);
                    }
                }
            }
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }

        // store data in the current configuration
        private void storeData() {
            List<Branch> newBranchData = new List<Branch>();
            foreach (ActiveBranchData branchData in data) {
                Branch newBranch = new Branch {
                    start = branchData.Start,
                    end = branchData.End
                };
                newBranchData.Add(newBranch);
            }
            // store
            ConfigurationManager.Instance.CurrentTreeConfig.tree.branches = newBranchData;
        }

        // Prints help and exits program
        private void printHelp() {
            Console.WriteLine("Usage: Scheduler --config {schedule.json}");
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine("-h/--h\t\t\tPrints this help screen");
            Console.WriteLine("-config/--config\tUse this configuration file");
            Environment.Exit(0);
        }
    }
    public class ActiveBranchData {
        public int Start { get; }
        public int End => Start + (Count - 1);
        public int Count { get; private set; }
        public Color Color { get; }

        public ActiveBranchData(int start, Color color) {
            Start = start;
            Count = 1;
            Color = color;
        }

        public void Add() { Count++; }

        public bool Contains(int index) {
            if (index >= Start && index < Start + Count)
                return true;
            return false;
        }
    }
}
