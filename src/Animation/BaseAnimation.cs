using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation {
    public class BaseAnimation : IAnimatable {
        public virtual string Name { get { throw new NotImplementedException(); } }
        public virtual bool isDebugAnimation { get { return false; } }
        public virtual bool isBranchAnimation { get { return false; } }

        // List of properties supplied by configuration
        private List<AnimationProperty> _configProperties;
        // List of properties registered by the animation
        private List<RegisteredProperty<object>> _registeredProperties;

        public BaseAnimation() {
            this._configProperties = new List<AnimationProperty>();
            this._registeredProperties = new List<RegisteredProperty<object>>();
        }

        public virtual void RegisterProperties() { }

        public void AddProperties(AnimationProperty[] properties) {
            foreach (AnimationProperty property in properties) {
                this._configProperties.Add(new AnimationProperty {
                    Name = property.Name,
                    Value = property.Value
                });
            }
        }

        public bool RegisterProperty(Ref<object> reference, string name, object defaultvalue, PrimType type) {
            if (_registeredProperties.Exists(prop => {
                return prop.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase);
            }))
                return false;
            _registeredProperties.Add(new RegisteredProperty<object> {
                Name = name,
                Reference = reference,
                DefaultValue = defaultvalue,
                Type = new PrimitiveType(type)
            });
            return true;
        }

        public void ResolveProperties() {
            foreach (RegisteredProperty<object> prop in _registeredProperties) {
                AnimationProperty configValue = _configProperties.SingleOrDefault(config => {
                    return config.Name.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase);
                });
                object value;
                if (configValue == null)
                    value = prop.DefaultValue;
                else
                    value = configValue.Value;
                if (prop.Type.IsBoolean)
                    prop.Reference.Value = prop.Type.AsBoolean(value);
                else if (prop.Type.IsFloat)
                    prop.Reference.Value = prop.Type.AsFloat(value);
                else if (prop.Type.IsInteger)
                    prop.Reference.Value = prop.Type.AsInteger(value);
                else if (prop.Type.IsString)
                    prop.Reference.Value = prop.Type.AsString(value);
                else
                    prop.Reference.Value = value;         // I hope this works
            }
        }

        public string[] GetPropertyNames() {
            string[] names = new string[_registeredProperties.Count];
            for (int i = 0; i < _registeredProperties.Count; i++) {
                RegisteredProperty<object> registeredProperty = _registeredProperties[i];
                names[i] = registeredProperty.Name;
            }
            return names;
        }

        public ActiveAnimationProperty[] GetActiveAnimationProperties() {
            ActiveAnimationProperty[] properties = new ActiveAnimationProperty[_registeredProperties.Count];
            for (int i = 0; i < _registeredProperties.Count; i++) {
                RegisteredProperty<object> registeredProperty = _registeredProperties[i];
                properties[i] = new ActiveAnimationProperty() {
                    Name = registeredProperty.Name,
                    Type = registeredProperty.Type,
                    CurrentValue = registeredProperty.Reference.Value
                };
            }
            return properties;
        }
    }
}
