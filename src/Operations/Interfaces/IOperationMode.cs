namespace ChristmasPi.Operations.Interfaces {
    public interface IOperationMode {
        string Name { get; }
        void Activate();
        void Deactivate();
    }
}
