using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>disk</remarks>
    public class Disk
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>md5</remarks>
        public string? MD5 { get; set; }

        /// <remarks>sha1</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>merge</remarks>
        public string? Merge { get; set; }

        /// <remarks>status</remarks>
        public string? Status { get; set; }

        /// <remarks>flags</remarks>
        public string? Flags { get; set; }
    }
}