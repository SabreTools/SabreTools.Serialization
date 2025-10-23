namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660
    /// aka ECMA-119
    /// Note: Currently assumes a logical sector size of 2048 bytes
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class Archive
    {
        /// <summary>
        /// System Area, made up of 16 logical sectors
        /// 32,768 bytes, assuming logical sector size of 2048 bytes
        /// ISO9660 spec does not specify the content
        /// </summary>
        public byte[] SystemArea { get; set; }

        // Data Area:

        /// <summary>
        /// Set of Volume Descriptors
        /// Valid ISO9660 have:
        /// - At least one Primary Volume Descriptor (Type = 1)
        /// - Zero or more Supplementary/Enhanced Volume Descriptors (Type = 2)
        /// - Zero or more Volume Partition Descriptors (Type = 3)
        /// - Zero or more Boot Volume Descriptors (Type = 0)
        /// - At least one Volume Descriptor Set Terminator (Type = 255), as the final element(s) in the set
        /// </summary>
        public VolumeDescriptor[] VolumeDescriptorSet { get; set; }

        /// <summary>
        /// Filesystem extent
        /// Each entry is either a FileEntry or a DirectoryEntry (containing other Entries)
        /// </summary>
        public Extent[]? Extents { get; set; }
    }
}
