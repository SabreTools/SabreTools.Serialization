using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/CIA#CIA_Header"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CIAHeader
    {
        /// <summary>
        /// Archive header size, usually 0x2020 bytes
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        /// Type
        /// </summary>
        public ushort Type;

        /// <summary>
        /// Version
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Certificate chain size
        /// </summary>
        public uint CertificateChainSize;

        /// <summary>
        /// Ticket size
        /// </summary>
        public uint TicketSize;

        /// <summary>
        /// TMD file size
        /// </summary>
        public uint TMDFileSize;

        /// <summary>
        /// Meta size (0 if no Meta data is present)
        /// </summary>
        public uint MetaSize;

        /// <summary>
        /// Content size
        /// </summary>
        public ulong ContentSize;

        /// <summary>
        /// Content Index
        /// </summary>
        /// <remarks>0x2000 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2000)]
        public byte[] ContentIndex = new byte[0x2000];
    }
}
