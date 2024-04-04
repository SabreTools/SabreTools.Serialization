namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize from strings
    /// </summary>
    public interface IStringSerializer<T>
    {
        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled string on successful serialization, null otherwise</returns>
        string? Serialize(T? obj);
    }
}