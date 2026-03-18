namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to write to files
    /// </summary>
    public interface IFileWriter<TModel>
    {
        /// <summary>
        /// Enable outputting debug information
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Serialize a <typeparamref name="TModel"/> into a file
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public bool SerializeFile(TModel? obj, string? path);
    }
}
