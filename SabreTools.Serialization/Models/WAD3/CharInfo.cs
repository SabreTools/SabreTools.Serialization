using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class CharInfo
    {
        public ushort StartOffset;

        public ushort CharWidth;
    }
}
