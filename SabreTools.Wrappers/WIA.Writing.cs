using System;
using System.Collections.Generic;
using System.IO;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Threading.Tasks;
#endif
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;
using SabreTools.Hashing;
using SabreTools.Numerics.Extensions;
using static SabreTools.Data.Models.NintendoDisc.Constants;
using static SabreTools.Data.Models.WIA.Constants;

namespace SabreTools.Wrappers
{
    public partial class WIA : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                string ext = IsRvz ? ".rvz" : ".wia";
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ext)
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            var writer = new Serialization.Writers.WIA { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }

        /// <summary>
        /// Compress a <see cref="NintendoDisc"/> wrapper to a WIA or RVZ file.
        /// </summary>
        public static bool ConvertFromDisc(NintendoDisc source,
            string outputPath,
            bool isRvz = false,
            WiaRvzCompressionType compressionType = WiaRvzCompressionType.None,
            int compressionLevel = 5,
            uint chunkSize = DefaultChunkSize)
        {
            if (string.IsNullOrEmpty(outputPath))
                return false;
            if (!isRvz && chunkSize != DefaultChunkSize)
                return false;
            if (isRvz && compressionType == WiaRvzCompressionType.Purge)
                return false;

            try
            {
                using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                return WriteWiaRvz(source, fs, isRvz, compressionType, Math.Max(1, Math.Min(22, compressionLevel)), chunkSize);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compress a <see cref="NintendoDisc"/> wrapper to a WIA or RVZ stream.
        /// Unlike <see cref="ConvertFromDisc"/>, exceptions are surfaced via
        /// <paramref name="exception"/> rather than silently swallowed.
        /// </summary>
        /// <param name="exception">Set to the exception thrown on failure, or null on success.</param>
        /// TODO: Determine why the exception can't just be thrown
        public static bool ConvertFromDiscToStream(NintendoDisc source,
            Stream dest,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            out Exception? exception)
        {
            exception = null;

            if (source is null)
            {
                exception = new ArgumentNullException(nameof(source));
                return false;
            }

            if (dest is null)
            {
                exception = new ArgumentNullException(nameof(dest));
                return false;
            }

            if (!isRvz && chunkSize != DefaultChunkSize)
            {
                exception = new ArgumentException("WIA chunkSize must equal DefaultChunkSize");
                return false;
            }

            if (isRvz && compressionType == WiaRvzCompressionType.Purge)
            {
                exception = new ArgumentException("RVZ does not support Purge compression");
                return false;
            }

            try
            {
                return WriteWiaRvz(source, dest, isRvz, compressionType, Math.Max(1, Math.Min(22, compressionLevel)), chunkSize);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Core pipeline for writing WIA and RVZ images
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private static bool WriteWiaRvz(NintendoDisc source,
            Stream dest,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize)
        {
            long isoSize = source.DataLength;
            if (isoSize <= 0)
                return false;

            byte[]? discHdr = source.ReadData(0, DiscHeaderStoredSize);
            if (discHdr is null)
                return false;

            Platform platform = DetectWiaPlatform(discHdr);
            return platform switch
            {
                Platform.GameCube => WriteGameCube(source, dest, isRvz, compressionType, compressionLevel, chunkSize, isoSize, discHdr),
                Platform.Wii => WriteWii(source, dest, isRvz, compressionType, compressionLevel, chunkSize, isoSize, discHdr),

                // These should never happen
                Platform.Unknown => false,
                _ => false,
            };
        }

        /// <summary>
        /// Write a GameCube WIA or RVZ image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <param name="isoSize"></param>
        /// <param name="discHdr"></param>
        /// <returns></returns>
        private static bool WriteGameCube(NintendoDisc source,
            Stream dest,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            long isoSize,
            byte[] discHdr)
        {
            const long rawDataStart = DiscHeaderStoredSize;
            long rawDataSize = isoSize - rawDataStart;
            if (rawDataSize <= 0)
                return false;

            uint numGroups = (uint)((rawDataSize + chunkSize - 1) / chunkSize);

            int groupEntrySize = isRvz ? RvzGroupEntrySize : WiaGroupEntrySize;

            long headersBound = Align(
                Header1Size + Header2Size + RawDataEntrySize + 0x100 + (numGroups * groupEntrySize),
                WiiBlockSize);

            dest.Write(new byte[headersBound], 0, (int)headersBound);
            long bytesWritten = headersBound;

            var groupEntries = new RvzGroupEntry[numGroups];
            var rawDedupMap = new Dictionary<WiaDedupKey2, RvzGroupEntry>();
            FileSystemTableReader? gcFst = isRvz ? BuildFileSystemTableReader(source) : null;

            GetCompressorData(compressionType, compressionLevel, out byte[] propData, out byte propSize);

            uint groupIdx = 0;
            long srcOff = rawDataStart;
            long remaining = rawDataSize;

            int batchSize = Math.Max(Environment.ProcessorCount * 4, 64);

            while (remaining > 0)
            {
                int thisBatch = (int)Math.Min(batchSize, (remaining + chunkSize - 1) / chunkSize);
                var work = new GcGroupWorkEntry[thisBatch];
                int actualBatch = 0;

                for (int i = 0; i < thisBatch && remaining > 0; i++)
                {
                    int toRead = (int)Math.Min(chunkSize, remaining);
                    byte[] raw = source.ReadData(srcOff, toRead);
                    if (raw.Length == 0)
                        break;

                    var gi = work[i] = new GcGroupWorkEntry
                    {
                        BytesRead = toRead,
                        SourceOffset = srcOff,
                    };

                    srcOff += toRead;
                    remaining -= toRead;
                    actualBatch++;

                    gi.IsAllSame = IsAllSame(raw, toRead);
                    gi.SameByte = raw[0];

                    if (gi.IsAllSame)
                    {
                        var dk = new WiaDedupKey2(gi.SameByte, toRead);
                        if (rawDedupMap.TryGetValue(dk, out var cached))
                        {
                            gi.IsDedupHit = true;
                            gi.DedupEntry = cached;
                            continue;
                        }

                        if (gi.SameByte == 0)
                            continue;
                    }

                    if (isRvz)
                    {
                        byte[]? packed = RvzPackEncoder.Pack(raw, 0, toRead, srcOff - toRead, out gi.RvzPackedSize, gcFst);
                        gi.MainData = packed ?? raw;
                        if (packed is null)
                            gi.RvzPackedSize = 0;
                    }
                    else
                    {
                        gi.MainData = raw;
                    }
                }

                if (actualBatch == 0)
                    break;

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                if (compressionType > WiaRvzCompressionType.Purge)
                {
                    WiaRvzCompressionType ct = compressionType;
                    int cl = compressionLevel;
                    byte[] pd = propData;
                    byte ps = propSize;
                    Parallel.For(0, actualBatch, w =>
                    {
                        var gi = work[w];
                        if (gi.MainData is not null && !gi.IsDedupHit)
                            gi.CompressedData = Compress(ct, gi.MainData, 0, gi.MainData.Length, cl, pd, ps);
                    });
                }
#endif

                for (int i = 0; i < actualBatch; i++)
                {
                    uint idx = groupIdx + (uint)i;
                    var gi = work[i];

                    if (gi.IsDedupHit)
                    {
                        groupEntries[idx] = gi.DedupEntry;
                    }
                    else if (gi.IsAllSame && gi.SameByte == 0)
                    {
                        var dk = new WiaDedupKey2(0, gi.BytesRead);
                        if (!rawDedupMap.TryGetValue(dk, out var ze))
                        {
                            ze = new RvzGroupEntry
                            {
                                DataOffset = (uint)(bytesWritten >> 2),
                                DataSize = 0,
                                RvzPackedSize = 0
                            };
                            rawDedupMap[dk] = ze;
                        }

                        groupEntries[idx] = ze;
                    }
                    else if (gi.MainData is not null)
                    {
                        uint groupOff = (uint)(bytesWritten >> 2);
                        uint storedSz = WriteRawGroupData(dest, ref bytesWritten, gi, isRvz, compressionType);
                        PadToFourBytes(dest, ref bytesWritten);

                        var entry = new RvzGroupEntry
                        {
                            DataOffset = groupOff,
                            DataSize = storedSz,
                            RvzPackedSize = gi.RvzPackedSize
                        };
                        groupEntries[idx] = entry;
                        if (gi.IsAllSame && gi.SameByte != 0)
                            rawDedupMap[new WiaDedupKey2(gi.SameByte, gi.BytesRead)] = entry;
                    }
                }

                groupIdx += (uint)actualBatch;
            }

            // Write tables
            dest.Seek(Header1Size + Header2Size, SeekOrigin.Begin);
            long tablePos = Header1Size + Header2Size;

            ulong rawEntriesOffset = (ulong)tablePos;
            var rawEntry = new RawDataEntry
            {
                DataOffset = DiscHeaderStoredSize,
                DataSize = (ulong)rawDataSize,
                GroupIndex = 0,
                NumberOfGroups = numGroups,
            };
            byte[] rawEntryBytes = SerializeRawDataEntry(rawEntry);
            byte[] rawEntryWritten = CompressTableDataWia(rawEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(rawEntryWritten, 0, rawEntryWritten.Length);
            tablePos += rawEntryWritten.Length;
            PadTableToFourBytes(dest, ref tablePos);

            ulong groupEntriesOffset = (ulong)tablePos;
            byte[] groupEntryBytes = SerializeGroupEntries(groupEntries, numGroups, isRvz);
            byte[] groupEntryWritten = CompressTableDataWia(groupEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(groupEntryWritten, 0, groupEntryWritten.Length);
            tablePos += groupEntryWritten.Length;

            WriteWiaHeaders(dest,
                discHdr,
                isRvz,
                WiaDiscType.GameCube,
                compressionType,
                compressionLevel,
                chunkSize,
                0,
                (ulong)tablePos,
                new byte[20], // no partition entries
                1u,
                rawEntriesOffset,
                (uint)rawEntryWritten.Length,
                numGroups,
                groupEntriesOffset,
                (uint)groupEntryWritten.Length,
                propData,
                propSize,
                isoSize,
                bytesWritten);
            dest.Flush();

            return true;
        }

        /// <summary>
        /// Write a Wii WIA or RVZ image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <param name="isoSize"></param>
        /// <param name="discHdr"></param>
        /// <returns></returns>
        private static bool WriteWii(NintendoDisc source,
            Stream dest,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            long isoSize,
            byte[] discHdr)
        {
            var partitions = ReadWiiPartitions(source);
            if (partitions is null)
                return false;

            var rawRegions = BuildRawRegions(partitions, isoSize);

            GetCompressorData(compressionType, compressionLevel, out byte[] propData, out byte propSize);

            int groupEntrySize = isRvz ? RvzGroupEntrySize : WiaGroupEntrySize;
            uint totalGroups = CalcTotalGroups(partitions, rawRegions, chunkSize);

            long headersBound = Align(
                Header1Size + Header2Size + (partitions.Count * PartitionEntrySize) + (rawRegions.Count * RawDataEntrySize) + 0x100 + (totalGroups * groupEntrySize),
                WiiBlockSize);

            dest.Write(new byte[headersBound], 0, (int)headersBound);
            long bytesWritten = headersBound;

            var allGroups = new List<RvzGroupEntry>();
            uint currentGrpIdx = 0;
            uint lastValidOff = 0;

            var dedupMap = new Dictionary<WiaDedupKey3, WiaDedup2>();
            var decDedupMap = new Dictionary<WiaDedupKey3, WiaDedup2>();
            var rawDedupMap = new Dictionary<WiaDedupKey2, RvzGroupEntry>();
            var wiaZeroDedup = new Dictionary<ulong, uint>();

            var regions = BuildDiscRegions(partitions, rawRegions);
            foreach (var region in regions)
            {
                if (region.IsPartition)
                {
                    ProcessWiiPartition(source,
                        dest,
                        region.PartitionInfo!,
                        ref bytesWritten,
                        allGroups,
                        ref currentGrpIdx,
                        ref lastValidOff,
                        dedupMap,
                        decDedupMap,
                        wiaZeroDedup,
                        isRvz,
                        compressionType,
                        compressionLevel,
                        chunkSize,
                        propData,
                        propSize);
                }
                else
                {
                    ProcessRawRegion(source,
                        dest,
                        region.RawInfo!,
                        ref bytesWritten,
                        allGroups,
                        ref currentGrpIdx,
                        ref lastValidOff,
                        rawDedupMap,
                        isRvz,
                        compressionType,
                        compressionLevel,
                        chunkSize,
                        propData,
                        propSize);
                }
            }

            // Write tables
            dest.Seek(Header1Size + Header2Size, SeekOrigin.Begin);
            long tablePos = Header1Size + Header2Size;

            ulong partEntriesOffset = (ulong)tablePos;
            byte[] partEntriesBytes = SerializePartitionEntries(dest, partitions);
            tablePos += partEntriesBytes.Length;
            PadTableToFourBytes(dest, ref tablePos);

            ulong rawEntriesOffset = (ulong)tablePos;
            byte[] rawEntryBytes = SerializeRawDataEntries(rawRegions);
            byte[] rawEntryWritten = CompressTableDataWia(rawEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(rawEntryWritten, 0, rawEntryWritten.Length);
            tablePos += rawEntryWritten.Length;
            PadTableToFourBytes(dest, ref tablePos);

            ulong groupEntriesOffset = (ulong)tablePos;
            using (var gms = new MemoryStream())
            {
                foreach (var e in allGroups)
                {
                    WriteGroupEntryWia(gms, e, isRvz);
                }

                byte[] gBytes = gms.ToArray();
                byte[] gWritten = CompressTableDataWia(gBytes, compressionType, compressionLevel, propData, propSize);
                dest.Write(gWritten, 0, gWritten.Length);
                tablePos += gWritten.Length;

                byte[] partHashData = ComputeSha1Wia(partEntriesBytes, 0, partEntriesBytes.Length);
                WriteWiaHeaders(dest,
                    discHdr,
                    isRvz,
                    WiaDiscType.Wii,
                    compressionType,
                    compressionLevel,
                    chunkSize,
                    (uint)partitions.Count, partEntriesOffset,
                    partHashData,
                    (uint)rawRegions.Count,
                    rawEntriesOffset,
                    (uint)rawEntryWritten.Length,
                    (uint)allGroups.Count,
                    groupEntriesOffset,
                    (uint)gWritten.Length,
                    propData,
                    propSize,
                    isoSize,
                    bytesWritten);
            }

            dest.Flush();
            return true;
        }

        /// <summary>
        /// Wii partition processing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="part"></param>
        /// <param name="bytesWritten"></param>
        /// <param name="groupEntries"></param>
        /// <param name="currentGrpIdx"></param>
        /// <param name="lastValidOff"></param>
        /// <param name="dedupMap"></param>
        /// <param name="decDedupMap"></param>
        /// <param name="wiaZeroDedup"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <param name="propData"></param>
        /// <param name="propSize"></param>
        private static void ProcessWiiPartition(NintendoDisc source,
            Stream dest,
            WiiPartInfo part,
            ref long bytesWritten,
            List<RvzGroupEntry> groupEntries,
            ref uint currentGrpIdx,
            ref uint lastValidOff,
            Dictionary<WiaDedupKey3, WiaDedup2> dedupMap,
            Dictionary<WiaDedupKey3, WiaDedup2> decDedupMap,
            Dictionary<ulong, uint> wiaZeroDedup,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            byte[] propData,
            byte propSize)
        {
            long remaining = (long)part.DataSize;
            long srcOff = (long)part.DataStart;
            ulong partKeyHash = BitConverter.ToUInt64(part.TitleKey, 0)
                              ^ BitConverter.ToUInt64(part.TitleKey, 8);

            part.FirstGroupIndex = currentGrpIdx;

            int blocksPerChunk = (int)chunkSize / WiiBlockSize;
            int chunksPerGroup = WiiBlocksPerGroup / blocksPerChunk;
            int wiiGroupSize = WiiGroupSize;

            int outerBatch = (chunksPerGroup == 1)
                ? Math.Max(Environment.ProcessorCount * 2, 16)
                : 1;

            var batchItems = new WiiBatchItem[outerBatch];
            var flatWork = new List<WiaFlatWorkItem>(outerBatch);
            long regionDecOff = 0;

            while (remaining > 0)
            {
                int actualBatch = 0;
                flatWork.Clear();

                for (int b = 0; b < outerBatch && remaining > 0; b++)
                {
                    int toRead = (int)Math.Min(wiiGroupSize, remaining);
                    byte[]? encGroup = source.ReadData(srcOff, toRead);
                    if (encGroup is null) break;

                    var item = batchItems[b] = new WiiBatchItem
                    {
                        BytesRead = toRead,
                        SrcOffset = srcOff,
                    };

                    srcOff += toRead;
                    remaining -= toRead;
                    actualBatch++;

                    bool encAllSame = (chunksPerGroup == 1) && IsAllSame(encGroup, toRead);
                    item.EncAllSame = encAllSame;
                    item.DedupKey = new WiaDedupKey3(partKeyHash, encGroup[0], toRead);

                    if (encAllSame && dedupMap.TryGetValue(item.DedupKey, out var reused))
                    {
                        item.IsInterDedupHit = true;
                        item.DedupResult = reused;
                        regionDecOff += (long)(toRead / WiiBlockSize) * WiiBlockDataSize;
                        continue;
                    }

                    int numBlocks = toRead / WiiBlockSize;
                    item.NumChunks = (numBlocks + blocksPerChunk - 1) / blocksPerChunk;

                    item.DecryptedAll = DecryptWiiGroup(encGroup, toRead, part.TitleKey);
                    item.AllExceptions = GenerateHashExceptions(encGroup, item.DecryptedAll, part.TitleKey, numBlocks);

                    item.PartWork = new WiiChunkWork[item.NumChunks];
                    for (int c = 0; c < item.NumChunks; c++)
                    {
                        int cBlockStart = c * blocksPerChunk;
                        int cBlockEnd = Math.Min(cBlockStart + blocksPerChunk, numBlocks);
                        int actualBlocks = cBlockEnd - cBlockStart;
                        int decOff = cBlockStart * WiiBlockDataSize;
                        int decLen = actualBlocks * WiiBlockDataSize;

                        byte[] procData = new byte[decLen];
                        if (item.DecryptedAll is not null && decLen > 0)
                            Array.Copy(item.DecryptedAll, decOff, procData, 0, decLen);

                        var chunkEx = new List<HashExceptionEntry>();
                        if (item.AllExceptions is not null)
                        {
                            foreach (var ex in item.AllExceptions)
                            {
                                int exBlock = ex.Offset / WiiBlockHeaderSize;
                                if (exBlock >= cBlockStart && exBlock < cBlockEnd)
                                {
                                    int localBlock = exBlock - cBlockStart;
                                    ushort localOff = (ushort)((localBlock * WiiBlockHeaderSize) + (ex.Offset % WiiBlockHeaderSize));
                                    chunkEx.Add(new HashExceptionEntry { Offset = localOff, Hash = ex.Hash });
                                }
                            }
                        }

                        bool isAllZeros = !isRvz
                            && chunksPerGroup == 1
                            && chunkEx.Count == 0
                            && procData.Length > 0
                            && IsAllSame(procData, procData.Length)
                            && procData[0] == 0;

                        bool decAllSame = !isRvz
                            && !isAllZeros
                            && chunksPerGroup == 1
                            && chunkEx.Count == 0
                            && procData.Length > 0
                            && IsAllSame(procData, procData.Length);

                        var decDedupKey = new WiaDedupKey3(partKeyHash, procData.Length > 0 ? procData[0] : (byte)0, procData.Length);

                        var pw = item.PartWork[c] = new WiiChunkWork
                        {
                            IsAllZeros = isAllZeros,
                            DecAllSame = decAllSame,
                            DecDedupKey = decDedupKey,
                        };

                        if (isAllZeros)
                            continue;

                        if (decAllSame && decDedupMap.TryGetValue(decDedupKey, out var decReused))
                        {
                            pw.IsDecDedupHit = true;
                            pw.DecDedupOffset = decReused.Offset;
                            pw.DecDedupDataSize = decReused.DataSize;
                            continue;
                        }

                        byte[] exListBytes = BuildExceptionList(chunkEx);
                        int unpaddedExLen = 2 + (chunkEx.Count * 22);

                        byte[] mainData;
                        uint rvzPackedSize = 0;
                        if (isRvz)
                        {
                            long baseDecOff = regionDecOff + ((long)c * (blocksPerChunk * WiiBlockDataSize));
                            byte[]? packed = RvzPackEncoder.Pack(procData, 0, procData.Length, baseDecOff, out rvzPackedSize);
                            mainData = packed ?? procData;
                            if (packed is null)
                                rvzPackedSize = 0;
                        }
                        else
                        {
                            mainData = procData;
                        }

                        pw.ExceptionListBytes = exListBytes;
                        pw.UnpaddedExLen = unpaddedExLen;
                        pw.MainDataBytes = mainData;
                        pw.RvzPackedSize = rvzPackedSize;

                        if (compressionType != WiaRvzCompressionType.None)
                            flatWork.Add(new WiaFlatWorkItem(b, c));
                    }

                    regionDecOff += (long)numBlocks * WiiBlockDataSize;
                }

                if (actualBatch == 0)
                    break;

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                // Phase 2: compress
                if (flatWork.Count > 0)
                {
                    int cl = compressionLevel;
                    byte[] pd = propData;
                    byte ps = propSize;
                    Parallel.For(0, flatWork.Count, idx =>
                    {
                        var fw = flatWork[idx];
                        var pw = batchItems[fw.BatchIndex].PartWork![fw.ChunkIndex];
                        if (compressionType > WiaRvzCompressionType.Purge)
                        {
                            byte[] toCompress = ConcatBytes(
                                pw.ExceptionListBytes,
                                0,
                                pw.UnpaddedExLen,
                                pw.MainDataBytes,
                                0,
                                pw.MainDataBytes.Length);
                            pw.CompressedData = Compress(compressionType, toCompress, 0, toCompress.Length, cl, pd, ps);
                        }
                        else if (compressionType == WiaRvzCompressionType.Purge)
                        {
                            pw.CompressedData = PurgeCompressor.Compress(pw.MainDataBytes, 0, pw.MainDataBytes.Length, pw.ExceptionListBytes);
                        }
                    });
                }
#endif

                // Phase 3: write
                for (int b = 0; b < actualBatch; b++)
                {
                    var item = batchItems[b];

                    if (item.IsInterDedupHit)
                    {
                        lastValidOff = item.DedupResult.Offset;
                        groupEntries.Add(new RvzGroupEntry
                        {
                            DataOffset = item.DedupResult.Offset,
                            DataSize = item.DedupResult.DataSize,
                            RvzPackedSize = 0
                        });
                        currentGrpIdx++;
                        continue;
                    }

                    for (int c = 0; c < item.NumChunks; c++)
                    {
                        var pw = item.PartWork![c];

                        if (pw.IsAllZeros)
                        {
                            uint wouldBeOff = (uint)(bytesWritten >> 2);
                            if (!wiaZeroDedup.TryGetValue(partKeyHash, out uint firstOff))
                            {
                                firstOff = wouldBeOff;
                                wiaZeroDedup[partKeyHash] = firstOff;
                            }

                            groupEntries.Add(new RvzGroupEntry
                            {
                                DataOffset = firstOff,
                                DataSize = 0,
                                RvzPackedSize = 0,
                            });
                        }
                        else if (pw.IsDecDedupHit)
                        {
                            groupEntries.Add(new RvzGroupEntry
                            {
                                DataOffset = pw.DecDedupOffset,
                                DataSize = pw.DecDedupDataSize,
                                RvzPackedSize = 0,
                            });
                        }
                        else
                        {
                            uint groupOff = (uint)(bytesWritten >> 2);
                            lastValidOff = groupOff;
                            uint storedSz = WriteWiiChunkData(dest, ref bytesWritten, pw, isRvz, compressionType);

                            groupEntries.Add(new RvzGroupEntry
                            {
                                DataOffset = groupOff,
                                DataSize = storedSz,
                                RvzPackedSize = pw.RvzPackedSize,
                            });

                            if (item.EncAllSame && c == 0)
                                dedupMap[item.DedupKey] = new WiaDedup2(groupOff, storedSz);
                            if (pw.DecAllSame && c == 0)
                                decDedupMap[pw.DecDedupKey] = new WiaDedup2(groupOff, storedSz);

                            PadToFourBytes(dest, ref bytesWritten);
                        }
                    }

                    currentGrpIdx++;
                }
            }

            part.NumberOfGroups = currentGrpIdx - part.FirstGroupIndex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="bytesWritten"></param>
        /// <param name="pw"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <returns></returns>
        private static uint WriteWiiChunkData(Stream dest,
            ref long bytesWritten,
            WiiChunkWork pw,
            bool isRvz,
            WiaRvzCompressionType compressionType)
        {
            if (pw.CompressedData is not null)
            {
                bool useC = !isRvz || pw.CompressedData.Length < pw.MainDataBytes.Length;
                if (useC && compressionType > WiaRvzCompressionType.Purge)
                {
                    dest.Write(pw.CompressedData, 0, pw.CompressedData.Length);
                    bytesWritten += pw.CompressedData.Length;
                    return isRvz
                        ? (uint)pw.CompressedData.Length | 0x80000000u
                        : (uint)pw.CompressedData.Length;
                }

                if (compressionType == WiaRvzCompressionType.Purge)
                {
                    dest.Write(pw.ExceptionListBytes, 0, pw.ExceptionListBytes.Length);
                    bytesWritten += pw.ExceptionListBytes.Length;
                    dest.Write(pw.CompressedData, 0, pw.CompressedData.Length);
                    bytesWritten += pw.CompressedData.Length;
                    return (uint)(pw.ExceptionListBytes.Length + pw.CompressedData.Length);
                }
            }

            dest.Write(pw.ExceptionListBytes, 0, pw.ExceptionListBytes.Length);
            bytesWritten += pw.ExceptionListBytes.Length;
            dest.Write(pw.MainDataBytes, 0, pw.MainDataBytes.Length);
            bytesWritten += pw.MainDataBytes.Length;
            return (uint)(pw.ExceptionListBytes.Length + pw.MainDataBytes.Length);
        }

        /// <summary>
        /// Raw region processing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="raw"></param>
        /// <param name="bytesWritten"></param>
        /// <param name="groupEntries"></param>
        /// <param name="currentGrpIdx"></param>
        /// <param name="lastValidOff"></param>
        /// <param name="rawDedupMap"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <param name="propData"></param>
        /// <param name="propSize"></param>
        private static void ProcessRawRegion(NintendoDisc source,
            Stream dest,
            RawDataEntry raw,
            ref long bytesWritten,
            List<RvzGroupEntry> groupEntries,
            ref uint currentGrpIdx,
            ref uint lastValidOff,
            Dictionary<WiaDedupKey2, RvzGroupEntry> rawDedupMap,
            bool isRvz,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            byte[] propData,
            byte propSize)
        {
            raw.GroupIndex = currentGrpIdx;

            long skip = (long)raw.DataOffset % WiiBlockSize;
            long adjOffset = (long)raw.DataOffset - skip;
            long remaining = (long)raw.DataSize + skip;
            long srcOff = adjOffset;

            while (remaining > 0)
            {
                int toRead = (int)Math.Min(chunkSize, remaining);
                byte[]? data = source.ReadData(srcOff, toRead);
                if (data is null)
                    break;

                bool isAllSame = IsAllSame(data, toRead);
                byte sameByte = data[0];

                if (isAllSame)
                {
                    var dk = new WiaDedupKey2(sameByte, toRead);
                    if (rawDedupMap.TryGetValue(dk, out var cached))
                    {
                        groupEntries.Add(cached);
                        currentGrpIdx++;
                        srcOff += toRead;
                        remaining -= toRead;
                        continue;
                    }

                    if (sameByte == 0)
                    {
                        var ze = new RvzGroupEntry
                        {
                            DataOffset = (uint)(bytesWritten >> 2),
                            DataSize = 0,
                            RvzPackedSize = 0,
                        };
                        rawDedupMap[dk] = ze;
                        groupEntries.Add(ze);
                        currentGrpIdx++;
                        srcOff += toRead;
                        remaining -= toRead;
                        continue;
                    }
                }

                byte[] mainData;
                uint rvzPackedSize = 0;
                if (isRvz)
                {
                    byte[]? packed = RvzPackEncoder.Pack(data, 0, toRead, srcOff, out rvzPackedSize);
                    mainData = packed ?? data;
                    if (packed is null)
                        rvzPackedSize = 0;
                }
                else
                {
                    mainData = data;
                }

                byte[]? compressed = null;
                if (compressionType > WiaRvzCompressionType.Purge)
                {
                    byte[] c2 = Compress(compressionType, mainData, 0, mainData.Length, compressionLevel, propData, propSize);
                    if (!isRvz || c2.Length < mainData.Length)
                        compressed = c2;
                }
                else if (compressionType == WiaRvzCompressionType.Purge)
                {
                    compressed = PurgeCompressor.Compress(mainData, 0, mainData.Length);
                }

                uint groupOff = (uint)(bytesWritten >> 2);
                lastValidOff = groupOff;
                uint storedSz;

                if (compressed is not null)
                {
                    bool useC = !isRvz || compressed.Length < mainData.Length;
                    if (useC)
                    {
                        dest.Write(compressed, 0, compressed.Length);
                        bytesWritten += compressed.Length;
                        storedSz = isRvz
                            ? (uint)compressed.Length | 0x80000000u
                            : (uint)compressed.Length;
                    }
                    else
                    {
                        dest.Write(mainData, 0, mainData.Length);
                        bytesWritten += mainData.Length;
                        storedSz = (uint)mainData.Length;
                    }
                }
                else
                {
                    dest.Write(mainData, 0, mainData.Length);
                    bytesWritten += mainData.Length;
                    storedSz = (uint)mainData.Length;
                }

                PadToFourBytes(dest, ref bytesWritten);

                var entry = new RvzGroupEntry
                {
                    DataOffset = groupOff,
                    DataSize = storedSz,
                    RvzPackedSize = rvzPackedSize,
                };
                groupEntries.Add(entry);
                if (isAllSame && sameByte != 0)
                    rawDedupMap[new WiaDedupKey2(sameByte, toRead)] = entry;

                currentGrpIdx++;
                srcOff += toRead;
                remaining -= toRead;
            }

            raw.NumberOfGroups = currentGrpIdx - raw.GroupIndex;
        }

        #region Wii Crypto Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="encGroup"></param>
        /// <param name="bytesRead"></param>
        /// <param name="titleKey"></param>
        /// <returns></returns>
        private static byte[]? DecryptWiiGroup(byte[] encGroup, int bytesRead, byte[] titleKey)
        {
            int numBlocks = bytesRead / WiiBlockSize;
            var result = new byte[numBlocks * WiiBlockDataSize];

            for (int i = 0; i < numBlocks; i++)
            {
                int off = i * WiiBlockSize;
                byte[] iv = new byte[16];
                Array.Copy(encGroup, off + 0x3D0, iv, 0, 16);

                byte[] encData = new byte[WiiBlockDataSize];
                Array.Copy(encGroup, off + WiiBlockHeaderSize, encData, 0, WiiBlockDataSize);

                byte[]? dec = WiiDecrypter.DecryptBlock(encData, titleKey, iv);
                if (dec is null)
                    return null;

                Array.Copy(dec, 0, result, i * WiiBlockDataSize, WiiBlockDataSize);
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encGroup"></param>
        /// <param name="decryptedData"></param>
        /// <param name="titleKey"></param>
        /// <param name="numBlocks"></param>
        /// <returns></returns>
        private static List<HashExceptionEntry> GenerateHashExceptions(byte[] encGroup,
            byte[]? decryptedData,
            byte[] titleKey,
            int numBlocks)
        {
            var exceptions = new List<HashExceptionEntry>();
            if (decryptedData is null)
                return exceptions;

            // Re-encrypt the decrypted data to obtain recomputed hashes
            byte[] reEncGroup = EncryptWiiGroup(decryptedData, titleKey, numBlocks);

            for (int blockIdx = 0; blockIdx < numBlocks; blockIdx++)
            {
                int blockOff = blockIdx * WiiBlockSize;

                byte[] encHashBlock = new byte[WiiBlockHeaderSize];
                Array.Copy(encGroup, blockOff, encHashBlock, 0, WiiBlockHeaderSize);

                byte[] origHash = AesCbc.Decrypt(encHashBlock, titleKey, new byte[16]) ?? new byte[WiiBlockHeaderSize];

                byte[] reEncHashBlock = new byte[WiiBlockHeaderSize];
                Array.Copy(reEncGroup, blockOff, reEncHashBlock, 0, WiiBlockHeaderSize);
                byte[] recompHash = AesCbc.Decrypt(reEncHashBlock, titleKey, new byte[16]) ?? new byte[WiiBlockHeaderSize];

                for (int off = 0; off < WiiBlockHeaderSize; off += 20)
                {
                    bool match = true;
                    for (int j = 0; j < 20 && (off + j) < WiiBlockHeaderSize; j++)
                    {
                        if (origHash[off + j] != recompHash[off + j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (!match)
                    {
                        byte[] hash = new byte[20];
                        Array.Copy(origHash, off, hash, 0, Math.Min(20, WiiBlockHeaderSize - off));
                        exceptions.Add(new HashExceptionEntry
                        {
                            Offset = (ushort)((blockIdx * WiiBlockHeaderSize) + off),
                            Hash = hash,
                        });
                    }
                }
            }

            return exceptions;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<WiiPartInfo>? ReadWiiPartitions(NintendoDisc source)
        {
            var result = new List<WiiPartInfo>();

            for (int group = 0; group < WiiPartitionGroupCount; group++)
            {
                byte[]? gEntry = source.ReadData(WiiPartitionTableAddress + (group * 8), 8);
                if (gEntry is null)
                    continue;

                int countPos = 0, offsetPos = 4;
                uint count = gEntry.ReadUInt32BigEndian(ref countPos);
                uint offset = gEntry.ReadUInt32BigEndian(ref offsetPos) << 2;
                if (count == 0 || offset == 0)
                    continue;

                for (int i = 0; i < count; i++)
                {
                    byte[]? pEntry = source.ReadData(offset + (i * 8), 8);
                    if (pEntry is null)
                        continue;

                    int partOffPos = 0;
                    long partOff = (long)pEntry.ReadUInt32BigEndian(ref partOffPos) << 2;

                    byte[]? sigType = source.ReadData(partOff, 4);
                    int sigTypePos = 0;
                    if (sigType is null || sigType.ReadUInt32BigEndian(ref sigTypePos) != 0x10001U)
                        continue;

                    byte[]? hdr = source.ReadData(partOff, 0x2C0);
                    if (hdr is null)
                        continue;

                    byte[] encKey = new byte[16];
                    Array.Copy(hdr, 0x1BF, encKey, 0, 16);
                    byte[] titleId = new byte[8];
                    Array.Copy(hdr, 0x1DC, titleId, 0, 8);
                    byte ckIdx = hdr[0x1F1];

                    byte[]? titleKey = source.WiiDecrypter.DecryptTitleKey(encKey, titleId, ckIdx);
                    if (titleKey is null)
                        continue;

                    int dataOffPos = 0x2B8, dataSzPos = 0x2BC;
                    ulong dataOff = (ulong)hdr.ReadUInt32BigEndian(ref dataOffPos) << 2;
                    ulong dataSize = (ulong)hdr.ReadUInt32BigEndian(ref dataSzPos) << 2;

                    result.Add(new WiiPartInfo
                    {
                        PartitionOffset = (ulong)partOff,
                        TitleKey = titleKey,
                        DataOffset = dataOff,
                        DataSize = dataSize,
                        DataStart = (ulong)partOff + dataOff,
                        DataEnd = (ulong)partOff + dataOff + dataSize,
                    });
                }
            }

            return result.Count == 0 ? null : result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="partitions"></param>
        /// <param name="isoSize"></param>
        /// <returns></returns>
        private static List<RawDataEntry> BuildRawRegions(List<WiiPartInfo> partitions, long isoSize)
        {
            var regions = new List<RawDataEntry>();
            partitions.Sort((a, b) => a.PartitionOffset.CompareTo(b.PartitionOffset));

            ulong cur = DiscHeaderStoredSize;
            foreach (var p in partitions)
            {
                if (cur < p.PartitionOffset)
                    regions.Add(new RawDataEntry { DataOffset = cur, DataSize = p.PartitionOffset - cur });

                regions.Add(new RawDataEntry { DataOffset = p.PartitionOffset, DataSize = p.DataOffset });
                cur = p.DataEnd;
            }

            if (cur < (ulong)isoSize)
                regions.Add(new RawDataEntry { DataOffset = cur, DataSize = (ulong)isoSize - cur });

            return regions;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="partitions"></param>
        /// <param name="rawRegions"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private static uint CalcTotalGroups(List<WiiPartInfo> partitions, List<RawDataEntry> rawRegions, uint chunkSize)
        {
            uint total = 0;
            foreach (var p in partitions)
            {
                total += (uint)((p.DataSize + chunkSize - 1) / chunkSize);
            }

            foreach (var r in rawRegions)
            {
                total += (uint)((r.DataSize + chunkSize - 1) / chunkSize);
            }

            return total;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="partitions"></param>
        /// <param name="rawRegions"></param>
        /// <returns></returns>
        private static List<DiscRegionEntry> BuildDiscRegions(List<WiiPartInfo> partitions, List<RawDataEntry> rawRegions)
        {
            var result = new List<DiscRegionEntry>();
            foreach (var p in partitions)
            {
                result.Add(new DiscRegionEntry { IsPartition = true, Offset = (long)p.DataStart, PartitionInfo = p });
            }

            foreach (var r in rawRegions)
            {
                result.Add(new DiscRegionEntry { IsPartition = false, Offset = (long)r.DataOffset, RawInfo = r });
            }

            result.Sort((a, b) => a.Offset.CompareTo(b.Offset));
            return result;
        }

        #endregion

        /// <summary>
        /// GcFst helper
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static FileSystemTableReader? BuildFileSystemTableReader(NintendoDisc source)
        {
            byte[]? hdr = source.ReadData(0x420, 12);
            if (hdr is null)
                return null;

            int fstOffPos = 4, fstSzPos = 8;
            uint fstOff = hdr.ReadUInt32BigEndian(ref fstOffPos);
            uint fstSize = hdr.ReadUInt32BigEndian(ref fstSzPos);
            if (fstOff == 0 || fstSize == 0)
                return null;

            byte[]? fstData = source.ReadData(fstOff, (int)fstSize);
            if (fstData is null)
                return null;

            return FileSystemTableReader.TryParse(fstData, offsetShift: 0);
        }

        #region Serialization - TODO: MOVE TO WRITER

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] SerializeRawDataEntry(RawDataEntry obj)
        {
            using var ms = new MemoryStream();
            Serialization.Writers.WIA.WriteRawDataEntry(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="count"></param>
        /// <param name="isRvz"></param>
        /// <returns></returns>
        private static byte[] SerializeGroupEntries(RvzGroupEntry[] entries, uint count, bool isRvz)
        {
            using var ms = new MemoryStream();
            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                WriteGroupEntryWia(ms, entry, isRvz);
            }

            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        /// <param name="isRvz"></param>
        private static void WriteGroupEntryWia(Stream stream, RvzGroupEntry obj, bool isRvz)
        {
            if (isRvz)
            {
                Serialization.Writers.WIA.WriteRvzGroupEntry(stream, obj);
            }
            else
            {
                var wiaEntry = new WiaGroupEntry { DataOffset = obj.DataOffset, DataSize = obj.DataSize };
                Serialization.Writers.WIA.WriteWiaGroupEntry(stream, wiaEntry);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="partitions"></param>
        /// <returns></returns>
        private static byte[] SerializePartitionEntries(Stream stream, List<WiiPartInfo> partitions)
        {
            using var ms = new MemoryStream();
            foreach (var partition in partitions)
            {
                // Write 16-byte key
                ms.Write(partition.TitleKey, 0, 16);
                stream.Write(partition.TitleKey, 0, 16);

                // DataEntry0: all of the partition
                ms.WriteBigEndian((uint)(partition.DataStart / WiiBlockSize));
                ms.WriteBigEndian((uint)(partition.DataSize / WiiBlockSize));
                ms.WriteBigEndian(partition.FirstGroupIndex);
                ms.WriteBigEndian(partition.NumberOfGroups);
                stream.WriteBigEndian((uint)(partition.DataStart / WiiBlockSize));
                stream.WriteBigEndian((uint)(partition.DataSize / WiiBlockSize));
                stream.WriteBigEndian(partition.FirstGroupIndex);
                stream.WriteBigEndian(partition.NumberOfGroups);

                // DataEntry1: zeros
                byte[] zeroPDE = new byte[PartitionDataEntrySize];
                ms.Write(zeroPDE, 0, zeroPDE.Length);
                stream.Write(zeroPDE, 0, zeroPDE.Length);
            }

            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="regions"></param>
        /// <returns></returns>
        private static byte[] SerializeRawDataEntries(List<RawDataEntry> regions)
        {
            using var ms = new MemoryStream();
            foreach (var region in regions)
            {
                ms.Write(SerializeRawDataEntry(region), 0, RawDataEntrySize);
            }

            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private static byte[] BuildExceptionList(List<HashExceptionEntry> exceptions)
        {
            using var ms = new MemoryStream();
            long pos = 0;
            ushort count = (ushort)exceptions.Count;
            ms.WriteByte((byte)(count >> 8));
            ms.WriteByte((byte)count);
            pos += 2;

            foreach (var ex in exceptions)
            {
                ms.WriteByte((byte)(ex.Offset >> 8));
                ms.WriteByte((byte)ex.Offset);
                ms.Write(ex.Hash, 0, 20);
                pos += 22;
            }

            while ((pos % 4) != 0)
            {
                ms.WriteByte(0);
                pos++;
            }

            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="propData"></param>
        /// <param name="propSize"></param>
        /// <returns></returns>
        private static byte[] CompressTableDataWia(byte[] data,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            byte[] propData,
            byte propSize)
        {
            if (compressionType == WiaRvzCompressionType.Purge)
                return PurgeCompressor.Compress(data, 0, data.Length);
            if (compressionType > WiaRvzCompressionType.Purge)
                return Compress(compressionType, data, 0, data.Length, compressionLevel, propData, propSize);

            return data;
        }

        #endregion

        #region Header Finalization

        /// <summary>
        ///
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="discHdr"></param>
        /// <param name="isRvz"></param>
        /// <param name="discType"></param>
        /// <param name="compressionType"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="chunkSize"></param>
        /// <param name="numPartitions"></param>
        /// <param name="partEntriesOffset"></param>
        /// <param name="partHash"></param>
        /// <param name="numRawData"></param>
        /// <param name="rawEntriesOffset"></param>
        /// <param name="rawEntriesSize"></param>
        /// <param name="numGroups"></param>
        /// <param name="groupEntriesOffset"></param>
        /// <param name="groupEntriesSize"></param>
        /// <param name="propData"></param>
        /// <param name="propSize"></param>
        /// <param name="isoSize"></param>
        /// <param name="fileSize"></param>
        private static void WriteWiaHeaders(Stream dest,
            byte[] discHdr,
            bool isRvz,
            WiaDiscType discType,
            WiaRvzCompressionType compressionType,
            int compressionLevel,
            uint chunkSize,
            uint numPartitions,
            ulong partEntriesOffset,
            byte[] partHash,
            uint numRawData,
            ulong rawEntriesOffset,
            uint rawEntriesSize,
            uint numGroups,
            ulong groupEntriesOffset,
            uint groupEntriesSize,
            byte[] propData,
            byte propSize,
            long isoSize,
            long fileSize)
        {
            var header2 = new WiaHeader2
            {
                DiscType = discType,
                CompressionType = compressionType,
                CompressionLevel = compressionLevel,
                ChunkSize = chunkSize,
                DiscHeader = discHdr,
                NumberOfPartitionEntries = numPartitions,
                PartitionEntrySize = PartitionEntrySize,
                PartitionEntriesOffset = partEntriesOffset,
                PartitionEntriesHash = partHash,
                NumberOfRawDataEntries = numRawData,
                RawDataEntriesOffset = rawEntriesOffset,
                RawDataEntriesSize = rawEntriesSize,
                NumberOfGroupEntries = numGroups,
                GroupEntriesOffset = groupEntriesOffset,
                GroupEntriesSize = groupEntriesSize,
                CompressorDataSize = propSize,
                CompressorData = propData,
            };

            byte[] h2Bytes = SerializeWiaHeader2(header2);
            byte[] h2Hash = ComputeSha1Wia(h2Bytes, 0, h2Bytes.Length);

            uint magic = isRvz ? RvzMagic : WiaMagic;
            uint ver = isRvz ? RvzVersion : WiaVersion;
            uint verC = isRvz ? RvzVersionWriteCompatible : WiaVersionWriteCompatible;

            var header1 = new WiaHeader1
            {
                Magic = magic,
                Version = ver,
                VersionCompatible = verC,
                Header2Size = Header2Size,
                Header2Hash = h2Hash,
                IsoFileSize = (ulong)isoSize,
                WiaFileSize = (ulong)fileSize,
                Header1Hash = new byte[20],
            };

            byte[] h1Bytes = SerializeWiaHeader1(header1);
            byte[] h1Hashable = new byte[h1Bytes.Length - 20];
            Array.Copy(h1Bytes, h1Hashable, h1Hashable.Length);
            header1.Header1Hash = ComputeSha1Wia(h1Hashable, 0, h1Hashable.Length);

            dest.Seek(0, SeekOrigin.Begin);
            dest.Write(SerializeWiaHeader1(header1), 0, Header1Size);
            dest.Write(h2Bytes, 0, h2Bytes.Length);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] SerializeWiaHeader1(WiaHeader1 obj)
        {
            using var ms = new MemoryStream();
            Serialization.Writers.WIA.WriteWiaHeader1(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] SerializeWiaHeader2(WiaHeader2 obj)
        {
            using var ms = new MemoryStream();
            Serialization.Writers.WIA.WriteWiaHeader2(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static byte[] ComputeSha1Wia(byte[] data, int offset, int count)
        {
            if (count == 0)
                return new byte[20];

            using var sha1 = new HashWrapper(HashType.SHA1);
            sha1.Process(data, offset, count);
            sha1.Terminate();

            return sha1.CurrentHashBytes ?? new byte[20];
        }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        /// TODO: Can this be replaced by <see cref="NintendoDiscExtensions.GetPlatform"/>
        private static Platform DetectWiaPlatform(byte[] header)
        {
            if (header.Length >= 0x1C)
            {
                uint wiiMagic = (uint)((header[0x18] << 24) | (header[0x19] << 16) | (header[0x1A] << 8) | header[0x1B]);
                if (wiiMagic == WiiMagicWord)
                    return Platform.Wii;
            }

            if (header.Length >= 4)
            {
                bool valid = true;
                for (int i = 0; i < 4; i++)
                {
                    char c = (char)header[i];
                    if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    return Platform.GameCube;
            }

            return Platform.Unknown;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="bytesWritten"></param>
        /// <param name="gi"></param>
        /// <param name="isRvz"></param>
        /// <param name="compressionType"></param>
        /// <returns></returns>
        private static uint WriteRawGroupData(Stream dest,
            ref long bytesWritten,
            GcGroupWorkEntry gi,
            bool isRvz,
            WiaRvzCompressionType compressionType)
        {
            if (gi.CompressedData is not null)
            {
                bool useC = !isRvz || gi.CompressedData.Length < gi.MainData!.Length;
                if (useC)
                {
                    dest.Write(gi.CompressedData, 0, gi.CompressedData.Length);
                    bytesWritten += gi.CompressedData.Length;
                    return isRvz
                        ? (uint)gi.CompressedData.Length | 0x80000000u
                        : (uint)gi.CompressedData.Length;
                }
            }

            if (compressionType == WiaRvzCompressionType.Purge && gi.MainData is not null)
            {
                byte[] comp = PurgeCompressor.Compress(gi.MainData, 0, gi.MainData.Length);
                dest.Write(comp, 0, comp.Length);
                bytesWritten += comp.Length;
                return (uint)comp.Length;
            }

            byte[] data = gi.MainData!;
            dest.Write(data, 0, data.Length);
            bytesWritten += data.Length;
            return (uint)data.Length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool IsAllSame(byte[] data, int length)
        {
            if (length == 0)
                return true;

            byte first = data[0];
            for (int i = 1; i < length; i++)
            {
                if (data[i] != first)
                    return false;
            }

            return true;
        }

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="aOff"></param>
        /// <param name="aLen"></param>
        /// <param name="b"></param>
        /// <param name="bOff"></param>
        /// <param name="bLen"></param>
        /// <returns></returns>
        private static byte[] ConcatBytes(byte[] a, int aOff, int aLen, byte[] b, int bOff, int bLen)
        {
            var r = new byte[aLen + bLen];

            if (aLen > 0)
                Array.Copy(a, aOff, r, 0, aLen);
            if (bLen > 0)
                Array.Copy(b, bOff, r, aLen, bLen);

            return r;
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytesWritten"></param>
        private static void PadToFourBytes(Stream stream, ref long bytesWritten)
        {
            int pad = (int)((-bytesWritten) & 3);
            if (pad > 0)
            {
                stream.Write(new byte[pad], 0, pad);
                bytesWritten += pad;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tablePos"></param>
        private static void PadTableToFourBytes(Stream stream, ref long tablePos)
        {
            long pad = (-tablePos) & 3;
            if (pad > 0)
            {
                stream.Write(new byte[pad], 0, (int)pad);
                tablePos += pad;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        private static long Align(long value, long align)
            => (value + align - 1) / align * align;

        #endregion

        #region Inner work types

        // Key: (byte sameByte, int bytesRead) — replaces ValueTuple<byte,int>
        private struct WiaDedupKey2 : IEquatable<WiaDedupKey2>
        {
            public byte SameByte;
            public int BytesRead;

            public WiaDedupKey2(byte sameByte, int bytesRead)
            {
                SameByte = sameByte;
                BytesRead = bytesRead;
            }

            public bool Equals(WiaDedupKey2 other) => SameByte == other.SameByte && BytesRead == other.BytesRead;
            public override bool Equals(object? obj) => obj is WiaDedupKey2 k && Equals(k);
            public override int GetHashCode() => (SameByte * 397) ^ BytesRead;
        }

        // Key: (ulong partKeyHash, byte sampleByte, int bytesRead) — replaces ValueTuple<ulong,byte,int>
        private struct WiaDedupKey3 : IEquatable<WiaDedupKey3>
        {
            public ulong PartKeyHash;
            public byte SampleByte;
            public int BytesRead;

            public WiaDedupKey3(ulong pkh, byte sb, int br)
            {
                PartKeyHash = pkh;
                SampleByte = sb;
                BytesRead = br;
            }

            public bool Equals(WiaDedupKey3 other) => PartKeyHash == other.PartKeyHash && SampleByte == other.SampleByte && BytesRead == other.BytesRead;
            public override bool Equals(object? obj) => obj is WiaDedupKey3 k && Equals(k);
            public override int GetHashCode() => (int)(PartKeyHash ^ (PartKeyHash >> 32)) ^ (SampleByte * 397) ^ BytesRead;
        }

        // Value: (uint offset, uint dataSize) — replaces ValueTuple<uint,uint>
        private struct WiaDedup2(uint offset, uint dataSize)
        {
            public uint Offset = offset;
            public uint DataSize = dataSize;
        }

        // Flat work item for Parallel.For — replaces (int b, int c) ValueTuple
        private struct WiaFlatWorkItem(int batchIndex, int chunkIndex)
        {
            public int BatchIndex = batchIndex;
            public int ChunkIndex = chunkIndex;
        }

        private sealed class GcGroupWorkEntry
        {
            public int BytesRead;
            public long SourceOffset;
            public bool IsAllSame;
            public byte SameByte;
            public bool IsDedupHit;
            public RvzGroupEntry DedupEntry = new();
            public byte[]? MainData;
            public uint RvzPackedSize;
            public byte[]? CompressedData = null;
        }

        private sealed class WiiChunkWork
        {
            public bool IsAllZeros;
            public bool IsDecDedupHit;
            public uint DecDedupOffset;
            public uint DecDedupDataSize;
            public byte[] ExceptionListBytes = [];
            public int UnpaddedExLen;
            public byte[] MainDataBytes = [];
            public uint RvzPackedSize;
            public byte[]? CompressedData = null;
            public bool DecAllSame;
            public WiaDedupKey3 DecDedupKey;
        }

        private sealed class WiiBatchItem
        {
            public int BytesRead;
            public long SrcOffset;
            public bool IsInterDedupHit;
            public WiaDedup2 DedupResult;
            public bool EncAllSame;
            public WiaDedupKey3 DedupKey;
            public byte[]? DecryptedAll;
            public List<HashExceptionEntry>? AllExceptions;
            public int NumChunks;
            public WiiChunkWork[]? PartWork;
        }

        private sealed class WiiPartInfo
        {
            public ulong PartitionOffset;
            public byte[] TitleKey = [];
            public ulong DataOffset;
            public ulong DataSize;
            public ulong DataStart;
            public ulong DataEnd;
            public uint FirstGroupIndex;
            public uint NumberOfGroups;
        }

        private sealed class DiscRegionEntry
        {
            public bool IsPartition;
            public long Offset;
            public WiiPartInfo? PartitionInfo;
            public RawDataEntry? RawInfo;
        }

        /// <summary>
        /// Converts the WIA/RVZ image to a flat ISO file at <paramref name="outputPath"/>.
        /// Re-encrypts Wii partition groups on the fly via the virtual stream.
        /// </summary>
        /// <param name="outputPath">Destination ISO file path.</param>
        /// <returns>true on success; false on any failure.</returns>
        public bool DumpIso(string? outputPath)
        {
            if (string.IsNullOrEmpty(outputPath))
                return false;

            long isoSize = (long)IsoFileSize;
            if (isoSize <= 0)
                return false;

            try
            {
                using var vStream = new WiaVirtualStream(this);
                using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

                const int BufSize = 2 * 1024 * 1024; // 2 MiB — aligns to WIA default / RVZ max chunk size
                byte[] buf = new byte[BufSize];
                long remaining = isoSize;

                while (remaining > 0)
                {
                    int toRead = (int)Math.Min(BufSize, remaining);
                    int read = vStream.Read(buf, 0, toRead);
                    if (read <= 0)
                        break;

                    fs.Write(buf, 0, read);
                    remaining -= read;
                }

                return remaining == 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
