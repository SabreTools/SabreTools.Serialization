namespace SabreTools.Serialization.Models.DosCenter
{
    /// <remarks>file</remarks>
    public class File
    {
        /// <remarks>name, attribute</remarks>
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        /// <remarks>size, attribute, numeric</remarks>
        [SabreTools.Models.Required]
        public string? Size { get; set; }

        /// <remarks>crc, attribute</remarks>
        [SabreTools.Models.Required]
        public string? CRC { get; set; }

        /// <remarks>sha1, attribute</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>date, attribute</remarks>
        public string? Date { get; set; }
    }
}