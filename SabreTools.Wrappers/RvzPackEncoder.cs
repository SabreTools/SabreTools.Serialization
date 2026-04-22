using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Encodes disc data into RVZ-Pack format by replacing predictable LFG
    /// (Lagged Fibonacci Generator) junk regions with compact seed descriptors.
    ///
    /// This is the exact inverse of <see cref="RvzPackDecompressor"/> and mirrors
    /// Dolphin's <c>RVZPack()</c> in <c>WIABlob.cpp</c>.
    ///
    /// Two-phase algorithm:
    /// <list type="number">
    ///   <item>Phase 1 (<see cref="ScanForJunk"/>): walk the buffer, identify LFG
    ///     junk regions, build a map keyed by end-offset.</item>
    ///   <item>Phase 2 (<see cref="EmitChunk"/>): for each chunk, use the map to
    ///     emit alternating real-data and junk-seed segments.</item>
    /// </list>
    /// </summary>
    internal static class RvzPackEncoder
    {
        // 17 u32s × 4 bytes = 68 bytes — minimum size to record a seed
        private const int SeedSizeBytes = LaggedFibonacciGenerator.SEED_SIZE * 4;

        private sealed class JunkRegion
        {
            public long    StartOffset;
            public uint[]? Seed;
        }

        /// <summary>Result of packing a single chunk: compressed payload and its logical size.</summary>
        internal struct ChunkResult
        {
            /// <summary>Packed payload, or <c>null</c> if the chunk contains no junk.</summary>
            public byte[]? Packed;
            /// <summary>Number of bytes the decompressor needs to consume from <see cref="Packed"/>.</summary>
            public uint RvzPackedSize;
        }

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------

        /// <summary>
        /// RVZ-pack a single chunk.
        /// Returns <c>null</c> if the chunk contains no junk (write raw instead).
        /// <paramref name="rvzPackedSize"/> is the number of bytes actually needed
        /// by the decompressor (may be &lt; packed.Length due to alignment).
        /// </summary>
        public static byte[]? Pack(byte[] data, int dataOffset, int size,
            long discDataOffset, out uint rvzPackedSize, GcFst? fst = null)
        {
            rvzPackedSize = 0;
            if (size <= 0)
                return null;

            var junkInfo = ScanForJunk(data, dataOffset, size, discDataOffset, fst);
            if (junkInfo.Count == 0)
                return null;

            ChunkResult r = EmitChunk(data, dataOffset, 0L, size, size, junkInfo);
            rvzPackedSize = r.RvzPackedSize;
            return r.Packed;
        }

        /// <summary>
        /// RVZ-pack a multi-chunk buffer (e.g. a full 2 MiB Wii group).
        /// Performs one Phase-1 scan over the entire buffer, then calls
        /// <see cref="EmitChunk"/> per chunk.
        /// </summary>
        /// <param name="data">Source buffer.</param>
        /// <param name="dataOffset">Start of data within <paramref name="data"/>.</param>
        /// <param name="totalSize">Total number of bytes to process.</param>
        /// <param name="bytesPerChunk">Size of each individual chunk.</param>
        /// <param name="numChunks">Number of chunks.</param>
        /// <param name="discDataOffset">Disc-partition byte offset of the first byte.</param>
        /// <param name="fst">Optional FST for file-boundary optimisation.</param>
        /// <returns>
        /// One <see cref="ChunkResult"/> per chunk;
        /// <c>Packed == null</c> means the chunk has no junk and should be written raw.
        /// </returns>
        public static ChunkResult[] PackGroup(
            byte[] data, int dataOffset, int totalSize,
            int bytesPerChunk, int numChunks,
            long discDataOffset, GcFst? fst = null)
        {
            var junkInfo = ScanForJunk(data, dataOffset, totalSize, discDataOffset, fst);

            var result = new ChunkResult[numChunks];
            for (int c = 0; c < numChunks; c++)
            {
                long chunkStart = (long)c * bytesPerChunk;
                long chunkEnd   = Math.Min(chunkStart + bytesPerChunk, totalSize);
                result[c] = EmitChunk(data, dataOffset, chunkStart, chunkEnd, totalSize, junkInfo);
            }

            return result;
        }

        // ---------------------------------------------------------------
        // Phase 1 — scan buffer for junk regions
        // ---------------------------------------------------------------

        private static SortedDictionary<long, JunkRegion> ScanForJunk(
            byte[] data, int dataOffset, int totalSize, long discDataOffset, GcFst? fst)
        {
            var junkInfo = new SortedDictionary<long, JunkRegion>();

            long position = 0;
            long dataOff  = discDataOffset;

            while (position < totalSize)
            {
                // Step 1: count and advance past leading zeros
                long zeroes = 0;
                while ((position + zeroes) < totalSize &&
                       data[dataOffset + position + zeroes] == 0)
                    zeroes++;

                if (zeroes > SeedSizeBytes)
                {
                    junkInfo[position + zeroes] = new JunkRegion
                    {
                        StartOffset = position,
                        Seed        = new uint[LaggedFibonacciGenerator.SEED_SIZE]
                    };
                }

                position += zeroes;
                dataOff  += zeroes;

                if (position >= totalSize)
                    break;

                // Step 2: compute aligned read window (next 0x8000 boundary)
                long nextBoundary = AlignUp(dataOff + 1, 0x8000);
                long bytesToRead  = Math.Min(nextBoundary - dataOff, totalSize - position);
                int  dataOffMod   = (int)(dataOff % 0x8000);

                // Step 3: ALWAYS call GetSeed unconditionally — no FST pre-check
                var seed          = new uint[LaggedFibonacciGenerator.SEED_SIZE];
                int reconstructed = LaggedFibonacciGenerator.GetSeed(
                    data, (int)(dataOffset + position), (int)bytesToRead, dataOffMod, seed);

                if (reconstructed > 0)
                {
                    junkInfo[position + reconstructed] = new JunkRegion
                    {
                        StartOffset = position,
                        Seed        = seed
                    };
                }

                // Step 4: FST skip AFTER GetSeed
                if (fst != null)
                {
                    long queryOff = dataOff + reconstructed;
                    GcFst.FileEntry? fileInfo = fst.FindFileInfo(queryOff);
                    if (fileInfo.HasValue)
                    {
                        long fileEnd = fileInfo.Value.FileEnd;
                        if (fileEnd < (dataOff + bytesToRead))
                        {
                            position += fileEnd - dataOff;
                            dataOff   = fileEnd;
                            continue;
                        }
                    }
                }

                // Step 5: normal advance by block window
                position += bytesToRead;
                dataOff  += bytesToRead;
            }

            return junkInfo;
        }

        // ---------------------------------------------------------------
        // Phase 2 — emit packed segments for a single chunk
        // ---------------------------------------------------------------

        private static ChunkResult EmitChunk(
            byte[] data, int dataOffset,
            long chunkStart, long chunkEnd, long totalSize,
            SortedDictionary<long, JunkRegion> junkInfo)
        {
            long currentOffset  = chunkStart;
            bool firstIteration = true;

            var  output     = new MemoryStream((int)(chunkEnd - chunkStart));
            uint packedSize = 0;

            while (currentOffset < chunkEnd)
            {
                long    remaining     = chunkEnd - currentOffset;
                long    nextJunkStart = chunkEnd;
                long    nextJunkEnd   = chunkEnd;
                uint[]? junkSeed      = null;

                if (remaining > SeedSizeBytes)
                {
                    foreach (var kvp in junkInfo)
                    {
                        // Dolphin Phase-2 condition:
                        //   key > currentOffset + SEED_SIZE_BYTES  AND
                        //   startOffset + SEED_SIZE_BYTES < chunkEnd
                        if ((kvp.Key > (currentOffset + SeedSizeBytes)) &&
                            ((kvp.Value.StartOffset + SeedSizeBytes) < chunkEnd))
                        {
                            nextJunkStart = Math.Max(currentOffset, kvp.Value.StartOffset);
                            nextJunkEnd   = Math.Min(chunkEnd, kvp.Key);
                            junkSeed      = kvp.Value.Seed;
                            break;
                        }
                    }
                }

                // On the first iteration, bail out if there is no junk in this chunk
                if (firstIteration)
                {
                    if (nextJunkStart == chunkEnd)
                        return new ChunkResult { Packed = null, RvzPackedSize = 0 };

                    firstIteration = false;
                }

                // Emit real-data segment before the junk region
                long nonJunkBytes = nextJunkStart - currentOffset;
                if (nonJunkBytes > 0)
                {
                    output.WriteBigEndian((uint)nonJunkBytes);
                    output.Write(data, (int)(dataOffset + currentOffset), (int)nonJunkBytes);
                    packedSize    += 4 + (uint)nonJunkBytes;
                    currentOffset += nonJunkBytes;
                }

                // Emit junk-seed segment
                long junkBytes = nextJunkEnd - currentOffset;
                if (junkBytes > 0 && junkSeed != null)
                {
                    output.WriteBigEndian(0x80000000u | (uint)junkBytes);
                    byte[] seedBytes = new byte[SeedSizeBytes];
                    Buffer.BlockCopy(junkSeed, 0, seedBytes, 0, SeedSizeBytes);
                    output.Write(seedBytes, 0, SeedSizeBytes);
                    packedSize    += 4 + (uint)SeedSizeBytes;
                    currentOffset += junkBytes;
                }

                if (junkSeed == null)
                    break;
            }

            return new ChunkResult { Packed = output.ToArray(), RvzPackedSize = packedSize };
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        private static long AlignUp(long value, long alignment) => (value + alignment - 1) & ~(alignment - 1);
    }
}
