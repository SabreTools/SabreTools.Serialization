namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Datetime format represented by decimal ASCII
    /// - Base (Primary/Supplementary/Enhanced) Volume Descriptor
    /// - Extended Attribute Record
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DecDateTime
    {
        /// <summary>
        /// 4-byte ASCII digits
        /// </summary>
        public byte[] Year { get; set; } = new byte[4];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Month { get; set; } = new byte[2];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Day { get; set; } = new byte[2];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Hour { get; set; } = new byte[2];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Minute { get; set; } = new byte[2];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Second { get; set; } = new byte[2];

        /// <summary>
        /// 2-byte ASCII digits
        /// </summary>
        public byte[] Centisecond { get; set; } = new byte[2];

        /// <summary>
        /// Time zone offset (from GMT = UTC 0), represented by a single byte
        /// Unit = 15min offset
        /// 0 = offset of -12 hours (UTC-12)
        /// 100 = offset of +13 hours (UTC+13)
        /// </summary>
        public byte TimezoneOffset { get; set; }
    }
}
