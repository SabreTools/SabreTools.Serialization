namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Timestamp (1/7.3)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class Timestamp
    {
        /// <summary>
        /// Type (upper 4 bits, 1 = local time) and time zone offset from UTC in minutes
        /// (lower 12 bits, signed, -2047 = not specified)
        /// </summary>
        public ushort TypeAndTimezone { get; set; }

        /// <summary>
        /// Year, 1 to 9999
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Month, 1 to 12
        /// </summary>
        public byte Month { get; set; }

        /// <summary>
        /// Day, 1 to 31
        /// </summary>
        public byte Day { get; set; }

        /// <summary>
        /// Hour, 0 to 23
        /// </summary>
        public byte Hour { get; set; }

        /// <summary>
        /// Minute, 0 to 59
        /// </summary>
        public byte Minute { get; set; }

        /// <summary>
        /// Second, 0 to 59
        /// </summary>
        public byte Second { get; set; }

        /// <summary>
        /// Centiseconds, 0 to 99
        /// </summary>
        public byte Centiseconds { get; set; }

        /// <summary>
        /// Hundreds of microseconds, 0 to 99
        /// </summary>
        public byte HundredsOfMicroseconds { get; set; }

        /// <summary>
        /// Microseconds, 0 to 99
        /// </summary>
        public byte Microseconds { get; set; }
    }
}
