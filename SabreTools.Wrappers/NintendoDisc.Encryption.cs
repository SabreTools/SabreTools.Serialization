using System.Security.Cryptography;
using SabreTools.Data.Models.NintendoDisc;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc
    {
        #region Wii Encryption / Decryption

        // TODO: Replace hardcoded common keys with a caller-supplied key provider and validator.
        // The intent is for consumers to inject keys (e.g. from a key file or secure store) rather
        // than having them embedded here, so these constants can be removed once that API exists.
        #region Common Keys

        /// <summary>
        /// Wii retail common key (index 0).
        /// Publicly known; used by Dolphin and other tools to decrypt title keys.
        /// </summary>
        private static readonly byte[] WiiCommonKeyRetail =
        {
            0xEB, 0xE4, 0x2A, 0x22, 0x5E, 0x85, 0x93, 0xE4,
            0x48, 0xD9, 0xC5, 0x45, 0x73, 0x81, 0xAA, 0xF7,
        };

        /// <summary>
        /// Wii Korean common key (index 1).
        /// Used for Korean-region titles.
        /// </summary>
        private static readonly byte[] WiiCommonKeyKorean =
        {
            0x63, 0xB8, 0x2B, 0xB4, 0xF4, 0x61, 0x4E, 0x2E,
            0x13, 0xF2, 0xFE, 0xFB, 0xBA, 0x4C, 0x9B, 0x7E,
        };

        #endregion

        /// <summary>
        /// Decrypt a Wii partition title key from the ticket data.
        /// </summary>
        /// <param name="encryptedTitleKey">16-byte encrypted title key from ticket offset 0x1BF</param>
        /// <param name="titleId">8-byte title ID from ticket offset 0x1DC (big-endian)</param>
        /// <param name="commonKeyIndex">
        /// Common key index from ticket offset 0x1F1: 0 = retail, 1 = Korean
        /// </param>
        /// <returns>Decrypted 16-byte title key, or null on error</returns>
        public static byte[]? DecryptTitleKey(byte[] encryptedTitleKey, byte[] titleId, byte commonKeyIndex)
        {
            if (encryptedTitleKey is null || encryptedTitleKey.Length != 16)
                return null;
            if (titleId is null || titleId.Length != 8)
                return null;

            byte[] commonKey = commonKeyIndex == 1 ? WiiCommonKeyKorean : WiiCommonKeyRetail;

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
#if NET20
            return null; // AES not available on net20
#else
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using var decryptor = aes.CreateDecryptor();
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
            catch
            {
                return null;
            }
#endif
        }

        #endregion
    }
}
