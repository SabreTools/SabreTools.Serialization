using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.NewExecutable
{
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class OSFixupRelocationRecord
    {
        /// <summary>
        /// Operating system fixup type.
        /// Floating-point fixups.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public OSFixupType FixupType;

        /// <summary>
        /// 0
        /// </summary>
        public ushort Reserved;
    }
}
