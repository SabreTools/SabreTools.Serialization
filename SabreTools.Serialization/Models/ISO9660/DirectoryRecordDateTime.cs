namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Datetime for a Directory Record
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryRecordDateTime
    {
        /// <summary>
        /// Number of years since 1900
        /// </summary>
        public byte YearsSince1990 { get; set; }

        /// <summary>
        /// Month of the year, 1-12
        /// </summary>
        public byte Month { get; set; }

        /// <summary>
        /// Day of the month, 1-31
        /// </summary>
        public byte Day { get; set; }

        /// <summary>
        /// Hour of the day, 0-23
        /// </summary>
        public byte Hour { get; set; }

        /// <summary>
        /// Minute of the hour, 0-59
        /// </summary>
        public byte Minute { get; set; }

        /// <summary>
        /// Second of the minute, 0-59
        /// </summary>
        public byte Second { get; set; }

        /// <summary>
        /// Time zone offset (from GMT = UTC 0), represented by a single byte
        /// Unit = 15min offset
        /// 0 = offset of -12 hours (UTC-12)
        /// 100 = offset of +13 hours (UTC+13)
        /// </summary>
        public byte TimezoneOffset { get; set; }
    }
}