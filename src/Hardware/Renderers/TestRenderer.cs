﻿using System;
using System.Drawing;
using System.IO;
using ChristmasPi.Data.Models;
using ChristmasPi.Hardware.Interfaces;

namespace ChristmasPi.Hardware.Renderers {
    public class TestRenderer : BaseRenderer, IRenderer {
        public new int LightCount => 1;
        public new bool AutoRender => false;
        public override event BeforeRenderHandler BeforeRenderEvent;
        public override event AfterRenderHandler AfterRenderEvent;
        private TextWriter writer;

        // write current value to file
        public override void Render(IRenderer renderer) {
            BeforeRenderEvent.Invoke(this, new RenderArgs());
            Color color = base.ledColors[0];
            writer.WriteLine($"{DateTime.Now.ToShortTimeString()} - RGB:({color.R},{color.G},{color.B})");
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
