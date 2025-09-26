namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to write to Streams
    /// </summary>
    public interface IStreamWriter<T>
    {
        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a Stream
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled object on success, null on error</returns>
        System.IO.Stream? SerializeStream(T? obj);
    }
}
