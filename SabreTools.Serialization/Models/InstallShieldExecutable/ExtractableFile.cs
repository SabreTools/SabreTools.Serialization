namespace SabreTools.Data.Models.InstallShieldExecutable
{
public class ExtractableFile
    {
        /// <summary>
        /// Name of the file, only ASCII characters(?)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Path of the file, only ASCII characters(?), seems to usually use \ filepaths
        /// </summary>
        public string? Path  { get; set; }

        /// <summary>
        /// Version of the file
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Length of the file. Stored in the installshield executable as a string.
        /// </summary>
        public uint Length { get; set; }
    }
}