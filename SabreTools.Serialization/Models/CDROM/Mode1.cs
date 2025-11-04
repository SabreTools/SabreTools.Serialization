namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM Mode1 sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class Mode1 : DataSector
    {
        /// <summary>
        /// User Data, 2048 bytes
        /// </summary>
        public byte[] UserData { get; set; } = new byte[2048];

        /// <summary>
        /// Error Detection Code, 4 bytes
        /// </summary>
        public byte[] EDC { get; set; } = new byte[4];

        /// <summary>
        /// Reserved 8 bytes
        /// </summary>
        public byte[] Intermediate { get; set; } = new byte[8];

        /// <summary>
        /// Error Correction Code, 4 bytes
        /// </summary>
        public byte[] ECC { get; set; } = new byte[276];
    }
}
