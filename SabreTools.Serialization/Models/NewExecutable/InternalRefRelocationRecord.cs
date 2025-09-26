using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NewExecutable
{
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class InternalRefRelocationRecord
    {
        /// <summary>
        /// Segment number for a fixed segment, or 0FFh for a
        /// movable segment.
        /// </summary>
        public byte SegmentNumber;

        /// <summary>
        /// Always 0
        /// </summary>
        public byte Reserved;

        /// <summary>
        /// Offset into segment if fixed segment, or ordinal
        /// number index into Entry Table if movable segment.
        /// </summary>
        public ushort Offset;
    }
}
