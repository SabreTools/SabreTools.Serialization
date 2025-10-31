using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// The exheader has two sections:
    /// - The actual exheader data, containing System Control Info (SCI) and Access Control Info (ACI);
    /// - A signed copy of NCCH HDR public key, and exheader ACI. This version of the ACI is used as limitation to the actual ACI.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class NCCHExtendedHeader
    {
        /// <summary>
        /// SCI
        /// </summary>
        public SystemControlInfo SCI;

        /// <summary>
        /// ACI
        /// </summary>
        public AccessControlInfo ACI;

        /// <summary>
        /// AccessDesc signature (RSA-2048-SHA256)
        /// </summary>
        /// <remarks>0x100 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] AccessDescSignature = new byte[0x100];

        /// <summary>
        /// NCCH HDR RSA-2048 public key
        /// </summary>
        /// <remarks>0x100 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] NCCHHDRPublicKey = new byte[0x100];

        /// <summary>
        /// ACI (for limitation of first ACI)
        /// </summary>
        public AccessControlInfo ACIForLimitations;
    }
}
