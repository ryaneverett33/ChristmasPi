﻿namespace ChristmasPi.Hardware.Interfaces {
    public interface IRenderer {
        /// <summary>
        /// The number of "Lights" attached to the renderer
        /// </summary>
        int LightCount { get; }
        /// <summary>
        /// Whether or not the renderer will automatically render changes or if a render must be invoked
        /// 
        /// </summary>
        bool AutoRender { get; }
        /// <summary>
        /// Renders a new frame
        /// </summary>
        /// <param name="obj">The Renderer object</param>
        void Render(IRenderer obj);
        /// <summary>
        /// Sets the color for a given LED to be rendered
        /// </summary>
        /// <param name="index">The position of the LED (zero-based)</param>
        /// <param name="color">The color the LED should be</param>
        void SetLEDColor(int index, System.Drawing.Color color);
        /// <summary>
        /// Sets the color off all LEDs in the renderer
        /// </summary>
        /// <param name="color">The new color</param>
        void SetAllLEDColors(System.Drawing.Color color);
        /// <summary>
        /// Starts any necessary render-needed tasks
        /// </summary>
        void Start();
        /// <summary>
        /// Stops any rendering activities that may be occuring
        /// </summary>
        void Stop();
    }
}