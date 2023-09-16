namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to and from files
    /// </summary>
    public interface IFileSerializer<T>
    {
        /// <summary>
        /// Deserialize a file into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="path">Path to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        T Deserialize(string path);
#else
        T? Deserialize(string? path);
#endif

        /// <summary>
        /// Sserialize a <typeparamref name="T"/> into a file
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
#if NET48
        bool Serialize(T obj, string path);
#else
        bool Serialize(T? obj, string? path);
#endif
    }
}