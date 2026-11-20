using System;
using System.Text;
using SabreTools.Data.Models.OperaFS;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// Builds a minimal OperaFS image in memory (2048-byte sectors)
    /// </summary>
    /// <remarks>
    /// Sectors:
    ///  0 Volume Descriptor, root directory of 2 blocks with avatars at sectors 15 and 1
    ///  1-2 copy of the root directory
    ///  3-4 copy of "Sub"
    ///  5-9 data of "README.TXT", "DATA.BIN", "LAST.BIN", "DEEP.BIN" and "OTHER.BIN"
    /// 10 "Deep" directory: "DEEP.BIN"
    /// 11 "Other" directory with avatars at sectors 11 and 14: "OTHER.BIN"
    /// 12-13 "Sub" directory with avatars at sectors 12 and 3: "DATA.BIN" (block 0), "Deep" and "LAST.BIN" (block 1)
    /// 14 copy of "Other"
    /// 15-16 root directory: "README.TXT" (block 0), "Sub" and "Other" (block 1)
    /// </remarks>
    internal static class OperaFSImageBuilder
    {
        public const int SectorSize = 2048;
        public const int Sectors = 17;

        /// <summary>
        /// Flags of a file record as recorded on discs
        /// </summary>
        private const DirectoryRecordFlags FileFlags = DirectoryRecordFlags.READ_ONLY;

        /// <summary>
        /// Flags of a directory record as recorded on discs
        /// </summary>
        private const DirectoryRecordFlags DirectoryFlags = DirectoryRecordFlags.DIRECTORY | DirectoryRecordFlags.READ_ONLY | DirectoryRecordFlags.SYSTEM;

        /// <summary>
        /// Build the image
        /// </summary>
        /// <param name="subBlockCount">Number of blocks of "Sub" in its entry</param>
        /// <param name="subNextBlock">Next block in the first block of "Sub"</param>
        /// <param name="subCopyNextBlock">Next block in the first block of the copy of "Sub", the same as <paramref name="subNextBlock"/> if null</param>
        /// <param name="deepSector">Sector of "Deep" in its entry</param>
        /// <param name="otherCopySector">Sector of the copy of "Other" in its entry</param>
        public static byte[] Build(uint subBlockCount = 2, int subNextBlock = 1, int? subCopyNextBlock = null, uint deepSector = 10, uint otherCopySector = 14)
        {
            var image = new byte[Sectors * SectorSize];

            // Volume Descriptor
            image[0] = 0x01;
            WriteAscii(image, 1, "ZZZZZ");
            image[6] = 0x01;
            WriteAscii(image, 40, "CD-ROM");
            int offset = 76;
            image.WriteBigEndian(ref offset, (uint)SectorSize);
            image.WriteBigEndian(ref offset, (uint)Sectors);
            image.WriteBigEndian(ref offset, 0u);
            image.WriteBigEndian(ref offset, 2u);
            image.WriteBigEndian(ref offset, (uint)SectorSize);
            image.WriteBigEndian(ref offset, 1u);
            image.WriteBigEndian(ref offset, 15u);
            image.WriteBigEndian(ref offset, 1u);

            // File data
            WriteAscii(image, 5 * SectorSize, "hello world");
            WriteAscii(image, 6 * SectorSize, "data");
            WriteAscii(image, 7 * SectorSize, "last");
            WriteAscii(image, 8 * SectorSize, "deep");
            WriteAscii(image, 9 * SectorSize, "other");

            WriteBlock(image, 10, -1, -1, Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL | DirectoryRecordFlags.DIRECTORY_FINAL, "DEEP.BIN", 4, 1, 8));

            WriteBlock(image, 11, -1, -1, Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL | DirectoryRecordFlags.DIRECTORY_FINAL, "OTHER.BIN", 5, 1, 9));
            WriteBlock(image, 14, -1, -1, Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL | DirectoryRecordFlags.DIRECTORY_FINAL, "OTHER.BIN", 5, 1, 9));

            WriteSub(image, 12, subNextBlock, deepSector);
            WriteSub(image, 3, subCopyNextBlock ?? subNextBlock, deepSector);

            WriteRoot(image, 15, subBlockCount, otherCopySector);
            WriteRoot(image, 1, subBlockCount, otherCopySector);

            return image;
        }

        private static void WriteSub(byte[] image, int sector, int nextBlock, uint deepSector)
        {
            WriteBlock(image, sector, nextBlock, -1, Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL, "DATA.BIN", 4, 1, 6));
            WriteBlock(image, sector + 1, -1, 0,
                Entry(DirectoryFlags, "Deep", SectorSize, 1, deepSector),
                Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL | DirectoryRecordFlags.DIRECTORY_FINAL, "LAST.BIN", 4, 1, 7));
        }

        private static void WriteRoot(byte[] image, int sector, uint subBlockCount, uint otherCopySector)
        {
            WriteBlock(image, sector, 1, -1, Entry(FileFlags | DirectoryRecordFlags.BLOCK_FINAL, "README.TXT", 11, 1, 5));
            WriteBlock(image, sector + 1, -1, 0,
                Entry(DirectoryFlags, "Sub", subBlockCount * SectorSize, subBlockCount, 12, 3),
                Entry(DirectoryFlags | DirectoryRecordFlags.BLOCK_FINAL | DirectoryRecordFlags.DIRECTORY_FINAL, "Other", SectorSize, 1, 11, otherCopySector));
        }

        /// <summary>
        /// Write a directory block: header, then the entries from offset 20
        /// </summary>
        private static void WriteBlock(byte[] image, int sector, int nextBlock, int previousBlock, params byte[][] entries)
        {
            int start = sector * SectorSize;
            int offset = start + 20;
            foreach (byte[] entry in entries)
            {
                Array.Copy(entry, 0, image, offset, entry.Length);
                offset += entry.Length;
            }

            uint firstFreeByte = (uint)(offset - start);
            offset = start;
            image.WriteBigEndian(ref offset, (uint)nextBlock);
            image.WriteBigEndian(ref offset, (uint)previousBlock);
            image.WriteBigEndian(ref offset, 0u);
            image.WriteBigEndian(ref offset, firstFreeByte);
            image.WriteBigEndian(ref offset, 20u);
        }

        /// <summary>
        /// Build a directory entry with one block per sector
        /// </summary>
        private static byte[] Entry(DirectoryRecordFlags flags, string name, uint byteCount, uint blockCount, params uint[] avatars)
        {
            var entry = new byte[68 + (4 * avatars.Length)];
            WriteAscii(entry, 8, "    ");
            WriteAscii(entry, 32, name);

            int offset = 0;
            entry.WriteBigEndian(ref offset, (uint)flags);
            offset = 12;
            entry.WriteBigEndian(ref offset, (uint)SectorSize);
            entry.WriteBigEndian(ref offset, byteCount);
            entry.WriteBigEndian(ref offset, blockCount);
            entry.WriteBigEndian(ref offset, 1u);
            offset = 64;
            entry.WriteBigEndian(ref offset, (uint)(avatars.Length - 1));
            foreach (uint avatar in avatars)
            {
                entry.WriteBigEndian(ref offset, avatar);
            }

            return entry;
        }

        private static void WriteAscii(byte[] data, int offset, string value)
            => Encoding.ASCII.GetBytes(value).CopyTo(data, offset);
    }
}
