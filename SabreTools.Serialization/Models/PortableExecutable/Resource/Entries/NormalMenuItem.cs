using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Contains information about each item in a menu resource that does not open a menu
    /// or a submenu. The structure definition provided here is for explanation only; it
    /// is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/normalmenuitem"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class NormalMenuItem : MenuItem
    {
        /// <summary>
        /// The type of menu item.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public MenuFlags NormalResInfo;

        /// <summary>
        /// A null-terminated Unicode string that contains the text for this menu item.
        /// There is no fixed limit on the size of this string.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? NormalMenuText;
    }
}
