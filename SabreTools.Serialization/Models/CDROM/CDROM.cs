namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM disc image, made up of data sectors, ISO 10149 / ECMA-130
    /// Intentionally not a mixed-mode CD disc image, pure CD-ROM disc (no audio sectors)
    /// This model intentionally does not take tracks into consideration
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class CDROM
    {
        /// <summary>
        /// CD-ROM data sectors
        /// </summary>
        public DataSector[] Sectors { get; set; } = [];
    }
}
