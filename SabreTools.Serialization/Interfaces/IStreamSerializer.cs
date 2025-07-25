namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to Streams
    /// </summary>
    public interface IStreamSerializer<T>
    {
        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a Stream
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled object on success, null on error</returns>
        System.IO.Stream? Serialize(T? obj);
    }
}
