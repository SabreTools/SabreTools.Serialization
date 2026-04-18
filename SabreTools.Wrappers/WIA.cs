using System;
using System.IO;
#if !NET20
using System.Security.Cryptography;
#endif
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;
using WiaConstants = SabreTools.Data.Models.WIA.Constants;
using WiaReader = SabreTools.Serialization.Readers.WIA;

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

                var model = new WiaReader().Deserialize(data);
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
                e.DataOffset = (ulong)ReadUInt32BE(plain, o) << 2;
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
                e.DataOffset = (ulong)ReadUInt32BE(plain, o) << 2;
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

        // Cache for on-demand decompression in ReadVirtual.
        private uint _cachedRawGroupIndex = uint.MaxValue;
        private byte[]? _cachedRawGroup;
        private uint _cachedEncGroupIndex = uint.MaxValue;
        private byte[]? _cachedEncGroup;

        /// <summary>
        /// Returns a NintendoDisc wrapper backed by a virtual stream that decompresses
        /// WIA/RVZ groups on demand, avoiding loading the entire ISO into memory.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            if (Model.Header1.IsoFileSize == 0)
                return null;

            var vStream = new WiaVirtualStream(this);
            return NintendoDisc.Create(vStream);
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes of the virtual decompressed ISO at
        /// <paramref name="offset"/> into <paramref name="buffer"/>, decompressing
        /// WIA/RVZ groups on demand. Returns the number of bytes read.
        /// </summary>
        internal int ReadVirtual(long offset, byte[] buffer, int bufferOffset, int count)
        {
            long isoSize = (long)Model.Header1.IsoFileSize;
            if (offset >= isoSize || count <= 0)
                return 0;

            count = (int)Math.Min(count, isoSize - offset);
            int totalRead = 0;

            while (totalRead < count)
            {
                long pos = offset + totalRead;
                int got = ReadVirtualChunk(pos, buffer, bufferOffset + totalRead, count - totalRead);
                if (got <= 0)
                {
                    // Advance past one "zero" byte to avoid infinite loops over gaps.
                    buffer[bufferOffset + totalRead] = 0;
                    totalRead++;
                }
                else
                {
                    totalRead += got;
                }
            }

            return totalRead;
        }

        /// <summary>
        /// Reads bytes for one contiguous segment of the virtual ISO starting at <paramref name="pos"/>.
        /// Returns 0 if the position is not covered by any known data entry (caller fills with zeros).
        /// </summary>
        private int ReadVirtualChunk(long pos, byte[] buffer, int bufferOffset, int count)
        {
            // 1. Disc header (first 0x80 bytes stored verbatim in Header2.DiscHeader)
            if (pos < WiaConstants.DiscHeaderStoredSize && Model.Header2.DiscHeader is { Length: > 0 })
            {
                int available = (int)Math.Min(WiaConstants.DiscHeaderStoredSize - pos, count);
                int srcAvail = Math.Min(available, Model.Header2.DiscHeader.Length - (int)pos);
                if (srcAvail > 0)
                    Array.Copy(Model.Header2.DiscHeader, (int)pos, buffer, bufferOffset, srcAvail);
                if (available > srcAvail)
                    Array.Clear(buffer, bufferOffset + srcAvail, available - srcAvail);
                return available;
            }

            uint chunkSize = Model.Header2.ChunkSize;
            var comp = Model.Header2.CompressionType;
            byte[] compData = Model.Header2.CompressorData ?? new byte[7];
            byte compDataSize = Model.Header2.CompressorDataSize;

            // 2. Raw data entries (non-partition disc data)
            if (Model.RawDataEntries is { Length: > 0 })
            {
                foreach (var rde in Model.RawDataEntries)
                {
                    if (rde.DataSize == 0 || rde.NumberOfGroups == 0)
                        continue;

                    long rdeStart = (long)rde.DataOffset;
                    long rdeEnd = rdeStart + (long)rde.DataSize;
                    if (pos < rdeStart || pos >= rdeEnd)
                        continue;

                    long skippedData = rdeStart % 0x8000;
                    long adjustedBase = rdeStart - skippedData;
                    long adjustedPos = pos - adjustedBase;
                    uint g = (uint)(adjustedPos / chunkSize);
                    int offsetInGroup = (int)(adjustedPos % chunkSize);

                    if (g >= rde.NumberOfGroups)
                        continue;

                    uint groupFileIdx = rde.GroupIndex + g;
                    byte[]? groupBytes = GetCachedRawGroup(groupFileIdx, comp, compData, compDataSize, chunkSize);
                    if (groupBytes is null)
                        return 0;

                    int available = groupBytes.Length - offsetInGroup;
                    if (available <= 0)
                        return 0;

                    int remainingInEntry = (int)Math.Min(rdeEnd - pos, count);
                    // Also clamp to the end of this group
                    long groupIsoEnd = adjustedBase + ((long)(g + 1) * chunkSize);
                    int remainingInGroup = (int)Math.Min(groupIsoEnd - pos, remainingInEntry);
                    int toCopy = Math.Min(available, remainingInGroup);
                    if (toCopy <= 0)
                        return 0;

                    Array.Copy(groupBytes, offsetInGroup, buffer, bufferOffset, toCopy);
                    return toCopy;
                }
            }

            // 3. Partition data entries (Wii encrypted partition data)
            if (Model.PartitionEntries is { Length: > 0 })
            {
                foreach (var pe in Model.PartitionEntries)
                {
                    int r = ReadPartitionChunk(pe.DataEntry0, pe.PartitionKey, pos,
                        buffer, bufferOffset, count, comp, compData, compDataSize, chunkSize);
                    if (r > 0) return r;
                    r = ReadPartitionChunk(pe.DataEntry1, pe.PartitionKey, pos,
                        buffer, bufferOffset, count, comp, compData, compDataSize, chunkSize);
                    if (r > 0) return r;
                }
            }

            return 0;
        }

        /// <summary>
        /// Reads <paramref name="length"/> bytes of decrypted Wii partition data beginning at
        /// <paramref name="partDataOffset"/>, a byte offset in the 0x7C00-block partition-data space.
        /// Spans across both DataEntry0 and DataEntry1 of the partition entry.
        /// Maps directly to the decompressed WIA/RVZ group data — no re-encryption is performed.
        /// </summary>
        internal byte[]? ReadDecryptedPartitionBytes(PartitionEntry pe, long partDataOffset, int length)
        {
            if (length <= 0 || pe is null)
                return null;

            const int WiiBlockSize     = 0x8000;
            const int WiiBlockDataSize = 0x7C00;

            uint chunkSize      = Model.Header2.ChunkSize;
            var  comp           = Model.Header2.CompressionType;
            byte[] compData     = Model.Header2.CompressorData ?? new byte[7];
            byte   compDataSize = Model.Header2.CompressorDataSize;
            int blocksPerGroup  = (int)(chunkSize / WiiBlockSize);

            byte[] result  = new byte[length];
            int  produced  = 0;

            // DataEntry0 covers [0 .. de0.NumberOfSectors * 0x7C00) in partition-data space.
            // DataEntry1 (if present) immediately follows.
            var de0 = pe.DataEntry0;
            var de1 = pe.DataEntry1;
            long de0DataSize = (long)de0.NumberOfSectors * WiiBlockDataSize;
            long de1DataSize = de1 is not null ? (long)de1.NumberOfSectors * WiiBlockDataSize : 0;

            while (produced < length)
            {
                long off = partDataOffset + produced;

                // Determine which DataEntry covers this offset
                PartitionDataEntry de;
                long deRelOff; // offset within this DataEntry's decrypted data space
                if (off < de0DataSize)
                {
                    de = de0;
                    deRelOff = off;
                }
                else if (de1 is not null && de1.NumberOfGroups > 0 && off < de0DataSize + de1DataSize)
                {
                    de = de1;
                    deRelOff = off - de0DataSize;
                }
                else
                {
                    break; // beyond available data
                }

                long blockNum      = deRelOff / WiiBlockDataSize;
                int  offsetInBlock = (int)(deRelOff % WiiBlockDataSize);
                long groupRelative = blockNum / blocksPerGroup;
                int  blockInGroup  = (int)(blockNum % blocksPerGroup);

                if (groupRelative >= de.NumberOfGroups)
                    break;

                uint groupFileIdx     = de.GroupIndex + (uint)groupRelative;
                long dataOffsetForLfg = groupRelative * blocksPerGroup * WiiBlockDataSize;

                byte[]? decrypted = ReadDecryptedGroupData(groupFileIdx, comp, compData, compDataSize,
                    blocksPerGroup, WiiBlockDataSize, dataOffsetForLfg);
                if (decrypted is null)
                    break;

                int offsetInGroup  = (blockInGroup * WiiBlockDataSize) + offsetInBlock;
                int available      = decrypted.Length - offsetInGroup;
                if (available <= 0)
                    break;

                int remainingInGroup = (blocksPerGroup * WiiBlockDataSize) - offsetInGroup;
                int toCopy = Math.Min(length - produced, Math.Min(available, remainingInGroup));
                if (toCopy <= 0)
                    break;

                Array.Copy(decrypted, offsetInGroup, result, produced, toCopy);
                produced += toCopy;
            }

            if (produced <= 0)
                return null;
            if (produced < length)
                Array.Resize(ref result, produced);
            return result;
        }

        private int ReadPartitionChunk(PartitionDataEntry de, byte[] partitionKey, long pos,
            byte[] buffer, int bufferOffset, int count,
            WiaRvzCompressionType comp, byte[] compData, byte compDataSize, uint chunkSize)
        {
            if (de.NumberOfSectors == 0 || de.NumberOfGroups == 0)
                return 0;

            const int WiiBlockSize = 0x8000;
            int blocksPerGroup = (int)(chunkSize / WiiBlockSize);
            long isoDataStart = (long)de.FirstSector * WiiBlockSize;
            long isoDataEnd = isoDataStart + ((long)de.NumberOfSectors * WiiBlockSize);

            if (pos < isoDataStart || pos >= isoDataEnd)
                return 0;

            long offsetInPartition = pos - isoDataStart;
            long blockNum = offsetInPartition / WiiBlockSize;
            int offsetInBlock = (int)(offsetInPartition % WiiBlockSize);

            long groupNum = blockNum / blocksPerGroup;
            int blockInGroup = (int)(blockNum % blocksPerGroup);

            if (groupNum >= de.NumberOfGroups)
                return 0;

            uint groupFileIdx = de.GroupIndex + (uint)groupNum;
            byte[]? encryptedGroup = GetCachedEncGroup(groupFileIdx, de, partitionKey,
                comp, compData, compDataSize, blocksPerGroup, chunkSize);
            if (encryptedGroup is null)
                return 0;

            int offsetInEncGroup = (blockInGroup * WiiBlockSize) + offsetInBlock;
            int available = encryptedGroup.Length - offsetInEncGroup;
            if (available <= 0)
                return 0;

            long remainingInEntry = isoDataEnd - pos;
            // Stay within this group
            long groupIsoEnd = isoDataStart + ((groupNum + 1) * blocksPerGroup * WiiBlockSize);
            long remainingInGroup = groupIsoEnd - pos;
            int toCopy = (int)Math.Min(count, Math.Min(Math.Min(available, remainingInEntry), remainingInGroup));
            if (toCopy <= 0)
                return 0;

            Array.Copy(encryptedGroup, offsetInEncGroup, buffer, bufferOffset, toCopy);
            return toCopy;
        }

        private byte[]? GetCachedRawGroup(uint groupFileIdx,
            WiaRvzCompressionType comp, byte[] compData, byte compDataSize, uint chunkSize)
        {
            if (_cachedRawGroupIndex == groupFileIdx)
                return _cachedRawGroup;

            byte[]? group = ReadGroupRaw(groupFileIdx, comp, compData, compDataSize, chunkSize);
            _cachedRawGroupIndex = groupFileIdx;
            _cachedRawGroup = group;
            return group;
        }

        private byte[]? GetCachedEncGroup(uint groupFileIdx, PartitionDataEntry de, byte[] partitionKey,
            WiaRvzCompressionType comp, byte[] compData, byte compDataSize, int blocksPerGroup, uint chunkSize)
        {
            if (_cachedEncGroupIndex == groupFileIdx)
                return _cachedEncGroup;

            long dataOffsetForLfg = (groupFileIdx - de.GroupIndex) * blocksPerGroup * 0x7C00;
            byte[]? decrypted = ReadDecryptedGroupData(groupFileIdx, comp, compData, compDataSize,
                blocksPerGroup, 0x7C00, dataOffsetForLfg);
            if (decrypted is null)
                return null;

            byte[] encrypted = EncryptWiiGroup(decrypted, partitionKey, blocksPerGroup);
            _cachedEncGroupIndex = groupFileIdx;
            _cachedEncGroup = encrypted;
            return encrypted;
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
                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset, (int)dataSize);
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
                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset, (int)ge.DataSize);
                return DecompressGroupBytes(fileData, 0, (int)ge.DataSize, comp,
                    compressorData, compressorDataSize, (int)chunkSize, false, false,
                    0, 0L, false, chunkSize);
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
                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset, (int)dataSize);
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
                byte[] fileData2 = ReadRangeFromSource((long)ge.DataOffset, (int)ge.DataSize);
                return DecompressGroupBytes(fileData2, 0, (int)ge.DataSize, comp,
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
                    // Exception lists are always present for Wii partition groups.
                    // When compressed (shouldDecompress=true), they are NOT padded to 4-byte alignment.
                    // When uncompressed (shouldDecompress=false), they ARE padded to 4-byte alignment.
                    int rvzDataStart = isWiiPartition
                        ? (shouldDecompress
                            ? SkipExceptionListsNoAlign(workingData, 0, workingData.Length, chunkSize)
                            : SkipExceptionLists(workingData, 0, workingData.Length, chunkSize))
                        : 0;
                    int rvzDataLen = workingData.Length - rvzDataStart;
                    byte[] rvzPayload = new byte[rvzDataLen];
                    Array.Copy(workingData, rvzDataStart, rvzPayload, 0, rvzDataLen);

                    var rvzDecomp = new RvzPackDecompressor(rvzPayload, rvzPackedSize, dataOffsetForLfg);
                    byte[] unpacked = new byte[expectedSize];
                    int bytesRead = rvzDecomp.Decompress(unpacked, 0, expectedSize);
                    if (bytesRead < expectedSize)
                        Array.Resize(ref unpacked, bytesRead);
                    return unpacked;
                }

                // Skip exception lists always present for Wii partition groups.
                // Compressed groups: no 4-byte alignment. Uncompressed groups: 4-byte aligned.
                int dataStart = isWiiPartition
                    ? (shouldDecompress
                        ? SkipExceptionListsNoAlign(workingData, 0, workingData.Length, chunkSize)
                        : SkipExceptionLists(workingData, 0, workingData.Length, chunkSize))
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
