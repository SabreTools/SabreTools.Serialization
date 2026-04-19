using System;
using System.Collections.Generic;
using System.IO;
#if !NET20
using System.Security.Cryptography;
#endif
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Threading.Tasks;
#endif
using NdConstants = SabreTools.Data.Models.NintendoDisc.Constants;
using WiaConst = SabreTools.Data.Models.WIA.Constants;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;

namespace SabreTools.Wrappers
{
    public partial class WIA : IWritable
    {
        // -----------------------------------------------------------------------
        // Public entry points
        // -----------------------------------------------------------------------

        /// <summary>
        /// Compress a <see cref="NintendoDisc"/> wrapper to a WIA or RVZ file.
        /// </summary>
        public static bool ConvertFromDisc(NintendoDisc source, string outputPath,
            bool isRvz = false,
            WiaRvzCompressionType compressionType = WiaRvzCompressionType.None,
            int compressionLevel = 5,
            uint chunkSize = WiaConst.DefaultChunkSize)
        {
            if (source is null)
                return false;
            if (string.IsNullOrEmpty(outputPath))
                return false;
            if (!isRvz && chunkSize != WiaConst.DefaultChunkSize)
                return false;
            if (isRvz && compressionType == WiaRvzCompressionType.Purge)
                return false;

            try
            {
                using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                return WriteWiaRvz(source, fs, isRvz, compressionType,
                    Math.Max(1, Math.Min(22, compressionLevel)), chunkSize);
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                string ext = (Model?.IsRvz == true) ? ".rvz" : ".wia";
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ext)
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            if (Model?.Header1 is null || Model?.Header2 is null)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            var writer = new Serialization.Writers.WIA { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }

        // -----------------------------------------------------------------------
        // Core pipeline
        // -----------------------------------------------------------------------

        private static bool WriteWiaRvz(NintendoDisc source, Stream dest,
            bool isRvz, WiaRvzCompressionType compressionType,
            int compressionLevel, uint chunkSize)
        {
            long isoSize = source.DataLength;
            if (isoSize <= 0)
                return false;

            byte[]? discHdr = source.ReadData(0, WiaConst.DiscHeaderStoredSize);
            if (discHdr is null)
                return false;

            Platform platform = DetectWiaPlatform(discHdr);
            if (platform == Platform.Unknown)
                return false;

            if (platform == Platform.Wii)
                return WriteWii(source, dest, isRvz, compressionType, compressionLevel, chunkSize, isoSize, discHdr);

            return WriteGameCube(source, dest, isRvz, compressionType, compressionLevel, chunkSize, isoSize, discHdr);
        }

        // -----------------------------------------------------------------------
        // GameCube path
        // -----------------------------------------------------------------------

        private static bool WriteGameCube(NintendoDisc source, Stream dest,
            bool isRvz, WiaRvzCompressionType compressionType,
            int compressionLevel, uint chunkSize,
            long isoSize, byte[] discHdr)
        {
            const long rawDataStart = WiaConst.DiscHeaderStoredSize;
            long rawDataSize = isoSize - rawDataStart;
            if (rawDataSize <= 0)
                return false;

            uint numGroups = (uint)((rawDataSize + chunkSize - 1) / chunkSize);

            int groupEntrySize = isRvz ? WiaConst.RvzGroupEntrySize : WiaConst.WiaGroupEntrySize;

            long headersBound = AlignWia(
                WiaConst.Header1Size + WiaConst.Header2Size +
                WiaConst.RawDataEntrySize + 0x100 +
                (numGroups * groupEntrySize),
                NdConstants.WiiBlockSize);

            dest.Write(new byte[headersBound], 0, (int)headersBound);
            long bytesWritten = headersBound;

            var groupEntries = new WiaRvzGroupEntry[numGroups];
            var rawDedupMap  = new Dictionary<WiaDedupKey2, WiaRvzGroupEntry>();
            GcFst? gcFst     = isRvz ? BuildGcFst(source) : null;

            WiaRvzCompressionHelper.GetCompressorData(compressionType, compressionLevel,
                out byte[] propData, out byte propSize);

            uint groupIdx = 0;
            long srcOff   = rawDataStart;
            long remaining = rawDataSize;

#if !NET20
            int batchSize = Math.Max(Environment.ProcessorCount * 4, 64);
#else
            int batchSize = 64;
#endif

            while (remaining > 0)
            {
                int thisBatch  = (int)Math.Min(batchSize, (remaining + chunkSize - 1) / chunkSize);
                var work       = new GcGroupWorkEntry[thisBatch];
                int actualBatch = 0;

                for (int w = 0; w < thisBatch && remaining > 0; w++)
                {
                    int toRead = (int)Math.Min(chunkSize, remaining);
                    byte[]? raw = source.ReadData(srcOff, toRead);
                    if (raw is null) break;

                    var gi = work[w] = new GcGroupWorkEntry
                    {
                        BytesRead    = toRead,
                        SourceOffset = srcOff,
                    };

                    srcOff    += toRead;
                    remaining -= toRead;
                    actualBatch++;

                    gi.IsAllSame = IsAllSameWia(raw, toRead);
                    gi.SameByte  = raw[0];

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
                        byte[]? packed = RvzPackEncoder.Pack(raw, 0, toRead, srcOff - toRead,
                            out gi.RvzPackedSize, gcFst);
                        gi.MainData = packed ?? raw;
                        if (packed is null) gi.RvzPackedSize = 0;
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
                        if (gi.MainData != null && !gi.IsDedupHit)
                            gi.CompressedData = WiaRvzCompressionHelper.Compress(ct, gi.MainData, 0, gi.MainData.Length, cl, pd, ps);
                    });
                }
#endif

                for (int w = 0; w < actualBatch; w++)
                {
                    uint idx = groupIdx + (uint)w;
                    var gi = work[w];

                    if (gi.IsDedupHit)
                    {
                        groupEntries[idx] = gi.DedupEntry;
                    }
                    else if (gi.IsAllSame && gi.SameByte == 0)
                    {
                        var dk = new WiaDedupKey2(0, gi.BytesRead);
                        if (!rawDedupMap.TryGetValue(dk, out var ze))
                        {
                            ze = new WiaRvzGroupEntry((uint)(bytesWritten >> 2), 0, 0);
                            rawDedupMap[dk] = ze;
                        }

                        groupEntries[idx] = ze;
                    }
                    else if (gi.MainData != null)
                    {
                        uint groupOff  = (uint)(bytesWritten >> 2);
                        uint storedSz  = WriteRawGroupData(dest, ref bytesWritten, gi,
                            isRvz, compressionType, compressionLevel, propData, propSize);
                        PadTo4Wia(dest, ref bytesWritten);

                        var entry = new WiaRvzGroupEntry(groupOff, storedSz, gi.RvzPackedSize);
                        groupEntries[idx] = entry;
                        if (gi.IsAllSame && gi.SameByte != 0)
                            rawDedupMap[new WiaDedupKey2(gi.SameByte, gi.BytesRead)] = entry;
                    }
                }

                groupIdx += (uint)actualBatch;
            }

