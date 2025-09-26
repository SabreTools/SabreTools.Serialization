namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to read from byte arrays
    /// </summary>
    public interface IByteReader<TModel>
    {
        /// <summary>
        /// Deserialize a byte array into <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled object on success, null on error</returns>
        TModel? Deserialize(byte[]? data, int offset);
    }
}
