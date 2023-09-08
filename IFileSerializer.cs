namespace SabreTools.Serialization
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
    }
}