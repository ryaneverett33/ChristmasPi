using System;
using System.Reflection;
using ChristmasPi.Operations.Interfaces;

namespace ChristmasPi.Operations.Utils {
    public static class PropertyHelper {
        public static object ResolveProperty(string propertyName, IOperationMode modeObj, Type modeType) {
            //throw new NotImplementedException();
            if (modeType.GetField(propertyName) is FieldInfo field) {
                if (field.IsPublic)
                    return field.GetValue(modeObj);
                else
                    throw new AccessViolationException($"Cannot access {propertyName} due to protection level");
            }
            else if (modeType.GetProperty(propertyName) is PropertyInfo property) {
                if (property.CanRead)
                    return property.GetValue(modeObj);
                else
                    throw new AccessViolationException($"Cannot access {propertyName} due to read inability");
            }
            else {
                throw new ArgumentException($"{propertyName} is not a valid field or property for {modeType.Name}");
            }
        }
    }
}