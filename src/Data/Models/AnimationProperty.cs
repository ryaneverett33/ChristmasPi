namespace ChristmasPi.Data.Models {
    public class AnimationProperty {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class RegisteredProperty<T> {
        public string Name { get; set; }
        public Ref<T> Reference;
        public object DefaultValue { get; set; }
    }
}