            // Write tables
            dest.Seek(WiaConst.Header1Size + WiaConst.Header2Size, SeekOrigin.Begin);
            long tablePos = WiaConst.Header1Size + WiaConst.Header2Size;

            ulong rawEntriesOffset = (ulong)tablePos;
            var rawEntry = new WiaRawDataEntry
            {
                DataOffset     = WiaConst.DiscHeaderStoredSize,
                DataSize       = (ulong)rawDataSize,
                GroupIndex     = 0,
                NumberOfGroups = numGroups,
            };
            byte[] rawEntryBytes    = SerializeRawDataEntry(rawEntry);
            byte[] rawEntryWritten  = CompressTableDataWia(rawEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(rawEntryWritten, 0, rawEntryWritten.Length);
            tablePos += rawEntryWritten.Length;
            PadTableTo4Wia(dest, ref tablePos);

            ulong groupEntriesOffset = (ulong)tablePos;
            byte[] groupEntryBytes   = SerializeGroupEntries(groupEntries, numGroups, isRvz);
            byte[] groupEntryWritten = CompressTableDataWia(groupEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(groupEntryWritten, 0, groupEntryWritten.Length);
            tablePos += groupEntryWritten.Length;

#if !NET20
            WriteWiaHeaders(dest, discHdr, isRvz, WiaDiscType.GameCube, compressionType, compressionLevel, chunkSize,
                0, (ulong)tablePos, new byte[20], // no partition entries
                1u, rawEntriesOffset, (uint)rawEntryWritten.Length,
                numGroups, groupEntriesOffset, (uint)groupEntryWritten.Length,
                propData, propSize, isoSize, bytesWritten);
#endif
            dest.Flush();
            return true;
        }

        // -----------------------------------------------------------------------
        // Wii path
        // -----------------------------------------------------------------------

        private static bool WriteWii(NintendoDisc source, Stream dest,
            bool isRvz, WiaRvzCompressionType compressionType,
            int compressionLevel, uint chunkSize,
            long isoSize, byte[] discHdr)
        {
#if NET20
            return false; // AES not available
#else
            var partitions = ReadWiiPartitions(source, isoSize);
            if (partitions is null) return false;

            var rawRegions = BuildRawRegions(source, partitions, isoSize);

            WiaRvzCompressionHelper.GetCompressorData(compressionType, compressionLevel,
                out byte[] propData, out byte propSize);

            int groupEntrySize = isRvz ? WiaConst.RvzGroupEntrySize : WiaConst.WiaGroupEntrySize;
            uint totalGroups   = CalcTotalGroups(partitions, rawRegions, chunkSize);

            long headersBound = AlignWia(
                WiaConst.Header1Size + WiaConst.Header2Size +
                (partitions.Count * WiaConst.PartitionEntrySize) +
                (rawRegions.Count * WiaConst.RawDataEntrySize) + 0x100 +
                (totalGroups * groupEntrySize),
                NdConstants.WiiBlockSize);

            dest.Write(new byte[headersBound], 0, (int)headersBound);
            long bytesWritten = headersBound;

            var allGroups     = new List<WiaRvzGroupEntry>();
            uint currentGrpIdx = 0;
            uint lastValidOff  = 0;

            var dedupMap     = new Dictionary<WiaDedupKey3, WiaDedup2>();
            var decDedupMap  = new Dictionary<WiaDedupKey3, WiaDedup2>();
            var rawDedupMap  = new Dictionary<WiaDedupKey2, WiaRvzGroupEntry>();
            var wiaZeroDedup = new Dictionary<ulong, uint>();

            var regions = BuildDiscRegions(partitions, rawRegions);
            foreach (var region in regions)
            {
                if (region.IsPartition)
                {
                    ProcessWiiPartition(source, dest, region.PartitionInfo!,
                        ref bytesWritten, allGroups, ref currentGrpIdx, ref lastValidOff,
                        dedupMap, decDedupMap, wiaZeroDedup,
                        isRvz, compressionType, compressionLevel, chunkSize, propData, propSize);
                }
                else
                {
                    ProcessRawRegion(source, dest, region.RawInfo!,
                        ref bytesWritten, allGroups, ref currentGrpIdx, ref lastValidOff,
                        rawDedupMap, isRvz, compressionType, compressionLevel, chunkSize, propData, propSize);
                }
            }

            // Write tables
            dest.Seek(WiaConst.Header1Size + WiaConst.Header2Size, SeekOrigin.Begin);
            long tablePos = WiaConst.Header1Size + WiaConst.Header2Size;

            ulong partEntriesOffset = (ulong)tablePos;
            byte[] partEntriesBytes = SerializePartitionEntries(dest, partitions);
            tablePos += partEntriesBytes.Length;
            PadTableTo4Wia(dest, ref tablePos);

            ulong rawEntriesOffset = (ulong)tablePos;
            byte[] rawEntryBytes   = SerializeRawDataEntries(rawRegions);
            byte[] rawEntryWritten = CompressTableDataWia(rawEntryBytes, compressionType, compressionLevel, propData, propSize);
            dest.Write(rawEntryWritten, 0, rawEntryWritten.Length);
            tablePos += rawEntryWritten.Length;
            PadTableTo4Wia(dest, ref tablePos);

            ulong groupEntriesOffset = (ulong)tablePos;
            using (var gms = new MemoryStream())
            {
                foreach (var e in allGroups)
                    WriteGroupEntryWia(gms, e, isRvz);
                byte[] gBytes   = gms.ToArray();
                byte[] gWritten = CompressTableDataWia(gBytes, compressionType, compressionLevel, propData, propSize);
                dest.Write(gWritten, 0, gWritten.Length);
                tablePos += gWritten.Length;

                byte[] partHashData = ComputeSha1Wia(partEntriesBytes, 0, partEntriesBytes.Length);
                WriteWiaHeaders(dest, discHdr, isRvz, WiaDiscType.Wii, compressionType, compressionLevel, chunkSize,
                    (uint)partitions.Count, partEntriesOffset, partHashData,
                    (uint)rawRegions.Count, rawEntriesOffset, (uint)rawEntryWritten.Length,
                    (uint)allGroups.Count, groupEntriesOffset, (uint)gWritten.Length,
                    propData, propSize, isoSize, bytesWritten);
            }

            dest.Flush();
            return true;
#endif
        }

        // -----------------------------------------------------------------------
        // Wii partition processing
        // -----------------------------------------------------------------------

#if !NET20
        private static void ProcessWiiPartition(NintendoDisc source, Stream dest,
            WiiPartInfo part, ref long bytesWritten,
            List<WiaRvzGroupEntry> groupEntries, ref uint currentGrpIdx,
            ref uint lastValidOff,
            Dictionary<WiaDedupKey3, WiaDedup2> dedupMap,
            Dictionary<WiaDedupKey3, WiaDedup2> decDedupMap,
            Dictionary<ulong, uint> wiaZeroDedup,
            bool isRvz, WiaRvzCompressionType compressionType,
            int compressionLevel, uint chunkSize,
            byte[] propData, byte propSize)
        {
            long remaining   = (long)part.DataSize;
            long srcOff      = (long)part.DataStart;
            ulong partKeyHash = BitConverter.ToUInt64(part.TitleKey, 0)
                              ^ BitConverter.ToUInt64(part.TitleKey, 8);

            part.FirstGroupIndex = currentGrpIdx;

            int blocksPerChunk  = (int)chunkSize / NdConstants.WiiBlockSize;
            int chunksPerGroup  = NdConstants.WiiBlocksPerGroup / blocksPerChunk;
            int wiiGroupSize    = NdConstants.WiiGroupSize;

            int outerBatch = (chunksPerGroup == 1)
                ? Math.Max(Environment.ProcessorCount * 2, 16) : 1;

            var batchItems = new WiiBatchItem[outerBatch];
            var flatWork   = new List<WiaFlatWorkItem>(outerBatch);
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

                    srcOff    += toRead;
                    remaining -= toRead;
                    actualBatch++;

                    bool encAllSame = (chunksPerGroup == 1) && IsAllSameWia(encGroup, toRead);
                    item.EncAllSame = encAllSame;
                    item.DedupKey   = new WiaDedupKey3(partKeyHash, encGroup[0], toRead);

                    if (encAllSame && dedupMap.TryGetValue(item.DedupKey, out var reused))
                    {
                        item.IsInterDedupHit = true;
                        item.DedupResult     = reused;
                        regionDecOff += (long)(toRead / NdConstants.WiiBlockSize) * NdConstants.WiiBlockDataSize;
                        continue;
                    }

                    int numBlocks  = toRead / NdConstants.WiiBlockSize;
                    item.NumChunks = (numBlocks + blocksPerChunk - 1) / blocksPerChunk;

                    item.DecryptedAll = DecryptWiiGroup(encGroup, toRead, part.TitleKey);
                    item.AllExceptions = GenerateHashExceptions(encGroup, toRead,
                        item.DecryptedAll, part.TitleKey, numBlocks);

                    item.PartWork = new WiiChunkWork[item.NumChunks];
                    for (int c = 0; c < item.NumChunks; c++)
                    {
                        int cBlockStart = c * blocksPerChunk;
                        int cBlockEnd   = Math.Min(cBlockStart + blocksPerChunk, numBlocks);
                        int actualBlocks = cBlockEnd - cBlockStart;
                        int decOff      = cBlockStart * NdConstants.WiiBlockDataSize;
                        int decLen      = actualBlocks * NdConstants.WiiBlockDataSize;

                        byte[] procData = new byte[decLen];
                        if (item.DecryptedAll != null && decLen > 0)
                            Array.Copy(item.DecryptedAll, decOff, procData, 0, decLen);

                        var chunkEx = new List<HashExceptionEntry>();
                        if (item.AllExceptions != null)
                        {
                            foreach (var ex in item.AllExceptions)
                            {
                                int exBlock = ex.Offset / NdConstants.WiiBlockHeaderSize;
                                if (exBlock >= cBlockStart && exBlock < cBlockEnd)
                                {
                                    int localBlock = exBlock - cBlockStart;
                                    ushort localOff = (ushort)((localBlock * NdConstants.WiiBlockHeaderSize)
                                        + (ex.Offset % NdConstants.WiiBlockHeaderSize));
                                    chunkEx.Add(new HashExceptionEntry { Offset = localOff, Hash = ex.Hash });
                                }
                            }
                        }

                        bool isAllZeros = !isRvz && chunksPerGroup == 1
                            && chunkEx.Count == 0 && procData.Length > 0
                            && IsAllSameWia(procData, procData.Length) && procData[0] == 0;

                        bool decAllSame = !isRvz && !isAllZeros && chunksPerGroup == 1
                            && chunkEx.Count == 0 && procData.Length > 0
                            && IsAllSameWia(procData, procData.Length);

                        var decDedupKey = new WiaDedupKey3(partKeyHash,
                            procData.Length > 0 ? procData[0] : (byte)0, procData.Length);

                        var pw = item.PartWork[c] = new WiiChunkWork
                        {
                            IsAllZeros  = isAllZeros,
                            DecAllSame  = decAllSame,
                            DecDedupKey = decDedupKey,
                        };

                        if (isAllZeros) continue;

                        if (decAllSame && decDedupMap.TryGetValue(decDedupKey, out var decReused))
                        {
                            pw.IsDecDedupHit    = true;
                            pw.DecDedupOffset   = decReused.Offset;
                            pw.DecDedupDataSize = decReused.DataSize;
                            continue;
                        }

                        byte[] exListBytes = BuildExceptionList(chunkEx);
                        int unpaddedExLen  = 2 + (chunkEx.Count * 22);

                        byte[] mainData;
                        uint rvzPackedSize = 0;
                        if (isRvz)
                        {
                            long baseDecOff = regionDecOff + ((long)c * (blocksPerChunk * NdConstants.WiiBlockDataSize));
                            byte[]? packed = RvzPackEncoder.Pack(procData, 0, procData.Length,
                                baseDecOff, out rvzPackedSize);
                            mainData = packed ?? procData;
                            if (packed is null) rvzPackedSize = 0;
                        }
                        else
                        {
                            mainData = procData;
                        }

                        pw.ExceptionListBytes = exListBytes;
                        pw.UnpaddedExLen      = unpaddedExLen;
                        pw.MainDataBytes      = mainData;
                        pw.RvzPackedSize      = rvzPackedSize;

                        if (compressionType != WiaRvzCompressionType.None)
                            flatWork.Add(new WiaFlatWorkItem(b, c));
                    }

                    regionDecOff += (long)numBlocks * NdConstants.WiiBlockDataSize;
                }

                if (actualBatch == 0) break;

                #if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                // Phase 2: compress
                if (flatWork.Count > 0)
                {
                    WiaRvzCompressionType ct = compressionType;
                    int cl = compressionLevel;
                    byte[] pd = propData;
                    byte ps = propSize;
                    Parallel.For(0, flatWork.Count, idx =>
                    {
                        var fw = flatWork[idx];
                        var pw = batchItems[fw.BatchIndex].PartWork![fw.ChunkIndex];
                        if (ct > WiaRvzCompressionType.Purge)
                        {
                            byte[] toCompress = ConcatBytesWia(
                                pw.ExceptionListBytes, 0, pw.UnpaddedExLen,
                                pw.MainDataBytes, 0, pw.MainDataBytes.Length);
                            pw.CompressedData = WiaRvzCompressionHelper.Compress(ct, toCompress, 0, toCompress.Length, cl, pd, ps);
                        }
                        else if (ct == WiaRvzCompressionType.Purge)
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
                        groupEntries.Add(new WiaRvzGroupEntry(
                            item.DedupResult.Offset,
                            item.DedupResult.DataSize,
                            0));
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

                            groupEntries.Add(new WiaRvzGroupEntry(firstOff, 0, 0));
                        }
                        else if (pw.IsDecDedupHit)
                        {
                            groupEntries.Add(new WiaRvzGroupEntry(
                                pw.DecDedupOffset,
                                pw.DecDedupDataSize,
                                0));
                        }
                        else
                        {
                            uint groupOff  = (uint)(bytesWritten >> 2);
                            lastValidOff   = groupOff;
                            uint storedSz  = WriteWiiChunkData(dest, ref bytesWritten, pw, isRvz, compressionType);

                            groupEntries.Add(new WiaRvzGroupEntry(
                                groupOff, storedSz, pw.RvzPackedSize));

                            if (item.EncAllSame && c == 0)
                                dedupMap[item.DedupKey] = new WiaDedup2(groupOff, storedSz);
                            if (pw.DecAllSame && c == 0)
                                decDedupMap[pw.DecDedupKey] = new WiaDedup2(groupOff, storedSz);

                            PadTo4Wia(dest, ref bytesWritten);
                        }
                    }

                    currentGrpIdx++;
                }
            }

