namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Filename entry in the NameTable
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class NameEntry
    {
        /// <summary>
        /// Filename length, with MSB set to 0 for filenames less than 127 long
        /// NodeLengthShort and NodeLengthLong fields are exclusive, and one must be present
        /// </summary>
        public byte? NodeLengthShort { get; set; }

        /// <summary>
        /// Filename length, with prefix byte's MSB set to 1 for filenames greater than 127 long
        /// NodeLengthShort and NodeLengthLong fields are exclusive, and one must be present
        /// </summary>
        public ushort? NodeLengthLong { get; set; }

        /// <summary>
        /// UTF-8 encoded file name
        /// </summary>
        /// <remarks>Maximum length of 2^15 - 1 bytes</remarks>
        public byte[] NodeName { get; set; } = [];
    }
}
