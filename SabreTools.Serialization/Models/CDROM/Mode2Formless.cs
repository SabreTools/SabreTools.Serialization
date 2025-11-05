namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM Mode1 sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class Mode2Formless : DataSector
    {
        /// <summary>
        /// User Data, 2336 bytes
        /// </summary>
        public byte[] UserData { get; set; } = new byte[2336];
    }
}