            part.NumberOfGroups = currentGrpIdx - part.FirstGroupIndex;
        }

        private static uint WriteWiiChunkData(Stream dest, ref long bytesWritten,
            WiiChunkWork pw, bool isRvz, WiaRvzCompressionType compressionType)
        {
            if (pw.CompressedData != null)
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
#endif

        // -----------------------------------------------------------------------
        // Raw region processing
        // -----------------------------------------------------------------------

#if !NET20
        private static void ProcessRawRegion(NintendoDisc source, Stream dest,
            RawRegionInfo raw, ref long bytesWritten,
            List<WiaRvzGroupEntry> groupEntries, ref uint currentGrpIdx,
            ref uint lastValidOff,
            Dictionary<WiaDedupKey2, WiaRvzGroupEntry> rawDedupMap,
            bool isRvz, WiaRvzCompressionType compressionType,
            int compressionLevel, uint chunkSize,
            byte[] propData, byte propSize)
        {
            raw.FirstGroupIndex = currentGrpIdx;

            long skip      = (long)raw.Offset % NdConstants.WiiBlockSize;
            long adjOffset = (long)raw.Offset - skip;
            long remaining = (long)raw.Size + skip;
            long srcOff    = adjOffset;

            while (remaining > 0)
            {
                int toRead = (int)Math.Min(chunkSize, remaining);
                byte[]? data = source.ReadData(srcOff, toRead);
                if (data is null) break;

                bool isAllSame = IsAllSameWia(data, toRead);
                byte sameByte  = data[0];

                if (isAllSame)
                {
                    var dk = new WiaDedupKey2(sameByte, toRead);
                    if (rawDedupMap.TryGetValue(dk, out var cached))
                    {
                        groupEntries.Add(cached);
                        currentGrpIdx++;
                        srcOff    += toRead;
                        remaining -= toRead;
                        continue;
                    }

                    if (sameByte == 0)
                    {
                        var ze = new WiaRvzGroupEntry((uint)(bytesWritten >> 2), 0, 0);
                        rawDedupMap[dk] = ze;
                        groupEntries.Add(ze);
                        currentGrpIdx++;
                        srcOff    += toRead;
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
                    if (packed is null) rvzPackedSize = 0;
                }
                else
                {
                    mainData = data;
                }

                byte[]? compressed = null;
                if (compressionType > WiaRvzCompressionType.Purge)
                {
                    byte[] c2 = WiaRvzCompressionHelper.Compress(compressionType, mainData, 0,
                        mainData.Length, compressionLevel, propData, propSize);
                    if (!isRvz || c2.Length < mainData.Length)
                        compressed = c2;
                }
                else if (compressionType == WiaRvzCompressionType.Purge)
                {
                    compressed = PurgeCompressor.Compress(mainData, 0, mainData.Length);
                }

                uint groupOff = (uint)(bytesWritten >> 2);
                lastValidOff  = groupOff;
                uint storedSz;

                if (compressed != null)
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

                PadTo4Wia(dest, ref bytesWritten);

                var entry = new WiaRvzGroupEntry(groupOff, storedSz, rvzPackedSize);
                groupEntries.Add(entry);
                if (isAllSame && sameByte != 0)
                    rawDedupMap[new WiaDedupKey2(sameByte, toRead)] = entry;

                currentGrpIdx++;
                srcOff    += toRead;
                remaining -= toRead;
            }

            raw.NumberOfGroups = currentGrpIdx - raw.FirstGroupIndex;
        }
#endif

        // -----------------------------------------------------------------------
        // Wii crypto helpers
        // -----------------------------------------------------------------------

#if !NET20
        private static byte[]? DecryptWiiGroup(byte[] encGroup, int bytesRead, byte[] titleKey)
        {
            int numBlocks = bytesRead / NdConstants.WiiBlockSize;
            var result    = new byte[numBlocks * NdConstants.WiiBlockDataSize];

            for (int i = 0; i < numBlocks; i++)
            {
                int off = i * NdConstants.WiiBlockSize;
                byte[] iv = new byte[16];
                Array.Copy(encGroup, off + 0x3D0, iv, 0, 16);

                byte[] encData = new byte[NdConstants.WiiBlockDataSize];
                Array.Copy(encGroup, off + NdConstants.WiiBlockHeaderSize, encData, 0, NdConstants.WiiBlockDataSize);

                byte[]? dec = NintendoDisc.DecryptBlock(encData, titleKey, iv);
                if (dec is null) return null;

                Array.Copy(dec, 0, result, i * NdConstants.WiiBlockDataSize, NdConstants.WiiBlockDataSize);
            }

            return result;
        }

        private static List<HashExceptionEntry> GenerateHashExceptions(
            byte[] encGroup, int bytesRead, byte[]? decryptedData, byte[] titleKey, int numBlocks)
        {
            var exceptions = new List<HashExceptionEntry>();
            if (decryptedData is null) return exceptions;

            // Re-encrypt the decrypted data to obtain recomputed hashes
            byte[] reEncGroup = EncryptWiiGroup(decryptedData, titleKey, numBlocks);

            for (int blockIdx = 0; blockIdx < numBlocks; blockIdx++)
            {
                int blockOff = blockIdx * NdConstants.WiiBlockSize;

                byte[] encHashBlock = new byte[NdConstants.WiiBlockHeaderSize];
                Array.Copy(encGroup, blockOff, encHashBlock, 0, NdConstants.WiiBlockHeaderSize);

                using var aes = Aes.Create();
                aes.Key     = titleKey;
                aes.IV      = new byte[16];
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                byte[] origHash;
                using (var dec = aes.CreateDecryptor())
                    origHash = dec.TransformFinalBlock(encHashBlock, 0, NdConstants.WiiBlockHeaderSize);

                byte[] reEncHashBlock = new byte[NdConstants.WiiBlockHeaderSize];
                Array.Copy(reEncGroup, blockOff, reEncHashBlock, 0, NdConstants.WiiBlockHeaderSize);
                byte[] recompHash;
                using (var dec = aes.CreateDecryptor())
                    recompHash = dec.TransformFinalBlock(reEncHashBlock, 0, NdConstants.WiiBlockHeaderSize);

                for (int off = 0; off < NdConstants.WiiBlockHeaderSize; off += 20)
                {
                    bool match = true;
                    for (int j = 0; j < 20 && (off + j) < NdConstants.WiiBlockHeaderSize; j++)
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
                        Array.Copy(origHash, off, hash, 0, Math.Min(20, NdConstants.WiiBlockHeaderSize - off));
                        exceptions.Add(new HashExceptionEntry
                        {
                            Offset = (ushort)((blockIdx * NdConstants.WiiBlockHeaderSize) + off),
                            Hash   = hash,
                        });
                    }
                }
            }

            return exceptions;
        }

        private static List<WiiPartInfo>? ReadWiiPartitions(NintendoDisc source, long isoSize)
        {
            var result = new List<WiiPartInfo>();

            for (int group = 0; group < NdConstants.WiiPartitionGroupCount; group++)
            {
                byte[]? gEntry = source.ReadData(NdConstants.WiiPartitionTableAddress + (group * 8), 8);
                if (gEntry is null) continue;

                uint count  = ReadBE32Wia(gEntry, 0);
                uint offset = ReadBE32Wia(gEntry, 4) << 2;
                if (count == 0 || offset == 0) continue;

                for (int i = 0; i < (int)count; i++)
                {
                    byte[]? pEntry = source.ReadData(offset + (i * 8), 8);
                    if (pEntry is null) continue;

                    long partOff = (long)ReadBE32Wia(pEntry, 0) << 2;

                    byte[]? sigType = source.ReadData(partOff, 4);
                    if (sigType is null || ReadBE32Wia(sigType, 0) != 0x10001U) continue;

                    byte[]? hdr = source.ReadData(partOff, 0x2C0);
                    if (hdr is null) continue;

                    byte[] encKey = new byte[16];
                    Array.Copy(hdr, 0x1BF, encKey, 0, 16);
                    byte[] titleId = new byte[8];
                    Array.Copy(hdr, 0x1DC, titleId, 0, 8);
                    byte ckIdx = hdr[0x1F1];

                    byte[]? titleKey = NintendoDisc.DecryptTitleKey(encKey, titleId, ckIdx);
                    if (titleKey is null) continue;

                    ulong dataOff  = (ulong)ReadBE32Wia(hdr, 0x2B8) << 2;
                    ulong dataSize = (ulong)ReadBE32Wia(hdr, 0x2BC) << 2;

                    result.Add(new WiiPartInfo
                    {
                        PartitionOffset = (ulong)partOff,
                        TitleKey        = titleKey,
                        DataOffset      = dataOff,
                        DataSize        = dataSize,
                        DataStart       = (ulong)partOff + dataOff,
                        DataEnd         = (ulong)partOff + dataOff + dataSize,
                    });
                }
            }

            return result.Count == 0 ? null : result;
        }

        private static List<RawRegionInfo> BuildRawRegions(NintendoDisc source,
            List<WiiPartInfo> partitions, long isoSize)
        {
            var regions = new List<RawRegionInfo>();
            partitions.Sort((a, b) => a.PartitionOffset.CompareTo(b.PartitionOffset));

            ulong cur = WiaConst.DiscHeaderStoredSize;
            foreach (var p in partitions)
            {
                if (cur < p.PartitionOffset)
                    regions.Add(new RawRegionInfo { Offset = cur, Size = p.PartitionOffset - cur });
                regions.Add(new RawRegionInfo { Offset = p.PartitionOffset, Size = p.DataOffset });
                cur = p.DataEnd;
            }

            if (cur < (ulong)isoSize)
                regions.Add(new RawRegionInfo { Offset = cur, Size = (ulong)isoSize - cur });

            return regions;
        }

        private static uint CalcTotalGroups(List<WiiPartInfo> partitions,
            List<RawRegionInfo> rawRegions, uint chunkSize)
        {
            uint total = 0;
            foreach (var p in partitions)
                total += (uint)((p.DataSize + chunkSize - 1) / chunkSize);
            foreach (var r in rawRegions)
                total += (uint)((r.Size + chunkSize - 1) / chunkSize);
            return total;
        }

        private static List<DiscRegionEntry> BuildDiscRegions(List<WiiPartInfo> partitions,
            List<RawRegionInfo> rawRegions)
        {
            var result = new List<DiscRegionEntry>();
            foreach (var p in partitions)
                result.Add(new DiscRegionEntry { IsPartition = true, Offset = (long)p.DataStart, PartitionInfo = p });
            foreach (var r in rawRegions)
                result.Add(new DiscRegionEntry { IsPartition = false, Offset = (long)r.Offset, RawInfo = r });
            result.Sort((a, b) => a.Offset.CompareTo(b.Offset));
            return result;
        }
#endif

        // -----------------------------------------------------------------------
        // GcFst helper
        // -----------------------------------------------------------------------

        private static GcFst? BuildGcFst(NintendoDisc source)
        {
            byte[]? hdr = source.ReadData(0x420, 12);
            if (hdr is null) return null;

            uint fstOff  = ReadBE32Wia(hdr, 4);
            uint fstSize = ReadBE32Wia(hdr, 8);
            if (fstOff == 0 || fstSize == 0) return null;

            byte[]? fstData = source.ReadData(fstOff, (int)fstSize);
            if (fstData is null) return null;

            return GcFst.TryParse(fstData, offsetShift: 0);
        }

        // -----------------------------------------------------------------------
        // Serialisation
        // -----------------------------------------------------------------------

        private static byte[] SerializeRawDataEntry(WiaRawDataEntry e)
        {
            using var ms = new MemoryStream();
            WriteBE64Wia(ms, e.DataOffset);
            WriteBE64Wia(ms, e.DataSize);
            WriteBE32Wia(ms, e.GroupIndex);
            WriteBE32Wia(ms, e.NumberOfGroups);
            return ms.ToArray();
        }

        private static byte[] SerializeGroupEntries(WiaRvzGroupEntry[] entries, uint count, bool isRvz)
        {
            using var ms = new MemoryStream();
            for (uint i = 0; i < count && i < (uint)entries.Length; i++)
                WriteGroupEntryWia(ms, entries[i], isRvz);
            return ms.ToArray();
        }

        private static void WriteGroupEntryWia(Stream s, WiaRvzGroupEntry e, bool isRvz)
        {
            WriteBE32Wia(s, e.DataOffset);
            WriteBE32Wia(s, e.DataSize);
            if (isRvz) WriteBE32Wia(s, e.RvzPackedSize);
        }

#if !NET20
        private static byte[] SerializePartitionEntries(Stream dest, List<WiiPartInfo> partitions)
        {
            using var ms = new MemoryStream();
            foreach (var p in partitions)
            {
                // Write 16-byte key
                ms.Write(p.TitleKey, 0, 16);
                dest.Write(p.TitleKey, 0, 16);

                // DataEntry0: all of the partition
                WriteBE32Wia(ms, (uint)(p.DataStart / NdConstants.WiiBlockSize));
                WriteBE32Wia(ms, (uint)(p.DataSize / NdConstants.WiiBlockSize));
                WriteBE32Wia(ms, p.FirstGroupIndex);
                WriteBE32Wia(ms, p.NumberOfGroups);
                WriteBE32Wia(dest, (uint)(p.DataStart / NdConstants.WiiBlockSize));
                WriteBE32Wia(dest, (uint)(p.DataSize / NdConstants.WiiBlockSize));
                WriteBE32Wia(dest, p.FirstGroupIndex);
                WriteBE32Wia(dest, p.NumberOfGroups);

                // DataEntry1: zeros
                byte[] zeroPDE = new byte[WiaConst.PartitionDataEntrySize];
                ms.Write(zeroPDE, 0, zeroPDE.Length);
                dest.Write(zeroPDE, 0, zeroPDE.Length);
            }

            return ms.ToArray();
        }

        private static byte[] SerializeRawDataEntries(List<RawRegionInfo> regions)
        {
            using var ms = new MemoryStream();
            foreach (var r in regions)
            {
                var e = new WiaRawDataEntry
                {
                    DataOffset     = r.Offset,
                    DataSize       = r.Size,
                    GroupIndex     = r.FirstGroupIndex,
                    NumberOfGroups = r.NumberOfGroups,
                };
                ms.Write(SerializeRawDataEntry(e), 0, WiaConst.RawDataEntrySize);
            }

            return ms.ToArray();
        }

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

            while ((pos % 4) != 0) { ms.WriteByte(0); pos++; }

            return ms.ToArray();
        }
#endif

