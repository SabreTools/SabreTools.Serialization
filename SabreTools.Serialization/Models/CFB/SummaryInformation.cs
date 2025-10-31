using System;

namespace SabreTools.Data.Models.CFB
{
    /// <see href="https://github.com/GNOME/msitools/blob/master/libmsi/libmsi-summary-info.c"/>
    public sealed class SummaryInformation
    {
        #region Set Header

        /// <summary>
        /// This field MUST be set to 0xFFFE. This field is a byte order mark for
        /// all integer fields, specifying little-endian byte order.
        /// </summary>
        public ushort ByteOrder { get; set; }

        /// <summary>
        /// Format
        /// </summary>
        public ushort Format { get; set; }

        /// <summary>
        /// Build
        /// </summary>
        public ushort Build { get; set; }

        /// <summary>
        /// Platform ID
        /// </summary>
        public ushort PlatformID { get; set; }

        /// <summary>
        /// CLSID
        /// </summary>
        public Guid CLSID { get; set; }

        /// <summary>
        /// 4 bytes of reserved data
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[4];

        #endregion

        #region Format Header

        /// <summary>
        /// Format ID, should be <see cref="Constants.FMTID_SummaryInformation"/>
        /// </summary>
        public Guid FormatID { get; set; }

        /// <summary>
        /// 16 bytes of unknown data
        /// </summary>
        public byte[] Unknown { get; set; } = new byte[16];

        #endregion

        #region Section Header

        /// <summary>
        /// Location of the section
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Section count(?)
        /// </summary>
        public uint SectionCount { get; set; }

        /// <summary>
        /// Property count
        /// </summary>
        public uint PropertyCount { get; set; }

        /// <summary>
        /// Properties
        /// </summary>
        /// <remarks>Each Variant might be followed by an index and offset value</remarks>
        public Variant[] Properties { get; set; }

        #endregion
    }
}
