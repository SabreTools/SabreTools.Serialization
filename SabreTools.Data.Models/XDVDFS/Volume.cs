using System.Collections.Generic;

namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Xbox DVD Filesystem (aka "XISO"), 2048-byte sector size
    /// Present in XGD1, XGD2, and XGD3 discs
    /// Gaps between directory and file extents are filled with pseudo-random filler data (unless intentionally wiped)
    /// Some early XGD1 filesystems use a simpler PRNG algorithm that can have its seed brute forced
    /// Gaps also include the security sector ranges, 4096 sectors each that cannot be read from discs (usually zeroed)
    /// </summary>
    /// <see href="https://xboxdevwiki.net/XDVDFS"/>
    /// <see href="https://github.dev/Deterous/XboxKit/"/>
    public class Volume
    {
        /// <summary>
        /// Reserved Area, made up of 32 sectors
        /// Contains pseudo-random filler data on-disc
        /// Some XDVDFS images zero the reserved area
        /// </summary>
        /// <remarks>65,536 bytes</remarks>
        public byte[] ReservedArea { get; set; } = new byte[0x10000];

        /// <summary>
        /// XDVDFS Volume Descriptor
        /// </summary>
        /// <remarks>2048 bytes</remarks>
        public VolumeDescriptor VolumeDescriptor { get; set; } = new();

        /// <summary>
        /// Xbox DVD Layout Descriptor, immediately follows Volume Descriptor
        /// XGD1: Contains version numbers and signature bytes
        /// XGD2: Zeroed apart from initial signature bytes
        /// XGD3: Sector not present
        /// </summary>
        /// <remarks>2048 bytes</remarks>
        public LayoutDescriptor? LayoutDescriptor { get; set; }

        /// <summary>
        /// Map of sector numbers and the directory descriptor at that sector number
        /// The root directory descriptor is not guaranteed to be the earliest
        /// </summary>
        public Dictionary<uint, DirectoryDescriptor> DirectoryDescriptors { get; set; } = [];
    }
}
