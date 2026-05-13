using System;
using System.IO;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;
using SabreTools.Hashing;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using SabreTools.IO.Extensions;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.ZStandard;
#endif
using static SabreTools.Data.Models.NintendoDisc.Constants;
using static SabreTools.Data.Models.WIA.Constants;
using WiaReader = SabreTools.Serialization.Readers.WIA;

namespace SabreTools.Wrappers
{
    public partial class WIA : WrapperBase<DiscImage>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "WIA / RVZ Compressed GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="DiscImage.GroupEntries"/>
        public WiaGroupEntry[]? GroupEntries => Model.GroupEntries;

        /// <inheritdoc cref="DiscImage.Header1"/>
        public WiaHeader1 Header1 => Model.Header1;

        /// <inheritdoc cref="DiscImage.Header2"/>
        public WiaHeader2 Header2 => Model.Header2;

        /// <summary>True if this is an RVZ file; false if this is a WIA file.</summary>
        public bool IsRvz => Header1.Magic == RvzMagic;

        /// <inheritdoc cref="DiscImage.PartitionEntries"/>
        public PartitionEntry[]? PartitionEntries => Model.PartitionEntries;

        /// <inheritdoc cref="DiscImage.RawDataEntries"/>
        public RawDataEntry[] RawDataEntries => Model.RawDataEntries;

        /// <inheritdoc cref="DiscImage.RvzGroupEntries"/>
        public RvzGroupEntry[]? RvzGroupEntries => Model.RvzGroupEntries;

        /// <summary>
        /// Total uncompressed ISO size in bytes
        /// </summary>
        public ulong IsoFileSize => Header1.IsoFileSize;

