namespace ChristmasPi.Data.Models {
    /// <summary>
    /// Stores configured values for animation properties
    /// </summary>
    public class AnimationInfo {
        public string Name { get; set; }
        public AnimationProperty[] Properties { get; set; }
    }
}