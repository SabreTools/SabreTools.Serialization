namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to files
    /// </summary>
    public interface IFileSerializer<T>
    {
        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a file
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        bool Serialize(T? obj, string? path);
    }
}
