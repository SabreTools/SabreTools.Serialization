using System;
using System.IO;
#if !NET20
using System.Security.Cryptography;
#endif
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;
using WiaConstants = SabreTools.Data.Models.WIA.Constants;

namespace SabreTools.Wrappers
{
    public partial class WIA : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "WIA / RVZ Compressed GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Header1"/>
        public WiaHeader1 Header1 => Model.Header1;

        /// <inheritdoc cref="Archive.Header2"/>
        public WiaHeader2 Header2 => Model.Header2;

        /// <inheritdoc cref="Archive.IsRvz"/>
        public bool IsRvz => Model.IsRvz;

        /// <inheritdoc cref="Archive.PartitionEntries"/>
        public PartitionEntry[]? PartitionEntries => Model.PartitionEntries;

        /// <inheritdoc cref="Archive.RawDataEntries"/>
        public RawDataEntry[] RawDataEntries => Model.RawDataEntries;

        /// <summary>
        /// Total uncompressed ISO size in bytes
        /// </summary>
        public ulong IsoFileSize => Model.Header1.IsoFileSize;

        /// <summary>
        /// Disc header parsed from the 128-byte raw disc header stored in Header2.
        /// </summary>
        public DiscHeader? DiscHeader
        {
            get
            {
                if (_discHeader is not null)
                    return _discHeader;
                byte[]? raw = Header2.DiscHeader;
                if (raw is null || raw.Length < 0x20)
                    return null;
                using var ms = new MemoryStream(raw);
                _discHeader = Serialization.Readers.NintendoDisc.ParseDiscHeaderOnly(ms);
                return _discHeader;
            }
        }

        private DiscHeader? _discHeader;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a WIA/RVZ wrapper from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WIA or RVZ image</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WIA wrapper on success, null on failure</returns>
        public static WIA? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a WIA/RVZ wrapper from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WIA or RVZ image</param>
        /// <returns>A WIA wrapper on success, null on failure</returns>
        public static WIA? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                long currentOffset = data.Position;

                var model = new Serialization.Readers.WIA().Deserialize(data);
                if (model is null)
                    return null;

                // The reader parsed the compressed table blobs as raw bytes.
                // Re-read and decompress them here now that we have the compression parameters.
                DecompressTables(model, data, currentOffset);

                return new WIA(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Re-reads the partition entries, raw data entries, and group entries from the source
        /// stream, decompresses them using the algorithm specified in Header2, and replaces the
        /// (garbage) values that the reader left in the model.
        /// </summary>
        private static void DecompressTables(Archive model, Stream data, long baseOffset)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            var comp = model.Header2.CompressionType;

            // None / Purge tables are stored as plain big-endian structs — already parsed correctly.
            if (comp == WiaRvzCompressionType.None || comp == WiaRvzCompressionType.Purge)
                return;

            var compData = model.Header2.CompressorData ?? new byte[7];
            byte compDataSize = model.Header2.CompressorDataSize;

            // --- Raw data entries (stored compressed) ---
            if (model.Header2.NumberOfRawDataEntries > 0 &&
                model.Header2.RawDataEntriesOffset > 0 &&
                model.Header2.RawDataEntriesSize > 0)
            {
                int count = (int)model.Header2.NumberOfRawDataEntries;
                int compressedSize = (int)model.Header2.RawDataEntriesSize;
                int expectedSize = count * WiaConstants.RawDataEntrySize;

                data.Seek(baseOffset + (long)model.Header2.RawDataEntriesOffset, SeekOrigin.Begin);
                byte[] buf = new byte[compressedSize];
                int read = data.Read(buf, 0, compressedSize);
                if (read < compressedSize)
                    return;

                byte[] plain = WiaRvzCompressionHelper.Decompress(
                    comp, buf, 0, compressedSize, compData, compDataSize);
                if (plain is null || plain.Length < expectedSize)
                    return;

                model.RawDataEntries = ParseRawDataEntries(plain, count);
            }

            // --- Group entries (stored compressed) ---
            if (model.Header2.NumberOfGroupEntries > 0 &&
                model.Header2.GroupEntriesOffset > 0 &&
                model.Header2.GroupEntriesSize > 0)
            {
                int count = (int)model.Header2.NumberOfGroupEntries;
                int compressedSize = (int)model.Header2.GroupEntriesSize;
                int entrySize = model.IsRvz ? WiaConstants.RvzGroupEntrySize : WiaConstants.WiaGroupEntrySize;
                int expectedSize = count * entrySize;

                data.Seek(baseOffset + (long)model.Header2.GroupEntriesOffset, SeekOrigin.Begin);
                byte[] buf = new byte[compressedSize];
                int read = data.Read(buf, 0, compressedSize);
                if (read < compressedSize)
                    return;

                byte[] plain = WiaRvzCompressionHelper.Decompress(
                    comp, buf, 0, compressedSize, compData, compDataSize);
                if (plain is null || plain.Length < expectedSize)
                    return;

                if (model.IsRvz)
                    model.RvzGroupEntries = ParseRvzGroupEntries(plain, count);
                else
                    model.GroupEntries = ParseWiaGroupEntries(plain, count);
            }
#endif
        }