        private static byte[] CompressTableDataWia(byte[] data,
            WiaRvzCompressionType ct, int cl, byte[] propData, byte propSize)
        {
            if (ct == WiaRvzCompressionType.Purge)
                return PurgeCompressor.Compress(data, 0, data.Length);
            if (ct > WiaRvzCompressionType.Purge)
                return WiaRvzCompressionHelper.Compress(ct, data, 0, data.Length, cl, propData, propSize);
            return data;
        }

        // -----------------------------------------------------------------------
        // Header finalisation
        // -----------------------------------------------------------------------

#if !NET20
        private static void WriteWiaHeaders(Stream dest, byte[] discHdr,
            bool isRvz, WiaDiscType discType,
            WiaRvzCompressionType compressionType, int compressionLevel, uint chunkSize,
            uint numPartitions, ulong partEntriesOffset, byte[] partHash,
            uint numRawData, ulong rawEntriesOffset, uint rawEntriesSize,
            uint numGroups, ulong groupEntriesOffset, uint groupEntriesSize,
            byte[] propData, byte propSize,
            long isoSize, long fileSize)
        {
            var header2 = new WiaHeader2
            {
                DiscType                 = discType,
                CompressionType          = compressionType,
                CompressionLevel         = compressionLevel,
                ChunkSize                = chunkSize,
                DiscHeader               = discHdr,
                NumberOfPartitionEntries = numPartitions,
                PartitionEntrySize       = WiaConst.PartitionEntrySize,
                PartitionEntriesOffset   = partEntriesOffset,
                PartitionEntriesHash     = partHash,
                NumberOfRawDataEntries   = numRawData,
                RawDataEntriesOffset     = rawEntriesOffset,
                RawDataEntriesSize       = rawEntriesSize,
                NumberOfGroupEntries     = numGroups,
                GroupEntriesOffset       = groupEntriesOffset,
                GroupEntriesSize         = groupEntriesSize,
                CompressorDataSize       = propSize,
                CompressorData           = propData,
            };

            byte[] h2Bytes = SerializeHeader2Wia(header2);
            byte[] h2Hash  = ComputeSha1Wia(h2Bytes, 0, h2Bytes.Length);

            uint magic = isRvz ? WiaConst.RvzMagic : WiaConst.WiaMagic;
            uint ver   = isRvz ? WiaConst.RvzVersion        : WiaConst.WiaVersion;
            uint verC  = isRvz ? WiaConst.RvzVersionWriteCompatible : WiaConst.WiaVersionWriteCompatible;

            var header1 = new WiaHeader1
            {
                Magic             = magic,
                Version           = ver,
                VersionCompatible = verC,
                Header2Size       = WiaConst.Header2Size,
                Header2Hash       = h2Hash,
                IsoFileSize       = (ulong)isoSize,
                WiaFileSize       = (ulong)fileSize,
                Header1Hash       = new byte[20],
            };

            byte[] h1Bytes    = SerializeHeader1Wia(header1);
            byte[] h1Hashable = new byte[h1Bytes.Length - 20];
            Array.Copy(h1Bytes, h1Hashable, h1Hashable.Length);
            header1.Header1Hash = ComputeSha1Wia(h1Hashable, 0, h1Hashable.Length);

            dest.Seek(0, SeekOrigin.Begin);
            dest.Write(SerializeHeader1Wia(header1), 0, WiaConst.Header1Size);
            dest.Write(h2Bytes, 0, h2Bytes.Length);
        }

