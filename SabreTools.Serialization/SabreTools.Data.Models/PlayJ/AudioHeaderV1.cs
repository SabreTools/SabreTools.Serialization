namespace SabreTools.Data.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header (V1)
    /// </summary>
    public sealed class AudioHeaderV1 : AudioHeader
    {
        /// <summary>
        /// Download track ID
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint TrackID { get; set; }

        /// <summary>
        /// Offset to unknown data block 1
        /// </summary>
        public uint UnknownOffset1 { get; set; }

        /// <summary>
        /// Offset to unknown data block 2
        /// </summary>
        public uint UnknownOffset2 { get; set; }

        /// <summary>
        /// Offset to unknown data block 3
        /// </summary>
        public uint UnknownOffset3 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Always 0x00000001</remarks>
        public uint Unknown1 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Typically 0x00000001 in download titles</remarks>
        public uint Unknown2 { get; set; }

        /// <summary>
        /// Track year
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint Year { get; set; }

        /// <summary>
        /// Track number
        /// </summary>
        public byte TrackNumber { get; set; }

        /// <summary>
        /// Subgenre
        /// </summary>
        public Subgenre Subgenre { get; set; }

        /// <summary>
        /// Track duration in seconds
        /// </summary>
        public uint Duration { get; set; }
    }
}