        /// <summary>Parses raw data entries from a plain (already decompressed) byte array.</summary>
        private static RawDataEntry[] ParseRawDataEntries(byte[] plain, int count)
        {
            var entries = new RawDataEntry[count];
            for (int i = 0; i < count; i++)
            {
                int o = i * WiaConstants.RawDataEntrySize;
                var e = new RawDataEntry();
                e.DataOffset = ReadUInt64BE(plain, o);
                e.DataSize = ReadUInt64BE(plain, o + 8);
                e.GroupIndex = ReadUInt32BE(plain, o + 16);
                e.NumberOfGroups = ReadUInt32BE(plain, o + 20);
                entries[i] = e;
            }

            return entries;
        }

        /// <summary>Parses WIA group entries from a plain (already decompressed) byte array.</summary>
        private static WiaGroupEntry[] ParseWiaGroupEntries(byte[] plain, int count)
        {
            var entries = new WiaGroupEntry[count];
            for (int i = 0; i < count; i++)
            {
                int o = i * WiaConstants.WiaGroupEntrySize;
                var e = new WiaGroupEntry();
                e.DataOffset = ReadUInt32BE(plain, o) << 2;
                e.DataSize = ReadUInt32BE(plain, o + 4);
                entries[i] = e;
            }

            return entries;
        }

        /// <summary>Parses RVZ group entries from a plain (already decompressed) byte array.</summary>
        private static RvzGroupEntry[] ParseRvzGroupEntries(byte[] plain, int count)
        {
            var entries = new RvzGroupEntry[count];
            for (int i = 0; i < count; i++)
            {
                int o = i * WiaConstants.RvzGroupEntrySize;
                var e = new RvzGroupEntry();
                e.DataOffset = ReadUInt32BE(plain, o) << 2;
                e.DataSize = ReadUInt32BE(plain, o + 4);
                e.RvzPackedSize = ReadUInt32BE(plain, o + 8);
                entries[i] = e;
            }

            return entries;
        }

        private static ulong ReadUInt64BE(byte[] b, int o)
        {
            return ((ulong)b[o] << 56) | ((ulong)b[o + 1] << 48) | ((ulong)b[o + 2] << 40) | ((ulong)b[o + 3] << 32)
                | ((ulong)b[o + 4] << 24) | ((ulong)b[o + 5] << 16) | ((ulong)b[o + 6] << 8) | b[o + 7];
        }

        private static uint ReadUInt32BE(byte[] b, int o)
        {
            return ((uint)b[o] << 24) | ((uint)b[o + 1] << 16) | ((uint)b[o + 2] << 8) | b[o + 3];
        }

        #endregion

        #region Inner Wrapper

        /// <summary>
        /// Decompress the full WIA/RVZ image to a MemoryStream and return a NintendoDisc wrapper.
        /// Returns null if decompression fails or the decompressed data is not a valid disc image.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            ulong isoSize = Model.Header1.IsoFileSize;
            if (isoSize == 0)
                return null;

