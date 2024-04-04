namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize from strings
    /// </summary>
    public interface IStringDeserializer<T>
    {
        /// <summary>
        /// Deserialize a string into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="str">String to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        T? Deserialize(string? str);
    }
}