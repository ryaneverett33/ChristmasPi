using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Util.Arguments {
    public class ArgumentHelper {

        private ArgParser parser;
        private HelpFormatter helpFormatter;
        public ArgumentHelper(object results) {
            helpFormatter = new HelpFormatter();
            parser = new ArgParser(results, helpFormatter);
            helpFormatter.GiveParser(parser);
        }

        public bool Parse(string[] args) {
            return parser.Parse(args);
        }
    }
}