            // Wii discs are ~4.38 GB — larger than int.MaxValue.
            // On .NET 6+ x64 with gcAllowVeryLargeObjects, this will succeed.
            // On older frameworks it will throw and be caught by the caller's try/catch.
            MemoryStream ms;
            try
            {
                ms = new MemoryStream();
                ms.SetLength((long)isoSize);
            }
            catch
            {
                return null;
            }

            // Write the first 0x80 bytes of the disc header directly from Header2.DiscHeader
            if (Model.Header2.DiscHeader is { Length: > 0 })
            {
                ms.Position = 0;
                int headerLen = Math.Min(Model.Header2.DiscHeader.Length, WiaConstants.DiscHeaderStoredSize);
                ms.Write(Model.Header2.DiscHeader, 0, headerLen);
            }

            uint chunkSize = Model.Header2.ChunkSize;
            var comp = Model.Header2.CompressionType;
            byte[] compressorData = Model.Header2.CompressorData ?? new byte[7];
            byte compressorDataSize = Model.Header2.CompressorDataSize;

            // Write raw data entries (non-partition: disc header continuation, partition table, region data, etc.)
            if (Model.RawDataEntries is { Length: > 0 })
            {
                foreach (var rde in Model.RawDataEntries)
                {
                    if (rde.NumberOfGroups == 0 || rde.DataSize == 0)
                        continue;

                    // Mimic Dolphin's skipped_data alignment
                    long skippedData = (long)rde.DataOffset % 0x8000;
                    long adjustedDataOffset = (long)rde.DataOffset - skippedData;

                    for (uint g = 0; g < rde.NumberOfGroups; g++)
                    {
                        uint groupIdx = rde.GroupIndex + g;
                        byte[]? groupBytes = ReadGroupRaw(groupIdx, comp, compressorData, compressorDataSize, chunkSize);
                        if (groupBytes is null)
                            continue;

                        long destOff = adjustedDataOffset + ((long)g * chunkSize);
                        // Clamp to ISO size and to the raw data entry boundary
                        long dataEnd = (long)(rde.DataOffset + rde.DataSize);
                        long writeOff = destOff + skippedData;
                        if (writeOff >= (long)isoSize || writeOff >= dataEnd)
                            continue;

                        int writeLen = (int)Math.Min(
                            Math.Min(groupBytes.Length - skippedData, dataEnd - writeOff),
                            (long)isoSize - writeOff);
                        if (writeLen <= 0)
                            continue;

                        ms.Position = writeOff;
                        ms.Write(groupBytes, (int)skippedData, writeLen);
                        skippedData = 0; // only applies to first group
                    }
                }
            }

            // Write Wii partition data entries (re-encrypt decrypted hash-stripped groups)
            if (Model.PartitionEntries is { Length: > 0 })
            {
                foreach (var pe in Model.PartitionEntries)
                {
                    WritePartitionData(pe.DataEntry0, pe.PartitionKey, ms, comp,
                        compressorData, compressorDataSize, chunkSize, isoSize);
                    WritePartitionData(pe.DataEntry1, pe.PartitionKey, ms, comp,
                        compressorData, compressorDataSize, chunkSize, isoSize);
                }
            }

