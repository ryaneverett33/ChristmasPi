using System;

namespace ChristmasPi.Util.Arguments {

    /// <summary>
    /// Specifies an argument
    /// </summary>
    [AttributeUsage(
   AttributeTargets.Field |
   AttributeTargets.Property,
   AllowMultiple = false)]
    public class Argument : Attribute {
        public string Key { get; private set; }
        public string Message { get; private set; }
        
        /// <summary>
        /// Specifies the behavior of the argument pertaining to different things like the help menu or parsing
        /// </summary>
        /// <see cref="ArgumentFlags"/>
        public ArgumentFlags Flags { get; private set; }

        /// <summary>
        /// Specifies the default value of the argument if not given or the argument doesn't expect a value
        /// </summary>
        /// <remarks>If a default value isn't given, the type's default will be used. A default value should be given 
        /// even if the Argument doesn't expect a value.</remarks
        /// <example>--useDebug might set the backing field (bool useDebug) to true with a default value of true</example>
        /// <example>--timeout 5 might set the backing field (int timeout) to 5 when specified or to it's default value if not given at runtime</example>
        public object DefaultValue { get; private set; }

        public Argument(string key, string message, object defaultvalue, ArgumentFlags flags) {
            if (flags.HasFlag(ArgumentFlags.Reserved))
                throw new FieldAccessException("User arguments are not allowed to use the Reserved flag");
            if (defaultvalue == null && flags.HasFlag(ArgumentFlags.HasValue))
                throw new ArgumentException("Default Value cannot be null with ArgumentFlags.HasValue");
            Key = key;
            Message = message;
            Flags = flags;
            DefaultValue = defaultvalue;
        }

        public Argument(string key, string message, object defaultvalue) {
            Key = key;
            Message = message;
            DefaultValue = defaultvalue;
        }

        protected internal static Argument Reserved(string key, string message, object defaultvalue) {
            Argument arg = new Argument(key, message, defaultvalue);
            arg.Flags = ArgumentFlags.Reserved;
            return arg;
        }
    }

    /// <summary>
    /// Specifies which section an argument belongs in
    /// </summary>
    [AttributeUsage(
   AttributeTargets.Field |
   AttributeTargets.Property,
   AllowMultiple = false)]
    public class HelpSection : Attribute {
        public string Section { get; private set; }

        public HelpSection(string section) {
            if (section == null)
                throw new ArgumentNullException("section");
            Section = section;
        }
    }

}
