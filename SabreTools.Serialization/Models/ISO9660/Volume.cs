namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// A volume (or disc image) with a ISO9660 / ECMA-119 filesystem
    /// A set of volumes (set of disc images) makes up an entire file system (files may be spread across volumes/discs)
    /// Note: Volume can be accessed in logical sectors, usually 2048 bytes, but can be other higher powers of 2
    /// Note: Volume is made up of logical blocks, usually 2048 bytes, but can be any power of two (minimum 512 / 2^9)
    ///       The logical block size cannot be smaller than the logical sector size
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class Volume
    {
        /// <summary>
        /// System Area, made up of 16 logical blocks
        /// 32,768 bytes, assuming logical block size of 2048 bytes
        /// ISO9660 does not specify the content of the System Area
        /// </summary>
        public byte[]? SystemArea { get; set; }

        #region Data Area

        /// <summary>
        /// Set of Volume Descriptors
        /// Valid ISO9660 volumes have:
        /// - At least one Primary Volume Descriptor (Type = 1)
        /// - Zero or more Supplementary/Enhanced Volume Descriptors (Type = 2)
        /// - Zero or more Volume Partition Descriptors (Type = 3)
        /// - Zero or more Boot Volume Descriptors (Type = 0)
        /// - At least one Volume Descriptor Set Terminator (Type = 255), as the final element(s) in the set
        /// </summary>
        public VolumeDescriptor[]? VolumeDescriptorSet { get; set; }

        /// <summary>
        /// List of path table records for each directory on the volume
        /// One set of path tables is provided for each Base Volume Descriptor
        /// </summary>
        public PathTableGroup[]? PathTableGroups { get; set; }

        /// <summary>
        /// The root directory(ies) pointed to by the Volume Descriptors' root directory records
        /// </summary>
        public Directory[]? RootDirectories { get; set; }

        #endregion
    }
}
