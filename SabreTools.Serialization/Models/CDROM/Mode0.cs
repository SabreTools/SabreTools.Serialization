namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM Mode1 sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class Mode0 : DataSector
    {
        /// <summary>
        /// User Data, 2336 bytes
        /// </summary>
        /// <remarks>Should be all 0x00</remarks>
        public byte[] UserData { get; set; } = new byte[2336];
    }
}
