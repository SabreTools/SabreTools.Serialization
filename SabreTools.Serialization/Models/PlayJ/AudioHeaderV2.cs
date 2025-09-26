namespace SabreTools.Serialization.Models.PlayJ
{
    /// <summary>
    /// PlayJ audio header / CDS entry header (V2)
    /// </summary>
    public sealed class AudioHeaderV2 : AudioHeader
    {
        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown1 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown2 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000000)
        /// </summary>
        public uint Unknown3 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000003)
        /// </summary>
        public uint Unknown4 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000001)
        /// </summary>
        public uint Unknown5 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000000)
        /// </summary>
        public uint Unknown6 { get; set; }

        /// <summary>
        /// Offset to unknown block 1, relative to the track ID
        /// </summary>
        public uint UnknownOffset1 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown7 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000004)
        /// </summary>
        public uint Unknown8 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000002)
        /// </summary>
        public uint Unknown9 { get; set; }

        /// <summary>
        /// Offset to unknown block 1, relative to the track ID
        /// </summary>
        /// <remarks>Always identical to <see cref="UnknownOffset1"/>?</remarks>
        public uint UnknownOffset2 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown10 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown11 { get; set; }

        /// <summary>
        /// Unknown (Always 0x0000005)
        /// </summary>
        public uint Unknown12 { get; set; }

        /// <summary>
        /// Unknown (Always 0x0000009)
        /// </summary>
        public uint Unknown13 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown14 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown15 { get; set; }

        /// <summary>
        /// Unknown (Always 0x0000000)
        /// </summary>
        public uint Unknown16 { get; set; }

        /// <summary>
        /// Unknown (Always 0x00000007)
        /// </summary>
        public uint Unknown17 { get; set; }

        /// <summary>
        /// Download track ID
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint TrackID { get; set; }

        /// <summary>
        /// Track year -- UNCONFIRMED
        /// </summary>
        /// <remarks>0xFFFFFFFF if unset</remarks>
        public uint Year { get; set; }

        /// <summary>
        /// Track number
        /// </summary>
        public uint TrackNumber { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint Unknown18 { get; set; }
    }
}