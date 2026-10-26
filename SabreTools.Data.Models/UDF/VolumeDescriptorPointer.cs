namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Volume Descriptor Pointer (3/10.3), continues the Volume Descriptor Sequence in another extent
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class VolumeDescriptorPointer : Descriptor
    {
        /// <summary>
        /// Volume Descriptor Sequence Number
        /// </summary>
        public uint VolumeDescriptorSequenceNumber { get; set; }

        /// <summary>
        /// Extent of the next part of the Volume Descriptor Sequence
        /// </summary>
        public ExtentDescriptor NextVolumeDescriptorSequenceExtent { get; set; } = new();

        /// <summary>
        /// Reserved, 484 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[484];
    }
}
