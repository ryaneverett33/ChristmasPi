using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BaseAnimation : IAnimation {
        public int LightCount => lightcount;
        public int FPS => fps;
        public bool isBranchAnimation => false;
        public virtual string Name { get { throw new NotImplementedException(); } }
        public virtual bool isDebugAnimation { get { return false; } }

        private int lightcount;
        private int fps;
        // List of properties supplied by configuration
        private List<AnimationProperty> _configProperties;
        // List of properties registered by the animation
        private List<RegisteredProperty<object>> _registeredProperties;
        public FrameList list;

        public BaseAnimation() { 
            list = new FrameList();
            this._configProperties = new List<AnimationProperty>();
            this._registeredProperties = new List<RegisteredProperty<object>>();
        }

        public virtual void RegisterProperties() { }

        public void AddProperties(AnimationProperty[] properties) {
            foreach (AnimationProperty property in properties) {
                this._configProperties.Add(new AnimationProperty {
                    Name = property.Name, 
                    Value = property.Value });
            }
        }
        
        public bool RegisterProperty(Ref<object> reference, string name, object defaultvalue) {
            if (_registeredProperties.Exists(prop => {
                return prop.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase);
            }))
                return false;
            _registeredProperties.Add(new RegisteredProperty<object> {
                Name = name,
                Reference = reference,
                DefaultValue = defaultvalue
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
                prop.Reference.Value = value;         // I hope this works
            }
        }

        public virtual RenderFrame[] GetFrames(int fps, int lightcount) {
            if (list.Count != 0 && this.fps == fps && this.lightcount == lightcount) {
                return list.ToFrames(fps);
            }
            else {
                // this.list = new FrameList();
                this.construct(lightcount, fps);
                return list.ToFrames(fps);
            }
        }

        public virtual void construct(int lightcount, int fps) {
            this.fps = fps;
            this.lightcount = lightcount;
        }
    }
}