using System;
using System.IO;
#if !NET20
using System.Security.Cryptography;
#endif

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Compresses data using the WIA PURGE format.
    ///
    /// PURGE layout produced:
    ///   [ { u32 offset BE, u32 size BE, data[size] } ] ...  (zero or more segments)
    ///   [ SHA-1 (20 bytes) ]
    ///
    /// Only non-zero byte runs are emitted as segments; consecutive non-zero regions
    /// separated by a gap of 8 or fewer zero bytes are merged into a single segment.
    /// The SHA-1 covers: <paramref name="precedingBytes"/> (e.g. exception-list prefix) +
    /// all segment headers and data bytes.
    ///
    /// This is the exact inverse of <see cref="PurgeDecompressor"/>.
    /// </summary>
    internal static class PurgeCompressor
    {
        /// <summary>
        /// Compress <paramref name="data"/>[<paramref name="offset"/> ..
        /// <paramref name="offset"/>+<paramref name="count"/>) into PURGE format.
        /// </summary>
        /// <param name="data">Source buffer.</param>
        /// <param name="offset">Start of data within <paramref name="data"/>.</param>
        /// <param name="count">Number of bytes to compress.</param>
        /// <param name="precedingBytes">
        /// Optional bytes that precede this payload in the WIA group
        /// (e.g. the serialised exception list).  Included in the SHA-1 but not emitted.
        /// Pass null or empty if there are none.
        /// </param>
        /// <returns>PURGE-compressed byte array (segments + 20-byte SHA-1).</returns>
        public static byte[] Compress(byte[] data, int offset, int count, byte[]? precedingBytes = null)
        {
            const int MaxGap = 8; // zero-byte runs of this length or fewer are bridged

            var output = new MemoryStream((count / 2) + 32);

            int end = offset + count;
            int pos = offset;

            while (pos < end)
            {
                // Skip leading zeros
                while (pos < end && data[pos] == 0)
                    pos++;

                if (pos >= end)
                    break;

                // pos is now the start of a non-zero run (segment start)
                int segStart = pos;
                int segEnd   = pos;

                // Extend the segment, bridging zero-gaps of <= MaxGap bytes
                while (segEnd < end)
                {
                    // advance through non-zero bytes
                    while (segEnd < end && data[segEnd] != 0)
                        segEnd++;

                    // peek ahead: count zero bytes
                    int zeroRun = 0;
                    while (segEnd + zeroRun < end && data[segEnd + zeroRun] == 0)
                        zeroRun++;

                    // If the gap is small enough (and there is more non-zero data after it),
                    // bridge the gap by including it in the segment.
                    if (zeroRun > 0 && zeroRun <= MaxGap && segEnd + zeroRun < end)
                    {
                        segEnd += zeroRun; // include zeros in segment, keep scanning
                    }
                    else
                    {
                        break; // end of segment
                    }
                }

                // Trim trailing zeros from segment end
                while (segEnd > segStart && data[segEnd - 1] == 0)
                    segEnd--;

                if (segEnd <= segStart)
                {
                    pos = segEnd + 1;
                    continue;
                }

                uint segOffset = (uint)(segStart - offset);
                uint segSize   = (uint)(segEnd - segStart);

                // Write {u32 offsetBE, u32 sizeBE, data[segSize]}
                WriteBeU32(output, segOffset);
                WriteBeU32(output, segSize);
                output.Write(data, segStart, (int)segSize);

                pos = segEnd;
            }

            byte[] segments = output.ToArray();

            // SHA-1 over: precedingBytes + segments
            byte[] hash = ComputeSha1(precedingBytes, segments);

            // Final result: segments + hash
            byte[] result = new byte[segments.Length + 20];
            Array.Copy(segments, 0, result, 0, segments.Length);
            Array.Copy(hash, 0, result, segments.Length, 20);
            return result;
        }

        private static byte[] ComputeSha1(byte[]? precedingBytes, byte[] segments)
        {
#if NET20
            return new byte[20];
#else
            using var sha1 = SHA1.Create();

            if (precedingBytes != null && precedingBytes.Length > 0)
                sha1.TransformBlock(precedingBytes, 0, precedingBytes.Length, null, 0);

            sha1.TransformFinalBlock(segments, 0, segments.Length);
            return sha1.Hash ?? new byte[20];
#endif
        }

        private static void WriteBeU32(Stream s, uint value)
        {
            s.WriteByte((byte)(value >> 24));
            s.WriteByte((byte)(value >> 16));
            s.WriteByte((byte)(value >> 8));
            s.WriteByte((byte)value);
        }
    }
}
