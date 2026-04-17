namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// Represents a parsed WIA or RVZ compressed disc image.
    /// Contains the two headers and all lookup tables.
    /// Actual group (compressed block) data is accessed via the source stream.
    /// </summary>
    public class Archive
    {
        /// <summary>
        /// WIA / RVZ primary header (0x48 bytes)
        /// </summary>
        public WiaHeader1 Header1 { get; set; } = new();

        /// <summary>
        /// WIA / RVZ secondary header (0xDC bytes)
        /// </summary>
        public WiaHeader2 Header2 { get; set; } = new();

        /// <summary>
        /// Wii partition entries. Null or empty for GameCube discs.
        /// </summary>
        public PartitionEntry[]? PartitionEntries { get; set; }

        /// <summary>
        /// Raw (non-partition) data region entries
        /// </summary>
        public RawDataEntry[] RawDataEntries { get; set; } = [];

        /// <summary>
        /// WIA group entries (populated when <see cref="IsRvz"/> is false)
        /// </summary>
        public WiaGroupEntry[]? GroupEntries { get; set; }

        /// <summary>
        /// RVZ group entries (populated when <see cref="IsRvz"/> is true)
        /// </summary>
        public RvzGroupEntry[]? RvzGroupEntries { get; set; }

        /// <summary>
        /// True if this is an RVZ file; false if this is a WIA file
        /// </summary>
        public bool IsRvz { get; set; }
    }
}
