using System.Collections.Generic;

namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM Mode 2 Form 2 sector
    /// Larger user data at expense of no error correction, just error detection
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class Mode2Form2 : Sector
    {
        /// <summary>
        /// Mode 2 subheader, 8 bytes
        /// </summary>
        public byte[] Subheader { get; set; } = new byte[8];

        /// <summary>
        /// User data, 2324 bytes
        /// </summary>
        public byte[] UserData { get; set; } = new byte[2324];

        /// <summary>
        /// Error Detection Code, 4 bytes
        /// </summary>
        public byte[] EDC { get; set; } = new byte[4];
    }
}
