using System;
using System.IO;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Numerics.Extensions;

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
                Directory.CreateDirectory(outputDirectory);

                if (Model.Platform == Platform.GameCube)
                    return ExtractGameCube(outputDirectory);
                else if (Model.Platform == Platform.Wii)
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

        #region GameCube extraction

        private bool ExtractGameCube(string dest)
        {
            string sysDir = Path.Combine(dest, "sys");
            Directory.CreateDirectory(sysDir);

            // sys/boot.bin (disc header, 0x000 – 0x43F)
            WriteRange(0, Constants.DiscHeaderSize, Path.Combine(sysDir, "boot.bin"));

            // sys/bi2.bin  (0x440 – 0x243F)
            WriteRange(Constants.Bi2Address, Constants.Bi2Size, Path.Combine(sysDir, "bi2.bin"));

            // sys/apploader.img
            WriteApploader(sysDir);

            // DOL offset stored without shift on GameCube
            long dolOffset = Model.Header.DolOffset;
            if (dolOffset > 0)
            {
                byte[]? dolHeader = ReadDisc(dolOffset, 0xE0);
                if (dolHeader != null)
                {
                    int dolSize = GetDolSize(dolHeader);
                    WriteRange(dolOffset, dolSize, Path.Combine(sysDir, "main.dol"));
                }
            }

            // FST offset stored without shift on GameCube
            long fstOffset = Model.Header.FstOffset;
            long fstSize   = Model.Header.FstSize;
            if (fstOffset > 0 && fstSize > 0)
            {
                WriteRange(fstOffset, (int)Math.Min(fstSize, int.MaxValue),
                    Path.Combine(sysDir, "fst.bin"));

                byte[]? fstData = ReadDisc(fstOffset, (int)Math.Min(fstSize, int.MaxValue));
                if (fstData != null)
                {
                    string filesDir = Path.Combine(dest, "files");
                    Directory.CreateDirectory(filesDir);
                    ExtractFstFiles(fstData, offsetShift: 0, filesDir, ReadDisc);
                }
            }

            return true;
        }

        #endregion

        #region Wii extraction

        private bool ExtractWii(string dest)
        {
            // Unencrypted disc header area
            string discDir = Path.Combine(dest, "disc");
            Directory.CreateDirectory(discDir);
            WriteRange(0, 0x100, Path.Combine(discDir, "header.bin"));
            WriteRange(Constants.WiiRegionDataAddress, Constants.WiiRegionDataSize,
                Path.Combine(discDir, "region.bin"));

            if (Model.PartitionTableEntries is null)
                return true;

            var typeCounters = new System.Collections.Generic.Dictionary<uint, int>();

            foreach (var pte in Model.PartitionTableEntries)
            {
                long partOffset = pte.Offset;
                if (partOffset <= 0 || partOffset >= _dataSource.Length)
                    continue;

                string partName = GetPartitionName(pte.Type, typeCounters);
                string partDir  = Path.Combine(dest, partName);
                Directory.CreateDirectory(partDir);

                ExtractWiiPartition(partOffset, partDir);
            }

            return true;
        }

        private void ExtractWiiPartition(long partOffset, string partDir)
        {
            // ticket.bin (unencrypted, 0x2A4 bytes at partition start)
            WriteRange(partOffset, Constants.WiiTicketSize, Path.Combine(partDir, "ticket.bin"));

            byte[]? ticketData = ReadDisc(partOffset, Constants.WiiTicketSize);
            if (ticketData is null || ticketData.Length < Constants.TicketCommonKeyIndexOffset + 1)
                return;

            // Decrypt title key
            byte[] encTitleKey = new byte[16];
            Array.Copy(ticketData, Constants.TicketEncryptedTitleKeyOffset, encTitleKey, 0, 16);
            byte[] titleId = new byte[8];
            Array.Copy(ticketData, Constants.TicketTitleIdOffset, titleId, 0, 8);
            byte commonKeyIdx = ticketData[Constants.TicketCommonKeyIndexOffset];

            byte[]? titleKey = DecryptTitleKey(encTitleKey, titleId, commonKeyIdx);
            if (titleKey is null)
                return;

            // TMD
            byte[]? tmdSizeBytes = ReadDisc(partOffset + Constants.WiiTmdSizeAddress, 4);
            int tmdSizePos = 0;
            uint tmdSize = tmdSizeBytes != null
                ? tmdSizeBytes.ReadUInt32BigEndian(ref tmdSizePos)
                : 0;
            byte[]? tmdOffBytes = ReadDisc(partOffset + Constants.WiiTmdOffsetAddress, 4);
            int tmdOffPos = 0;
            uint tmdOffShifted = tmdOffBytes != null
                ? tmdOffBytes.ReadUInt32BigEndian(ref tmdOffPos)
                : 0;
            long tmdOffset = (long)tmdOffShifted << 2;
            if (tmdSize > 0 && tmdOffset > 0)
                WriteRange(partOffset + tmdOffset, (int)tmdSize, Path.Combine(partDir, "tmd.bin"));

            // cert.bin
            byte[]? certSizeBytes = ReadDisc(partOffset + Constants.WiiCertSizeAddress, 4);
            int certSizePos = 0;
            uint certSize = certSizeBytes != null
                ? certSizeBytes.ReadUInt32BigEndian(ref certSizePos)
                : 0;
            byte[]? certOffBytes = ReadDisc(partOffset + Constants.WiiCertOffsetAddress, 4);
            int certOffPos = 0;
            uint certOffShifted = certOffBytes != null
                ? certOffBytes.ReadUInt32BigEndian(ref certOffPos)
                : 0;
            long certOffset = (long)certOffShifted << 2;
            if (certSize > 0 && certOffset > 0)
                WriteRange(partOffset + certOffset, (int)certSize, Path.Combine(partDir, "cert.bin"));

            // h3.bin
            byte[]? h3OffBytes = ReadDisc(partOffset + Constants.WiiH3OffsetAddress, 4);
            int h3OffPos = 0;
            uint h3OffShifted = h3OffBytes != null
                ? h3OffBytes.ReadUInt32BigEndian(ref h3OffPos)
                : 0;
            long h3Offset = (long)h3OffShifted << 2;
            if (h3Offset > 0)
                WriteRange(partOffset + h3Offset, Constants.WiiH3Size, Path.Combine(partDir, "h3.bin"));

            // Encrypted partition data start
            byte[]? dataOffBytes = ReadDisc(partOffset + Constants.WiiDataOffsetAddress, 4);
            int dataOffPos = 0;
            uint dataOffShifted = dataOffBytes != null
                ? dataOffBytes.ReadUInt32BigEndian(ref dataOffPos)
                : 0;
            long dataOffset = (long)dataOffShifted << 2;
            if (dataOffset <= 0)
                return;

            long absDataOffset = partOffset + dataOffset;

            string sysDir = Path.Combine(partDir, "sys");
            Directory.CreateDirectory(sysDir);

            // Read boot block from decrypted partition (block 0, offset 0 within data)
            byte[]? bootBlock = ReadDecryptedPartitionRange(absDataOffset, titleKey, 0, Constants.DiscHeaderSize);
            if (bootBlock is null)
                return;

            File.WriteAllBytes(Path.Combine(sysDir, "boot.bin"), bootBlock);

            // bi2.bin
            byte[]? bi2 = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                Constants.Bi2Address, Constants.Bi2Size);
            if (bi2 != null)
                File.WriteAllBytes(Path.Combine(sysDir, "bi2.bin"), bi2);

            // apploader
            WriteWiiApploader(absDataOffset, titleKey, sysDir);

            // DOL — stored offset is shifted <<2 in Wii partition
            int dolOffPos = 0x420;
            uint dolOffShifted = bootBlock.ReadUInt32BigEndian(ref dolOffPos);
            long dolOff = (long)dolOffShifted << 2;
            if (dolOff > 0)
            {
                byte[]? dolHdr = ReadDecryptedPartitionRange(absDataOffset, titleKey, dolOff, 0xE0);
                if (dolHdr != null)
                {
                    int dolSize = GetDolSize(dolHdr);
                    byte[]? dol = ReadDecryptedPartitionRange(absDataOffset, titleKey, dolOff, dolSize);
                    if (dol != null)
                        File.WriteAllBytes(Path.Combine(sysDir, "main.dol"), dol);
                }
            }

            // FST — stored offset shifted <<2 in Wii partition
            int fstOffPos = 0x424;
            int fstSzPos  = 0x428;
            uint fstOffShifted = bootBlock.ReadUInt32BigEndian(ref fstOffPos);
            uint fstSzShifted  = bootBlock.ReadUInt32BigEndian(ref fstSzPos);
            long fstOff = (long)fstOffShifted << 2;
            long fstSize = (long)fstSzShifted << 2;  // also stored >>2 on Wii
            if (fstOff > 0 && fstSize > 0)
            {
                byte[]? fstData = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                    fstOff, (int)Math.Min(fstSize, int.MaxValue));
                if (fstData != null)
                {
                    File.WriteAllBytes(Path.Combine(sysDir, "fst.bin"), fstData);
                    string filesDir = Path.Combine(partDir, "files");
                    Directory.CreateDirectory(filesDir);
                    ExtractFstFiles(fstData, offsetShift: 2, filesDir,
                        (offset, length) => ReadDecryptedPartitionRange(absDataOffset, titleKey, offset, length));
                }
            }
        }

        #endregion

        #region FST extraction

        private void ExtractFstFiles(byte[] fstData, int offsetShift, string filesDir,
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
            long stringTableOffset = rootCount * 12;

            ExtractFstDirectory(fstData, 1, (int)rootCount, stringTableOffset,
                filesDir, offsetShift, readFunc);
        }

        /// <summary>
        /// Recursively extracts FST entries [start, end) into <paramref name="currentDir"/>.
        /// Returns the index of the next entry after this directory.
        /// </summary>
        private int ExtractFstDirectory(byte[] fstData, int start, int end,
            long stringTableOffset, string currentDir, int offsetShift,
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
                uint flagsAndNameOff = fstData.ReadUInt32BigEndian(ref fstEntryPos);
                byte flags      = (byte)(flagsAndNameOff >> 24);
                bool isDir      = (flags & 1) != 0;
                uint nameOff    = flagsAndNameOff & 0x00FFFFFFu;
                uint fileOffRaw = fstData.ReadUInt32BigEndian(ref fstEntryPos);
                uint fileSize   = fstData.ReadUInt32BigEndian(ref fstEntryPos);

                string name = ReadFstString(fstData, stringTableOffset + nameOff);
                if (string.IsNullOrEmpty(name))
                {
                    i++;
                    continue;
                }

                // Sanitize name: replace path separators and reject/flatten dot-segments
                name = name.Replace('/', '_').Replace('\\', '_');
                if (name == "." || name == "..")
                    name = "_";
                name = name.TrimStart('.');

                if (isDir)
                {
                    // fileOffRaw = parent entry index; fileSize = last entry index in this dir
                    int nextEntry = (int)fileSize;
                    string subDir = Path.Combine(currentDir, name);
                    Directory.CreateDirectory(subDir);
                    i = ExtractFstDirectory(fstData, i + 1, nextEntry, stringTableOffset,
                        subDir, offsetShift, readFunc);
                }
                else
                {
                    string outPath = Path.Combine(currentDir, name);
                    string? outDir = Path.GetDirectoryName(outPath);
                    if (!string.IsNullOrEmpty(outDir))
                        Directory.CreateDirectory(outDir);

                    if (fileSize == 0)
                    {
                        // Zero-byte file — create empty
                        File.WriteAllBytes(outPath, new byte[0]);
                    }
                    else
                    {
                        long discOffset = (long)fileOffRaw << offsetShift;
                        byte[]? fileData = readFunc(discOffset, (int)Math.Min(fileSize, int.MaxValue));
                        if (fileData != null)
                            File.WriteAllBytes(outPath, fileData);
                    }

                    i++;
                }
            }

            return i;
        }

        private static string ReadFstString(byte[] fstData, long offset)
        {
            if (offset < 0 || offset >= fstData.Length)
                return string.Empty;

            int start = (int)offset;
            int end = start;
            while (end < fstData.Length && fstData[end] != 0)
                end++;

            return System.Text.Encoding.ASCII.GetString(fstData, start, end - start);
        }

        #endregion

        #region Apploader helpers

        private void WriteApploader(string sysDir)
        {
            byte[]? hdr = ReadDisc(Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (hdr is null) return;

            int codeSizePos    = Constants.ApploaderCodeSizeOffset;
            int trailerSizePos = Constants.ApploaderTrailerSizeOffset;
            uint codeSize    = hdr.ReadUInt32BigEndian(ref codeSizePos);
            uint trailerSize = hdr.ReadUInt32BigEndian(ref trailerSizePos);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            WriteRange(Constants.ApploaderAddress, totalSize, Path.Combine(sysDir, "apploader.img"));
        }

        private void WriteWiiApploader(long absDataOffset, byte[] titleKey, string sysDir)
        {
            byte[]? hdr = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (hdr is null) return;

            int wiiCodeSizePos    = Constants.ApploaderCodeSizeOffset;
            int wiiTrailerSizePos = Constants.ApploaderTrailerSizeOffset;
            uint codeSize    = hdr.ReadUInt32BigEndian(ref wiiCodeSizePos);
            uint trailerSize = hdr.ReadUInt32BigEndian(ref wiiTrailerSizePos);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            byte[]? apploader = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                Constants.ApploaderAddress, totalSize);
            if (apploader != null)
                File.WriteAllBytes(Path.Combine(sysDir, "apploader.img"), apploader);
        }

        #endregion

        #region DOL size calculation

        private static int GetDolSize(byte[] dolHeader)
        {
            // DOL header: 7 text section offsets (0x00), 11 data section offsets (0x1C),
            // 7 text sizes (0x90), 11 data sizes (0xAC), BSS offset (0xD8), BSS size (0xDC),
            // entry point (0xE0). Max (offset + size) over all sections gives the DOL size.
            if (dolHeader is null || dolHeader.Length < 0xE0)
                return 0;

            int maxEnd = 0;
            // Text sections (7): offset table at 0x00, size table at 0x90
            for (int s = 0; s < 7; s++)
            {
                int offPos = s * 4;
                int szPos  = 0x90 + (s * 4);
                int off = (int)dolHeader.ReadUInt32BigEndian(ref offPos);
                int sz  = (int)dolHeader.ReadUInt32BigEndian(ref szPos);
                if (off > 0 && sz > 0) maxEnd = Math.Max(maxEnd, off + sz);
            }
            // Data sections (11): offset table at 0x1C, size table at 0xAC
            for (int s = 0; s < 11; s++)
            {
                int offPos = 0x1C + (s * 4);
                int szPos  = 0xAC + (s * 4);
                int off = (int)dolHeader.ReadUInt32BigEndian(ref offPos);
                int sz  = (int)dolHeader.ReadUInt32BigEndian(ref szPos);
                if (off > 0 && sz > 0) maxEnd = Math.Max(maxEnd, off + sz);
            }

            return maxEnd;
        }

        #endregion

        #region Wii partition block decryption helpers

        /// <summary>
        /// Reads <paramref name="length"/> bytes at <paramref name="partitionDataOffset"/> within
        /// the decrypted partition data, decrypting 0x8000-byte blocks as needed.
        /// <paramref name="absDataOffset"/> is the absolute ISO offset where the encrypted data begins.
        /// </summary>
        private byte[]? ReadDecryptedPartitionRange(long absDataOffset, byte[] titleKey,
            long partitionDataOffset, int length)
        {
            if (length <= 0) return null;

            var result = new byte[length];
            int produced = 0;

            while (produced < length)
            {
                long dataOff = partitionDataOffset + produced;
                long blockNum = dataOff / Constants.WiiBlockDataSize;
                int  offsetInBlock = (int)(dataOff % Constants.WiiBlockDataSize);

                long encBlockOffset = absDataOffset + (blockNum * Constants.WiiBlockSize);
                byte[]? encBlock = ReadDisc(encBlockOffset, Constants.WiiBlockSize);
                if (encBlock is null || encBlock.Length < Constants.WiiBlockSize)
                    break;

                // IV is at offset 0x3D0 of the raw (still-encrypted) block.
                // Matches Dolphin / DolphinIsoLib WiiPartitionDecryptor.DecryptBlock.
                byte[] iv = new byte[16];
                Array.Copy(encBlock, 0x3D0, iv, 0, 16);

                // Decrypt the 0x7C00 data portion (bytes 0x400–0x7FFF of the raw block)
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

        #endregion

        #region Misc helpers

        private void WriteRange(long offset, int length, string filePath)
        {
            if (length <= 0) return;
            byte[]? data = ReadDisc(offset, length);
            if (data is null) return;
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllBytes(filePath, data);
        }

        private byte[]? ReadDisc(long offset, int length)
        {
            if (length <= 0 || offset < 0) return null;
            byte[] data = ReadRangeFromSource(offset, length);
            return data.Length == length ? data : null;
        }

        /// <summary>Total byte length of the raw disc image data.</summary>
        internal long DataLength => _dataSource.Length;

        /// <summary>
        /// Read <paramref name="length"/> bytes from the disc image at <paramref name="offset"/>.
        /// Returns null if the range is out of bounds or a short read occurs.
        /// </summary>
        internal byte[]? ReadData(long offset, int length) => ReadDisc(offset, length);

        private static string GetPartitionName(uint type,
            System.Collections.Generic.Dictionary<uint, int> counters)
        {
            // Matches DolphinIsoLib WiiDiscExtractor.PartitionFolderName exactly.
            // Known types: 0→GM+counter, 1→UP+counter, 2→CH+counter.
            // Unknown: if all 4 bytes are printable ASCII, use the raw 4-char string (no prefix, no counter).
            // Otherwise fall back to P{globalIndex} — we use the cumulative counter sum as the index.
            string code;
            switch (type)
            {
                case 0: code = "GM"; break;
                case 1: code = "UP"; break;
                case 2: code = "CH"; break;
                default:
                    byte b0 = (byte)(type >> 24), b1 = (byte)(type >> 16),
                         b2 = (byte)(type >>  8), b3 = (byte)type;
                    if (b0 >= 0x20 && b0 <= 0x7E && b1 >= 0x20 && b1 <= 0x7E &&
                        b2 >= 0x20 && b2 <= 0x7E && b3 >= 0x20 && b3 <= 0x7E)
                        return System.Text.Encoding.ASCII.GetString(new byte[] { b0, b1, b2, b3 });
                    // Non-printable: use global partition index (sum of all counter values so far)
                    int globalIdx = 0;
                    foreach (var v in counters.Values) globalIdx += v;
                    return $"P{globalIdx}";
            }

            int idx = counters.TryGetValue(type, out int cv) ? cv : 0;
            counters[type] = idx + 1;
            return $"{code}{idx}";
        }

        #endregion
    }
}
