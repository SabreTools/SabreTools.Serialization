namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the zip64 extended
    /// information "extra" block. If one of the size or
    /// offset fields in the Local or Central directory
    /// record is too small to hold the required data,
    /// a Zip64 extended information record is created.
    /// The order of the fields in the zip64 extended
    /// information record is fixed, but the fields MUST
    /// only appear if the corresponding Local or Central
    /// directory record field is set to 0xFFFF or 0xFFFFFFFF.
    ///
    /// This entry in the Local header MUST include BOTH original
    /// and compressed file size fields. If encrypting the
    /// central directory and bit 13 of the general purpose bit
    /// flag is set indicating masking, the value stored in the
    /// Local Header for the original file size will be zero.
    /// </summary>
    /// <remarks>Header ID = 0x0001</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class Zip64ExtendedInformationExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Original uncompressed file size
        /// </summary>
        /// <remarks>Only exists if parent entry corresponding value is max</remarks>
        public ulong? OriginalSize { get; set; }

        /// <summary>
        /// Size of compressed data
        /// </summary>
        /// <remarks>Only exists if parent entry corresponding value is max</remarks>
        public ulong? CompressedSize { get; set; }

        /// <summary>
        /// Offset of local header record
        /// </summary>
        /// <remarks>Only exists if parent entry corresponding value is max</remarks>
        public ulong? RelativeHeaderOffset { get; set; }

        /// <summary>
        /// Number of the disk on which this file starts
        /// </summary>
        /// <remarks>Only exists if parent entry corresponding value is max</remarks>
        public uint? DiskStartNumber { get; set; }
    }
}
