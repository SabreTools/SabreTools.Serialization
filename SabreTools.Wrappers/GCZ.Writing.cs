using System;
using System.IO;
using SabreTools.Data.Models.GCZ;

namespace SabreTools.Wrappers
{
    public partial class GCZ : IWritable
    {
        /// <summary>
        /// Compress a NintendoDisc wrapper to a GCZ file at the given path.
        /// </summary>
        /// <param name="source">Decompressed disc image to compress.</param>
        /// <param name="outputPath">Destination file path.</param>
        /// <param name="blockSize">
        /// GCZ block size: 32 KiB, 64 KiB, or 128 KiB.
        /// Defaults to <see cref="Constants.DefaultBlockSize"/> (32 KiB).
        /// </param>
        /// <returns>True on success, false on failure.</returns>
        public static bool ConvertFromDisc(NintendoDisc source, string outputPath,
            uint blockSize = Constants.DefaultBlockSize)
        {
            if (source is null)
                return false;
            if (string.IsNullOrEmpty(outputPath))
                return false;
            if (blockSize != Constants.BlockSize32K &&
                blockSize != Constants.BlockSize64K &&
                blockSize != Constants.BlockSize128K)
                return false;

            try
            {
                using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                return WriteGcz(source, fs, blockSize);
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Re-serialise the structural metadata (header + tables) only.
            // Full round-trip compression from an already-GCZ source requires ConvertFromDisc.
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".gcz")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            if (Model?.Header is null)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            var writer = new Serialization.Writers.GCZ { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }

        // -----------------------------------------------------------------------
        // Core GCZ compression pipeline (ISO → GCZ)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Write a GCZ image to <paramref name="destination"/> from a decompressed disc source.
        /// Matches Dolphin's CompressFileToBlob() in CompressedBlob.cpp.
        /// </summary>
        private static bool WriteGcz(NintendoDisc source, Stream destination, uint blockSize)
        {
            long sourceSize = source.DataLength;
            if (sourceSize <= 0)
                return false;

            uint numBlocks = (uint)((sourceSize + blockSize - 1) / blockSize);

            // ---- Step 1: Write placeholder header (will be patched at end) ----
            long headerPos = destination.Position;
            var header = new GczHeader
            {
                MagicCookie        = Constants.MagicCookie,
                SubType            = 0,
                CompressedDataSize = 0,
                DataSize           = (ulong)sourceSize,
                BlockSize          = blockSize,
                NumBlocks          = numBlocks,
            };
            WriteHeader(destination, header);

            // ---- Step 2: Reserve block-pointer table (8 bytes each) ----
            long blockTablePos = destination.Position;
            var blockPointers  = new ulong[numBlocks];
            destination.Position += (long)numBlocks * 8;

            // ---- Step 3: Reserve block-hash table (4 bytes each) ----
            var blockHashes = new uint[numBlocks];
            destination.Position += (long)numBlocks * 4;

            // ---- Step 4: Data section starts here ----
            long dataStartPos   = destination.Position;
            var  readBuf        = new byte[blockSize];
            var  compressBuf    = new byte[(int)blockSize * 2];

            for (uint bi = 0; bi < numBlocks; bi++)
            {
                long blockOffset   = (long)bi * blockSize;
                int  blockDataSize = (int)Math.Min(blockSize, sourceSize - blockOffset);

                byte[]? raw = source.ReadData(blockOffset, blockDataSize);
                if (raw is null || raw.Length != blockDataSize)
                    return false;

                if (blockDataSize < readBuf.Length)
                    Array.Copy(raw, readBuf, blockDataSize);
                else
                    readBuf = raw;

                // Record pointer as offset relative to data section start
                ulong blockPointer = (ulong)(destination.Position - dataStartPos);

                int  compressedSize;
                bool useCompression = TryCompressBlock(readBuf, blockDataSize, compressBuf, out compressedSize);

                if (useCompression)
                {
                    blockPointers[bi] = blockPointer;
                    destination.Write(compressBuf, 0, compressedSize);
                    blockHashes[bi]   = Adler32(compressBuf, compressedSize);
                }
                else
                {
                    blockPointers[bi] = blockPointer | Constants.UncompressedFlag;
                    destination.Write(readBuf, 0, blockDataSize);
                    blockHashes[bi]   = Adler32(readBuf, blockDataSize);
                }
            }

            // ---- Step 5: Patch header with final compressed-data size ----
            long finalEnd = destination.Position;
            header.CompressedDataSize = (ulong)(finalEnd - dataStartPos);

            // ---- Step 6: Write block-pointer table ----
            destination.Position = blockTablePos;
            foreach (ulong ptr in blockPointers)
                WriteUInt64LE(destination, ptr);

            // ---- Step 7: Write block-hash table ----
            foreach (uint h in blockHashes)
                WriteUInt32LE(destination, h);

            // ---- Step 8: Patch header ----
            destination.Position = headerPos;
            WriteHeader(destination, header);

            destination.Position = finalEnd;
            destination.Flush();
            return true;
        }

        // -----------------------------------------------------------------------
        // Compression helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Attempts to zlib-compress <paramref name="inputSize"/> bytes of <paramref name="input"/>
        /// into <paramref name="output"/>.  Returns true and sets <paramref name="compressedSize"/>
        /// when the result is smaller than 97 % of the original (Dolphin's threshold).
        /// GCZ uses the zlib framing: 2-byte header (0x78 0x9C) + deflate stream + 4-byte Adler-32 tail.
        /// </summary>
#if NET20 || NET35 || NET40
        private static bool TryCompressBlock(byte[] input, int inputSize, byte[] output, out int compressedSize)
        {
            // DeflateStream leaveOpen overload and CompressionLevel are not available on net20/net35/net40.
            // Fall back to storing all blocks uncompressed on those targets.
            compressedSize = 0;
            return false;
        }
#else
        private static bool TryCompressBlock(byte[] input, int inputSize, byte[] output, out int compressedSize)
        {
            using (var ms = new MemoryStream(output))
            {
                ms.WriteByte(0x78);
                ms.WriteByte(0x9C);

                using (var ds = new System.IO.Compression.DeflateStream(
                    ms, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true))
                {
                    ds.Write(input, 0, inputSize);
                }

                uint adler = Adler32(input, inputSize);
                ms.WriteByte((byte)(adler >> 24));
                ms.WriteByte((byte)(adler >> 16));
                ms.WriteByte((byte)(adler >> 8));
                ms.WriteByte((byte)adler);

                compressedSize = (int)ms.Position;
            }

            int threshold = inputSize * 97 / 100;
            return compressedSize < threshold;
        }
#endif

        /// <summary>Adler-32 checksum (zlib/deflate standard).</summary>
        private static uint Adler32(byte[] data, int length)
        {
            const uint MOD = 65521;
            uint a = 1, b = 0;
            for (int i = 0; i < length; i++)
            {
                a = (a + data[i]) % MOD;
                b = (b + a) % MOD;
            }

            return (b << 16) | a;
        }

        // -----------------------------------------------------------------------
        // Little-endian binary write helpers
        // -----------------------------------------------------------------------

        private static void WriteHeader(Stream s, GczHeader h)
        {
            WriteUInt32LE(s, h.MagicCookie);
            WriteUInt32LE(s, h.SubType);
            WriteUInt64LE(s, h.CompressedDataSize);
            WriteUInt64LE(s, h.DataSize);
            WriteUInt32LE(s, h.BlockSize);
            WriteUInt32LE(s, h.NumBlocks);
        }

        private static void WriteUInt32LE(Stream s, uint v)
        {
            s.WriteByte((byte)v);
            s.WriteByte((byte)(v >> 8));
            s.WriteByte((byte)(v >> 16));
            s.WriteByte((byte)(v >> 24));
        }

        private static void WriteUInt64LE(Stream s, ulong v)
        {
            WriteUInt32LE(s, (uint)v);
            WriteUInt32LE(s, (uint)(v >> 32));
        }
    }
}
