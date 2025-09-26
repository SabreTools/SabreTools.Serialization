using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.NewExecutable
{
    /// <summary>
    /// A table of resources for this type follows. The following is
    /// the format of each resource (8 bytes each):
    /// </summary>
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    /// <see href="https://wiki.osdev.org/NE"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ResourceTypeResourceEntry
    {
        /// <summary>
        /// File offset to the contents of the resource data,
        /// relative to beginning of file. The offset is in terms
        /// of the alignment shift count value specified at
        /// beginning of the resource table.
        /// </summary>
        /// <remarks>Byte offset is: Offset * (1 << <see cref="ResourceTable.AlignmentShiftCount"/>)</remarks>
        public ushort Offset;

        /// <summary>
        /// Length of the resource in the file (in bytes).
        /// </summary>
        /// <remarks>Byte length is: Length * (1 << <see cref="ResourceTable.AlignmentShiftCount"/>)</remarks>
        public ushort Length;

        /// <summary>
        /// Flag word.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ResourceTypeResourceFlag FlagWord;

        /// <summary>
        /// Resource ID. This is an integer type if the high-order
        /// bit is set (8000h), otherwise it is the offset to the
        /// resource string, the offset is relative to the
        /// beginning of the resource table.
        /// </summary>
        public ushort ResourceID;

        /// <summary>
        /// Reserved.
        /// </summary>
        public uint Reserved;
    }
}
