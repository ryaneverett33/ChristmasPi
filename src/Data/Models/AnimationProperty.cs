namespace ChristmasPi.Data.Models {
    /// <summary>
    /// The configured value for a given property
    /// </summary>
    public class AnimationProperty {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// A registered property ready to be given its value
    /// </summary>
    public class RegisteredProperty<T> {
        public string Name { get; set; }
        public PrimitiveType Type { get; set; }
        public Ref<T> Reference;
        public object DefaultValue { get; set; }
    }

    /// <summary>
    /// The current evaluated state for a given property
    /// </summary>
    public sealed class ActiveAnimationProperty {
        public string Name { get; set; }
        public PrimitiveType Type { get; set; }
        public object CurrentValue { get; set; }
    }
}