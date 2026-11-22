using System;
using System.Text;

namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// Builds a minimal UDF 1.02 image in memory (2048-byte sectors, one partition at sector 260)
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
    /// Layouts other than Type 1 record the "Sub" contents in blocks 23-24, the "data.bin" entry across the block
    /// boundary, and add "big.bin" to them: File Entry in block 12, one extent over blocks 13-20.
    /// Virtual layouts record these blocks in the Virtual partition (blocks 1, 21 and 22 unused): blocks 0-16 in
    /// blocks 1-17, blocks 17-23 in blocks 21-27 and block 24 in block 30 of the Type 1 partition, the Virtual
    /// Allocation Table's File Entry in block 39.
    /// </remarks>
    internal static class UDFImageBuilder
    {
        public const int SectorSize = 2048;
        public const int PartitionStart = 260;
        public const int PartitionLength = 40;
        public const int Sectors = 300;
        public const int DataBinLength = 3000;
        public const int BigBinLength = (8 * SectorSize) - 100;
        public const int PacketLength = 4;
        public const int VirtualBlocks = 25;
        public static readonly int[] SparingTableSectors = [40, 44];

        /// <summary>
        /// Build the image
        /// </summary>
        /// <param name="corruptRootEntry">True to break the checksum of the root directory File Entry</param>
        /// <param name="corruptRootEntryCrc">True to change a byte of the root directory File Entry that only the CRC covers</param>
        /// <param name="emptyPartitionMap">True to record the partition map with length 0 and a map count of 3</param>
        /// <param name="pointerLoop">True to replace the Terminating Descriptor by a Volume Descriptor Pointer back to the start of the sequence</param>
        /// <param name="subName">Name of the "Sub" directory in the root directory</param>
        /// <param name="subLoop">True to add a directory "loop" to "Sub" that points back to the root directory</param>
        /// <param name="subPastPartition">True to record the "Sub" contents in a block past the end of the partition</param>
        /// <param name="mediaPastEnd">True to record "Media" in a partition block past the end of the image</param>
        /// <param name="mediaPastPartition">True to record "Media" in a block past the end of the partition</param>
        /// <param name="layout">Partition layout</param>
        /// <param name="numberOfSparingTables">Number of Sparing Table copies in the Sparable map, at sectors 40 and 44</param>
        /// <param name="sparingTableLength">Number of entries of the Sparing Table</param>
        /// <param name="firstSparingTable">Damage to the first copy of the Sparing Table</param>
        /// <param name="vatCandidate">File Entry in the last sector that is not the Virtual Allocation Table</param>
        /// <param name="danglingVirtualMap">True to add a Type 1 map and a Virtual map of partition 1, which has no Partition Descriptor</param>
        public static byte[] Build(bool corruptRootEntry = false,
            bool corruptRootEntryCrc = false,
            bool emptyPartitionMap = false,
            bool pointerLoop = false,
            string subName = "Sub",
            bool subLoop = false,
            bool subPastPartition = false,
            bool mediaPastEnd = false,
            bool mediaPastPartition = false,
            UDFLayout layout = UDFLayout.Type1,
            byte numberOfSparingTables = 2,
            int sparingTableLength = 4,
            SparingTableDamage firstSparingTable = SparingTableDamage.None,
            VatCandidate vatCandidate = VatCandidate.None,
            bool danglingVirtualMap = false)
        {
            var image = new byte[Sectors * SectorSize];
            bool isVirtual = layout == UDFLayout.Virtual || layout == UDFLayout.Virtual150;
            ushort reference = (ushort)(isVirtual ? 1 : 0);

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

            // Logical Volume Descriptor: block size, domain, File Set Descriptor at block 0, the partition maps
            int lvd = 34 * SectorSize;
            WriteUInt32(image, lvd + 16, 2);
            WriteDString(image, lvd + 84, 128, "TESTVOLUME");
            WriteUInt32(image, lvd + 212, SectorSize);
            WriteAscii(image, lvd + 217, "*OSTA UDF Compliant");
            image[lvd + 240] = 0x02;
            image[lvd + 241] = 0x01;
            WriteLongAd(image, lvd + 248, SectorSize, 0, reference);

            int map = lvd + 440;
            int mapCount = 1;
            if (layout == UDFLayout.Sparable)
            {
                WriteType2Map(image, map, "*UDF Sparable Partition", 0x0201, 0);
                image[map + 40] = PacketLength;
                image[map + 42] = numberOfSparingTables;
                WriteUInt32(image, map + 44, 56 + (8 * sparingTableLength));
                WriteUInt32(image, map + 48, SparingTableSectors[0]);
                WriteUInt32(image, map + 52, SparingTableSectors[1]);
                map += 64;
            }
            else
            {
                image[map] = 1;
                image[map + 1] = (byte)(emptyPartitionMap ? 0 : 6);
                image[map + 2] = 1;
                map += 6;
                if (danglingVirtualMap)
                {
                    image[map] = 1;
                    image[map + 1] = 6;
                    image[map + 2] = 1;
                    image[map + 4] = 1;
                    map += 6;
                    mapCount++;
                }

                if (isVirtual || danglingVirtualMap)
                {
                    ushort revision = (ushort)(layout == UDFLayout.Virtual150 ? 0x0150 : 0x0200);
                    WriteType2Map(image, map, "*UDF Virtual Partition", revision, (ushort)(danglingVirtualMap ? 1 : 0));
                    map += 64;
                    mapCount++;
                }
            }

            WriteUInt32(image, lvd + 264, map - lvd - 440);
            WriteUInt32(image, lvd + 268, emptyPartitionMap ? 3 : mapCount);
            WriteTag(image, lvd, 6, 34, map - lvd);

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
            WriteLongAd(image, fsd + 400, SectorSize, 2, reference);
            WriteTag(image, fsd, 256, 0, 512);

            // Root directory contents
            int root = Block(3);
            int length = 0;
            length += WriteFid(image, root + length, 3, 0x0A, 2, [], reference);
            length += WriteFid(image, root + length, 3, 0x00, 4, Name8("README.TXT"), reference);
            length += WriteFid(image, root + length, 3, 0x04, 4, Name8("deleted.txt"), reference);
            length += WriteFid(image, root + length, 3, 0x02, 5, Name8(subName), reference);
            length += WriteFid(image, root + length, 3, 0x00, 11, Name8("Media"), reference);
            WriteFileEntry(image, 2, 4, 0, length, ShortAd(length, 3));
            if (corruptRootEntry)
                image[Block(2) + 4] ^= 0xFF;
            if (corruptRootEntryCrc)
                image[Block(2) + 36] ^= 0xFF;

            // "README.TXT", embedded
            WriteFileEntry(image, 4, 5, 3, 11, Encoding.ASCII.GetBytes("hello world"));

            // "Sub" contents, in other layouts across two blocks
            bool extended = layout != UDFLayout.Type1;
            int subBlock = extended ? 23 : 6;
            int sub = Block(subBlock);
            length = 0;
            length += WriteFid(image, sub + length, (uint)subBlock, 0x0A, 2, [], reference);
            length += WriteFid(image, sub + length, (uint)subBlock, 0x00, 7, Name16("data.bin"), reference, extended ? 2000 : 0);
            if (extended)
                length += WriteFid(image, sub + length, (uint)(subBlock + (length / SectorSize)), 0x00, 12, Name8("big.bin"), reference);
            if (subLoop)
                length += WriteFid(image, sub + length, (uint)(subBlock + (length / SectorSize)), 0x02, 2, Name8("loop"), reference);

            WriteFileEntry(image, 5, 4, 0, length, ShortAd(length, subPastPartition ? PartitionLength + 10 : (uint)subBlock));

            // "data.bin", long allocation descriptors continued in an Allocation Extent Descriptor
            var ads = new byte[32];
            WriteLongAd(ads, 0, SectorSize, 8, reference);
            WriteLongAd(ads, 16, (3u << 30) | SectorSize, 10, reference);
            WriteFileEntry(image, 7, 5, 1, DataBinLength, ads);
            for (int i = 0; i < DataBinLength; i++)
            {
                image[Block(8) + i] = DataBinByte(i);
            }

            int aed = Block(10);
            WriteUInt32(image, aed + 20, 16);
            WriteLongAd(image, aed + 24, DataBinLength - SectorSize, 9, reference);
            WriteTag(image, aed, 258, 10, 40);

            // "Media", allocated but not recorded, or recorded past the end of the image or of the partition
            if (mediaPastEnd || mediaPastPartition)
                WriteFileEntry(image, 11, 5, 0, 100, ShortAd(100, PartitionLength + 10));
            else
                WriteFileEntry(image, 11, 5, 0, 100, ShortAd((1u << 30) | 100, 0));

            // "big.bin", one extent over 8 blocks
            if (layout != UDFLayout.Type1)
            {
                WriteFileEntry(image, 12, 5, 0, BigBinLength, ShortAd(BigBinLength, 13));
                for (int i = 0; i < BigBinLength; i++)
                {
                    image[Block(13) + i] = BigBinByte(i);
                }
            }

            if (layout == UDFLayout.Sparable)
            {
                RelocatePacket(image, 4, 104);
                RelocatePacket(image, 16, 100);
                RelocatePacket(image, 24, 108);
                WriteSparingTable(image, SparingTableSectors[1], sparingTableLength, SparingTableDamage.None);
                WriteSparingTable(image, SparingTableSectors[0], sparingTableLength, firstSparingTable);
            }
            else if (isVirtual)
            {
                image = MoveToVirtualPartition(image, layout, vatCandidate);
            }

            return image;
        }

        /// <summary>
        /// Offset of a partition block in the image
        /// </summary>
        public static int Block(int block) => (PartitionStart + block) * SectorSize;

        /// <summary>
        /// Byte of "data.bin"
        /// </summary>
        public static byte DataBinByte(int i) => (byte)(i % 251);

        /// <summary>
        /// Byte of "big.bin"
        /// </summary>
        public static byte BigBinByte(int i) => (byte)(i * 7 % 253);

        /// <summary>
        /// Contents of a file whose bytes are given by their offset
        /// </summary>
        public static byte[] Contents(int length, Func<int, byte> fileByte)
        {
            var contents = new byte[length];
            for (int i = 0; i < length; i++)
            {
                contents[i] = fileByte(i);
            }

            return contents;
        }

        /// <summary>
        /// Block of the Type 1 partition that a block of the Virtual partition is recorded in
        /// </summary>
        public static uint PhysicalBlock(int block) => (uint)(block < 17 ? block + 1 : block < 24 ? block + 4 : block + 6);

        private static void WriteStructure(byte[] image, int sector, string identifier)
        {
            int offset = sector * SectorSize;
            image[offset] = 0;
            WriteAscii(image, offset + 1, identifier);
            image[offset + 6] = 1;
        }

        private static void WriteType2Map(byte[] image, int offset, string identifier, ushort revision, ushort partitionNumber)
        {
            image[offset] = 2;
            image[offset + 1] = 64;
            WriteAscii(image, offset + 5, identifier);
            image[offset + 28] = (byte)revision;
            image[offset + 29] = (byte)(revision >> 8);
            image[offset + 36] = 1;
            image[offset + 38] = (byte)partitionNumber;
            image[offset + 39] = (byte)(partitionNumber >> 8);
        }

        /// <summary>
        /// Copy a packet to the sectors it is relocated to, and fill the packet with garbage
        /// </summary>
        private static void RelocatePacket(byte[] image, int packet, int sector)
        {
            Array.Copy(image, Block(packet), image, sector * SectorSize, PacketLength * SectorSize);
            for (int i = Block(packet); i < Block(packet + PacketLength); i++)
            {
                image[i] = 0xEE;
            }
        }

        /// <summary>
        /// Write a Sparing Table relocating packets 4, 16 and 24, a damaged one relocates them to empty sectors
        /// </summary>
        private static void WriteSparingTable(byte[] image, int sector, int entries, SparingTableDamage damage)
        {
            bool damaged = damage != SparingTableDamage.None;
            int offset = sector * SectorSize;
            WriteAscii(image, offset + 17, damage == SparingTableDamage.SparingIdentifier ? "*UDF Sparing Tablx" : "*UDF Sparing Table");
            image[offset + 48] = (byte)entries;
            image[offset + 49] = (byte)(entries >> 8);
            WriteUInt32(image, offset + 52, 1);
            for (int i = 0; i < entries; i++)
            {
                uint original = i switch { 0 => 4, 1 => 16, 2 => 24, 3 => 0xFFFFFFF0, _ => 0xFFFFFFFF };
                uint mapped = i switch { 0 => damaged ? 140u : 104u, 1 => damaged ? 150u : 100u, 2 => damaged ? 160u : 108u, _ => 0 };
                WriteUInt32(image, offset + 56 + (8 * i), original);
                WriteUInt32(image, offset + 60 + (8 * i), mapped);
            }

            WriteTag(image, offset, (ushort)(damage == SparingTableDamage.TagIdentifier ? 8 : 0), (uint)sector, 56 + (8 * entries));
            if (damage == SparingTableDamage.Checksum)
                image[offset + 4] ^= 0xFF;
        }

        /// <summary>
        /// Record the partition blocks in the Virtual partition and write the Virtual Allocation Table
        /// </summary>
        private static byte[] MoveToVirtualPartition(byte[] image, UDFLayout layout, VatCandidate candidate)
        {
            var moved = new byte[image.Length];
            Array.Copy(image, moved, Block(0));

            var entries = new uint[VirtualBlocks];
            for (int block = 0; block < entries.Length; block++)
            {
                if (block == 1 || block == 21 || block == 22)
                {
                    entries[block] = 0xFFFFFFFF;
                    continue;
                }

                entries[block] = PhysicalBlock(block);
                Array.Copy(image, Block(block), moved, Block((int)entries[block]), SectorSize);
            }

            int vatBlock = candidate == VatCandidate.None ? 39 : 38;
            if (layout == UDFLayout.Virtual)
            {
                var vat = new byte[152 + (4 * entries.Length)];
                vat[0] = 152;
                WriteDString(vat, 4, 128, "TESTVOLUME");
                WriteUInt32(vat, 132, 0xFFFFFFFF);
                WriteUInt32(vat, 136, 4);
                WriteUInt32(vat, 140, 2);
                vat[145] = 0x02;
                vat[147] = 0x02;
                vat[149] = 0x02;
                for (int i = 0; i < entries.Length; i++)
                {
                    WriteUInt32(vat, 152 + (4 * i), entries[i]);
                }

                WriteFileEntry(moved, vatBlock, 248, 3, vat.Length, vat);
            }
            else
            {
                int vat = Block(37);
                for (int i = 0; i < entries.Length; i++)
                {
                    WriteUInt32(moved, vat + (4 * i), entries[i]);
                }

                WriteAscii(moved, vat + (4 * entries.Length) + 1, "*UDF Virtual Alloc Tbl");
                moved[vat + (4 * entries.Length) + 24] = 0x50;
                moved[vat + (4 * entries.Length) + 25] = 0x01;
                WriteUInt32(moved, vat + (4 * entries.Length) + 32, 0xFFFFFFFF);
                WriteFileEntry(moved, vatBlock, 0, 0, (4 * entries.Length) + 36, ShortAd((4 * entries.Length) + 36, 37));
            }

            if (candidate == VatCandidate.NotATable || candidate == VatCandidate.RegularFile)
            {
                WriteFileEntry(moved, 39, (byte)(candidate == VatCandidate.RegularFile ? 5 : 0), 0, 120, ShortAd(120, 36));
            }
            else if (candidate == VatCandidate.TooLarge)
            {
                // A header, then entries of unused blocks, one more than fits the partition and the largest header
                const int informationLength = ushort.MaxValue + (4 * PartitionLength) + 4;
                int header = Block(36);
                moved[header] = 152;
                for (int i = header + 152; i < header + SectorSize; i++)
                {
                    moved[i] = 0xFF;
                }

                var candidateAds = new byte[16];
                WriteUInt32(candidateAds, 0, SectorSize);
                WriteUInt32(candidateAds, 4, 36);
                WriteUInt32(candidateAds, 8, (1u << 30) | (informationLength - SectorSize));
                WriteFileEntry(moved, 39, 248, 0, informationLength, candidateAds);
            }
            else if (candidate == VatCandidate.Truncated)
            {
                var header = new byte[152];
                header[0] = 152;
                WriteFileEntry(moved, 39, 248, 3, 300, header);
            }
            else if (candidate == VatCandidate.ExtendedDescriptors)
            {
                WriteFileEntry(moved, 39, 248, 2, 152, new byte[20]);
            }
            else if (candidate == VatCandidate.PastPartition)
            {
                WriteFileEntry(moved, 39, 0, 0, 120, ShortAd(120, PartitionLength + 10));
            }

            return moved;
        }

        private static int WriteFid(byte[] image, int offset, uint block, byte characteristics, uint icb, byte[] name, ushort reference, int implementationUse = 0)
        {
            image[offset + 16] = 1;
            image[offset + 18] = characteristics;
            image[offset + 19] = (byte)name.Length;
            WriteLongAd(image, offset + 20, SectorSize, icb, reference);
            image[offset + 36] = (byte)implementationUse;
            image[offset + 37] = (byte)(implementationUse >> 8);
            Array.Copy(name, 0, image, offset + 38 + implementationUse, name.Length);
            int length = (38 + implementationUse + name.Length + 3) & ~3;
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

        private static void WriteLongAd(byte[] data, int offset, uint length, uint block, ushort reference)
        {
            WriteUInt32(data, offset, length);
            WriteUInt32(data, offset + 4, block);
            data[offset + 8] = (byte)reference;
            data[offset + 9] = (byte)(reference >> 8);
        }

        private static void WriteLongAd(byte[] data, int offset, int length, uint block, ushort reference)
            => WriteLongAd(data, offset, (uint)length, block, reference);

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
