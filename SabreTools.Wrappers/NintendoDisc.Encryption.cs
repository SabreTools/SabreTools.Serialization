using SabreTools.Data.Models.NintendoDisc;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc
    {
        #region Wii Encryption / Decryption

        /// <summary>
        /// Resolves a Wii common key by its ticket index (0 = retail, 1 = Korean).
        /// Must be set by the caller before invoking <see cref="DecryptTitleKey"/>.
        /// If <see langword="null"/>, or the delegate returns <see langword="null"/> for a given
        /// index, decryption will return <see langword="null"/>.
        /// </summary>
        public static System.Func<byte, byte[]?>? CommonKeyProvider { get; set; }

        /// <summary>
        /// Decrypt a Wii partition title key from the ticket data.
        /// </summary>
        /// <param name="encryptedTitleKey">16-byte encrypted title key from ticket offset 0x1BF</param>
        /// <param name="titleId">8-byte title ID from ticket offset 0x1DC (big-endian)</param>
        /// <param name="commonKeyIndex">
        /// Common key index from ticket offset 0x1F1: 0 = retail, 1 = Korean
        /// </param>
        /// <returns>Decrypted 16-byte title key, or null if no key is available for the given index</returns>
        public static byte[]? DecryptTitleKey(byte[] encryptedTitleKey, byte[] titleId, byte commonKeyIndex)
        {
            if (encryptedTitleKey is null || encryptedTitleKey.Length != 16)
                return null;
            if (titleId is null || titleId.Length != 8)
                return null;

            byte[]? commonKey = CommonKeyProvider?.Invoke(commonKeyIndex);
            if (commonKey is null || commonKey.Length != 16)
                return null;

            // IV is the 8-byte title ID padded with zeros to 16 bytes
            byte[] iv = new byte[16];
            System.Array.Copy(titleId, 0, iv, 0, 8);

            return DecryptAesCbc(encryptedTitleKey, commonKey, iv);
        }

        /// <summary>
        /// Decrypt one Wii block of data (0x7C00 bytes) using AES-128-CBC.
        /// </summary>
        /// <param name="encryptedData">0x7C00 bytes of encrypted block data</param>
        /// <param name="titleKey">16-byte partition title key</param>
        /// <param name="iv">16-byte initialization vector (last 16 bytes of the preceding hash block)</param>
        /// <returns>Decrypted 0x7C00-byte block data, or null on error</returns>
        public static byte[]? DecryptBlock(byte[] encryptedData, byte[] titleKey, byte[] iv)
        {
            if (encryptedData is null || encryptedData.Length != Constants.WiiBlockDataSize)
                return null;
            if (titleKey is null || titleKey.Length != 16)
                return null;
            if (iv is null || iv.Length != 16)
                return null;

            return DecryptAesCbc(encryptedData, titleKey, iv);
        }

        private static byte[]? DecryptAesCbc(byte[] data, byte[] key, byte[] iv)
        {
            return AesCbc.Decrypt(data, key, iv);
        }

        #endregion
    }
}
