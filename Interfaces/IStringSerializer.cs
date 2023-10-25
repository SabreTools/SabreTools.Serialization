namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to and from strings
    /// </summary>
    public interface IStringSerializer<T>
    {
        /// <summary>
        /// Deserialize a string into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="str">String to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        T Deserialize(string str);
#else
        T? Deserialize(string? str);
#endif

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled string on successful serialization, null otherwise</returns>
#if NET48
        string Serialize(T obj);
#else
        string? Serialize(T? obj);
#endif
    }
}