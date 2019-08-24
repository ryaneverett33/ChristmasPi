namespace ChristmasPi.Hardware.Interfaces {
    public interface IRenderer {
        void Render(IRenderer obj);
        void SetLEDColor(int index, System.Drawing.Color color);
    }
}