            ms.Position = 0;
            return NintendoDisc.Create(ms);
        }

        /// <summary>
        /// Reads and decompresses one raw (non-partition) group.
        /// Returns <c>chunkSize</c> bytes of raw ISO data, or null on failure.
        /// </summary>
        private byte[]? ReadGroupRaw(uint groupIdx, WiaRvzCompressionType comp,
            byte[] compressorData, byte compressorDataSize, uint chunkSize)
        {
            if (Model.IsRvz)
            {
                if (Model.RvzGroupEntries is null || groupIdx >= Model.RvzGroupEntries.Length)
                    return null;
                var ge = Model.RvzGroupEntries[groupIdx];
                bool isRvzCompressed = (ge.DataSize & 0x80000000u) != 0;
                uint dataSize = ge.DataSize & 0x7FFFFFFFu;
                if (dataSize == 0)
                    return new byte[chunkSize];
                byte[] fileData = ReadRangeFromSource(ge.DataOffset, (int)dataSize);
                return DecompressGroupBytes(fileData, 0, (int)dataSize, comp,
                    compressorData, compressorDataSize, (int)chunkSize, Model.IsRvz, isRvzCompressed,
                    ge.RvzPackedSize, groupIdx * chunkSize, false, chunkSize);
            }
            else
            {
                if (Model.GroupEntries is null || groupIdx >= Model.GroupEntries.Length)
                    return null;
                var ge = Model.GroupEntries[groupIdx];
                if (ge.DataSize == 0)
                    return new byte[chunkSize];
                byte[] fileData = ReadRangeFromSource(ge.DataOffset, (int)ge.DataSize);
                return DecompressGroupBytes(fileData, 0, (int)ge.DataSize, comp,
                    compressorData, compressorDataSize, (int)chunkSize, false, false,
                    0, 0L, false, chunkSize);
            }
        }

        /// <summary>
        /// Reads, decompresses, and re-encrypts one Wii partition group, writing the resulting
        /// ISO-layout encrypted blocks into <paramref name="ms"/> at the correct offset.
        /// </summary>
        private void WritePartitionData(PartitionDataEntry de, byte[] partitionKey,
            MemoryStream ms, WiaRvzCompressionType comp, byte[] compressorData,
            byte compressorDataSize, uint chunkSize, ulong isoSize)
        {
            if (de.NumberOfSectors == 0 || de.NumberOfGroups == 0)
                return;

            // WIA stores Wii partition data as decrypted hash-stripped blocks.
            // Each group covers (chunkSize / 0x8000) encrypted sectors.
            const int WiiBlockSize     = 0x8000; // full ISO block (hash + data)
            const int WiiBlockDataSize = 0x7C00; // data portion only

            int blocksPerGroup = (int)(chunkSize / WiiBlockSize);
            long isoDataStart = de.FirstSector * WiiBlockSize;

            for (uint g = 0; g < de.NumberOfGroups; g++)
            {
                uint groupIdx = de.GroupIndex + g;
                // dataOffsetForLfg is the partition-local decrypted offset of this group.
                // Mirrors DolphinIsoLib GetDecryptedPartitionOffsetForGroup:
                //   localGroupIdx * (chunkSize / WiiBlockSize) * WiiBlockDataSize
                long dataOffsetForLfg = g * blocksPerGroup * WiiBlockDataSize;
                byte[]? decryptedGroup = ReadDecryptedGroupData(groupIdx, comp,
                    compressorData, compressorDataSize, blocksPerGroup, WiiBlockDataSize,
                    dataOffsetForLfg);
                if (decryptedGroup is null)
                    continue;

                // Re-encrypt: for each block, generate H0/H1/H2, encrypt hash block with
                // zero IV, extract IV from offset 0x3D0, encrypt data block with that IV.
                byte[] encryptedGroup = EncryptWiiGroup(decryptedGroup, partitionKey, blocksPerGroup);

                long groupIsoOffset = isoDataStart + (g * (blocksPerGroup * WiiBlockSize));
                long isoEnd = isoDataStart + (de.NumberOfSectors * WiiBlockSize);

                int writeLen = (int)Math.Min(
                    Math.Min(encryptedGroup.Length, isoEnd - groupIsoOffset),
                    (long)isoSize - groupIsoOffset);
                if (writeLen <= 0)
                    continue;

                ms.Position = groupIsoOffset;
                ms.Write(encryptedGroup, 0, writeLen);
            }
        }

        /// <summary>
        /// Reads and decompresses a Wii partition group, returning the hash-stripped decrypted data.
        /// </summary>
        private byte[]? ReadDecryptedGroupData(uint groupIdx, WiaRvzCompressionType comp,
            byte[] compressorData, byte compressorDataSize, int blocksPerGroup, int blockDataSize,
            long dataOffsetForLfg)
        {
            int decryptedGroupSize = blocksPerGroup * blockDataSize;

            if (Model.IsRvz)
            {
                if (Model.RvzGroupEntries is null || groupIdx >= Model.RvzGroupEntries.Length)
                    return null;
                var ge = Model.RvzGroupEntries[groupIdx];
                bool isRvzCompressed = (ge.DataSize & 0x80000000u) != 0;
                uint dataSize = ge.DataSize & 0x7FFFFFFFu;
                if (dataSize == 0)
                    return new byte[decryptedGroupSize];
                byte[] fileData = ReadRangeFromSource(ge.DataOffset, (int)dataSize);
                return DecompressGroupBytes(fileData, 0, (int)dataSize, comp,
                    compressorData, compressorDataSize, decryptedGroupSize, Model.IsRvz, isRvzCompressed,
                    ge.RvzPackedSize, dataOffsetForLfg, true,
                    Model.Header2.ChunkSize);
            }
            else
            {
                if (Model.GroupEntries is null || groupIdx >= Model.GroupEntries.Length)
                    return null;
                var ge = Model.GroupEntries[groupIdx];
                if (ge.DataSize == 0)
                    return new byte[decryptedGroupSize];
                byte[] fileData = ReadRangeFromSource(ge.DataOffset, (int)ge.DataSize);
                return DecompressGroupBytes(fileData, 0, (int)ge.DataSize, comp,
                    compressorData, compressorDataSize, decryptedGroupSize, false, false,
                    0, 0L, true,
                    Model.Header2.ChunkSize);
            }
        }

        /// <summary>
        /// Decompresses raw group bytes according to the WIA compression type and strips any
        /// exception-list header, returning the plain data payload.
        /// </summary>
        private static byte[]? DecompressGroupBytes(byte[] fileData, int offset, int length,
            WiaRvzCompressionType comp, byte[] compressorData, byte compressorDataSize,
            int expectedSize, bool isRvz, bool isRvzCompressed,
            uint rvzPackedSize, long dataOffsetForLfg, bool isWiiPartition,
            uint chunkSize = 2 * 1024 * 1024)
        {
            if (fileData is null || fileData.Length < length)
                return null;

            // Mirrors DolphinIsoLib WiaRvzReader::ReadGroupCore logic:
            // Decompress first (Bzip2/LZMA/LZMA2/Zstd), then RVZ-unpack junk regions if present.
            bool shouldDecompress = comp > WiaRvzCompressionType.Purge && (!isRvz || isRvzCompressed);

            if (comp == WiaRvzCompressionType.None)
            {
                // NONE: exception lists precede data with 4-byte alignment for Wii partitions
                int dataStart = isWiiPartition ? SkipExceptionLists(fileData, offset, length, chunkSize) : offset;
                int mainLen = length - (dataStart - offset);
                byte[] noneData = new byte[expectedSize];
                Array.Copy(fileData, dataStart, noneData, 0, Math.Min(mainLen, expectedSize));
                return noneData;
            }
            else if (comp == WiaRvzCompressionType.Purge)
            {
                // Exception list precedes the Purge payload; capture it for SHA-1, then decompress.
                int purgeStart = isWiiPartition ? SkipExceptionLists(fileData, offset, length, chunkSize) : offset;
                int exceptionLen = purgeStart - offset;
                byte[]? exceptionBytes = exceptionLen > 0
                    ? new byte[exceptionLen] : null;
                if (exceptionBytes != null)
                    Array.Copy(fileData, offset, exceptionBytes, 0, exceptionLen);
                int purgeLen = length - exceptionLen;
                return PurgeDecompressor.Decompress(fileData, purgeStart, purgeLen, expectedSize, exceptionBytes);
            }
            else
            {
                // Bzip2 / LZMA / LZMA2 / Zstd — delegate to WiaRvzCompressionHelper
                byte[]? workingData;
                if (shouldDecompress)
                {
                    try
                    {
                        workingData = WiaRvzCompressionHelper.Decompress(
                            comp, fileData, offset, length, compressorData, compressorDataSize);
                    }
                    catch
                    {
                        return null;
                    }

                    if (workingData is null)
                        return null;
                }
                else
                {
                    workingData = fileData;
                }

                // RVZ-pack step: junk regions are stored as LFG seeds rather than raw bytes.
                if (isRvz && rvzPackedSize > 0)
                {
                    var rvzDecomp = new RvzPackDecompressor(workingData, rvzPackedSize, dataOffsetForLfg);
                    byte[] unpacked = new byte[expectedSize];
                    int bytesRead = rvzDecomp.Decompress(unpacked, 0, expectedSize);
                    if (bytesRead < expectedSize)
                        Array.Resize(ref unpacked, bytesRead);
                    return unpacked;
                }

                // Skip exception lists that are compressed together with the data
                int dataStart = isWiiPartition
                    ? SkipExceptionListsNoAlign(workingData, 0, workingData.Length, chunkSize)
                    : 0;
                int mainLen = workingData.Length - dataStart;
                byte[] data = new byte[expectedSize];
                Array.Copy(workingData, dataStart, data, 0, Math.Min(mainLen, expectedSize));
                return data;
            }
        }

        /// <summary>
        /// Skips the packed exception-list header at the start of group data (NONE/Purge path).
        /// Exception lists are 4-byte-aligned after the last list.
        /// Returns the offset of the first data byte.
        /// </summary>
        private static int SkipExceptionLists(byte[] data, int offset, int length, uint chunkSize = 2 * 1024 * 1024)
        {
            // Number of exception lists = max(1, chunkSize / WiiGroupSize).
            // For WIA chunkSize==2MiB this is always 1.
            // For RVZ sub-2MiB chunks this is also 1 (chunkSize <= groupSize).
            const uint WiiGroupSize = 2 * 1024 * 1024; // 0x200000
            int numLists = Math.Max(1, (int)(chunkSize / WiiGroupSize));

            int pos = offset;
            for (int i = 0; i < numLists && pos + 2 <= offset + length; i++)
            {
                ushort count = (ushort)((data[pos] << 8) | data[pos + 1]);
                pos += 2;
                // Each exception entry is 2 + 20 = 22 bytes
                pos += count * 22;
                // 4-byte alignment after last list
                if (i == numLists - 1)
                    pos = (pos + 3) & ~3;
            }

            return pos;
        }

        /// <summary>
        /// Skips exception lists in compressed group data (Bzip2/LZMA/etc.) where
        /// lists are NOT 4-byte aligned.
        /// </summary>
        private static int SkipExceptionListsNoAlign(byte[] data, int offset, int length, uint chunkSize = 2 * 1024 * 1024)
        {
            const uint WiiGroupSize = 2 * 1024 * 1024;
            int numLists = Math.Max(1, (int)(chunkSize / WiiGroupSize));

            int pos = offset;
            for (int i = 0; i < numLists && pos + 2 <= offset + length; i++)
            {
                ushort count = (ushort)((data[pos] << 8) | data[pos + 1]);
                pos += 2;
                pos += count * 22;
            }

            return pos;
        }

        /// <summary>
        /// Re-encrypts one decrypted hash-stripped Wii group back into standard ISO-layout
        /// encrypted 0x8000-byte blocks. Mirrors Dolphin's VolumeWii::EncryptGroup.
        /// </summary>
        private static byte[] EncryptWiiGroup(byte[] decryptedData, byte[] key, int blocksPerGroup)
        {
#if NET20
            // AES not available on net20; return a zero buffer so the wrapper can still be created
            return new byte[blocksPerGroup * 0x8000];
#else
            const int WiiBlockSize     = 0x8000;
            const int WiiBlockDataSize = 0x7C00;
            const int WiiBlockHashSize = 0x0400;
            const int H0Count = 31;
            const int H1Count = 8;
            const int H2Count = 8;
            const int HashLen  = 20;

            // --- Build H0 / H1 / H2 hash arrays ---
            // H0[block][h0] = SHA1 of 0x400-byte chunk h0 within data block 'block'
            byte[][][] h0 = new byte[blocksPerGroup][][];
            for (int b = 0; b < blocksPerGroup; b++)
            {
                h0[b] = new byte[H0Count][];
                int blockBase = b * WiiBlockDataSize;
                for (int h = 0; h < H0Count; h++)
                {
                    int src = blockBase + (h * 0x400);
                    int len = Math.Min(0x400, decryptedData.Length - src);
                    h0[b][h] = ComputeSha1(decryptedData, src < decryptedData.Length ? src : 0, Math.Max(0, len));
                }
            }

            // H1[h1Group][slot] = SHA1 of block (h1Group*8+slot)'s 31 H0 hashes
            byte[][][] h1 = new byte[H1Count][][];
            for (int g = 0; g < H1Count; g++)
            {
                h1[g] = new byte[H1Count][];
                for (int s = 0; s < H1Count; s++)
                {
                    int blockIdx = (g * H1Count) + s;
                    if (blockIdx >= blocksPerGroup)
                    {
                        h1[g][s] = new byte[HashLen];
                        continue;
                    }

                    byte[] h0Concat = new byte[H0Count * HashLen];
                    for (int i = 0; i < H0Count; i++)
                        Array.Copy(h0[blockIdx][i], 0, h0Concat, i * HashLen, HashLen);
                    h1[g][s] = ComputeSha1(h0Concat, 0, h0Concat.Length);
                }
            }

            // H2[h2Idx] = SHA1 of H1 group h2Idx's 8 hashes (same for every block)
            byte[][] h2 = new byte[H2Count][];
            for (int i = 0; i < H2Count; i++)
            {
                int grp = Math.Min(i, h1.Length - 1);
                byte[] h1Concat = new byte[H1Count * HashLen];
                for (int s = 0; s < H1Count; s++)
                    Array.Copy(h1[grp][s], 0, h1Concat, s * HashLen, HashLen);
                h2[i] = ComputeSha1(h1Concat, 0, h1Concat.Length);
            }

            byte[] result = new byte[blocksPerGroup * WiiBlockSize];

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            for (int b = 0; b < blocksPerGroup; b++)
            {
                // Serialize hash block
                byte[] hashBlock = new byte[WiiBlockHashSize];
                int off = 0;

                // H0 (31 * 20 = 0x26C)
                for (int i = 0; i < H0Count; i++) { Array.Copy(h0[b][i], 0, hashBlock, off, HashLen); off += HashLen; }

                off += 0x14; // padding0

                // H1 for this block's group (8 * 20 = 0xA0)
                int h1Grp = b / H1Count;
                if (h1Grp < h1.Length)
                {
                    for (int i = 0; i < H1Count; i++) { Array.Copy(h1[h1Grp][i], 0, hashBlock, off, HashLen); off += HashLen; }
                }
                else
                {
                    off += H1Count * HashLen;
                }

                off += 0x20; // padding1

                // H2 (8 * 20 = 0xA0)
                for (int i = 0; i < H2Count; i++) { Array.Copy(h2[i], 0, hashBlock, off, HashLen); off += HashLen; }
                // Note: off is now 0x3D4; IV will sit at 0x3D0 after encryption

                // Encrypt hash block with IV = zero
                aes.IV = new byte[16];
                byte[] encHashBlock;
                using (var enc = aes.CreateEncryptor())
                    encHashBlock = enc.TransformFinalBlock(hashBlock, 0, WiiBlockHashSize);

                // Extract IV for data block from offset 0x3D0 of the encrypted hash block
                byte[] iv = new byte[16];
                Array.Copy(encHashBlock, 0x3D0, iv, 0, 16);

                // Encrypt data block
                int dataSrc = b * WiiBlockDataSize;
                int dataLen = Math.Min(WiiBlockDataSize, decryptedData.Length - dataSrc);
                byte[] dataBlock = new byte[WiiBlockDataSize];
                if (dataLen > 0)
                    Array.Copy(decryptedData, dataSrc, dataBlock, 0, dataLen);

                aes.IV = iv;
                byte[] encDataBlock;
                using (var enc = aes.CreateEncryptor())
                    encDataBlock = enc.TransformFinalBlock(dataBlock, 0, WiiBlockDataSize);

                int dest = b * WiiBlockSize;
                Array.Copy(encHashBlock, 0, result, dest, WiiBlockHashSize);
                Array.Copy(encDataBlock, 0, result, dest + WiiBlockHashSize, WiiBlockDataSize);
            }

            return result;
#endif
        }

#if !NET20
        private static byte[] ComputeSha1(byte[] data, int offset, int count)
        {
            if (count == 0)
                return new byte[20];

            using var sha1 = SHA1.Create();
            return sha1.ComputeHash(data, offset, count);
        }
#endif

        #endregion
    }
}
