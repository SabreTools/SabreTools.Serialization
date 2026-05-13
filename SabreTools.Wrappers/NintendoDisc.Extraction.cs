using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Numerics.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            try
            {
                if (Platform == Platform.GameCube)
                    return ExtractGameCube(outputDirectory);
                else if (Platform == Platform.Wii)
                    return ExtractWii(outputDirectory);

                return false;
            }
            catch (Exception ex)
            {
                if (includeDebug)
                    Console.Error.WriteLine(ex);

                return false;
            }
        }

        #region Pre-decrypted reader override

        /// <summary>
        /// When set, <see cref="ReadDecryptedPartitionRange"/> calls this delegate instead of
        /// performing AES-CBC decryption.  Used by WIA/RVZ extraction, where partition data is
        /// already stored decrypted and the encrypt-then-decrypt round-trip is unnecessary.
        /// Signature: (absDataOffset, partitionDataOffset, length) -> decrypted bytes or null.
        /// </summary>
        internal Func<long, long, int, byte[]?>? _preDecryptedReader;

        #endregion

        #region GameCube extraction

        /// <summary>
        /// Extract GameCube data from a disc
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the extraction was successful, false otherwise</returns>
        private bool ExtractGameCube(string outputDirectory)
        {
            string sysDir = Path.Combine(outputDirectory, "sys");
            Directory.CreateDirectory(sysDir);

            // sys/boot.bin (disc header, 0x000 - 0x43F)
            WriteRange(0, Constants.DiscHeaderSize, Path.Combine(sysDir, "boot.bin"));

            // sys/bi2.bin  (0x440 - 0x243F)
            WriteRange(Constants.Bi2Address, Constants.Bi2Size, Path.Combine(sysDir, "bi2.bin"));

            // sys/apploader.img
            WriteGCApploader(sysDir);

            // DOL offset stored without shift on GameCube
            long dolOffset = Header.DolOffset;
            if (dolOffset > 0)
            {
                byte[] dolHeaderBytes = ReadRangeFromSource(dolOffset, 0xE0);
                if (dolHeaderBytes.Length > 0)
                {
                    int dolHeaderOffset = 0;
                    var dolHeader = Serialization.Readers.NintendoDisc.ParseDOLHeader(dolHeaderBytes, ref dolHeaderOffset);

                    int dolSize = GetDolSize(dolHeader);
                    WriteRange(dolOffset, dolSize, Path.Combine(sysDir, "main.dol"));
                }
            }

            // FST offset stored without shift on GameCube
            long fstOffset = Header.FstOffset;
            long fstSize = Header.FstSize;
            if (fstOffset > 0 && fstSize > 0)
            {
                WriteRange(fstOffset, (int)Math.Min(fstSize, int.MaxValue), Path.Combine(sysDir, "fst.bin"));

                byte[] fstData = ReadRangeFromSource(fstOffset, (int)Math.Min(fstSize, int.MaxValue));
                if (fstData.Length > 0)
                {
                    string filesDir = Path.Combine(outputDirectory, "files");
                    Directory.CreateDirectory(filesDir);
                    ExtractFstFiles(fstData, offsetShift: 0, filesDir, ReadRangeFromSource);
                }
            }

            return true;
        }

        #endregion

        #region Wii extraction

        /// <summary>
        /// Extract Wii data from a disc
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the extraction was successful, false otherwise</returns>
        private bool ExtractWii(string outputDirectory)
        {
            // Unencrypted disc header area
            string discDir = Path.Combine(outputDirectory, "disc");
            Directory.CreateDirectory(discDir);

            WriteRange(0, 0x100, Path.Combine(discDir, "header.bin"));
            WriteRange(Constants.WiiRegionDataAddress, Constants.WiiRegionDataSize, Path.Combine(discDir, "region.bin"));

            if (Model.PartitionTableEntries is null)
                return true;

            var typeCounters = new Dictionary<uint, int>();

            foreach (var pte in Model.PartitionTableEntries)
            {
                long partOffset = pte.Offset << 2;
                if (partOffset <= 0 || partOffset >= _dataSource.Length)
                    continue;

                string partName = GetPartitionName(pte.Type, typeCounters);
                string partDir = Path.Combine(outputDirectory, partName);
                Directory.CreateDirectory(partDir);

                ExtractWiiPartition(partOffset, partDir);
            }

            return true;
        }

        /// <summary>
        /// Extract a Wii partition
        /// </summary>
        /// <param name="partitionOffset">Offset to the partition</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        private void ExtractWiiPartition(long partitionOffset, string outputDirectory)
        {
            // ticket.bin (unencrypted, 0x2A4 bytes at partition start)
            WriteRange(partitionOffset, Constants.WiiTicketSize, Path.Combine(outputDirectory, "ticket.bin"));

            byte[]? ticketData = ReadRangeFromSource(partitionOffset, Constants.WiiTicketSize);
            if (ticketData is null || ticketData.Length < Constants.TicketCommonKeyIndexOffset + 1)
                return;

            #region Title Key

            // Decrypt title key
            byte[] encTitleKey = new byte[16];
            Array.Copy(ticketData, Constants.TicketEncryptedTitleKeyOffset, encTitleKey, 0, 16);
            byte[] titleId = new byte[8];
            Array.Copy(ticketData, Constants.TicketTitleIdOffset, titleId, 0, 8);
            byte commonKeyIdx = ticketData[Constants.TicketCommonKeyIndexOffset];

            byte[]? titleKey = DecryptTitleKey(encTitleKey, titleId, commonKeyIdx);
            if (titleKey is null)
                return;

            #endregion

            #region TMD

            byte[] tmdSizeBytes = ReadRangeFromSource(partitionOffset + Constants.WiiTmdSizeAddress, 4);
            int tmdSizePos = 0;
            uint tmdSize = tmdSizeBytes.Length > 0
                ? tmdSizeBytes.ReadUInt32BigEndian(ref tmdSizePos)
                : 0;

            byte[] tmdOffBytes = ReadRangeFromSource(partitionOffset + Constants.WiiTmdOffsetAddress, 4);
            int tmdOffPos = 0;
            uint tmdOffShifted = tmdOffBytes.Length > 0
                ? tmdOffBytes.ReadUInt32BigEndian(ref tmdOffPos)
                : 0;

            long tmdOffset = (long)tmdOffShifted << 2;
            if (tmdSize > 0 && tmdOffset > 0)
                WriteRange(partitionOffset + tmdOffset, (int)tmdSize, Path.Combine(outputDirectory, "tmd.bin"));

            #endregion

            #region cert.bin

            byte[] certSizeBytes = ReadRangeFromSource(partitionOffset + Constants.WiiCertSizeAddress, 4);
            int certSizePos = 0;
            uint certSize = certSizeBytes.Length > 0
                ? certSizeBytes.ReadUInt32BigEndian(ref certSizePos)
                : 0;

            byte[] certOffBytes = ReadRangeFromSource(partitionOffset + Constants.WiiCertOffsetAddress, 4);
            int certOffPos = 0;
            uint certOffShifted = certOffBytes.Length > 0
                ? certOffBytes.ReadUInt32BigEndian(ref certOffPos)
                : 0;

            long certOffset = (long)certOffShifted << 2;
            if (certSize > 0 && certOffset > 0)
                WriteRange(partitionOffset + certOffset, (int)certSize, Path.Combine(outputDirectory, "cert.bin"));

            #endregion

            #region h3.bin

            byte[] h3OffBytes = ReadRangeFromSource(partitionOffset + Constants.WiiH3OffsetAddress, 4);
            int h3OffPos = 0;
            uint h3OffShifted = h3OffBytes.Length > 0
                ? h3OffBytes.ReadUInt32BigEndian(ref h3OffPos)
                : 0;

            long h3Offset = (long)h3OffShifted << 2;
            if (h3Offset > 0)
                WriteRange(partitionOffset + h3Offset, Constants.WiiH3Size, Path.Combine(outputDirectory, "h3.bin"));

            #endregion

            // Encrypted partition data start
            byte[] dataOffBytes = ReadRangeFromSource(partitionOffset + Constants.WiiDataOffsetAddress, 4);
            int dataOffPos = 0;
            uint dataOffShifted = dataOffBytes.Length > 0
                ? dataOffBytes.ReadUInt32BigEndian(ref dataOffPos)
                : 0;

            long dataOffset = (long)dataOffShifted << 2;
            if (dataOffset <= 0)
                return;

            long absDataOffset = partitionOffset + dataOffset;

            string sysDir = Path.Combine(outputDirectory, "sys");
            Directory.CreateDirectory(sysDir);

            // Read boot block from decrypted partition (block 0, offset 0 within data)
            byte[]? bootBlock = ReadDecryptedPartitionRange(absDataOffset, titleKey, 0, Constants.DiscHeaderSize);
            if (bootBlock is null)
                return;

            File.WriteAllBytes(Path.Combine(sysDir, "boot.bin"), bootBlock);

            // bi2.bin
            byte[]? bi2 = ReadDecryptedPartitionRange(absDataOffset, titleKey, Constants.Bi2Address, Constants.Bi2Size);
            if (bi2 is not null)
                File.WriteAllBytes(Path.Combine(sysDir, "bi2.bin"), bi2);

            // apploader
            WriteWiiApploader(absDataOffset, titleKey, sysDir);

            #region DOL

            // Stored offset is shifted <<2 in Wii partition
            int dolOffPos = 0x420;
            uint dolOffShifted = bootBlock.ReadUInt32BigEndian(ref dolOffPos);
            long dolOff = (long)dolOffShifted << 2;
            if (dolOff > 0)
            {
                byte[]? dolHeaderBytes = ReadDecryptedPartitionRange(absDataOffset, titleKey, dolOff, 0xE0);
                if (dolHeaderBytes is not null)
                {
                    int dolHeaderOffset = 0;
                    var dolHeader = Serialization.Readers.NintendoDisc.ParseDOLHeader(dolHeaderBytes, ref dolHeaderOffset);

                    int dolSize = GetDolSize(dolHeader);
                    byte[]? dol = ReadDecryptedPartitionRange(absDataOffset, titleKey, dolOff, dolSize);
                    if (dol is not null)
                        File.WriteAllBytes(Path.Combine(sysDir, "main.dol"), dol);
                }
            }

            #endregion

            #region FST

            // Stored offset shifted <<2 in Wii partition
            int fstOffPos = 0x424;
            uint fstOffShifted = bootBlock.ReadUInt32BigEndian(ref fstOffPos);
            long fstOff = (long)fstOffShifted << 2;

            int fstSzPos = 0x428;
            uint fstSzShifted = bootBlock.ReadUInt32BigEndian(ref fstSzPos);
            long fstSize = (long)fstSzShifted << 2;  // also stored >>2 on Wii

            if (fstOff > 0 && fstSize > 0)
            {
                byte[]? fstData = ReadDecryptedPartitionRange(absDataOffset, titleKey, fstOff, (int)Math.Min(fstSize, int.MaxValue));
                if (fstData is not null)
                {
                    File.WriteAllBytes(Path.Combine(sysDir, "fst.bin"), fstData);
                    string filesDir = Path.Combine(outputDirectory, "files");
                    Directory.CreateDirectory(filesDir);
                    ExtractFstFiles(fstData,
                        offsetShift: 2,
                        filesDir,
                        (offset, length) => ReadDecryptedPartitionRange(absDataOffset, titleKey, offset, length));
                }
            }

            #endregion
        }

        #endregion

        #region FST Extraction

        /// <summary>
        ///
        /// </summary>
        /// <param name="fstData"></param>
        /// <param name="offsetShift"></param>
        /// <param name="filesDir"></param>
        /// <param name="readFunc"></param>
        private void ExtractFstFiles(byte[] fstData,
            int offsetShift,
            string filesDir,
            Func<long, int, byte[]?> readFunc)
        {
            if (fstData is null || fstData.Length < 12)
                return;

            // Root entry is at offset 0; its fileSize field = total entry count
            int rootPos = 8;
            uint rootCount = fstData.ReadUInt32BigEndian(ref rootPos);
            if (rootCount < 1 || rootCount > 1024 * 1024)
                return;

            // String table immediately follows all entries
            int stringTableOffset = (int)(rootCount * 12);

            ExtractFstDirectory(fstData, 1, (int)rootCount, stringTableOffset, filesDir, offsetShift, readFunc);
        }

        /// <summary>
        /// Recursively extracts FST entries [start, end) into <paramref name="currentDir"/>.
        /// Returns the index of the next entry after this directory.
        /// </summary>
        private int ExtractFstDirectory(byte[] fstData,
            int start,
            int end,
            int stringTableOffset,
            string currentDir,
            int offsetShift,
            Func<long, int, byte[]?> readFunc)
        {
            int i = start;
            while (i < end)
            {
                int fstBase = i * 12;
                if ((fstBase + 12) > fstData.Length)
                    break;

                // Each FST entry is 12 bytes: [flags(1) | nameOff(3)] [fileOffRaw(4)] [fileSize(4)]
                int fstEntryPos = fstBase;
                var entry = Serialization.Readers.NintendoDisc.ParseFileSystemTableEntry(fstData, ref fstEntryPos);

                int nameOffset = stringTableOffset + (int)(entry.NameOffset & 0x00FFFFFF);
                string? name = fstData.ReadNullTerminatedAnsiString(ref nameOffset);
                if (string.IsNullOrEmpty(name))
                {
                    i++;
                    continue;
                }

                // Sanitize name: replace path separators and reject/flatten dot-segments
                name = name!.Replace('/', '_').Replace('\\', '_');
                if (name == "." || name == "..")
                    name = "_";

                name = name.TrimStart('.');

                byte flags = (byte)(entry.NameOffset >> 24);
                bool isDir = (flags & 1) != 0;
                if (isDir)
                {
                    // fileOffRaw = parent entry index; fileSize = last entry index in this dir
                    int nextEntry = (int)entry.FileSize;
                    string subDir = Path.Combine(currentDir, name);
                    Directory.CreateDirectory(subDir);
                    i = ExtractFstDirectory(fstData, i + 1, nextEntry, stringTableOffset, subDir, offsetShift, readFunc);
                }
                else
                {
                    string outPath = Path.Combine(currentDir, name);
                    string? outDir = Path.GetDirectoryName(outPath);
                    if (!string.IsNullOrEmpty(outDir))
                        Directory.CreateDirectory(outDir);

                    if (entry.FileSize == 0)
                    {
                        // Zero-byte file — create empty
                        File.WriteAllBytes(outPath, []);
                    }
                    else
                    {
                        long discOffset = (long)entry.FileOffset << offsetShift;
                        byte[]? fileData = readFunc(discOffset, (int)Math.Min(entry.FileSize, int.MaxValue));
                        if (fileData is not null)
                            File.WriteAllBytes(outPath, fileData);
                    }

                    i++;
                }
            }

            return i;
        }

        #endregion

        #region Apploader Writing

        /// <summary>
        /// Write GameCube apploader.img to the output directory, if possible
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        private bool WriteGCApploader(string outputDirectory)
        {
            byte[] header = ReadRangeFromSource(Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (header.Length == 0)
                return false;

            int index = Constants.ApploaderCodeSizeOffset;
            uint codeSize = header.ReadUInt32BigEndian(ref index);

            index = Constants.ApploaderTrailerSizeOffset;
            uint trailerSize = header.ReadUInt32BigEndian(ref index);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            WriteRange(Constants.ApploaderAddress, totalSize, Path.Combine(outputDirectory, "apploader.img"));
            return true;
        }

        /// <summary>
        /// Write Wii apploader.img to the output directory, if possible
        /// </summary>
        /// <param name="absDataOffset"></param>
        /// <param name="titleKey"></param>
        /// <param name="sysDir"></param>
        private void WriteWiiApploader(long absDataOffset, byte[] titleKey, string sysDir)
        {
            byte[]? header = ReadDecryptedPartitionRange(absDataOffset, titleKey, Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (header is null)
                return;

            int index = Constants.ApploaderCodeSizeOffset;
            uint codeSize = header.ReadUInt32BigEndian(ref index);

            index = Constants.ApploaderTrailerSizeOffset;
            uint trailerSize = header.ReadUInt32BigEndian(ref index);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            byte[]? apploader = ReadDecryptedPartitionRange(absDataOffset, titleKey, Constants.ApploaderAddress, totalSize);
            if (apploader is not null)
                File.WriteAllBytes(Path.Combine(sysDir, "apploader.img"), apploader);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Total byte length of the raw disc image data
        /// </summary>
        internal long DataLength => _dataSource.Length;

        /// <summary>
        /// Read <paramref name="length"/> bytes from the disc image at <paramref name="offset"/>.
        /// </summary>
        internal byte[] ReadData(long offset, int length) => ReadRangeFromSource(offset, length);

        /// <summary>
        /// Get the size of the DOL based on the header
        /// </summary>
        /// <param name="dolHeader">DOL header to retrieve the size for</param>
        /// <returns>The size of the DOL on success, 0 otherwise</returns>
        private static int GetDolSize(DOLHeader dolHeader)
        {
            if (dolHeader.SectionOffsetTable.Length != 18)
                return 0;
            if (dolHeader.SectionLengthsTable.Length != 18)
                return 0;

            int maxEnd = 0;

            // Loop through the 18 offset and size entries
            for (int i = 0; i < 18; i++)
            {
                uint offset = dolHeader.SectionOffsetTable[i];
                uint size = dolHeader.SectionLengthsTable[i];
                if (offset > 0 && size > 0)
                    maxEnd = (int)Math.Max(maxEnd, offset + size);
            }

            return maxEnd;
        }

        /// <summary>
        /// Get the name of a given partition type
        /// </summary>
        /// <param name="type">Parition type</param>
        /// <param name="counters">Dictionary of counters used for global indexing</param>
        /// <returns>String representing the partition name</returns>
        private static string GetPartitionName(uint type, Dictionary<uint, int> counters)
        {
            string code;
            switch (type)
            {
                // GM + counter
                case 0: code = "GM"; break;

                // UP + counter
                case 1: code = "UP"; break;

                // CH + counter
                case 2: code = "CH"; break;

                // Unknown: if all 4 bytes are printable ASCII, use the raw 4-char string (no prefix, no counter).
                // Otherwise fall back to P{globalIndex} — we use the cumulative counter sum as the index.
                default:
                    byte b0 = (byte)(type >> 24),
                        b1 = (byte)(type >> 16),
                        b2 = (byte)(type >> 8),
                        b3 = (byte)type;

                    if (b0 >= 0x20 && b0 <= 0x7E
                        && b1 >= 0x20 && b1 <= 0x7E
                        && b2 >= 0x20 && b2 <= 0x7E
                        && b3 >= 0x20 && b3 <= 0x7E)
                    {
                        return Encoding.ASCII.GetString([b0, b1, b2, b3]);
                    }

                    // Non-printable: use global partition index (sum of all counter values so far)
                    int globalIndex = 0;
                    foreach (var v in counters.Values)
                    {
                        globalIndex += v;
                    }

                    return $"P{globalIndex}";
            }

            int index = counters.TryGetValue(type, out int cv) ? cv : 0;
            counters[type] = index + 1;
            return $"{code}{index}";
        }

        /// <summary>
        /// Reads <paramref name="length"/> bytes at <paramref name="partitionDataOffset"/> within
        /// the decrypted partition data, decrypting 0x8000-byte blocks as needed.
        /// <paramref name="absDataOffset"/> is the absolute ISO offset where the encrypted data begins.
        /// </summary>
        private byte[]? ReadDecryptedPartitionRange(long absDataOffset, byte[] titleKey, long partitionDataOffset, int length)
        {
            if (length <= 0)
                return null;

            // WIA/RVZ fast path: data is already decrypted; skip the AES round-trip.
            if (_preDecryptedReader is not null)
                return _preDecryptedReader(absDataOffset, partitionDataOffset, length);

            var result = new byte[length];
            int produced = 0;

            while (produced < length)
            {
                long dataOff = partitionDataOffset + produced;
                long blockNum = dataOff / Constants.WiiBlockDataSize;
                int offsetInBlock = (int)(dataOff % Constants.WiiBlockDataSize);

                long encBlockOffset = absDataOffset + (blockNum * Constants.WiiBlockSize);
                byte[] encBlock = ReadRangeFromSource(encBlockOffset, Constants.WiiBlockSize);
                if (encBlock.Length < Constants.WiiBlockSize)
                    break;

                // IV is at offset 0x3D0 of the raw (still-encrypted) block.
                // Matches Dolphin / DolphinIsoLib WiiPartitionDecryptor.DecryptBlock.
                byte[] iv = new byte[16];
                Array.Copy(encBlock, 0x3D0, iv, 0, 16);

                // Decrypt the 0x7C00 data portion (bytes 0x400-0x7FFF of the raw block)
                byte[] encData = new byte[Constants.WiiBlockDataSize];
                Array.Copy(encBlock, Constants.WiiBlockHeaderSize, encData, 0, Constants.WiiBlockDataSize);

                byte[]? decData = DecryptBlock(encData, titleKey, iv);
                if (decData is null)
                    break;

                int canCopy = Math.Min(Constants.WiiBlockDataSize - offsetInBlock, length - produced);
                Array.Copy(decData, offsetInBlock, result, produced, canCopy);
                produced += canCopy;
            }

            return produced == length ? result : null;
        }

        /// <summary>
        /// Write a range from the underlying data source to a file
        /// </summary>
        /// <param name="offset">Offset into the data source</param>
        /// <param name="length">Length of the data to read and write</param>
        /// <param name="filePath">Full path to the expected output file</param>
        private void WriteRange(long offset, int length, string filePath)
        {
            if (length <= 0)
                return;

            byte[] data = ReadRangeFromSource(offset, length);
            if (data.Length == 0)
                return;

            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(filePath, data);
        }

        #endregion
    }
}
