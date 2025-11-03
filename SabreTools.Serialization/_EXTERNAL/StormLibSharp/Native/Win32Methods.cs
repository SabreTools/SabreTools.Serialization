using System.Runtime.InteropServices;

namespace StormLibSharp.Native
{
    internal static class Win32Methods
    {
        [DllImport("kernel32", SetLastError = false, ExactSpelling = false)]
        public static extern int GetLastError();
    }
}
