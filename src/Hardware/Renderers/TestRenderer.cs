using System;
using System.Drawing;
using System.IO;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Hardware;
using ChristmasPi.Hardware.Interfaces;

namespace ChristmasPi.Hardware.Renderers {
    public class TestRenderer : BaseRenderer, IRenderer {
        public override bool AutoRender => false;
        public override event BeforeRenderHandler BeforeRenderEvent;
        public override event AfterRenderHandler AfterRenderEvent;
        private TextWriter writer;

        public TestRenderer(int lightcount) : base(lightcount) { }

        // write current value to file
        public override void Render(IRenderer renderer) {
            if (writer == null) {
                Console.WriteLine("Cannot render, failed to call Start method");
                return;
            }
            if (BeforeRenderEvent != null)
                BeforeRenderEvent.Invoke(this, new RenderArgs());
            for (int i = 0; i< ledColors.Length; i++) {
                Color color = base.ledColors[i];
                writer.WriteLine($"{DateTime.Now.ToShortTimeString()} - RGB:({color.R},{color.G},{color.B})");
            }
            writer.WriteLine();
            writer.Flush();
            if (AfterRenderEvent != null)
                AfterRenderEvent.Invoke(this, new RenderArgs());
        }
        // Create file for writing out values to
        public override void Start() {
            stopped = false;
            writer = File.CreateText("test-renderer.txt");
        }
        // Cleanup file
        public override void Stop() {
            if (!stopped) {
                writer.Flush();
                writer.Dispose();
                stopped = true;
            }
        }

        public override void Dispose() {
            writer.Dispose();
            base.Dispose();
        }
    }
}
