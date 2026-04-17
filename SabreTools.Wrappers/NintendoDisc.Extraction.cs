using System;
using System.IO;
using SabreTools.Data.Models.NintendoDisc;

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
            catch
            {
                return false;
            }
        }

        // -----------------------------------------------------------------------
        // GameCube extraction
        // -----------------------------------------------------------------------

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

        // -----------------------------------------------------------------------
        // Wii extraction
        // -----------------------------------------------------------------------

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
            uint tmdSize = tmdSizeBytes != null
                ? (uint)((tmdSizeBytes[0] << 24) | (tmdSizeBytes[1] << 16) | (tmdSizeBytes[2] << 8) | tmdSizeBytes[3])
                : 0;
            byte[]? tmdOffBytes = ReadDisc(partOffset + Constants.WiiTmdOffsetAddress, 4);
            uint tmdOffShifted = tmdOffBytes != null
                ? (uint)((tmdOffBytes[0] << 24) | (tmdOffBytes[1] << 16) | (tmdOffBytes[2] << 8) | tmdOffBytes[3])
                : 0;
            long tmdOffset = (long)tmdOffShifted << 2;
            if (tmdSize > 0 && tmdOffset > 0)
                WriteRange(partOffset + tmdOffset, (int)tmdSize, Path.Combine(partDir, "tmd.bin"));

            // cert.bin
            byte[]? certSizeBytes = ReadDisc(partOffset + Constants.WiiCertSizeAddress, 4);
            uint certSize = certSizeBytes != null
                ? (uint)((certSizeBytes[0] << 24) | (certSizeBytes[1] << 16) | (certSizeBytes[2] << 8) | certSizeBytes[3])
                : 0;
            byte[]? certOffBytes = ReadDisc(partOffset + Constants.WiiCertOffsetAddress, 4);
            uint certOffShifted = certOffBytes != null
                ? (uint)((certOffBytes[0] << 24) | (certOffBytes[1] << 16) | (certOffBytes[2] << 8) | certOffBytes[3])
                : 0;
            long certOffset = (long)certOffShifted << 2;
            if (certSize > 0 && certOffset > 0)
                WriteRange(partOffset + certOffset, (int)certSize, Path.Combine(partDir, "cert.bin"));

            // h3.bin
            byte[]? h3OffBytes = ReadDisc(partOffset + Constants.WiiH3OffsetAddress, 4);
            uint h3OffShifted = h3OffBytes != null
                ? (uint)((h3OffBytes[0] << 24) | (h3OffBytes[1] << 16) | (h3OffBytes[2] << 8) | h3OffBytes[3])
                : 0;
            long h3Offset = (long)h3OffShifted << 2;
            if (h3Offset > 0)
                WriteRange(partOffset + h3Offset, Constants.WiiH3Size, Path.Combine(partDir, "h3.bin"));

            // Encrypted partition data start
            byte[]? dataOffBytes = ReadDisc(partOffset + Constants.WiiDataOffsetAddress, 4);
            uint dataOffShifted = dataOffBytes != null
                ? (uint)((dataOffBytes[0] << 24) | (dataOffBytes[1] << 16) | (dataOffBytes[2] << 8) | dataOffBytes[3])
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
            uint dolOffShifted = (uint)((bootBlock[0x420] << 24) | (bootBlock[0x421] << 16)
                | (bootBlock[0x422] << 8) | bootBlock[0x423]);
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
            uint fstOffShifted = (uint)((bootBlock[0x424] << 24) | (bootBlock[0x425] << 16)
                | (bootBlock[0x426] << 8) | bootBlock[0x427]);
            uint fstSzField = (uint)((bootBlock[0x428] << 24) | (bootBlock[0x429] << 16)
                | (bootBlock[0x42A] << 8) | bootBlock[0x42B]);
            long fstOff = (long)fstOffShifted << 2;
            if (fstOff > 0 && fstSzField > 0)
            {
                byte[]? fstData = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                    fstOff, (int)Math.Min(fstSzField, int.MaxValue));
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

        // -----------------------------------------------------------------------
        // FST extraction
        // -----------------------------------------------------------------------

        private void ExtractFstFiles(byte[] fstData, int offsetShift, string filesDir,
            Func<long, int, byte[]?> readFunc)
        {
            if (fstData is null || fstData.Length < 12)
                return;

            // Root entry is at offset 0; its fileSize field = total entry count
            uint rootCount = (uint)((fstData[8] << 24) | (fstData[9] << 16)
                | (fstData[10] << 8) | fstData[11]);
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

                byte flags      = fstData[fstBase];
                bool isDir      = (flags & 1) != 0;
                uint nameOff    = (uint)((fstData[fstBase + 1] << 16) | (fstData[fstBase + 2] << 8) | fstData[fstBase + 3]);
                uint fileOffRaw = (uint)((fstData[fstBase + 4] << 24) | (fstData[fstBase + 5] << 16) | (fstData[fstBase + 6] << 8) | fstData[fstBase + 7]);
                uint fileSize   = (uint)((fstData[fstBase + 8] << 24) | (fstData[fstBase + 9] << 16) | (fstData[fstBase + 10] << 8) | fstData[fstBase + 11]);

                string name = ReadFstString(fstData, stringTableOffset + nameOff);
                if (string.IsNullOrEmpty(name))
                {
                    i++;
                    continue;
                }

                // Sanitize name
                name = name.Replace('/', '_').Replace('\\', '_');

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
                    long discOffset = (long)fileOffRaw << offsetShift;
                    if (discOffset > 0 && fileSize > 0)
                    {
                        byte[]? fileData = readFunc(discOffset, (int)Math.Min(fileSize, int.MaxValue));
                             if (fileData != null)
                            {
                                string outPath = Path.Combine(currentDir, name);
                                string? outDir = Path.GetDirectoryName(outPath);
                                if (!string.IsNullOrEmpty(outDir))
                                    Directory.CreateDirectory(outDir);
                                File.WriteAllBytes(outPath, fileData);
                            }
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

        // -----------------------------------------------------------------------
        // Apploader helpers
        // -----------------------------------------------------------------------

        private void WriteApploader(string sysDir)
        {
            byte[]? hdr = ReadDisc(Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (hdr is null) return;

            uint codeSize    = (uint)((hdr[Constants.ApploaderCodeSizeOffset] << 24)
                | (hdr[Constants.ApploaderCodeSizeOffset + 1] << 16)
                | (hdr[Constants.ApploaderCodeSizeOffset + 2] << 8)
                |  hdr[Constants.ApploaderCodeSizeOffset + 3]);
            uint trailerSize = (uint)((hdr[Constants.ApploaderTrailerSizeOffset] << 24)
                | (hdr[Constants.ApploaderTrailerSizeOffset + 1] << 16)
                | (hdr[Constants.ApploaderTrailerSizeOffset + 2] << 8)
                |  hdr[Constants.ApploaderTrailerSizeOffset + 3]);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            WriteRange(Constants.ApploaderAddress, totalSize, Path.Combine(sysDir, "apploader.img"));
        }

        private void WriteWiiApploader(long absDataOffset, byte[] titleKey, string sysDir)
        {
            byte[]? hdr = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                Constants.ApploaderAddress, Constants.ApploaderHeaderSize);
            if (hdr is null) return;

            uint codeSize    = (uint)((hdr[Constants.ApploaderCodeSizeOffset] << 24)
                | (hdr[Constants.ApploaderCodeSizeOffset + 1] << 16)
                | (hdr[Constants.ApploaderCodeSizeOffset + 2] << 8)
                |  hdr[Constants.ApploaderCodeSizeOffset + 3]);
            uint trailerSize = (uint)((hdr[Constants.ApploaderTrailerSizeOffset] << 24)
                | (hdr[Constants.ApploaderTrailerSizeOffset + 1] << 16)
                | (hdr[Constants.ApploaderTrailerSizeOffset + 2] << 8)
                |  hdr[Constants.ApploaderTrailerSizeOffset + 3]);

            int totalSize = Constants.ApploaderHeaderSize + (int)codeSize + (int)trailerSize;
            byte[]? apploader = ReadDecryptedPartitionRange(absDataOffset, titleKey,
                Constants.ApploaderAddress, totalSize);
            if (apploader != null)
                File.WriteAllBytes(Path.Combine(sysDir, "apploader.img"), apploader);
        }

        // -----------------------------------------------------------------------
        // DOL size calculation
        // -----------------------------------------------------------------------

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
                int off = (int)ReadBE32(dolHeader, s * 4);
                int sz  = (int)ReadBE32(dolHeader, 0x90 + (s * 4));
                if (off > 0 && sz > 0) maxEnd = Math.Max(maxEnd, off + sz);
            }
            // Data sections (11): offset table at 0x1C, size table at 0xAC
            for (int s = 0; s < 11; s++)
            {
                int off = (int)ReadBE32(dolHeader, 0x1C + (s * 4));
                int sz  = (int)ReadBE32(dolHeader, 0xAC + (s * 4));
                if (off > 0 && sz > 0) maxEnd = Math.Max(maxEnd, off + sz);
            }

            return maxEnd;
        }

        // -----------------------------------------------------------------------
        // Wii partition block decryption helpers
        // -----------------------------------------------------------------------

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

                // IV is last 16 bytes of the hash block (offset 0x3F0 in the encrypted hash block)
                byte[] iv = new byte[16];
                Array.Copy(encBlock, 0x3F0, iv, 0, 16);

                // Decrypt the 0x7C00 data portion
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

        // -----------------------------------------------------------------------
        // Misc helpers
        // -----------------------------------------------------------------------

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
            string prefix = type switch
            {
                0 => "DATA",
                1 => "UPDATE",
                2 => "CHANNEL",
                _ => $"P{type:X}"
            };

            int idx = counters.TryGetValue(type, out int v) ? v : 0;
            counters[type] = idx + 1;
            return $"{prefix}{idx}";
        }

        private static uint ReadBE32(byte[] data, int offset) => (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
}

