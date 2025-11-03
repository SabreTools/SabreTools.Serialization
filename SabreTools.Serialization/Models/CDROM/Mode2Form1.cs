using System.Collections.Generic;

namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM Mode 2 Form 1 sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class Mode2Form1 : Sector
    {
        /// <summary>
        /// Mode 2 subheader, 8 bytes
        /// </summary>
        public byte[] Subheader { get; set; } = new byte[8];

        /// <summary>
        /// User data, 2048 bytes
        /// </summary>
        public byte[] UserData { get; set; } = new byte[2048];

        /// <summary>
        /// Error Detection Code, 4 bytes
        /// </summary>
        public byte[] EDC { get; set; } = new byte[4];

        /// <summary>
        /// Error Correction Code, 4 bytes
        /// </summary>
        public byte[] ECC { get; set; } = new byte[276];
    }
}
