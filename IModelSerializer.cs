namespace SabreTools.Serialization
{
    /// <summary>
    /// Defines how to serialize to and from files
    /// </summary>
    public interface IFileSerializer<T, U>
    {
        /// <summary>
        /// Deserialize a <typeparamref name="T"/> into <typeparamref name="u"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize from</typeparam>
        /// <typeparam name="U">Type of object to deserialize to</typeparam>
        /// <param name="obj">Object to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        U Deserialize(T obj);
#else
        U? Deserialize(T? obj);
#endif
    }
}