        private static byte[] SerializeHeader1Wia(WiaHeader1 h)
        {
            using var ms = new MemoryStream();
            WriteLE32Wia(ms, h.Magic);
            WriteBE32Wia(ms, h.Version);
            WriteBE32Wia(ms, h.VersionCompatible);
            WriteBE32Wia(ms, h.Header2Size);
            ms.Write(h.Header2Hash, 0, 20);
            WriteBE64Wia(ms, h.IsoFileSize);
            WriteBE64Wia(ms, h.WiaFileSize);
            ms.Write(h.Header1Hash, 0, 20);
            return ms.ToArray();
        }

        private static byte[] SerializeHeader2Wia(WiaHeader2 h)
        {
            using var ms = new MemoryStream();
            WriteBE32Wia(ms, (uint)h.DiscType);
            WriteBE32Wia(ms, (uint)h.CompressionType);
            WriteBE32Wia(ms, (uint)h.CompressionLevel);
            WriteBE32Wia(ms, h.ChunkSize);
            byte[] dh = h.DiscHeader ?? new byte[WiaConst.DiscHeaderStoredSize];
            ms.Write(dh, 0, Math.Min(dh.Length, WiaConst.DiscHeaderStoredSize));
            if (dh.Length < WiaConst.DiscHeaderStoredSize)
                ms.Write(new byte[WiaConst.DiscHeaderStoredSize - dh.Length], 0,
                    WiaConst.DiscHeaderStoredSize - dh.Length);
            WriteBE32Wia(ms, h.NumberOfPartitionEntries);
            WriteBE32Wia(ms, h.PartitionEntrySize);
            WriteBE64Wia(ms, h.PartitionEntriesOffset);
            ms.Write(h.PartitionEntriesHash ?? new byte[20], 0, 20);
            WriteBE32Wia(ms, h.NumberOfRawDataEntries);
            WriteBE64Wia(ms, h.RawDataEntriesOffset);
            WriteBE32Wia(ms, h.RawDataEntriesSize);
            WriteBE32Wia(ms, h.NumberOfGroupEntries);
            WriteBE64Wia(ms, h.GroupEntriesOffset);
            WriteBE32Wia(ms, h.GroupEntriesSize);
            ms.WriteByte(h.CompressorDataSize);
            byte[] prop = h.CompressorData ?? new byte[7];
            ms.Write(prop, 0, Math.Min(prop.Length, 7));
            if (prop.Length < 7)
                ms.Write(new byte[7 - prop.Length], 0, 7 - prop.Length);
            return ms.ToArray();
        }

