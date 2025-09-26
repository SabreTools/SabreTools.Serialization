using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#InitialData"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class InitialData
    {
        /// <summary>
        /// Card seed keyY (first u64 is Media ID (same as first NCCH partitionId))
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[]? CardSeedKeyY;

        /// <summary>
        /// Encrypted card seed (AES-CCM, keyslot 0x3B for retail cards, see CTRCARD_SECSEED)
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[]? EncryptedCardSeed;

        /// <summary>
        /// Card seed AES-MAC
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[]? CardSeedAESMAC;

        /// <summary>
        /// Card seed nonce
        /// </summary>
        /// <remarks>0x0C bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
        public byte[]? CardSeedNonce;

        /// <summary>
        /// Reserved3
        /// </summary>
        /// <remarks>0xC4 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC4)]
        public byte[]? Reserved;

        /// <summary>
        /// Copy of first NCCH header (excluding RSA signature)
        /// </summary>
        public NCCHHeader? BackupHeader;
    }
}
