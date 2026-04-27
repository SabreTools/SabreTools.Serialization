using System.Collections.Generic;

namespace SabreTools.Data.Models.OperaFS
{
    /// <summary>
    /// Opera Filesystem (or user-data-only disc image) present on 3DO and M2 discs
    /// Usually contained within a CDROM disc image (2352-byte bin file)
    /// All fields are Big-Endian
    /// </summary>
    /// <see href="https://groups.google.com/g/rec.games.video.3do/c/1U3qrmLSYMQ"/>
    public sealed class FileSystem
    {
        /// <summary>
        /// Volume Descriptor
        /// </summary>
        public VolumeDescriptor VolumeDescriptor { get; set; } = new();

        /// <summary>
        /// Map of all directories in filesystem, and their offsets
        /// Duplicate directories exist at different offsets
        /// </summary>
        public Dictionary<uint, DirectoryDescriptor> Directories { get; set; } = [];
    }
}
