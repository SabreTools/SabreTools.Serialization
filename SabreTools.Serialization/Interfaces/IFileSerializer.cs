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
        T? DeserializeImpl(string? path);

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a file
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        bool SerializeImpl(T? obj, string? path);
    }
}