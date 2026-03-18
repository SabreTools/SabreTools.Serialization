namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to write to strings
    /// </summary>
    public interface IStringWriter<TModel>
    {
        /// <summary>
        /// Enable outputting debug information
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Serialize a <typeparamref name="TModel"/> into a string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled string on successful serialization, null otherwise</returns>
        public string? Serialize(TModel? obj);
    }
}
