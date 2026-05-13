using System;
using SabreTools.Hashing;

// TODO: Remove when IO is updated
namespace SabreTools.Wrappers
{
    public class WiiDecrypter
    {
        /// <summary>
        /// Retail common key (index 0)
        /// </summary>
        public byte[] RetailCommonKey
        {
            get;
            set
            {
                if (ValidateRetailCommonKey(value))
                    field = value;
            }
        } = [];

        /// <summary>
        /// Korean common key (index 1)
        /// </summary>
        public byte[] KoreanCommonKey
        {
            get;
            set
            {
                if (ValidateKoreanCommonKey(value))
                    field = value;
            }
        } = [];

        #region Internal Test Values

        /// <summary>
        /// Korean common key SHA-256 hash
        /// </summary>
        private const string KoreanCommonKeySHA256 = "b9f42ca27a1e178f0f14ebf1a05d486fa8db8d08875336c4e6e8dfae29f2901c";

        /// <summary>
        /// Retail common key SHA-256 hash
        /// </summary>
        private const string RetailCommonKeySHA256 = "de38aeab4fe0c36d828a47e6fd315100e7ce234d3b00aa25e6ad6f5ff2824af8";

        #endregion

        #region Decryption

        /// <summary>
        /// Decrypt a Wii partition title key from the ticket data.
        /// </summary>
        /// <param name="encryptedTitleKey">16-byte encrypted title key from ticket offset 0x1BF</param>
        /// <param name="titleId">8-byte title ID from ticket offset 0x1DC (big-endian)</param>
        /// <param name="commonKeyIndex">Common key index to use for decryption</param>
        /// <returns>Decrypted 16-byte title key, or null if no key is available for the given index</returns>
        public byte[]? DecryptTitleKey(byte[]? encryptedTitleKey, byte[]? titleId, int commonKeyIndex)
        {
            if (encryptedTitleKey is null || encryptedTitleKey.Length != 16)
                return null;
            if (titleId is null || titleId.Length != 8)
                return null;

            byte[] commonKey;
            if (commonKeyIndex == 0)
                commonKey = RetailCommonKey;
            else if (commonKeyIndex == 1)
                commonKey = KoreanCommonKey;
            else
                return null;

            return DecryptTitleKey(encryptedTitleKey, titleId, commonKey);
        }

        /// <summary>
        /// Decrypt a Wii partition title key from the ticket data.
        /// </summary>
        /// <param name="encryptedTitleKey">16-byte encrypted title key from ticket offset 0x1BF</param>
        /// <param name="titleId">8-byte title ID from ticket offset 0x1DC (big-endian)</param>
        /// <param name="commonKey">Common key to use for decryption</param>
        /// <returns>Decrypted 16-byte title key, or null if no key is available for the given index</returns>
        public static byte[]? DecryptTitleKey(byte[]? encryptedTitleKey, byte[]? titleId, byte[] commonKey)
        {
            if (encryptedTitleKey is null || encryptedTitleKey.Length != 16)
                return null;
            if (titleId is null || titleId.Length != 8)
                return null;

            if (commonKey.Length != 16)
                return null;

            // IV is the 8-byte title ID padded with zeros to 16 bytes
            byte[] iv = new byte[16];
            Array.Copy(titleId, 0, iv, 0, 8);

            return AesCbc.Decrypt(encryptedTitleKey, commonKey, iv);
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
            if (encryptedData is null || encryptedData.Length != 0x7C00)
                return null;
            if (titleKey is null || titleKey.Length != 16)
                return null;
            if (iv is null || iv.Length != 16)
                return null;

            return AesCbc.Decrypt(encryptedData, titleKey, iv);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validate the Korean common key based on hash and length
        /// </summary>
        /// <param name="commonKey">Korean common key to validate</param>
        /// <returns>True if the key was valid, false otherwise</returns>
        public static bool ValidateKoreanCommonKey(byte[]? commonKey)
        {
            // Ignore invalid values
            if (commonKey is null || commonKey.Length != 16)
                return false;

            // Hash the key and compare
            string? actualHash = HashTool.GetByteArrayHash(commonKey, HashType.SHA256);
            return string.Equals(actualHash, KoreanCommonKeySHA256, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validate the retail common key based on hash and length
        /// </summary>
        /// <param name="commonKey">Retail common key to validate</param>
        /// <returns>True if the key was valid, false otherwise</returns>
        public static bool ValidateRetailCommonKey(byte[]? commonKey)
        {
            // Ignore invalid values
            if (commonKey is null || commonKey.Length != 16)
                return false;

            // Hash the key and compare
            string? actualHash = HashTool.GetByteArrayHash(commonKey, HashType.SHA256);
            return string.Equals(actualHash, RetailCommonKeySHA256, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
