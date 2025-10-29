namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Represents a wrapper around a top-level model
    /// </summary>
    public interface IWrapper
    {
        /// <summary>
        /// Get a human-readable description of the wrapper
        /// </summary>
        string Description();
    }
}
