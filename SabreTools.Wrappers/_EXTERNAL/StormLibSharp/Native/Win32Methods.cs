using System.Runtime.InteropServices;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
namespace StormLibSharp.Native
{
    internal static class Win32Methods
    {
        [DllImport("kernel32", SetLastError = false, ExactSpelling = false)]
        public static extern int GetLastError();
    }
}
