using System;
using System.Drawing;
using System.IO;
using ChristmasPi.Hardware.Interfaces;

namespace ChristmasPi.Hardware.Renderers {
    public class TestRenderer : BaseRenderer, IRenderer {
        public new int LightCount => 1;
        public new bool AutoRender => false;
        private TextWriter writer;

        // write current value to file
        public override void Render(IRenderer renderer) {
            Color color = base.ledColors[0];
            writer.WriteLine($"{DateTime.Now.ToShortTimeString()} - RGB:({color.R},{color.G},{color.B})");
        }
        // Create file for writing out values to
        public override void Start() {
            writer = File.CreateText("test-renderer.txt");
        }
        // Cleanup file
        public override void Stop() {
            writer.Flush();
            writer.Dispose();
        }
    }
}
