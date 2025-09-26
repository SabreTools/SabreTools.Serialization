using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.SeparatedValue
{
    /// <summary>
    /// Standardized variant of a row
    /// </summary>
    public class Row
    {
        /// <remarks>File Name</remarks>
        public string? FileName { get; set; }

        /// <remarks>Internal Name</remarks>
        public string? InternalName { get; set; }

        /// <remarks>Description</remarks>
        public string? Description { get; set; }

        /// <remarks>Game Name</remarks>
        [Required]
        public string? GameName { get; set; }

        /// <remarks>Game Description</remarks>
        public string? GameDescription { get; set; }

        /// <remarks>Type</remarks>
        [Required]
        public string? Type { get; set; }

        /// <remarks>Rom Name</remarks>
        public string? RomName { get; set; }

        /// <remarks>Disk Name</remarks>
        public string? DiskName { get; set; }

        /// <remarks>Size, Numeric</remarks>
        public string? Size { get; set; }

        /// <remarks>CRC</remarks>
        public string? CRC { get; set; }

        /// <remarks>MD5</remarks>
        public string? MD5 { get; set; }

        /// <remarks>SHA1</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>SHA256</remarks>
        public string? SHA256 { get; set; }

        /// <remarks>SHA384, Optional</remarks>
        public string? SHA384 { get; set; }

        /// <remarks>SHA512, Optional</remarks>
        public string? SHA512 { get; set; }

        /// <remarks>SpamSum, Optional</remarks>
        public string? SpamSum { get; set; }

        /// <remarks>Status, Nodump</remarks>
        public string? Status { get; set; }
    }
}