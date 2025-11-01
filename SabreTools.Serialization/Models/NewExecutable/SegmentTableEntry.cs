using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NewExecutable
{
    /// <summary>
    /// The segment table contains an entry for each segment in the executable
    /// file. The number of segment table entries are defined in the segmented
    /// EXE header. The first entry in the segment table is segment number 1.
    /// The following is the structure of a segment table entry.
    /// </summary>
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    /// <see href="https://wiki.osdev.org/NE"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class SegmentTableEntry
    {
        /// <summary>
        /// Logical-sector offset (n byte) to the contents of the segment
        /// data, relative to the beginning of the file. Zero means no
        /// file data.
        /// </summary>
        /// <remarks>Byte offset is: Offset * (1 << <see cref="ExecutableHeader.SegmentAlignmentShiftCount"/>)</remarks>
        public ushort Offset { get; set; }

        /// <summary>
        /// Length of the segment in the file, in bytes. Zero means 64K.
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// Flag word.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public SegmentTableEntryFlag FlagWord;

        /// <summary>
        /// Minimum allocation size of the segment, in bytes. Total size
        /// of the segment. Zero means 64K.
        /// </summary>
        public ushort MinimumAllocationSize { get; set; }

        /// <summary>
        /// Segment data
        /// </summary>
        /// <remarks>
        /// Data is not sequential to the entry header. It lives at
        /// the <see cref="Offset"/> and has a size of <see cref="Length"/>
        /// </remarks>
        public byte[] Data { get; set; } = [];

        /// <summary>
        /// Per-segment data
        /// </summary>
        /// <remarks>
        /// This only exists if <see cref="FlagWord"/> has a flag value
        /// of <see cref="SegmentTableEntryFlag.RELOCINFO"/>. It immediately
        /// follows <see cref="Data"/>.
        /// </remarks>
        public PerSegmentData? PerSegmentData { get; set; }
    }
}
