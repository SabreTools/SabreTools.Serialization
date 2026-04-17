using SabreTools.Numerics;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Volume Descriptor, for STFS packages
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class SVODDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// Volume descriptor size (Should be 0x24)
        /// </summary>
        public byte VolumeDescriptorSize { get; set; }

        /// <summary>
        /// Block Cache Element Count
        /// </summary>
        public byte BlockCacheElementCount { get; set; }

        /// <summary>
        /// Worker Thread Processor
        /// </summary>
        public byte WorkerThreadProcessor { get; set; }

        /// <summary>
        /// Worker Thread Priority
        /// </summary>
        public byte WorkerThreadPriority { get; set; }

        /// <summary>
        /// Hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] Hash { get; set; } = new byte[20];

        /// <summary>
        /// Device features
        /// </summary>
        public byte DeviceFeatures { get; set; }

        /// <summary>
        /// Data Block Count
        /// </summary>
        /// <remarks>Little-endian, 3-byte uint24</remarks>
        public UInt24 DataBlockCount { get; set; } = new();

        /// <summary>
        /// Data Block Offset
        /// </summary>
        /// <remarks>Little-endian, 3-byte uint24</remarks>
        public UInt24 DataBlockOffset { get; set; } = new();

        /// <summary>
        /// Padding, should be zeroed
        /// </summary>
        /// <remarks>5 bytes</remarks>
        public byte[] Padding { get; set; } = new byte[5];
    }
}
