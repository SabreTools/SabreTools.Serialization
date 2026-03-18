using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Common base class for menu item types
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/normalmenuitem"/>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/popupmenuitem"/>
    [StructLayout(LayoutKind.Sequential)]
    public abstract class MenuItem { }
}
