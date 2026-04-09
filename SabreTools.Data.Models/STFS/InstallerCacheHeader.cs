using SabreTools.Numerics;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Volume Descriptor, for System or Title Cache Installer STFS packages
    /// </summary>
    public class InstallerCacheHeader : InstallerHeader
    {
        /// <summary>
        /// Resume state enum
        /// See Enums.ResumeState
        /// </summary>
        /// <remarks>If present, 4 bytes</remarks>
        public uint ResumeState { get; set; }

        /// <summary>
        /// Current file index
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong CurrentFileIndex { get; set; }

        /// <summary>
        /// Number of bytes processed
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong BytesProcessed { get; set; }

        /// <summary>
        /// Datetime for last modified
        /// </summary>
        /// <remarks>Microsoft FILETIME, 4 bytes</remarks>
        public byte[] LastModifiedDateTime { get; set; } = new byte[4];

        /// <summary>
        /// Cache resume data
        /// </summary>
        /// <remarks>5584 bytes</remarks>
        public byte[] ResumeData { get; set; } = new byte[5584];
    }
}
