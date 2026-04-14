namespace SabreTools.Wrappers
{
    /// <summary>
    /// Represents an item that can have its model written directly
    /// </summary>
    public interface IWritable
    {
        /// <summary>
        /// Write to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if writing succeeded, false otherwise</returns>
        public bool Write(string outputDirectory, bool includeDebug);
    }
}
