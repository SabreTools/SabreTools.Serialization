using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.SafeDisc
{
    /// <see href="https://web.archive.org/web/20241008171855/http://blog.w4kfu.com/tag/safedisc"/>
    [StructLayout(LayoutKind.Sequential)]
    public class EncryptedFileEntry
    {
        /// <summary>
        /// 0xA8726B03
        /// </summary>
        public uint Signature1;

        /// <summary>
        /// 0xEF01996C
        /// </summary>
        public uint Signature2;

        public uint FileNumber;

        public uint Offset1;

        public uint Offset2;

        public uint Unknown1;

        public uint Unknown2;

        /// <remarks>0x0D bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0D)]
        public byte[]? Name;
    }
}
