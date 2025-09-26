using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.DosCenter
{
    /// <remarks>file</remarks>
    public class File
    {
        /// <remarks>name, attribute</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>size, attribute, numeric</remarks>
        [Required]
        public string? Size { get; set; }

        /// <remarks>crc, attribute</remarks>
        [Required]
        public string? CRC { get; set; }

        /// <remarks>sha1, attribute</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>date, attribute</remarks>
        public string? Date { get; set; }
    }
}