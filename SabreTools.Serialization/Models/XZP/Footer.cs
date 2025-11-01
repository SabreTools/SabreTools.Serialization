using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Footer
    {
        public uint FileLength;

        /// <summary>
        /// "tFzX"
        /// </summary>
        /// <remarks>4 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Signature = string.Empty;
    }
}
