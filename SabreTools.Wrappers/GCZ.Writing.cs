using System;
using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.IO.Compression.Deflate;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class GCZ : IWritable
    {
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

            var writer = new Serialization.Writers.GCZ { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }

        #region Core GCZ Compression Pipeline (ISO -> GCZ)

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
        public static bool ConvertFromDisc(NintendoDisc source, string outputPath, uint blockSize = Constants.DefaultBlockSize)
        {
            if (string.IsNullOrEmpty(outputPath))
                return false;

            if (blockSize != Constants.BlockSize32K
                && blockSize != Constants.BlockSize64K
                && blockSize != Constants.BlockSize128K)
            {
                return false;
            }

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
                MagicCookie = Constants.MagicCookie,
                SubType = 0,
                CompressedDataSize = 0,
                DataSize = (ulong)sourceSize,
                BlockSize = blockSize,
                NumBlocks = numBlocks,
            };
            Serialization.Writers.GCZ.WriteHeader(destination, header);

            // ---- Step 2: Reserve block-pointer table (8 bytes each) ----
            long blockTablePos = destination.Position;
            var blockPointers = new ulong[numBlocks];
            destination.SeekIfPossible(numBlocks * 8, SeekOrigin.Current);

            // ---- Step 3: Reserve block-hash table (4 bytes each) ----
            var blockHashes = new uint[numBlocks];
            destination.SeekIfPossible(numBlocks * 4, SeekOrigin.Current);

            // ---- Step 4: Data section starts here ----
            long dataStartPos = destination.Position;
            var readBuf = new byte[blockSize];
            var compressBuf = new byte[(int)blockSize * 2];

            for (int i = 0; i < numBlocks; i++)
            {
                long blockOffset = i * blockSize;
                int blockDataSize = (int)Math.Min(blockSize, sourceSize - blockOffset);

                byte[] raw = source.ReadData(blockOffset, blockDataSize);
                if (raw.Length != blockDataSize)
                    return false;

                if (blockDataSize < readBuf.Length)
                    Array.Copy(raw, readBuf, blockDataSize);
                else
                    readBuf = raw;

                // Record pointer as offset relative to data section start
                ulong blockPointer = (ulong)(destination.Position - dataStartPos);

                bool useCompression = TryCompressBlock(readBuf, blockDataSize, compressBuf, out int compressedSize);
                if (useCompression)
                {
                    blockPointers[i] = blockPointer;
                    destination.Write(compressBuf, 0, compressedSize);
                    blockHashes[i] = Adler.Adler32(1, compressBuf, 0, compressedSize);
                }
                else
                {
                    blockPointers[i] = blockPointer | Constants.UncompressedFlag;
                    destination.Write(readBuf, 0, blockDataSize);
                    blockHashes[i] = Adler.Adler32(1, readBuf, 0, blockDataSize);
                }
            }

            // ---- Step 5: Patch header with final compressed-data size ----
            long finalEnd = destination.Position;
            header.CompressedDataSize = (ulong)(finalEnd - dataStartPos);

            // ---- Step 6: Write block-pointer table ----
            destination.SeekIfPossible(blockTablePos, SeekOrigin.Begin);
            foreach (ulong ptr in blockPointers)
            {
                destination.WriteLittleEndian(ptr);
            }

            // ---- Step 7: Write block-hash table ----
            foreach (uint h in blockHashes)
            {
                destination.WriteLittleEndian(h);
            }

            // ---- Step 8: Patch header ----
            destination.SeekIfPossible(headerPos, SeekOrigin.Begin);
            Serialization.Writers.GCZ.WriteHeader(destination, header);

            destination.Position = finalEnd;
            destination.Flush();
            return true;
        }

        #endregion

        #region Compression Helpers

        /// <summary>
        /// Attempts to zlib-compress <paramref name="inputSize"/> bytes of <paramref name="input"/>
        /// into <paramref name="output"/>.  Returns true and sets <paramref name="compressedSize"/>
        /// when the result is smaller than 97 % of the original (Dolphin's threshold).
        /// GCZ uses the zlib framing: 2-byte header (0x78 0x9C) + deflate stream + 4-byte Adler-32 tail.
        /// </summary>
        private static bool TryCompressBlock(byte[] input, int inputSize, byte[] output, out int compressedSize)
        {
            using (var ms = new MemoryStream(output))
            {
                ms.WriteByte(0x78);
                ms.WriteByte(0x9C);

                using (var ds = new DeflateStream(ms, CompressionMode.Compress, leaveOpen: true))
                {
                    ds.Write(input, 0, inputSize);
                }

                uint adler = Adler.Adler32(1, input, 0, inputSize);
                ms.WriteByte((byte)(adler >> 24));
                ms.WriteByte((byte)(adler >> 16));
                ms.WriteByte((byte)(adler >> 8));
                ms.WriteByte((byte)adler);

                compressedSize = (int)ms.Position;
            }

            int threshold = inputSize * 97 / 100;
            return compressedSize < threshold;
        }

        #endregion
    }
}
