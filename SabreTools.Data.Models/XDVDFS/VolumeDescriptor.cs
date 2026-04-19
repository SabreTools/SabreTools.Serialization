namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// XDVDFS Volume Descriptor, present at sector 32 (offset 0x10000) of an Xbox DVD Filesystem
    /// Present on XGD1, XGD2, and XGD3 discs
    /// </summary>
    /// <see href="https://multimedia.cx/xdvdfs.html"/>
    /// <see href="https://github.com/Deterous/XboxKit/"/>
    public class VolumeDescriptor
    {
        /// <summary>
        /// Volume descriptor magic, start
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] StartSignature { get; set; } = new byte[20]; // VolumeDescriptorSignature

        /// <summary>
        /// UInt32 sector location of the root directory descriptor
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public uint RootOffset { get; set; }

        /// <summary>
        /// UInt32 size of the root directory descriptor in bytes
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public uint RootSize { get; set; }

        /// <summary>
        /// Win32 FILETIME filesystem mastering timestamp
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public long MasteringTimestamp { get; set; }

        /// <summary>
        /// Unknown byte, seemingly 0x00 for XGD1, and 0x01 for XGD2 and XGD3
        /// </summary>
        /// <remarks>1991 bytes</remarks>
        public byte UnknownByte { get; set; }

        /// <summary>
        /// Seemingly unused bytes in first sector that are expected to be zeroed
        /// </summary>
        /// <remarks>1991 bytes</remarks>
        public byte[] Reserved { get; set; } = new byte[1991];

        /// <summary>
        /// Volume descriptor magic, start
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] EndSignature { get; set; } = new byte[20]; // VolumeDescriptorSignature
    }
}
