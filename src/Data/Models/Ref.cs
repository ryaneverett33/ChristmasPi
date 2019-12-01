using System;

namespace ChristmasPi.Data.Models {
    /// <summary>
    /// A way to store references to value types
    /// </summary>
    /// <source>https://stackoverflow.com/a/2258102</source>
    public sealed class Ref<T> {
        private Func<T> getter;
        private Action<T> setter;
        public Ref(Func<T> getter, Action<T> setter) {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value {
            get { return getter(); }
            set { setter(value); }
        }
    }
}