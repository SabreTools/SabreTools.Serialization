using System;
using System.Text;

namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// Builds a minimal UDF 1.02 image in memory (2048-byte sectors, one Type 1 partition at sector 260)
    /// </summary>
    /// <remarks>
    /// Partition blocks:
    ///  0 File Set Descriptor
    ///  2 root directory File Entry, contents in block 3:
    ///    parent, "README.TXT" (embedded data), "deleted.txt" (deleted), "Sub" (directory),
    ///    "Media" (last entry, no padding)
    ///  4 "README.TXT" File Entry, data "hello world" embedded
    ///  5 "Sub" File Entry, contents in block 6: parent, "data.bin" (16-bit name)
    ///  7 "data.bin" File Entry, long_ad to block 8 (2048 bytes), then an Allocation Extent Descriptor
    ///    in block 10 with a long_ad to block 9 (952 bytes)
    ///  8-9 "data.bin" data (byte i = i % 251)
    /// 11 "Media" File Entry, one short_ad of 100 bytes allocated but not recorded
    /// </remarks>
    internal static class UDFImageBuilder
    {
        public const int SectorSize = 2048;
        public const int PartitionStart = 260;
        public const int PartitionLength = 40;
        public const int Sectors = 300;
        public const int DataBinLength = 3000;

        /// <summary>
        /// Build the image
        /// </summary>
        /// <param name="corruptRootEntry">True to break the checksum of the root directory File Entry</param>
        /// <param name="corruptRootEntryCrc">True to change a byte of the root directory File Entry that only the CRC covers</param>
        /// <param name="emptyPartitionMap">True to record the partition map with length 0 and a map count of 3</param>
        /// <param name="pointerLoop">True to replace the Terminating Descriptor by a Volume Descriptor Pointer back to the start of the sequence</param>
        /// <param name="subName">Name of the "Sub" directory in the root directory</param>
        /// <param name="subLoop">True to add a directory "loop" to "Sub" that points back to the root directory</param>
        /// <param name="mediaPastEnd">True to record "Media" in a partition block past the end of the image</param>
        public static byte[] Build(bool corruptRootEntry = false,
            bool corruptRootEntryCrc = false,
            bool emptyPartitionMap = false,
            bool pointerLoop = false,
            string subName = "Sub",
            bool subLoop = false,
            bool mediaPastEnd = false)
        {
            var image = new byte[Sectors * SectorSize];

            // Volume Recognition Sequence
            WriteStructure(image, 16, "BEA01");
            WriteStructure(image, 17, "NSR02");
            WriteStructure(image, 18, "TEA01");

            // Anchor Volume Descriptor Pointer: main and reserve sequence at sector 32, 4 sectors
            int avdp = 256 * SectorSize;
            WriteUInt32(image, avdp + 16, 4 * SectorSize);
            WriteUInt32(image, avdp + 20, 32);
            WriteUInt32(image, avdp + 24, 4 * SectorSize);
            WriteUInt32(image, avdp + 28, 32);
            WriteTag(image, avdp, 2, 256, 512);

            // Primary Volume Descriptor
            int pvd = 32 * SectorSize;
            WriteDString(image, pvd + 24, 32, "TESTVOLUME");
            WriteTag(image, pvd, 1, 32, 512);

            // Partition Descriptor: partition 0 at sector 260
            int pd = 33 * SectorSize;
            WriteUInt32(image, pd + 16, 1);
            image[pd + 22] = 0;
            WriteUInt32(image, pd + 188, PartitionStart);
            WriteUInt32(image, pd + 192, mediaPastEnd ? PartitionLength + 20 : PartitionLength);
            WriteTag(image, pd, 5, 33, 512);

            // Logical Volume Descriptor: block size, domain, File Set Descriptor at block 0, one Type 1 map
            int lvd = 34 * SectorSize;
            WriteUInt32(image, lvd + 16, 2);
            WriteDString(image, lvd + 84, 128, "TESTVOLUME");
            WriteUInt32(image, lvd + 212, SectorSize);
            WriteAscii(image, lvd + 217, "*OSTA UDF Compliant");
            image[lvd + 240] = 0x02;
            image[lvd + 241] = 0x01;
            WriteLongAd(image, lvd + 248, SectorSize, 0);
            WriteUInt32(image, lvd + 264, 6);
            WriteUInt32(image, lvd + 268, emptyPartitionMap ? 3 : 1);
            image[lvd + 440] = 1;
            image[lvd + 441] = (byte)(emptyPartitionMap ? 0 : 6);
            image[lvd + 442] = 1;
            WriteTag(image, lvd, 6, 34, 446);

            // Terminating Descriptor, or a Volume Descriptor Pointer back to the start of the sequence
            int td = 35 * SectorSize;
            if (pointerLoop)
            {
                WriteUInt32(image, td + 20, 4 * SectorSize);
                WriteUInt32(image, td + 24, 32);
                WriteTag(image, td, 3, 35, 512);
            }
            else
            {
                WriteTag(image, td, 8, 35, 512);
            }

            // File Set Descriptor: root directory ICB at block 2
            int fsd = Block(0);
            WriteDString(image, fsd + 304, 32, "TESTSET");
            WriteLongAd(image, fsd + 400, SectorSize, 2);
            WriteTag(image, fsd, 256, 0, 512);

            // Root directory contents
            int root = Block(3);
            int length = 0;
            length += WriteFid(image, root + length, 3, 0x0A, 2, []);
            length += WriteFid(image, root + length, 3, 0x00, 4, Name8("README.TXT"));
            length += WriteFid(image, root + length, 3, 0x04, 4, Name8("deleted.txt"));
            length += WriteFid(image, root + length, 3, 0x02, 5, Name8(subName));
            length += WriteFid(image, root + length, 3, 0x00, 11, Name8("Media"));
            WriteFileEntry(image, 2, 4, 0, length, ShortAd(length, 3));
            if (corruptRootEntry)
                image[Block(2) + 4] ^= 0xFF;
            if (corruptRootEntryCrc)
                image[Block(2) + 36] ^= 0xFF;

            // "README.TXT", embedded
            WriteFileEntry(image, 4, 5, 3, 11, Encoding.ASCII.GetBytes("hello world"));

            // "Sub" contents
            int sub = Block(6);
            length = 0;
            length += WriteFid(image, sub + length, 6, 0x0A, 2, []);
            length += WriteFid(image, sub + length, 6, 0x00, 7, Name16("data.bin"));
            if (subLoop)
                length += WriteFid(image, sub + length, 6, 0x02, 2, Name8("loop"));

            WriteFileEntry(image, 5, 4, 0, length, ShortAd(length, 6));

            // "data.bin", long allocation descriptors continued in an Allocation Extent Descriptor
            var ads = new byte[32];
            WriteLongAd(ads, 0, SectorSize, 8);
            WriteLongAd(ads, 16, (3u << 30) | SectorSize, 10);
            WriteFileEntry(image, 7, 5, 1, DataBinLength, ads);
            for (int i = 0; i < DataBinLength; i++)
            {
                image[Block(8) + i] = (byte)(i % 251);
            }

            int aed = Block(10);
            WriteUInt32(image, aed + 20, 16);
            WriteLongAd(image, aed + 24, DataBinLength - SectorSize, 9);
            WriteTag(image, aed, 258, 10, 40);

            // "Media", allocated but not recorded, or recorded past the end of the image
            if (mediaPastEnd)
                WriteFileEntry(image, 11, 5, 0, 100, ShortAd(100, PartitionLength + 10));
            else
                WriteFileEntry(image, 11, 5, 0, 100, ShortAd((1u << 30) | 100, 0));

            return image;
        }

        /// <summary>
        /// Offset of a partition block in the image
        /// </summary>
        public static int Block(int block) => (PartitionStart + block) * SectorSize;

        private static void WriteStructure(byte[] image, int sector, string identifier)
        {
            int offset = sector * SectorSize;
            image[offset] = 0;
            WriteAscii(image, offset + 1, identifier);
            image[offset + 6] = 1;
        }

        private static int WriteFid(byte[] image, int offset, uint block, byte characteristics, uint icb, byte[] name)
        {
            image[offset + 16] = 1;
            image[offset + 18] = characteristics;
            image[offset + 19] = (byte)name.Length;
            WriteLongAd(image, offset + 20, SectorSize, icb);
            Array.Copy(name, 0, image, offset + 38, name.Length);
            int length = (38 + name.Length + 3) & ~3;
            WriteTag(image, offset, 257, block, length);
            return length;
        }

        private static void WriteFileEntry(byte[] image, int block, byte fileType, ushort adType, long informationLength, byte[] ads)
        {
            int offset = Block(block);
            image[offset + 16 + 4] = 4;
            image[offset + 16 + 11] = fileType;
            image[offset + 16 + 18] = (byte)adType;
            WriteUInt32(image, offset + 56, (uint)informationLength);
            WriteUInt32(image, offset + 172, (uint)ads.Length);
            Array.Copy(ads, 0, image, offset + 176, ads.Length);
            WriteTag(image, offset, 261, (uint)block, 176 + ads.Length);
        }

        private static byte[] ShortAd(uint length, uint position)
        {
            var ad = new byte[8];
            WriteUInt32(ad, 0, length);
            WriteUInt32(ad, 4, position);
            return ad;
        }

        private static byte[] ShortAd(int length, uint position) => ShortAd((uint)length, position);

        private static void WriteLongAd(byte[] data, int offset, uint length, uint block)
        {
            WriteUInt32(data, offset, length);
            WriteUInt32(data, offset + 4, block);
        }

        private static void WriteLongAd(byte[] data, int offset, int length, uint block)
            => WriteLongAd(data, offset, (uint)length, block);

        private static byte[] Name8(string name)
        {
            var bytes = new byte[name.Length + 1];
            bytes[0] = 8;
            Encoding.ASCII.GetBytes(name, 0, name.Length, bytes, 1);
            return bytes;
        }

        private static byte[] Name16(string name)
        {
            var bytes = new byte[(name.Length * 2) + 1];
            bytes[0] = 16;
            Encoding.BigEndianUnicode.GetBytes(name, 0, name.Length, bytes, 1);
            return bytes;
        }

        private static void WriteDString(byte[] image, int offset, int fieldLength, string value)
        {
            image[offset] = 8;
            WriteAscii(image, offset + 1, value);
            image[offset + fieldLength - 1] = (byte)(value.Length + 1);
        }

        private static void WriteAscii(byte[] image, int offset, string value)
            => Encoding.ASCII.GetBytes(value, 0, value.Length, image, offset);

        private static void WriteUInt32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)value;
            data[offset + 1] = (byte)(value >> 8);
            data[offset + 2] = (byte)(value >> 16);
            data[offset + 3] = (byte)(value >> 24);
        }

        private static void WriteUInt32(byte[] data, int offset, int value) => WriteUInt32(data, offset, (uint)value);

        /// <summary>
        /// Write a descriptor tag covering the given descriptor length (version 2, CRC, checksum)
        /// </summary>
        private static void WriteTag(byte[] image, int offset, ushort identifier, uint location, int length)
        {
            image[offset] = (byte)identifier;
            image[offset + 1] = (byte)(identifier >> 8);
            image[offset + 2] = 2;
            ushort crc = Crc(image, offset + 16, length - 16);
            image[offset + 8] = (byte)crc;
            image[offset + 9] = (byte)(crc >> 8);
            image[offset + 10] = (byte)(length - 16);
            image[offset + 11] = (byte)((length - 16) >> 8);
            WriteUInt32(image, offset + 12, location);

            byte checksum = 0;
            for (int i = 0; i < 16; i++)
            {
                if (i != 4)
                    checksum += image[offset + i];
            }

            image[offset + 4] = checksum;
        }

        private static ushort Crc(byte[] data, int offset, int length)
        {
            ushort crc = 0;
            for (int i = offset; i < offset + length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int bit = 0; bit < 8; bit++)
                {
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
                }
            }

            return crc;
        }
    }
}
