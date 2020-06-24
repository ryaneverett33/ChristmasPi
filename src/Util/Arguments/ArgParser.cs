using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace ChristmasPi.Util.Arguments {
    public class ArgParser {
        private object resultObj;
        private HelpFormatter helpFormatter;
        private Type resultType;
        private FieldInfo[] resultFields;
        private PropertyInfo[] resultProperties;

        public RuntimeArgument[] Arguments { get; private set; }


        public ArgParser(object results, HelpFormatter helpFormatter) {
            if (results == null)
                throw new ArgumentNullException("results");
            if (helpFormatter == null)
                throw new ArgumentNullException("helpFormatter");
            this.helpFormatter = helpFormatter;
            resultObj = results;
            resultType = results.GetType();
            resultFields = resultType.GetFields();
            resultProperties = resultType.GetProperties();
            loadArguments();
        }

        // Loads field and properties with argument attributes into the Arguments property
        private void loadArguments() {
            List<RuntimeArgument> runtimeArguments = new List<RuntimeArgument>();
            // load fields and properties as runtime arguments
            foreach (FieldInfo field in resultFields) {
                if (containsAttribute(field.CustomAttributes, typeof(Argument))) {
                    Argument arg = (Argument)field.GetCustomAttribute(typeof(Argument));
                    HelpSection section = null;
                    if (containsAttribute(field.CustomAttributes, typeof(HelpSection)))
                        section = (HelpSection)field.GetCustomAttribute(typeof(HelpSection));
                    runtimeArguments.Add(new RuntimeArgument(field, arg, section));
                }
            }
            foreach (PropertyInfo property in resultProperties) {
                if (containsAttribute(property.CustomAttributes, typeof(Argument))) {
                    Argument arg = (Argument)property.GetCustomAttribute(typeof(Argument));
                    HelpSection section = null;
                    if (containsAttribute(property.CustomAttributes, typeof(HelpSection)))
                        section = (HelpSection)property.GetCustomAttribute(typeof(HelpSection));
                    runtimeArguments.Add(new RuntimeArgument(property, arg, section));
                }
            }
            runtimeArguments.AddRange(getReservedArguments());
            Arguments = runtimeArguments.ToArray();
        }

        /// <summary>
        /// Parse the input string and load result values
        /// </summary>
        /// <param name="args">Array of arguments</param>
        /// <returns>True if parsing was successful, false if parsing encountered an error</returns>
        public bool Parse(string[] args) {
            // Create a list of acceptable arguments from this.Arguments. Every time an argument is encountered during value assignment, remove from list
            // Create a list of encountered arguments
            // Parse the input array and create a list of key/value pairs (values can be null). If key is not in acceptable list, throw an error
            // Iterate through the list of encountered arguments and do value assignment, remove acceptable argument after assignment
            //      If size(acceptable) = 0 during value assignment, throw an error
            List<RuntimeArgument> acceptable = new List<RuntimeArgument>(Arguments);
            List<EncounteredPair> encountered = new List<EncounteredPair>();
            string[] cleanArgs = (from arg in args where arg.Length > 0 select arg).ToArray(); // get rid of empty spaces
            for (int i = 0; i < args.Length; i++) {
                string arg = cleanArgs[i];
                if (arg[0] == '-') {
                    string stripped;
                    if (arg[1] == '-')
                        stripped = arg.Substring(2, arg.Length - 2);
                    else
                        stripped = arg.Substring(1, arg.Length - 1);
                    // check if arg pair
                    if (i + 1 < args.Length) {
                        string nextarg = cleanArgs[i + 1];
                        if (nextarg[0] != '-') {
                            encountered.Add(new EncounteredPair(stripped, nextarg));
                            i = i + 1;
                        }
                        else {
                            // add single argument to encountered
                            encountered.Add(new EncounteredPair(stripped));
                        }
                    }
                    else {
                        // add single argument to encountered
                        encountered.Add(new EncounteredPair(stripped));
                    }
                }
                else {
                    Console.WriteLine("Invalid Argument Format");
                    return false;
                }
            }
            foreach (EncounteredPair pair in encountered) {
                RuntimeArgument argument = getArgument(pair.Key, ref acceptable);
                if (argument == null) {
                    Console.WriteLine("Unknown argument");
                    return false;
                }
                // check for reserved argument
                if (argument.IsReserved) {
                    if (handleReservedArgument(argument))
                        continue;
                    else
                        return false;
                }
                // do value assign
                if (argument.HasValue) {
                    // expecting value, so use the supplied (pair) value
                    if (pair.HasValue) {
                        // parse value
                        object value = null;
                        if (!tryParse(pair.Value, argument.Type, ref value)) {
                            Console.WriteLine($"Unable to parse string to {argument.Type} for {argument.ArgKey}");
                            return false;
                        }
                        argument.SetValue(resultObj, value);
                    }
                        
                    else {
                        Console.WriteLine($"{argument.ArgKey} requires a value");
                        return false;
                    }
                }
                else {
                    // not expecting value, use default value or type default if nothing is available
                    if (argument.HasDefaultValue)
                        argument.SetValue(resultObj, argument.DefaultValue);
                    else {
                        Console.WriteLine($"No default value specified for {argument.ArgKey}, using {argument.Type} default");
                        argument.SetValue(resultObj, default);
                    }
                }
            }
            if (acceptable.Count > 0) {
                // set the rest of the acceptable arguments to their default value
                foreach (RuntimeArgument argument in acceptable) {
                    if (argument.IsRequired) {
                        // error since value is not given
                        Console.WriteLine($"Argument {argument.ArgKey} is required but not given");
                        return false;
                    }
                    if (argument.HasValue) {
                        // set value
                        if (argument.HasDefaultValue)
                            argument.SetValue(resultObj, argument.DefaultValue);
                        else {
                            Console.WriteLine($"Argument {argument.ArgKey} should have a default value, but one isn't supplied");
                        }
                    }
                }
            }
            return true;
        }

        // Checks if an attribute list contains an attribute of a given type
        private bool containsAttribute(IEnumerable<CustomAttributeData> attributes, Type type) {
            if (attributes == null)
                throw new ArgumentNullException("attributes");
            if (type == null)
                throw new ArgumentNullException("type");
            foreach (CustomAttributeData attribute in attributes) {
                if (attribute.AttributeType == type)
                    return true;
            }
            return false;
        }

        // linear search to get (and remove) argument if it exists
        private RuntimeArgument getArgument(string argKey, ref List<RuntimeArgument> arguments) {
            foreach (RuntimeArgument argument in arguments) {
                if (argKey.Equals(argument.ArgKey, argument.IsCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) {
                    arguments.Remove(argument);
                    return argument;
                }
            }
            return null;
        }

        // parses object from string with a given type
        // Credit to https://stackoverflow.com/a/3965927
        private bool tryParse(string value, Type type, ref object result) {
            try {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                result = converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        // gets a list of reserved arguments
        private RuntimeArgument[] getReservedArguments() {
            return new RuntimeArgument[] {
                RuntimeArgument.Reserved("h", "Displays the help menu")
            };
        }

        // handle the argument. Return true if can continue, false if exit
        private bool handleReservedArgument(RuntimeArgument argument) {
            if (argument.ArgKey.Equals("h")) {
                // display help
                helpFormatter.PrintHelp();
                return false;
            }
            else {
                throw new NotImplementedException();
            }
        }
    }

    struct EncounteredPair {
        public string Key;
        public string Value;
        public bool HasValue => Value != null;

        public EncounteredPair(string key, string value) {
            Key = key;
            Value = value;
        }

        public EncounteredPair(string key) {
            Key = key;
            Value = null;
        }
    }

}
