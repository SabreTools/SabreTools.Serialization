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
        T? Deserialize(string? str);

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled string on successful serialization, null otherwise</returns>
        string? Serialize(T? obj);
    }
}