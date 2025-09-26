namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Defines how to read from files
    /// </summary>
    public interface IFileReader<TModel>
    {
        /// <summary>
        /// Deserialize a file into <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="path">Path to deserialize from</param>
        /// <returns>Filled object on success, null on error</returns>
        TModel? Deserialize(string? path);
    }
}
