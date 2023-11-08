namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to serialize to and from models
    /// </summary>
    public interface IModelSerializer<T, U>
    {
        /// <summary>
        /// Deserialize a <typeparamref name="U"/> into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <typeparam name="U">Type of object to deserialize from</typeparam>
        /// <param name="obj">Object to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        T? Deserialize(U? obj);

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into <typeparamref name="U"/>
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <typeparam name="U">Type of object to serialize to</typeparam>
        /// <param name="obj">Object to serialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        U? Serialize(T? obj);
    }
}