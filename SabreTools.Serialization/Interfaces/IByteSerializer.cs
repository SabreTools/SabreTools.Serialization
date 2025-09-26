namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to write to byte arrays
    /// </summary>
    public interface IByteWriter<T>
    {
        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a byte array
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Filled object on success, null on error</returns>
        byte[]? SerializeArray(T? obj);
    }
}
