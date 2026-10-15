namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Anchor Volume Descriptor Pointer (3/10.2), recorded at sector 256 (and at the end of the volume)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class AnchorVolumeDescriptorPointer : Descriptor
    {
        /// <summary>
        /// Extent of the Main Volume Descriptor Sequence
        /// </summary>
        public ExtentDescriptor MainVolumeDescriptorSequenceExtent { get; set; } = new();

        /// <summary>
        /// Extent of the Reserve Volume Descriptor Sequence
        /// </summary>
        public ExtentDescriptor ReserveVolumeDescriptorSequenceExtent { get; set; } = new();

        /// <summary>
        /// Reserved, 480 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[480];
    }
}
