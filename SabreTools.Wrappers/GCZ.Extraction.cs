using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.GCZ.Constants;

namespace SabreTools.Wrappers
{
    public partial class GCZ : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            var inner = GetInnerWrapper();
            if (inner is null)
                return false;

            return inner.Extract(outputDirectory, includeDebug);
        }

        #region Inner Wrapper

        /// <summary>
        /// Returns a NintendoDisc wrapper backed by a virtual stream that decompresses
        /// GCZ blocks on demand, avoiding loading the entire ISO into memory.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            if (BlockPointers.Length == 0)
                return null;
            if (Header.DataSize == 0)
                return null;

            var stream = new GczVirtualStream(this);
            return NintendoDisc.Create(stream);
        }

        /// <summary>
        /// Decompresses a single GCZ block by index and returns its raw bytes.
        /// Returns null on failure; returns a zero-filled block if the compressed size is zero.
        /// </summary>
        internal byte[]? DecompressBlock(int blockIndex)
        {
            if (blockIndex < 0 || blockIndex >= BlockPointers.Length)
                return null;

            ulong ptr = BlockPointers[blockIndex];
            long blockFileOffset = DataOffset + (long)(ptr & ~UncompressedFlag);

            ulong nextRaw = (blockIndex + 1 < BlockPointers.Length)
                ? BlockPointers[blockIndex + 1] & ~UncompressedFlag
                : CompressedDataSize;

            int compSize = (int)(nextRaw - (ptr & ~UncompressedFlag));
            if (compSize <= 0)
                return new byte[BlockSize];

            byte[] raw = ReadRangeFromSource(blockFileOffset, compSize);
            if (raw.Length != compSize)
                return null;

            // Verify Adler-32 checksum on the compressed (raw) data before decompressing
            if (BlockHashes is not null && blockIndex < BlockHashes.Length)
            {
                uint actual = Adler.Adler32(1, raw, 0, raw.Length);
                if (actual != BlockHashes[blockIndex])
                    return null;
            }

            // If the data is raw, just return
            if ((ptr & UncompressedFlag) != 0)
                return raw;

            // GCZ blocks are zlib-framed: 2-byte header + deflate data + 4-byte Adler-32 trailer.
            // Strip the frame and feed raw deflate data to DeflateStream.
            if (raw.Length < 6)
                return null;

            try
            {
                using var cs = new MemoryStream(raw, 2, raw.Length - 6);
                using var ds = new DeflateStream(cs, CompressionMode.Decompress);
                using var os = new MemoryStream();

                ds.BlockCopy(os, blockSize: 4096);

                return os.ToArray();
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
