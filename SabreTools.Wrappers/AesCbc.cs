using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// AES-128-CBC encrypt/decrypt helpers used by NintendoDisc and WIA/RVZ.
    /// </summary>
    /// <remarks>
    /// Implemented directly via BouncyCastle because <c>SabreTools.Security.Cryptography</c>
    /// currently only exposes AES-CTR.  When an <c>AESCBC</c> wrapper is added to that
    /// library, replace the bodies of <see cref="Decrypt"/> and <see cref="Encrypt"/> with
    /// the equivalent <c>AESCBC.Decrypt</c> / <c>AESCBC.Encrypt</c> calls and remove the
    /// BouncyCastle using directives from this file.
    /// </remarks>
    internal static class AesCbc
    {
        /// <summary>
        /// Decrypts <paramref name="data"/> with AES-128-CBC (no padding).
        /// Returns null if any argument is invalid or decryption fails.
        /// </summary>
        /// <param name="data">Ciphertext to decrypt.</param>
        /// <param name="key">16-byte AES key.</param>
        /// <param name="iv">16-byte initialisation vector.</param>
        public static byte[]? Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // TODO: replace with AESCBC.Decrypt(data, key, iv) once
            //       SabreTools.Security.Cryptography adds an AES-CBC wrapper.
            try
            {
                var cipher = CreateCipher(forEncryption: false, key, iv);
                return cipher.DoFinal(data);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Encrypts <paramref name="data"/> with AES-128-CBC (no padding).
        /// Returns null if any argument is invalid or encryption fails.
        /// </summary>
        /// <param name="data">Plaintext to encrypt.</param>
        /// <param name="key">16-byte AES key.</param>
        /// <param name="iv">16-byte initialisation vector.</param>
        public static byte[]? Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            // TODO: replace with AESCBC.Encrypt(data, key, iv) once
            //       SabreTools.Security.Cryptography adds an AES-CBC wrapper.
            try
            {
                var cipher = CreateCipher(forEncryption: true, key, iv);
                return cipher.DoFinal(data);
            }
            catch
            {
                return null;
            }
        }

        private static IBufferedCipher CreateCipher(bool forEncryption, byte[] key, byte[] iv)
        {
            var keyParam = new KeyParameter(key);
            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(forEncryption, new ParametersWithIV(keyParam, iv));
            return cipher;
        }
    }
}
