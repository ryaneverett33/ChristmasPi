using System;
using System.Reflection;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Util.Arguments {
    public class RuntimeArgument {

        // The help message
        public bool HasMessage { get; internal set; }
        // Whether DefaultValue has value or not
        public bool HasDefaultValue { get; internal set; }
        // Whether the argument is case sensitive
        public bool IsCaseSensitive => argInfo.Flags.HasFlag(ArgumentFlags.CaseSensitive);
        // Whether an argument should have a value following it
        public bool HasValue => argInfo.Flags.HasFlag(ArgumentFlags.HasValue);
        // Whether the argument is required
        public bool IsRequired => argInfo.Flags.HasFlag(ArgumentFlags.Required);
        // Whether the argument is reserved for special purposes (such as displaying help)
        public bool IsReserved => argInfo.Flags.HasFlag(ArgumentFlags.Reserved);
        // The key used to set the argument (ex: --sec 2 -> ArgKey: sec)
        public string ArgKey { get; internal set; }
        // The optional message to be displayed in the help menu
        public string Message { get; internal set; }
        // The underlying type
        public Type Type => backingObject.Type;
        // The specified default value
        public object DefaultValue => argInfo.DefaultValue;
        // The help section this argument belongs in
        public string Section => helpSection != null ? helpSection.Section : null;

        /// <summary>
        /// Describes where the value should be stored
        /// </summary>
        private BackingObject backingObject;

        /// <summary>
        /// The argument attribute
        /// </summary>
        private Argument argInfo;

        /// <summary>
        /// The section attribute
        /// </summary>
        private HelpSection helpSection;

        /// <summary>
        /// Creates a new RuntimeArgument whose backing object is a property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="argInfo"></param>
        public RuntimeArgument(PropertyInfo property, Argument argInfo, HelpSection helpSection) : this(argInfo, helpSection) {
            if (property == null)
                throw new ArgumentNullException("property");
            backingObject = new BackingObject(property);
            if (HasDefaultValue) {
                if (backingObject.Type != argInfo.DefaultValue.GetType())
                    throw new TypeMismatchException(backingObject.Type, argInfo.DefaultValue.GetType());
            }
        }

        /// <summary>
        /// Creates a new RuntimeArgument whose backing object is a field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="argInfo"></param>
        public RuntimeArgument(FieldInfo field, Argument argInfo, HelpSection helpSection) : this(argInfo, helpSection) {
            if (field == null)
                throw new ArgumentNullException("field");
            backingObject = new BackingObject(field);
            if (HasDefaultValue) {
                if (backingObject.Type != argInfo.DefaultValue.GetType())
                    throw new TypeMismatchException(backingObject.Type, argInfo.DefaultValue.GetType());
            }
        }

        private RuntimeArgument(Argument argInfo, HelpSection helpSection) {
            if (argInfo == null)
                throw new ArgumentNullException("argInfo");
            ArgKey = argInfo.Key;
            Message = argInfo.Message;
            HasMessage = Message != null;
            HasDefaultValue = argInfo.DefaultValue != null;
            this.argInfo = argInfo;
            this.helpSection = helpSection;
        }

        /// <summary>
        /// Sets the value of an evaluated argument
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public void SetValue(object instance, object value) {
            if (value == null)
                throw new ArgumentNullException("value");
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (backingObject.Type != value.GetType())
                throw new TypeMismatchException(backingObject.Type, value.GetType());
            if (backingObject.IsProperty)
                backingObject.PropertyInfo.SetValue(instance, value);
            else
                backingObject.FieldInfo.SetValue(instance, value);
        }

        /// <summary>
        /// Creates a reserved argument with a given key and help message
        /// </summary>
        /// <param name="key">The argument key (ex: h for -h</param>
        /// <param name="message">Help message</param>
        /// <returns>A filled-out RuntimeArgument object with ArgumentFlags.Reserved applied</returns>
        public static RuntimeArgument Reserved(string key, string message) {
            Argument arg = Argument.Reserved(key, message, null);
            HelpSection helpSection = new HelpSection("Reserved");
            return new RuntimeArgument(arg, helpSection);
        }
    }

    struct BackingObject {
        public bool IsProperty;
        public FieldInfo FieldInfo;
        public PropertyInfo PropertyInfo;
        public Type Type => IsProperty ? PropertyInfo.PropertyType : FieldInfo.FieldType;

        public BackingObject(PropertyInfo property) {
            IsProperty = true;
            FieldInfo = null;
            PropertyInfo = property;
        }
        public BackingObject(FieldInfo field) {
            IsProperty = false;
            FieldInfo = field;
            PropertyInfo = null;
        }
    }
}
