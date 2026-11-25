namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// Partition layout of a built image
    /// </summary>
    public enum UDFLayout
    {
        /// <summary>
        /// One Type 1 partition
        /// </summary>
        Type1,

        /// <summary>
        /// One Sparable partition with packet length 4, packets 4, 16 and 24 relocated to sectors 104, 100 and 108
        /// </summary>
        Sparable,

        /// <summary>
        /// A Virtual partition on a Type 1 partition, UDF 2.00 Virtual Allocation Table embedded in its File Entry
        /// </summary>
        Virtual,

        /// <summary>
        /// A Virtual partition on a Type 1 partition, UDF 1.50 Virtual Allocation Table recorded in block 37
        /// </summary>
        Virtual150,
    }
}
