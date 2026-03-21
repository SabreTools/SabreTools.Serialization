using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.VPK
{
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class SignatureSection
    {
        /// <summary>
        /// Always seen as 160 (0xA0) bytes
        /// </summary>
        public uint PublicKeySize;

        /// <summary>
        /// <see cref="PublicKeySize"/>
        /// </summary>
        public byte[] PublicKey = [];

        /// <summary>
        /// Always seen as 128 (0x80) bytes
        /// </summary>
        public uint SignatureSize;

        /// <summary>
        /// <see cref="SignatureSize"/>
        /// </summary>
        public byte[] Signature = [];
    }
}
