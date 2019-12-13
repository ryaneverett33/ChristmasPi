namespace ChristmasPi.Operations.Interfaces {
    public interface IOperationMode {
        /// <summary>
        /// The name of the operating mode
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Performs any needed tasks to start the operation
        /// </summary>
        void Activate(bool defaultmode);

        /// <summary>
        /// Stops all operations
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Gets information about the operating mode.
        /// </summary>
        /// <returns>An annonymous type with info about the mode</returns>
        /// <remarks>This data is typically converted to JSON after calling()</remarks>
        /// <see cref="TreeController.GetTreeMode()" />
        object Info();

        /// <summary>
        /// Gets the value of a specific property
        /// </summary>
        /// <param name="property">The property to return</param>
        /// <returns>The value of the property on success, null on failure</returns>
        object GetProperty(string property);
    }
}
