namespace InfoPrint
{
    /// <summary>
    /// Set of options for the test executable
    /// </summary>
    internal sealed class Options
    {
        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Output information to file only, skip printing to console
        /// </summary>
        public bool FileOnly { get; set; }

        /// <summary>
        /// Print external file hashes
        /// </summary>
        public bool Hash { get; set; }

#if NETCOREAPP
        /// <summary>
        /// Enable JSON output
        /// </summary>
        public bool Json { get; set; }
#endif
    }
}
