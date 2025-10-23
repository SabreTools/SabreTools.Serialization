namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Volume Partition Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x03
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class VolumePartitionDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// 32-byte name of the intended system that can use this record
        /// a-characters only
        /// </summary>
        public byte[] SystemIdentifier { get; set; }

        /// <summary>
        /// 32-byte name of this volume partition
        /// d-characters only
        /// </summary>
        public byte[] VolumePartitionIdentifier { get; set; }

        /// <summary>
        /// Logical block number of the first logical block allocated to this volume partition
        /// </summary>
        public int VolumePartitionLocation { get; set; }

        /// <summary>
        /// Number of logical blocks allocated to this volume partition
        /// </summary>
        public int VolumePartitionSize { get; set; }

        /// <summary>
        /// 1960 bytes for System Use, contents not defined by ISO9660
        /// </summary>
        public byte[] SystemUse { get; set; }
    }
}
