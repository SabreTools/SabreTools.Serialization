using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Contains information about the menu items in a menu resource that open a menu
    /// or a submenu. The structure definition provided here is for explanation only;
    /// it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/popupmenuitem"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class PopupMenuItem : MenuItem
    {
        /// <summary>
        /// Describes the menu item.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public MenuFlags PopupItemType;

        /// <summary>
        /// Describes the menu item.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public MenuFlags PopupState;

        /// <summary>
        /// A numeric expression that identifies the menu item that is passed in the
        /// WM_COMMAND message.
        /// </summary>
        public uint PopupID;

        /// <summary>
        /// A set of bit flags that specify the type of menu item.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public MenuFlags PopupResInfo;

        /// <summary>
        /// A null-terminated Unicode string that contains the text for this menu item.
        /// There is no fixed limit on the size of this string.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? PopupMenuText;
    }
}
