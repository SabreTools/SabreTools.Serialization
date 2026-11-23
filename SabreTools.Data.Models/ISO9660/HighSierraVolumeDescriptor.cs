using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Abstract High Sierra Volume Descriptor
    /// Each VolumeDescriptor consists of 1 logical sector (usually 2048 bytes)
    /// </summary>
    /// <remarks>
    /// The first 15 bytes are a fixed header, the remaining bytes are application specific
    /// The header starts with the logical block number of the volume descriptor (bytes 0-7),
    /// followed by the type (byte 8), the identifier (bytes 9-13) and the version (byte 14)
    /// Identifier is set to Constants.HighSierraIdentifier ("CDROM")
    /// </remarks>
    /// <see href="https://github.com/torvalds/linux/blob/master/include/uapi/linux/iso_fs.h"/>
    public abstract class HighSierraVolumeDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// Logical block number of this volume descriptor
        /// </summary>
        public BothInt32 LogicalBlockNumber { get; set; } = 0;
    }
}