        /// <summary>
        /// Disc header parsed from the 128-byte raw disc header stored in Header2.
        /// </summary>
        public DiscHeader? DiscHeader
        {
            get
            {
                if (field is not null)
                    return field;

                byte[]? raw = Header2.DiscHeader;
                if (raw is null || raw.Length < 0x20)
                    return null;

                using var ms = new MemoryStream(raw);
                field = Serialization.Readers.NintendoDisc.ParseDiscHeader(ms);
                return field;
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WIA(DiscImage model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(DiscImage model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(DiscImage model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WIA(DiscImage model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(DiscImage model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(DiscImage model, Stream data, long offset, long length) : base(model, data, offset, length) { }

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

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                // The reader parsed the compressed table blobs as raw bytes.
                // Re-read and decompress them here now that we have the compression parameters.
                DecompressTables(model, data, currentOffset);
#endif

                return new WIA(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Re-reads the partition entries, raw data entries, and group entries from the source
        /// stream, decompresses them using the algorithm specified in Header2, and replaces the
        /// (garbage) values that the reader left in the model.
        /// </summary>
        private static void DecompressTables(DiscImage model, Stream data, long baseOffset)
        {
            var comp = model.Header2.CompressionType;

            // None / Purge tables are stored as plain big-endian structs — already parsed correctly.
            if (comp == WiaRvzCompressionType.None || comp == WiaRvzCompressionType.Purge)
                return;

            var compData = model.Header2.CompressorData ?? new byte[7];
            byte compDataSize = model.Header2.CompressorDataSize;

            // --- Raw data entries (stored compressed) ---
            if (model.Header2.NumberOfRawDataEntries > 0
                && model.Header2.RawDataEntriesOffset > 0
                && model.Header2.RawDataEntriesSize > 0)
            {
                int count = (int)model.Header2.NumberOfRawDataEntries;
                int compressedSize = (int)model.Header2.RawDataEntriesSize;
                int expectedSize = count * RawDataEntrySize;

                data.Seek(baseOffset + (long)model.Header2.RawDataEntriesOffset, SeekOrigin.Begin);
                byte[] buf = new byte[compressedSize];
                int read = data.Read(buf, 0, compressedSize);
                if (read < compressedSize)
                    return;

                byte[] plain = Decompress(comp, buf, 0, compressedSize, compData, compDataSize);
                if (plain is null || plain.Length < expectedSize)
                    return;

                model.RawDataEntries = ParseRawDataEntries(plain, count);
            }

            // --- Group entries (stored compressed) ---
            if (model.Header2.NumberOfGroupEntries > 0
                && model.Header2.GroupEntriesOffset > 0
                && model.Header2.GroupEntriesSize > 0)
            {
                int count = (int)model.Header2.NumberOfGroupEntries;
                int compressedSize = (int)model.Header2.GroupEntriesSize;
                int entrySize = model.Header1.Magic == RvzMagic ? RvzGroupEntrySize : WiaGroupEntrySize;
                int expectedSize = count * entrySize;

                data.Seek(baseOffset + (long)model.Header2.GroupEntriesOffset, SeekOrigin.Begin);
                byte[] buf = new byte[compressedSize];
                int read = data.Read(buf, 0, compressedSize);
                if (read < compressedSize)
                    return;

                byte[] plain = Decompress(comp, buf, 0, compressedSize, compData, compDataSize);
                if (plain is null || plain.Length < expectedSize)
                    return;

                if (model.Header1.Magic == RvzMagic)
                    model.RvzGroupEntries = ParseRvzGroupEntries(plain, count);
                else
                    model.GroupEntries = ParseWiaGroupEntries(plain, count);
            }
        }

        /// <summary>
        /// Parses raw data entries from a plain (already decompressed) byte array.
        /// </summary>
        private static RawDataEntry[] ParseRawDataEntries(byte[] plain, int count)
        {
            var entries = new RawDataEntry[count];

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                entries[i] = WiaReader.ParseRawDataEntry(plain, ref offset);
            }

            return entries;
        }

        /// <summary>
        /// Parses WIA group entries from a plain (already decompressed) byte array.
        /// </summary>
        private static WiaGroupEntry[] ParseWiaGroupEntries(byte[] plain, int count)
        {
            var entries = new WiaGroupEntry[count];

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                entries[i] = WiaReader.ParseWiaGroupEntry(plain, ref offset);
            }

            return entries;
        }

        /// <summary>
        /// Parses RVZ group entries from a plain (already decompressed) byte array.
        /// </summary>
        private static RvzGroupEntry[] ParseRvzGroupEntries(byte[] plain, int count)
        {
            var entries = new RvzGroupEntry[count];

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                entries[i] = WiaReader.ParseRvzGroupEntry(plain, ref offset);
            }

            return entries;
        }
#endif

        #endregion

        #region Compression

        /// <summary>
        /// Dictionary sizes per compression level 1-9 (index 0 unused).
        /// </summary>
        /// <remarks>Mirrors Dolphin WIACompression.cpp dict_size choices.</remarks>
        private static readonly int[] DictSizes =
        [
            0,         // 0: unused
            1 << 16,   // 1:  64 KiB
            1 << 20,   // 2:   1 MiB
            1 << 22,   // 3:   4 MiB
            1 << 22,   // 4:   4 MiB
            1 << 23,   // 5:   8 MiB
            1 << 23,   // 6:   8 MiB
            1 << 24,   // 7:  16 MiB
            1 << 25,   // 8:  32 MiB
            1 << 26,   // 9:  64 MiB
        ];

        /// <summary>
        ///
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int GetDictSize(int level)
            => DictSizes[Math.Max(1, Math.Min(9, level))];

        /// <summary>
        /// Returns the raw LZMA2 dict-size property byte for a given dictionary size.
        /// </summary>
        private static uint Lzma2DictSize(byte p) => (uint)((2 | (p & 1)) << ((p / 2) + 11));

        /// <summary>
        ///
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static byte EncodeLzma2DictSize(uint d)
        {
            byte e = 0;
            while (e < 40 && d > Lzma2DictSize(e))
            {
                e++;
            }

            return e;
        }

        /// <summary>
        /// Fills the compressor-data bytes for <see cref="WiaHeader2.CompressorData"/>
        /// and <see cref="WiaHeader2.CompressorDataSize"/>.
        /// LZMA: 5 bytes. LZMA2: 1 byte. Others: 0 bytes.
        /// </summary>
        internal static void GetCompressorData(WiaRvzCompressionType type, int level, out byte[] propData, out byte propSize)
        {
            propData = new byte[7];
            int dictSize = GetDictSize(level);

            switch (type)
            {
                case WiaRvzCompressionType.LZMA:
                    propData[0] = 0x5D; // propByte for default pb=2,lp=0,lc=3
                    propData[1] = (byte)dictSize;
                    propData[2] = (byte)(dictSize >> 8);
                    propData[3] = (byte)(dictSize >> 16);
                    propData[4] = (byte)(dictSize >> 24);
                    propSize = 5;
                    break;

                case WiaRvzCompressionType.LZMA2:
                    propData[0] = EncodeLzma2DictSize((uint)dictSize);
                    propSize = 1;
                    break;

                // All cases below default to 0
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                case WiaRvzCompressionType.Bzip2:
                case WiaRvzCompressionType.Zstd:
                default:
                    propSize = 0;
                    break;
            }
        }

        /// <summary>
        /// Compress <paramref name="data"/> using the specified algorithm.
        /// </summary>
        internal static byte[] Compress(WiaRvzCompressionType type,
            byte[] data,
            int offset,
            int length,
            int level,
            byte[] compressorData,
            byte compressorDataSize)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            switch (type)
            {
                case WiaRvzCompressionType.Bzip2:
                    return CompressBzip2(data, offset, length);
                case WiaRvzCompressionType.LZMA:
                    return CompressLzma(data, offset, length, level, isLzma2: false);
                case WiaRvzCompressionType.LZMA2:
                    return CompressLzma(data, offset, length, level, isLzma2: true);
                case WiaRvzCompressionType.Zstd:
                    return CompressZstd(data, offset, length, level);

                // Do not use compression
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                default:
                    throw new ArgumentException($"Cannot compress type {type}", nameof(type));
            }
#else
            throw new PlatformNotSupportedException("WIA/RVZ compression requires .NET 4.6.2 or later.");
#endif
        }

        /// <summary>
        /// Decompress <paramref name="data"/> using the specified algorithm.
        /// </summary>
        internal static byte[] Decompress(WiaRvzCompressionType type,
            byte[] data,
            int offset,
            int length,
            byte[] compressorData,
            byte compressorDataSize)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            switch (type)
            {
                case WiaRvzCompressionType.Bzip2:
                    return DecompressBzip2(data, offset, length);

                case WiaRvzCompressionType.LZMA:
                    byte[] lzmaProps = new byte[compressorDataSize];
                    Array.Copy(compressorData, lzmaProps, compressorDataSize);
                    return DecompressLzma(data, offset, length, lzmaProps, isLzma2: false);

                case WiaRvzCompressionType.LZMA2:
                    byte[] lzma2Props = new byte[compressorDataSize];
                    Array.Copy(compressorData, lzma2Props, compressorDataSize);
                    return DecompressLzma(data, offset, length, lzma2Props, isLzma2: true);

                case WiaRvzCompressionType.Zstd:
                    return DecompressZstd(data, offset, length);

                // Do not use compression
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                default:
                    throw new ArgumentException($"Cannot decompress type {type}", nameof(type));
            }
#else
            throw new PlatformNotSupportedException("WIA/RVZ decompression requires .NET 4.6.2 or later.");
#endif
        }

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Compress data using bzip2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressBzip2(byte[] data, int offset, int length)
        {
            using var outMs = new MemoryStream();
            using (var bz2 = BZip2Stream.Create(outMs, CompressionMode.Compress, false, true))
            {
                bz2.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using bzip2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressBzip2(byte[] data, int offset, int length)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var bz2 = BZip2Stream.Create(inMs, CompressionMode.Decompress, false, false);
            using var outMs = new MemoryStream();

            bz2.BlockCopy(outMs);

            return outMs.ToArray();
        }

        /// <summary>
        /// Compress data using LZMA/LZMA2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="level">LZMA/LZMA2 level</param>
        /// <param name="isLzma2">Indicates if LZMA2 is used</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressLzma(byte[] data, int offset, int length, int level, bool isLzma2)
        {
            int dictSize = GetDictSize(level);
            using var outMs = new MemoryStream();
            using (var lzma = LzmaStream.Create(new LzmaEncoderProperties(true, dictSize), isLzma2, outMs))
            {
                lzma.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using LZMA/LZMA2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="props">LZMA properties</param>
        /// <param name="isLzma2">Indicates if LZMA2 is used</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressLzma(byte[] data, int offset, int length, byte[] props, bool isLzma2)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var lzma = LzmaStream.Create(props, inMs, length, -1, null, isLzma2, false);
            using var outMs = new MemoryStream();

            lzma.BlockCopy(outMs);

            return outMs.ToArray();
        }

        /// <summary>
        /// Compress data using Zstd
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="level">Zstd level</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressZstd(byte[] data, int offset, int length, int level)
        {
            using var outMs = new MemoryStream();
            using (var zstd = new ZStandardStream(outMs, CompressionMode.Compress, level))
            {
                zstd.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using Zstd
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressZstd(byte[] data, int offset, int length)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var zstd = new ZStandardStream(inMs);
            using var outMs = new MemoryStream();

            zstd.BlockCopy(outMs);

            return outMs.ToArray();
        }
#endif

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
            if (Header1.IsoFileSize == 0)
                return null;

            var vStream = new WiaVirtualStream(this);
            var disc = NintendoDisc.Create(vStream);
            if (disc is null)
                return null;

            // For Wii discs: WIA/RVZ stores partition data already decrypted.
            // Wire a pre-decrypted reader so NintendoDisc.Extraction bypasses its
            // AES-CBC decrypt pass and reads directly from our decompressed groups.
            if (PartitionEntries is not null && PartitionEntries.Length > 0)
                disc._preDecryptedReader = PreDecryptedReader;

            return disc;
        }

        /// <summary>
        /// Used by <see cref="NintendoDisc._preDecryptedReader"/>.
        /// Matches <paramref name="absDataOffset"/> (absolute ISO offset of the encrypted data
        /// area) to the corresponding WIA <see cref="PartitionEntry"/> by comparing it with
        /// de.FirstSector * 0x8000, then delegates to
        /// <see cref="ReadDecryptedPartitionBytes"/>.
        /// </summary>
        private byte[]? PreDecryptedReader(long absDataOffset, long partitionDataOffset, int length)
        {
            if (PartitionEntries is null)
                return null;

            foreach (var entry in PartitionEntries)
            {
                // The data area of this partition starts at de.FirstSector * 0x8000
                long deIsoStart = (long)entry.DataEntry0.FirstSector * WiiBlockSize;
                long deIsoEnd = deIsoStart + ((long)entry.DataEntry0.NumberOfSectors * WiiBlockSize);

                if (absDataOffset >= deIsoStart && absDataOffset < deIsoEnd)
                    return ReadDecryptedPartitionBytes(entry, partitionDataOffset, length);

                if (entry.DataEntry1 is { NumberOfSectors: > 0 })
                {
                    long de1Start = (long)entry.DataEntry1.FirstSector * WiiBlockSize;
                    long de1End = de1Start + ((long)entry.DataEntry1.NumberOfSectors * WiiBlockSize);
                    if (absDataOffset >= de1Start && absDataOffset < de1End)
                        return ReadDecryptedPartitionBytes(entry, partitionDataOffset, length);
                }
            }

            return null;
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes of the virtual decompressed ISO at
        /// <paramref name="offset"/> into <paramref name="buffer"/>, decompressing
        /// WIA/RVZ groups on demand. Returns the number of bytes read.
        /// </summary>
        internal int ReadVirtual(long offset, byte[] buffer, int bufferOffset, int count)
        {
            long isoSize = (long)Header1.IsoFileSize;
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
            if (pos < DiscHeaderStoredSize && Header2.DiscHeader.Length > 0)
            {
                int available = (int)Math.Min(DiscHeaderStoredSize - pos, count);
                int srcAvail = Math.Min(available, Header2.DiscHeader.Length - (int)pos);
                if (srcAvail > 0)
                    Array.Copy(Header2.DiscHeader, (int)pos, buffer, bufferOffset, srcAvail);

                if (available > srcAvail)
                    Array.Clear(buffer, bufferOffset + srcAvail, available - srcAvail);

                return available;
            }

            uint chunkSize = Header2.ChunkSize;
            var comp = Header2.CompressionType;
            byte[] compData = Header2.CompressorData;
            byte compDataSize = Header2.CompressorDataSize;

            // 2. Raw data entries (non-partition disc data)
            if (RawDataEntries.Length > 0)
            {
                foreach (var entry in RawDataEntries)
                {
                    if (entry.DataSize == 0 || entry.NumberOfGroups == 0)
                        continue;

                    long rdeStart = (long)entry.DataOffset;
                    long rdeEnd = rdeStart + (long)entry.DataSize;
                    if (pos < rdeStart || pos >= rdeEnd)
                        continue;

                    long skippedData = rdeStart % 0x8000;
                    long adjustedBase = rdeStart - skippedData;
                    long adjustedPos = pos - adjustedBase;
                    uint g = (uint)(adjustedPos / chunkSize);
                    int offsetInGroup = (int)(adjustedPos % chunkSize);

                    if (g >= entry.NumberOfGroups)
                        continue;

                    uint groupFileIdx = entry.GroupIndex + g;
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
            if (PartitionEntries is not null && PartitionEntries.Length > 0)
            {
                foreach (var pe in PartitionEntries)
                {
                    int ret = ReadPartitionChunk(pe.DataEntry0,
                        pe.PartitionKey,
                        pos,
                        buffer,
                        bufferOffset,
                        count,
                        comp,
                        compData,
                        compDataSize,
                        chunkSize);
                    if (ret > 0)
                        return ret;

                    ret = ReadPartitionChunk(pe.DataEntry1,
                        pe.PartitionKey,
                        pos,
                        buffer,
                        bufferOffset,
                        count,
                        comp,
                        compData,
                        compDataSize,
                        chunkSize);
                    if (ret > 0)
                        return ret;
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

            uint chunkSize = Header2.ChunkSize;
            var comp = Header2.CompressionType;
            byte[] compData = Header2.CompressorData ?? new byte[7];
            byte compDataSize = Header2.CompressorDataSize;
            int blocksPerGroup = (int)(chunkSize / WiiBlockSize);

            byte[] result = new byte[length];
            int produced = 0;

            // DataEntry0 covers [0 .. de0.NumberOfSectors * 0x7C00) in partition-data space
            var de0 = pe.DataEntry0;
            long de0DataSize = (long)de0.NumberOfSectors * WiiBlockDataSize;

            // DataEntry1 (if present) immediately follows
            var de1 = pe.DataEntry1;
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

                long blockNum = deRelOff / WiiBlockDataSize;
                int offsetInBlock = (int)(deRelOff % WiiBlockDataSize);
                long groupRelative = blockNum / blocksPerGroup;
                int blockInGroup = (int)(blockNum % blocksPerGroup);

                if (groupRelative >= de.NumberOfGroups)
                    break;

                uint groupFileIdx = de.GroupIndex + (uint)groupRelative;
                long dataOffsetForLfg = groupRelative * blocksPerGroup * WiiBlockDataSize;

                byte[]? decrypted = ReadDecryptedGroupData(groupFileIdx,
                    comp,
                    compData,
                    compDataSize,
                    blocksPerGroup,
                    WiiBlockDataSize,
                    dataOffsetForLfg);
                if (decrypted is null)
                    break;

                int offsetInGroup = (blockInGroup * WiiBlockDataSize) + offsetInBlock;
                int available = decrypted.Length - offsetInGroup;
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="de"></param>
        /// <param name="partitionKey"></param>
        /// <param name="pos"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="count"></param>
        /// <param name="comp"></param>
        /// <param name="compData"></param>
        /// <param name="compDataSize"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private int ReadPartitionChunk(PartitionDataEntry de,
            byte[] partitionKey,
            long pos,
            byte[] buffer,
            int bufferOffset,
            int count,
            WiaRvzCompressionType comp,
            byte[] compData,
            byte compDataSize,
            uint chunkSize)
        {
            if (de.NumberOfSectors == 0 || de.NumberOfGroups == 0)
                return 0;

            if (chunkSize == 0)
                return 0;

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
            byte[]? encryptedGroup = GetCachedEncGroup(groupFileIdx, de, partitionKey, comp, compData, compDataSize, blocksPerGroup);
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="groupFileIdx"></param>
        /// <param name="comp"></param>
        /// <param name="compData"></param>
        /// <param name="compDataSize"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private byte[]? GetCachedRawGroup(uint groupFileIdx,
            WiaRvzCompressionType comp,
            byte[] compData,
            byte compDataSize,
            uint chunkSize)
        {
            if (_cachedRawGroupIndex == groupFileIdx)
                return _cachedRawGroup;

            byte[]? group = ReadGroupRaw(groupFileIdx, comp, compData, compDataSize, chunkSize);
            _cachedRawGroupIndex = groupFileIdx;
            _cachedRawGroup = group;
            return group;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="groupFileIdx"></param>
        /// <param name="de"></param>
        /// <param name="partitionKey"></param>
        /// <param name="comp"></param>
        /// <param name="compData"></param>
        /// <param name="compDataSize"></param>
        /// <param name="blocksPerGroup"></param>
        /// <returns></returns>
        private byte[]? GetCachedEncGroup(uint groupFileIdx,
            PartitionDataEntry de,
            byte[] partitionKey,
            WiaRvzCompressionType comp,
            byte[] compData,
            byte compDataSize,
            int blocksPerGroup)
        {
            if (_cachedEncGroupIndex == groupFileIdx)
                return _cachedEncGroup;

            long dataOffsetForLfg = (groupFileIdx - de.GroupIndex) * blocksPerGroup * 0x7C00;
            byte[]? decrypted = ReadDecryptedGroupData(groupFileIdx, comp, compData, compDataSize, blocksPerGroup, 0x7C00, dataOffsetForLfg);
            if (decrypted is null)
                return null;

            byte[] encrypted = EncryptWiiGroup(decrypted, partitionKey, blocksPerGroup);
            _cachedEncGroupIndex = groupFileIdx;
            _cachedEncGroup = encrypted;
            return encrypted;
        }

        /// <summary>
        /// Reads and decompresses one raw (non-partition) group.
        /// Returns chunkSize bytes of raw ISO data, or null on failure.
        /// </summary>
        private byte[]? ReadGroupRaw(uint groupIdx,
            WiaRvzCompressionType comp,
            byte[] compressorData,
            byte compressorDataSize,
            uint chunkSize)
        {
            if (IsRvz)
            {
                if (RvzGroupEntries is null || groupIdx >= RvzGroupEntries.Length)
                    return null;

                var ge = RvzGroupEntries[groupIdx];
                bool isRvzCompressed = (ge.DataSize & 0x80000000u) != 0;
                uint dataSize = ge.DataSize & 0x7FFFFFFFu;
                if (dataSize == 0)
                    return new byte[chunkSize];

                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset << 2, (int)dataSize);
                return DecompressGroupBytes(fileData,
                    0,
                    (int)dataSize,
                    comp,
                    compressorData,
                    compressorDataSize,
                    (int)chunkSize,
                    IsRvz,
                    isRvzCompressed,
                    ge.RvzPackedSize,
                    groupIdx * chunkSize,
                    isWiiPartition: false,
                    chunkSize);
            }
            else
            {
                if (GroupEntries is null || groupIdx >= GroupEntries.Length)
                    return null;

                var ge = GroupEntries[groupIdx];
                if (ge.DataSize == 0)
                    return new byte[chunkSize];

                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset << 2, (int)ge.DataSize);
                return DecompressGroupBytes(fileData,
                    0,
                    (int)ge.DataSize,
                    comp,
                    compressorData,
                    compressorDataSize,
                    (int)chunkSize,
                    IsRvz,
                    false,
                    0,
                    0,
                    false,
                    chunkSize);
            }
        }

        /// <summary>
        /// Reads and decompresses a Wii partition group, returning the hash-stripped decrypted data.
        /// </summary>
        private byte[]? ReadDecryptedGroupData(uint groupIdx,
            WiaRvzCompressionType comp,
            byte[] compressorData,
            byte compressorDataSize,
            int blocksPerGroup,
            int blockDataSize,
            long dataOffsetForLfg)
        {
            int decryptedGroupSize = blocksPerGroup * blockDataSize;

            if (IsRvz)
            {
                if (RvzGroupEntries is null || groupIdx >= RvzGroupEntries.Length)
                    return null;

                var ge = RvzGroupEntries[groupIdx];
                bool isRvzCompressed = (ge.DataSize & 0x80000000u) != 0;
                uint dataSize = ge.DataSize & 0x7FFFFFFFu;
                if (dataSize == 0)
                    return new byte[decryptedGroupSize];

                byte[] fileData = ReadRangeFromSource((long)ge.DataOffset << 2, (int)dataSize);
                return DecompressGroupBytes(fileData,
                    0,
                    (int)dataSize,
                    comp,
                    compressorData,
                    compressorDataSize,
                    decryptedGroupSize,
                    IsRvz,
                    isRvzCompressed,
                    ge.RvzPackedSize,
                    dataOffsetForLfg,
                    true,
                    Header2.ChunkSize);
            }
            else
            {
                if (GroupEntries is null || groupIdx >= GroupEntries.Length)
                    return null;

                var ge = GroupEntries[groupIdx];
                if (ge.DataSize == 0)
                    return new byte[decryptedGroupSize];

                byte[] fileData2 = ReadRangeFromSource((long)ge.DataOffset << 2, (int)ge.DataSize);
                return DecompressGroupBytes(fileData2,
                    0,
                    (int)ge.DataSize,
                    comp,
                    compressorData,
                    compressorDataSize,
                    decryptedGroupSize,
                    IsRvz,
                    false,
                    0,
                    0L,
                    true,
                    Header2.ChunkSize);
            }
        }

        /// <summary>
        /// Decompresses raw group bytes according to the WIA compression type and strips any
        /// exception-list header, returning the plain data payload.
        /// </summary>
        private static byte[]? DecompressGroupBytes(byte[] fileData,
            int offset,
            int length,
            WiaRvzCompressionType comp,
            byte[] compressorData,
            byte compressorDataSize,
            int expectedSize,
            bool isRvz,
            bool isRvzCompressed,
            uint rvzPackedSize,
            long dataOffsetForLfg,
            bool isWiiPartition,
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
                byte[]? exceptionBytes = exceptionLen > 0 ? new byte[exceptionLen] : null;
                if (exceptionBytes is not null)
                    Array.Copy(fileData, offset, exceptionBytes, 0, exceptionLen);

                int purgeLen = length - exceptionLen;
                return PurgeDecompressor.Decompress(fileData, purgeStart, purgeLen, expectedSize, exceptionBytes);
            }
            else
            {
                // Bzip2 / LZMA / LZMA2 / Zstd — delegate to compression helpers
                byte[]? workingData;
                if (shouldDecompress)
                {
                    try
                    {
                        workingData = Decompress(comp, fileData, offset, length, compressorData, compressorDataSize);
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
        internal static byte[] EncryptWiiGroup(byte[] decryptedData, byte[] key, int blocksPerGroup)
        {
            const int H0Count = 31;
            const int H1Count = 8;
            const int H2Count = 8;
            const int HashLen = 20;

            // --- Build H0 / H1 / H2 hash arrays ---
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
                    {
                        Array.Copy(h0[blockIdx][i], 0, h0Concat, i * HashLen, HashLen);
                    }

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
                {
                    Array.Copy(h1[grp][s], 0, h1Concat, s * HashLen, HashLen);
                }

                h2[i] = ComputeSha1(h1Concat, 0, h1Concat.Length);
            }

            byte[] result = new byte[blocksPerGroup * WiiBlockSize];

            for (int b = 0; b < blocksPerGroup; b++)
            {
                // Serialize hash block
                byte[] hashBlock = new byte[WiiBlockHashSize];
                int off = 0;

                // H0 (31 * 20 = 0x26C)
                for (int i = 0; i < H0Count; i++)
                {
                    Array.Copy(h0[b][i], 0, hashBlock, off, HashLen);
                    off += HashLen;
                }

                off += 0x14; // padding0

                // H1 for this block's group (8 * 20 = 0xA0)
                int h1Grp = b / H1Count;
                if (h1Grp < h1.Length)
                {
                    for (int i = 0; i < H1Count; i++)
                    {
                        Array.Copy(h1[h1Grp][i], 0, hashBlock, off, HashLen);
                        off += HashLen;
                    }
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
                byte[] encHashBlock = AesCbc.Encrypt(hashBlock, key, new byte[16]) ?? new byte[WiiBlockHashSize];

                // Extract IV for data block from offset 0x3D0 of the encrypted hash block
                byte[] iv = new byte[16];
                Array.Copy(encHashBlock, 0x3D0, iv, 0, 16);

                // Encrypt data block
                int dataSrc = b * WiiBlockDataSize;
                int dataLen = Math.Min(WiiBlockDataSize, decryptedData.Length - dataSrc);
                byte[] dataBlock = new byte[WiiBlockDataSize];
                if (dataLen > 0)
                    Array.Copy(decryptedData, dataSrc, dataBlock, 0, dataLen);

                byte[] encDataBlock = AesCbc.Encrypt(dataBlock, key, iv) ?? new byte[WiiBlockDataSize];

                int dest = b * WiiBlockSize;
                Array.Copy(encHashBlock, 0, result, dest, WiiBlockHashSize);
                Array.Copy(encDataBlock, 0, result, dest + WiiBlockHashSize, WiiBlockDataSize);
            }

            return result;
        }

        /// <summary>
        /// Get a segmented SHA-1 hash for input data
        /// </summary>
        private static byte[] ComputeSha1(byte[] data, int offset, int count)
        {
            if (count == 0)
                return new byte[20];

            using var sha1 = new HashWrapper(HashType.SHA1);
            sha1.Process(data, offset, count);
            sha1.Terminate();
            return sha1.CurrentHashBytes ?? new byte[20];
        }

        #endregion
    }
}
