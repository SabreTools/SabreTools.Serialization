namespace SabreTools.Serialization.Models.NewExecutable
{
    /// <summary>
    /// The location and size of the per-segment data is defined in the
    /// segment table entry for the segment. If the segment has relocation
    /// fixups, as defined in the segment table entry flags, they directly
    /// follow the segment data in the file. The relocation fixup information
    /// is defined as follows:
    /// </summary>
    /// <remarks>
    /// To find the relocation data for a segment, seek to:
    ///     <see cref="SegmentTableEntry.Offset"/>
    ///     * (1 << <see cref="ExecutableHeader.SegmentAlignmentShiftCount"/>)
    ///     + <see cref="SegmentTableEntry.Length"/> 
    /// </remarks>
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    /// <see href="https://wiki.osdev.org/NE"/>
    public sealed class PerSegmentData
    {
        /// <summary>
        /// Number of relocation records that follow.
        /// </summary>
        public ushort RelocationRecordCount { get; set; }

        /// <summary>
        /// A table of relocation records follows.
        /// </summary>
        public RelocationRecord[]? RelocationRecords { get; set; }
    }
}
