namespace SabreTools.Serialization.Interfaces
{
    public interface IWrapper
    {
        /// <summary>
        /// Get a human-readable description of the wrapper
        /// </summary>
        string Description();
        
#if !NETFRAMEWORK
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        string ExportJSON();
#endif
    }
}