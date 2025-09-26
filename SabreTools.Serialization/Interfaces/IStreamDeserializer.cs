namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to read from Streams
    /// </summary>
    public interface IStreamReader<TModel>
    {
        /// <summary>
        /// Deserialize a Stream into <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object on success, null on error</returns>
        TModel? Deserialize(System.IO.Stream? data);
    }
}
