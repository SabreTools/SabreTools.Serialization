using System.Collections.Generic;

namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// A volume (or disc image) with a UDF / ECMA-167 file system
    /// </summary>
    /// <remarks>
    /// Logical sectors and logical blocks are 2048 bytes on optical media
    /// File system structures are addressed by logical block address (partition reference number + logical block);
    /// dictionaries are keyed by ((long)PartitionReferenceNumber &lt;&lt; 32) | LogicalBlockNumber
    /// </remarks>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class Volume
    {
        /// <summary>
        /// Volume Recognition Sequence starting at sector 16
        /// </summary>
        public VolumeStructureDescriptor[] VolumeRecognitionSequence { get; set; } = [];

        /// <summary>
        /// Anchor Volume Descriptor Pointer
        /// </summary>
        public AnchorVolumeDescriptorPointer? AnchorVolumeDescriptorPointer { get; set; }

        /// <summary>
        /// Volume Descriptor Sequence (Primary, Partition, Logical Volume, ... up to the Terminating Descriptor)
        /// </summary>
        public Descriptor[] VolumeDescriptorSequence { get; set; } = [];

        /// <summary>
        /// File Set Descriptor
        /// </summary>
        public FileSetDescriptor? FileSetDescriptor { get; set; }

        /// <summary>
        /// File Entries (and Extended File Entries) by logical block address
        /// </summary>
        public Dictionary<long, FileEntry> FileEntries { get; set; } = [];

        /// <summary>
        /// Allocation Extent Descriptors by logical block address
        /// </summary>
        public Dictionary<long, AllocationExtentDescriptor> AllocationExtentDescriptors { get; set; } = [];

        /// <summary>
        /// File Identifier Descriptors of each directory, by logical block address of the directory's File Entry
        /// </summary>
        public Dictionary<long, FileIdentifierDescriptor[]> Directories { get; set; } = [];
    }
}
