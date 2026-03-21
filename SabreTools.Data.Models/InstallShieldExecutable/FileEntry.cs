namespace SabreTools.Data.Models.InstallShieldExecutable
{
    /// <summary>
    /// Set of attributes for each fileEntry in an InstallShield Executable
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Path of the file, seems to usually use \ filepaths
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Version of the file
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Length of the file. Stored in the installshield executable as a string.
        /// </summary>
        public ulong Length { get; set; }

        /// <summary>
        /// Offset of the file.
        /// </summary>
        /// <remarks>This is not stored in the installshield executable, but it needs to be stored here for extraction.</remarks>
        public long Offset { get; set; }
    }
}
