using System.IO;

namespace SabreTools.Serialization
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
    }
}