using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.PortableExecutable.Resource.Entries
{
    /// <summary>
    /// Common base class for menu item types
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/menuheader"/>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/menuex-template-header"/>
    [StructLayout(LayoutKind.Sequential)]
    public abstract class MenuHeader { }
}
