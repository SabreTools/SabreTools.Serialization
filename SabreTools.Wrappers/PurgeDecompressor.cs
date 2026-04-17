using System;
using System.Security.Cryptography;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Decompresses WIA PURGE-compressed group data.
    ///
    /// PURGE layout (uncompressed exception-list prefix already stripped by caller):
    ///   [ { u32 offset BE, u32 size BE } { data[size] } ] ...  (zero or more segments)
    ///   [ SHA-1 (20 bytes) ]
    ///
    /// The SHA-1 covers: <paramref name="precedingBytes"/> (exception-list bytes, if any) +
    /// all segment headers and their data bytes.
    /// Bytes in the output not covered by any segment are implicitly 0x00.
    ///
    /// References: Dolphin WIACompression.cpp — PurgeDecompressor / PurgeCompressor
    ///             docs/WiaAndRvz.md — wia_segment_t section
    /// </summary>
    internal static class PurgeDecompressor
    {
        private const int SHA1_SIZE = 20;
        private const int SEGMENT_HEADER_SIZE = 8; // u32 offset + u32 size

        /// <summary>
        /// Decompresses a PURGE-compressed block.
        /// </summary>
        /// <param name="input">Buffer containing the compressed data.</param>
        /// <param name="inputOffset">Byte offset within <paramref name="input"/> where compressed data starts.</param>
        /// <param name="inputLength">Number of bytes of compressed data (segments + trailing SHA-1).</param>
        /// <param name="decompressedSize">Expected decompressed output size in bytes.</param>
        /// <param name="precedingBytes">
        /// Bytes that precede the compressed data in the SHA-1 computation — the uncompressed
        /// exception-list section for Wii partition groups. Pass null for non-Wii groups.
        /// </param>
        /// <returns>
        /// The decompressed byte array, or <c>null</c> if the data is malformed or the
        /// trailing SHA-1 does not match.
        /// </returns>
        public static byte[]? Decompress(
            byte[] input, int inputOffset, int inputLength,
            int decompressedSize,
            byte[]? precedingBytes = null)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (inputLength < SHA1_SIZE) return null;

            byte[] output = new byte[decompressedSize];
            int pos = inputOffset;
            int dataEnd = inputOffset + inputLength - SHA1_SIZE;

#if NET20 || NET35
            using (var sha1 = SHA1.Create())
#else
            using (var sha1 = SHA1.Create())
#endif
            {
                if (precedingBytes != null && precedingBytes.Length > 0)
                    sha1.TransformBlock(precedingBytes, 0, precedingBytes.Length, null, 0);

                while (pos < dataEnd)
                {
                    if (pos + SEGMENT_HEADER_SIZE > dataEnd)
                        return null;

                    uint segOffset = ReadUInt32BE(input, pos);
                    uint segSize   = ReadUInt32BE(input, pos + 4);

                    sha1.TransformBlock(input, pos, SEGMENT_HEADER_SIZE, null, 0);
                    pos += SEGMENT_HEADER_SIZE;

                    if (segSize == 0)
                        continue;

                    if (pos + (int)segSize > dataEnd)
                        return null;

                    if (segOffset + segSize > (uint)decompressedSize)
                        return null;

                    Array.Copy(input, pos, output, (int)segOffset, (int)segSize);
                    sha1.TransformBlock(input, pos, (int)segSize, null, 0);
                    pos += (int)segSize;
                }

                sha1.TransformFinalBlock(new byte[0], 0, 0);

                byte[]? computed = sha1.Hash;
                if (computed is null) return null;
                for (int i = 0; i < SHA1_SIZE; i++)
                {
                    if (computed[i] != input[dataEnd + i])
                        return null;
                }
            }

            return output;
        }

        private static uint ReadUInt32BE(byte[] data, int offset) => (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
}
