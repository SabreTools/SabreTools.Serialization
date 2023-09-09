namespace SabreTools.Serialization
{
    /// <summary>
    /// Defines how to serialize to and from byte arrays
    /// </summary>
    public interface IByteSerializer<T>
    {
        /// <summary>
        /// Deserialize a byte array into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled object on success, null on error</returns>
#if NET48
        T Deserialize(byte[] data, int offset);
#else
        T? Deserialize(byte[]? data, int offset);
#endif

        // TODO: Add serialization method
    }
}