        private static byte[] ComputeSha1Wia(byte[] data, int offset, int count)
        {
            if (count == 0) return new byte[20];
            using var sha1 = SHA1.Create();
            return sha1.ComputeHash(data, offset, count);
        }
#endif

        // -----------------------------------------------------------------------
        // Platform detection
        // -----------------------------------------------------------------------

        private static Platform DetectWiaPlatform(byte[] header)
        {
            if (header.Length >= 0x1C)
            {
                uint wiiMagic = (uint)((header[0x18] << 24) | (header[0x19] << 16) | (header[0x1A] << 8) | header[0x1B]);
                if (wiiMagic == NdConstants.WiiMagicWord)
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

                if (valid) return Platform.GameCube;
            }

            return Platform.Unknown;
        }

        // -----------------------------------------------------------------------
        // Misc helpers
        // -----------------------------------------------------------------------

        private static uint WriteRawGroupData(Stream dest, ref long bytesWritten,
            GcGroupWorkEntry gi, bool isRvz,
            WiaRvzCompressionType compressionType, int compressionLevel,
            byte[] propData, byte propSize)
        {
            if (gi.CompressedData != null)
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

            if (compressionType == WiaRvzCompressionType.Purge && gi.MainData != null)
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

        private static bool IsAllSameWia(byte[] data, int length)
        {
            if (length == 0) return true;
            byte first = data[0];
            for (int i = 1; i < length; i++)
            {
                if (data[i] != first) return false;
            }

            return true;
        }

        #if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        private static byte[] ConcatBytesWia(byte[] a, int aOff, int aLen, byte[] b, int bOff, int bLen)
        {
            var r = new byte[aLen + bLen];
            if (aLen > 0) Array.Copy(a, aOff, r, 0, aLen);
            if (bLen > 0) Array.Copy(b, bOff, r, aLen, bLen);
            return r;
        }
#endif

        private static void PadTo4Wia(Stream s, ref long bytesWritten)
        {
            int pad = (int)((-bytesWritten) & 3);
            if (pad > 0) { s.Write(new byte[pad], 0, pad); bytesWritten += pad; }
        }

        private static void PadTableTo4Wia(Stream s, ref long tablePos)
        {
            long pad = (-tablePos) & 3;
            if (pad > 0) { s.Write(new byte[pad], 0, (int)pad); tablePos += pad; }
        }

        private static long AlignWia(long value, long align) => (value + align - 1) / align * align;

        private static uint SwapBE(uint v) => (v << 24) | ((v << 8) & 0x00FF0000u) | ((v >> 8) & 0x0000FF00u) | (v >> 24);

        private static uint ReadBE32Wia(byte[] d, int o) => (uint)((d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3]);

        private static void WriteBE32Wia(Stream s, uint v)
        {
            s.WriteByte((byte)(v >> 24));
            s.WriteByte((byte)(v >> 16));
            s.WriteByte((byte)(v >> 8));
            s.WriteByte((byte)v);
        }

        private static void WriteBE64Wia(Stream s, ulong v)
        {
            WriteBE32Wia(s, (uint)(v >> 32));
            WriteBE32Wia(s, (uint)v);
        }

        #if !NET20
        private static void WriteLE32Wia(Stream s, uint v)
        {
            s.WriteByte((byte)v);
            s.WriteByte((byte)(v >> 8));
            s.WriteByte((byte)(v >> 16));
            s.WriteByte((byte)(v >> 24));
        }
#endif

        // -----------------------------------------------------------------------
        // Inner work types — explicit structs, no ValueTuple (net20 compatibility)
        // -----------------------------------------------------------------------

        // Key: (byte sameByte, int bytesRead) — replaces ValueTuple<byte,int>
        private struct WiaDedupKey2 : IEquatable<WiaDedupKey2>
        {
            public byte SameByte;
            public int  BytesRead;

            public WiaDedupKey2(byte sameByte, int bytesRead)
            {
                SameByte  = sameByte;
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
            public byte  SampleByte;
            public int   BytesRead;

            public WiaDedupKey3(ulong pkh, byte sb, int br)
            {
                PartKeyHash = pkh;
                SampleByte  = sb;
                BytesRead   = br;
            }

            public bool Equals(WiaDedupKey3 other) => PartKeyHash == other.PartKeyHash && SampleByte == other.SampleByte && BytesRead == other.BytesRead;
            public override bool Equals(object? obj) => obj is WiaDedupKey3 k && Equals(k);
            public override int GetHashCode() => (int)(PartKeyHash ^ (PartKeyHash >> 32)) ^ (SampleByte * 397) ^ BytesRead;
        }

        // Value: (uint offset, uint dataSize) — replaces ValueTuple<uint,uint>
        private struct WiaDedup2
        {
            public uint Offset;
            public uint DataSize;

            public WiaDedup2(uint offset, uint dataSize) { Offset = offset; DataSize = dataSize; }
        }

        // Group entry holding DataOffset, DataSize, RvzPackedSize
        private struct WiaRvzGroupEntry
        {
            public uint DataOffset;
            public uint DataSize;
            public uint RvzPackedSize;

            public WiaRvzGroupEntry(uint dataOffset, uint dataSize, uint rvzPackedSize)
            {
                DataOffset    = dataOffset;
                DataSize      = dataSize;
                RvzPackedSize = rvzPackedSize;
            }
        }

        // Raw data entry local struct (avoids confusion with model RawDataEntry)
        private struct WiaRawDataEntry
        {
            public ulong DataOffset;
            public ulong DataSize;
            public uint  GroupIndex;
            public uint  NumberOfGroups;
        }

        // Flat work item for Parallel.For — replaces (int b, int c) ValueTuple
        private struct WiaFlatWorkItem
        {
            public int BatchIndex;
            public int ChunkIndex;

            public WiaFlatWorkItem(int b, int c) { BatchIndex = b; ChunkIndex = c; }
        }

        private sealed class GcGroupWorkEntry
        {
            public int     BytesRead;
            public long    SourceOffset;
            public bool    IsAllSame;
            public byte    SameByte;
            public bool    IsDedupHit;
            public WiaRvzGroupEntry DedupEntry;
            public byte[]? MainData;
            public uint    RvzPackedSize;
            public byte[]? CompressedData = null;
        }

#if !NET20
        private sealed class WiiChunkWork
        {
            public bool    IsAllZeros;
            public bool    IsDecDedupHit;
            public uint    DecDedupOffset;
            public uint    DecDedupDataSize;
            public byte[]  ExceptionListBytes = new byte[0];
            public int     UnpaddedExLen;
            public byte[]  MainDataBytes = new byte[0];
            public uint    RvzPackedSize;
            public byte[]? CompressedData = null;
            public bool    DecAllSame;
            public WiaDedupKey3 DecDedupKey;
        }

        private sealed class WiiBatchItem
        {
            public int     BytesRead;
            public long    SrcOffset;
            public bool    IsInterDedupHit;
            public WiaDedup2 DedupResult;
            public bool    EncAllSame;
            public WiaDedupKey3 DedupKey;
            public byte[]? DecryptedAll;
            public List<HashExceptionEntry>? AllExceptions;
            public int     NumChunks;
            public WiiChunkWork[]? PartWork;
        }

        private sealed class WiiPartInfo
        {
            public ulong PartitionOffset;
            public byte[] TitleKey = new byte[0];
            public ulong DataOffset;
            public ulong DataSize;
            public ulong DataStart;
            public ulong DataEnd;
            public uint  FirstGroupIndex;
            public uint  NumberOfGroups;
        }

        private sealed class RawRegionInfo
        {
            public ulong Offset;
            public ulong Size;
            public uint  FirstGroupIndex;
            public uint  NumberOfGroups;
        }

        private sealed class DiscRegionEntry
        {
            public bool           IsPartition;
            public long           Offset;
            public WiiPartInfo?   PartitionInfo;
            public RawRegionInfo? RawInfo;
        }
#endif
    }
}
