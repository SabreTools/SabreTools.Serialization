namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to write to Streams
    /// </summary>
    public interface IStreamWriter<TModel>
    {
        /// <summary>
        /// Enable outputting debug information
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Serialize a <typeparamref name="TModel"/> into a Stream
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled object on success, null on error</returns>
        public System.IO.Stream? SerializeStream(TModel? obj);
    }
}
