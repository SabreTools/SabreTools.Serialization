using System.IO;

namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to and from Streams
    /// </summary>
    public interface IStreamSerializer<T>
    {
        /// <summary>
        /// Deserialize a Stream into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        T Deserialize(Stream data);
#else
        T? Deserialize(Stream? data);
#endif

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a Stream
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        Stream Serialize(T obj);
#else
        Stream? Serialize(T? obj);
#endif
    }
}