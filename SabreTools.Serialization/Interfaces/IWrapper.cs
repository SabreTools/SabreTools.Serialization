namespace SabreTools.Serialization.Interfaces
{
    /// <summary>
    /// Represents a wrapper around a top-level model
    /// </summary>
    /// <typeparam name="TModel">Top-level model for the wrapper</typeparam>
    public interface IWrapper<TModel>
    {
        /// <summary>
        /// Get a human-readable description of the wrapper
        /// </summary>
        string Description();

        /// <summary>
        /// Get the backing model
        /// </summary>
        TModel GetModel();
        
#if !NETFRAMEWORK
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        string ExportJSON();
#endif
    }
}