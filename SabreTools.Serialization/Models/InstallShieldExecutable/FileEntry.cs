namespace SabreTools.Data.Models.InstallShieldExecutable
{
    public class FileEntry
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string? Name { get; set; }

        /// <summary>
        /// Path of the file, seems to usually use \ filepaths
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string? Path { get; set; }

        /// <summary>
        /// Version of the file
        /// </summary>
        /// <remarks>May only contain ASCII (7-bit) characters</remarks>
        public string? Version { get; set; }

        /// <summary>
        /// Length of the file. Stored in the installshield executable as a string.
        /// </summary>
        public ulong Length { get; set; }
    }
}
