using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Data.Models.ISO9660.SystemUseEntries;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public partial class ISO9660Tests
    {
        /// <summary>
        /// CD-ROM XA system use information of a directory record
        /// </summary>
        private static readonly byte[] XASystemUse = [0x00, 0x00, 0x00, 0x00, 0x8D, 0x55, 0x58, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];

        #region ParseSystemUseEntry

        [Fact]
        public void ParseSystemUseEntry_ContinuationArea_ReadsFields()
        {
            byte[] data = Entry("CE", 1, BothEndian(30), BothEndian(100), BothEndian(237));

            var actual = Assert.IsType<ContinuationArea>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal("CE", Encoding.ASCII.GetString(actual.Signature));
            Assert.Equal(28, actual.Length);
            Assert.Equal(1, actual.Version);
            Assert.Equal(30, actual.BlockLocation.LittleEndian);
            Assert.Equal(100u, actual.Offset.LittleEndian);
            Assert.Equal(237u, actual.AreaLength.LittleEndian);
        }

        [Fact]
        public void ParseSystemUseEntry_PaddingField_ReadsPadding()
        {
            byte[] data = Entry("PD", 1, [0x00, 0x00, 0x00]);

            var actual = Assert.IsType<PaddingField>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(new byte[3], actual.Padding);
        }

        [Fact]
        public void ParseSystemUseEntry_SharingProtocolIndicator_ReadsFields()
        {
            byte[] data = Entry("SP", 1, [0xBE, 0xEF, 14]);

            var actual = Assert.IsType<SharingProtocolIndicator>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(new byte[] { 0xBE, 0xEF }, actual.CheckBytes);
            Assert.Equal(14, actual.BytesSkipped);
        }

        [Fact]
        public void ParseSystemUseEntry_SharingProtocolTerminator_ReadsEntry()
        {
            byte[] data = Entry("ST", 1);

            var actual = Assert.IsType<SharingProtocolTerminator>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(4, actual.Length);
        }

        [Fact]
        public void ParseSystemUseEntry_ExtensionsReference_ReadsFields()
        {
            byte[] data = Entry("ER", 1, [10, 3, 3, 1], Encoding.ASCII.GetBytes("RRIP_1991A"), Encoding.ASCII.GetBytes("DESSRC"));

            var actual = Assert.IsType<ExtensionsReference>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(1, actual.ExtensionVersion);
            Assert.Equal("RRIP_1991A", Encoding.ASCII.GetString(actual.Identifier));
            Assert.Equal("DES", Encoding.ASCII.GetString(actual.Descriptor));
            Assert.Equal("SRC", Encoding.ASCII.GetString(actual.Source));
        }

        [Fact]
        public void ParseSystemUseEntry_ExtensionSelector_ReadsFields()
        {
            byte[] data = Entry("ES", 1, [2]);

            var actual = Assert.IsType<ExtensionSelector>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(2, actual.ExtensionSequence);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ParseSystemUseEntry_PosixFileAttributes_ReadsFields(bool serialNumber)
        {
            // RRIP 1.12 records the file serial number, which makes the entry 44 bytes long instead of 36
            byte[] data = serialNumber
                ? Entry("PX", 1, BothEndian(0x41ED), BothEndian(2), BothEndian(1000), BothEndian(100), BothEndian(7))
                : Entry("PX", 1, BothEndian(0x41ED), BothEndian(2), BothEndian(1000), BothEndian(100));

            var actual = Assert.IsType<PosixFileAttributes>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(0x41EDu, actual.FileMode.LittleEndian);
            Assert.Equal(2u, actual.Links.LittleEndian);
            Assert.Equal(1000u, actual.UserId.LittleEndian);
            Assert.Equal(100u, actual.GroupId.LittleEndian);
            uint? expectedSerialNumber = serialNumber ? 7u : null;
            Assert.Equal(expectedSerialNumber, actual.SerialNumber?.LittleEndian);
        }

        [Fact]
        public void ParseSystemUseEntry_PosixDeviceNumber_ReadsFields()
        {
            byte[] data = Entry("PN", 1, BothEndian(0), BothEndian(0x0801));

            var actual = Assert.IsType<PosixDeviceNumber>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(0u, actual.DeviceNumberHigh.LittleEndian);
            Assert.Equal(0x0801u, actual.DeviceNumberLow.LittleEndian);
        }

        [Fact]
        public void ParseSystemUseEntry_SymbolicLink_ReadsComponents()
        {
            // "/usr/bin"
            byte[] data = Entry("SL", 1, [0x00, 0x08, 0], [0x00, 3], Encoding.ASCII.GetBytes("usr"), [0x00, 3], Encoding.ASCII.GetBytes("bin"));

            var actual = Assert.IsType<SymbolicLink>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal((SymbolicLinkFlags)0, actual.Flags);
            Assert.Equal(3, actual.Components.Length);
            Assert.Equal(ComponentFlags.ROOT, actual.Components[0].Flags);
            Assert.Empty(actual.Components[0].Content);
            Assert.Equal("usr", Encoding.ASCII.GetString(actual.Components[1].Content));
            Assert.Equal(3, actual.Components[2].ComponentLength);
            Assert.Equal("bin", Encoding.ASCII.GetString(actual.Components[2].Content));
        }

        [Fact]
        public void ParseSystemUseEntry_AlternateName_ReadsFields()
        {
            byte[] data = Entry("NM", 1, [0x01], Encoding.ASCII.GetBytes("Long name"));

            var actual = Assert.IsType<AlternateName>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(AlternateNameFlags.CONTINUE, actual.Flags);
            Assert.Equal("Long name", Encoding.ASCII.GetString(actual.NameContent));
        }

        [Fact]
        public void ParseSystemUseEntry_ChildLink_ReadsFields()
        {
            byte[] data = Entry("CL", 1, BothEndian(40));

            var actual = Assert.IsType<ChildLink>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(40, actual.ChildDirectoryLocation.LittleEndian);
        }

        [Fact]
        public void ParseSystemUseEntry_ParentLink_ReadsFields()
        {
            byte[] data = Entry("PL", 1, BothEndian(20));

            var actual = Assert.IsType<ParentLink>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(20, actual.ParentDirectoryLocation.LittleEndian);
        }

        [Fact]
        public void ParseSystemUseEntry_RelocatedDirectory_ReadsEntry()
        {
            byte[] data = Entry("RE", 1);

            Assert.IsType<RelocatedDirectory>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));
        }

        [Fact]
        public void ParseSystemUseEntry_ShortFormTimeStamps_ReadsTimeStamps()
        {
            // Modification, access and attribute change times
            byte[] data = Entry("TF", 1, [0x0E], [124, 11, 25, 8, 30, 0, 4], [124, 11, 26, 9, 31, 1, 4], [124, 11, 27, 10, 32, 2, 4]);

            var actual = Assert.IsType<TimeStamps>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(TimeStampFlags.MODIFY | TimeStampFlags.ACCESS | TimeStampFlags.ATTRIBUTES, actual.Flags);
            Assert.Equal(new byte[] { 25, 26, 27 }, actual.ShortFormTimeStamps.Select(t => t.Day));
            Assert.Empty(actual.LongFormTimeStamps);
        }

        [Fact]
        public void ParseSystemUseEntry_LongFormTimeStamps_ReadsTimeStamps()
        {
            byte[] data = Entry("TF", 1, [0x82], Encoding.ASCII.GetBytes("2024112508300000"), [4]);

            var actual = Assert.IsType<TimeStamps>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            var timeStamp = Assert.Single(actual.LongFormTimeStamps);
            Assert.Equal("2024", Encoding.ASCII.GetString(timeStamp.Year));
            Assert.Equal(4, timeStamp.TimezoneOffset);
            Assert.Empty(actual.ShortFormTimeStamps);
        }

        [Fact]
        public void ParseSystemUseEntry_SparseFile_ReadsFields()
        {
            byte[] data = Entry("SF", 1, BothEndian(1), BothEndian(4096), [2]);

            var actual = Assert.IsType<SparseFile>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(1u, actual.VirtualFileSizeHigh.LittleEndian);
            Assert.Equal(4096u, actual.VirtualFileSizeLow.LittleEndian);
            Assert.Equal(2, actual.TableDepth);
        }

        [Fact]
        public void ParseSystemUseEntry_ExtensionsInUse_ReadsFields()
        {
            byte[] data = Entry("RR", 1, [0x89]);

            var actual = Assert.IsType<ExtensionsInUse>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(ExtensionsInUseFlags.POSIX_FILE_ATTRIBUTES | ExtensionsInUseFlags.ALTERNATE_NAME | ExtensionsInUseFlags.TIME_STAMPS, actual.Flags);
        }

        [Fact]
        public void ParseSystemUseEntry_UnknownSignature_KeepsBytes()
        {
            // Apple extension for HFS: type, creator and Finder flags
            byte[] finderInfo = [.. Encoding.ASCII.GetBytes("TEXTttxt"), 0x01, 0x00];
            byte[] data = Entry("AA", 2, finderInfo);

            var actual = Assert.IsType<UnknownEntry>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal("AA", Encoding.ASCII.GetString(actual.Signature));
            Assert.Equal(14, actual.Length);
            Assert.Equal(2, actual.Version);
            Assert.Equal(finderInfo, actual.Data);
        }

        [Theory]
        [InlineData("CE", 27)]
        [InlineData("SP", 8)]
        [InlineData("ST", 5)]
        [InlineData("ER", 7)]
        [InlineData("ER", 9)]
        [InlineData("ES", 6)]
        [InlineData("PX", 40)]
        [InlineData("PN", 21)]
        [InlineData("SL", 4)]
        [InlineData("SL", 6)]
        [InlineData("NM", 4)]
        [InlineData("CL", 13)]
        [InlineData("PL", 11)]
        [InlineData("RE", 5)]
        [InlineData("TF", 4)]
        [InlineData("TF", 6)]
        [InlineData("SF", 12)]
        [InlineData("RR", 4)]
        public void ParseSystemUseEntry_LengthNotOfType_KeepsBytes(string signature, int length)
        {
            byte[] data = Entry(signature, 1, new byte[length - 4]);

            var actual = Assert.IsType<UnknownEntry>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(signature, Encoding.ASCII.GetString(actual.Signature));
            Assert.Equal(length, actual.Length);
            Assert.Equal(new byte[length - 4], actual.Data);
        }

        [Fact]
        public void ParseSystemUseEntry_SymbolicLinkComponentPastEnd_KeepsBytes()
        {
            byte[] data = Entry("SL", 1, [0x00, 0x00, 5], Encoding.ASCII.GetBytes("usr"));

            var actual = Assert.IsType<UnknownEntry>(ISO9660.ParseSystemUseEntry(new MemoryStream(data)));

            Assert.Equal(data.Skip(4), actual.Data);
        }

        [Theory]
        [InlineData(new byte[] { 0x50, 0x58, 0x24 })]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(new byte[] { 0x50, 0x58, 0x03, 0x01 })]
        [InlineData(new byte[] { 0x50, 0x58, 0x24, 0x01, 0x00, 0x00, 0x00, 0x00 })]
        public void ParseSystemUseEntry_EntryDoesNotFit_Null(byte[] data)
        {
            var actual = ISO9660.ParseSystemUseEntry(new MemoryStream(data));

            Assert.Null(actual);
        }

        #endregion

        #region Directory Records

        [Fact]
        public void ParseDirectoryDescriptors_SharingProtocolIndicator_SkipsBytesInHierarchy()
        {
            // The root directory at sector 20 holds FILE;1 and SUB at sector 21, every record but the root "." starts with 2 skipped bytes
            byte[] image = new byte[32 * 2048];
            byte[] skipped = [0xAA, 0xBB];
            WriteDirectory(image, 20, Concat(SharingProtocolIndicator(2), Entry("RR", 1, [0x81])),
                ("FILE;1", 30, NoFlags, Concat(skipped, PosixFileAttributes())),
                ("SUB", 21, FileFlags.DIRECTORY, Concat(skipped, Entry("NM", 1, [0x00], Encoding.ASCII.GetBytes("Sub")))));
            WriteDirectory(image, 21, Concat(skipped, Entry("NM", 1, [0x02])));

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var root = GetRecords(actual, 20);
            Assert.Equal(new[] { typeof(SharingProtocolIndicator), typeof(ExtensionsInUse) }, root[0].SystemUseEntries.Select(e => e.GetType()));
            Assert.IsType<PosixFileAttributes>(Assert.Single(root[2].SystemUseEntries));
            Assert.IsType<AlternateName>(Assert.Single(root[3].SystemUseEntries));
            var sub = GetRecords(actual, 21);
            Assert.Equal(AlternateNameFlags.CURRENT, Assert.IsType<AlternateName>(Assert.Single(sub[0].SystemUseEntries)).Flags);
        }

        [Fact]
        public void ParseDirectoryDescriptors_SharingProtocolIndicatorAfterXA_SkipsXA()
        {
            // The Continuation Area at sector 22 is only read if the directory hierarchy uses SUSP
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, Concat(XASystemUse, SharingProtocolIndicator(14), ContinuationArea(22, 0, 36)),
                ("FILE;1", 30, NoFlags, Concat(XASystemUse, PosixFileAttributes())));
            PosixFileAttributes().CopyTo(image, 22 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var root = GetRecords(actual, 20);
            Assert.Equal(new[] { typeof(SharingProtocolIndicator), typeof(ContinuationArea), typeof(PosixFileAttributes) }, root[0].SystemUseEntries.Select(e => e.GetType()));
            Assert.IsType<PosixFileAttributes>(Assert.Single(root[2].SystemUseEntries));
        }

        [Fact]
        public void ParseDirectoryDescriptors_NoSharingProtocolIndicator_ReadsEntriesAfterXA()
        {
            // Without SUSP, the Apple extension for HFS follows the CD-ROM XA system use information if there is any
            byte[] image = new byte[32 * 2048];
            byte[] appleExtension = Entry("AA", 2, Encoding.ASCII.GetBytes("TEXTttxt"), [0x00, 0x00]);
            WriteDirectory(image, 20, [],
                ("FIRST;1", 30, NoFlags, Concat(XASystemUse, appleExtension)),
                ("SECOND;1", 31, NoFlags, appleExtension));

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var root = GetRecords(actual, 20);
            Assert.Empty(root[0].SystemUseEntries);
            Assert.Equal("AA", Encoding.ASCII.GetString(Assert.IsType<UnknownEntry>(Assert.Single(root[2].SystemUseEntries)).Signature));
            Assert.Equal("AA", Encoding.ASCII.GetString(Assert.IsType<UnknownEntry>(Assert.Single(root[3].SystemUseEntries)).Signature));
        }

        [Theory]
        [InlineData(new byte[] { 0x53, 0x51, 0x07, 0x01, 0xBE, 0xEF, 0x00 })]
        [InlineData(new byte[] { 0x53, 0x50, 0x08, 0x01, 0xBE, 0xEF, 0x00, 0x00 })]
        [InlineData(new byte[] { 0x53, 0x50, 0x07, 0x01, 0xBE, 0xEE, 0x00 })]
        [InlineData(new byte[] { 0x53, 0x50, 0x04, 0x01 })]
        public void ParseDirectoryDescriptors_NoValidSharingProtocolIndicator_IgnoresContinuationArea(byte[] indicator)
        {
            // FILE;1 points to a Continuation Area at sector 22, which is only read if the directory hierarchy uses SUSP
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, indicator, ("FILE;1", 30, NoFlags, ContinuationArea(22, 0, 32)));
            Entry("NM", 1, [0x00], Encoding.ASCII.GetBytes("Long name"), new byte[18]).CopyTo(image, 22 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.IsType<ContinuationArea>(Assert.Single(GetRecords(actual, 20)[2].SystemUseEntries));
        }

        [Fact]
        public void ParseDirectoryDescriptors_ContinuationArea_ReadsItsEntriesAfterTheField()
        {
            byte[] image = new byte[32 * 2048];
            byte[] extensionsReference = Entry("ER", 1, [10, 0, 0, 1], Encoding.ASCII.GetBytes("RRIP_1991A"));
            WriteDirectory(image, 20, Concat(SharingProtocolIndicator(0), ContinuationArea(22, 100, extensionsReference.Length), Entry("RR", 1, [0x81])));
            extensionsReference.CopyTo(image, (22 * 2048) + 100);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var entries = GetRecords(actual, 20)[0].SystemUseEntries;
            Assert.Equal(new[] { typeof(SharingProtocolIndicator), typeof(ContinuationArea), typeof(ExtensionsInUse), typeof(ExtensionsReference) }, entries.Select(e => e.GetType()));
            Assert.Equal("RRIP_1991A", Encoding.ASCII.GetString(((ExtensionsReference)entries[3]).Identifier));
        }

        [Fact]
        public void ParseDirectoryDescriptors_SharingProtocolTerminator_EndsItsArea()
        {
            // The Continuation Area before the terminator is read, the entry after it is not
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, Concat(SharingProtocolIndicator(0), ContinuationArea(22, 0, 36), Entry("ST", 1), Entry("RR", 1, [0x81])));
            PosixFileAttributes().CopyTo(image, 22 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var entries = GetRecords(actual, 20)[0].SystemUseEntries;
            Assert.Equal(new[] { typeof(SharingProtocolIndicator), typeof(ContinuationArea), typeof(SharingProtocolTerminator), typeof(PosixFileAttributes) }, entries.Select(e => e.GetType()));
        }

        [Theory]
        [InlineData(22, 2040, 36)]
        [InlineData(31, 0, 36)]
        [InlineData(32, 0, 36)]
        [InlineData(-1, 0, 36)]
        public void ParseDirectoryDescriptors_ContinuationAreaNotInBlockOfVolume_NotRead(int location, int offset, int length)
        {
            // The image ends in the middle of sector 31, entries are recorded at the start of sector 21, which directly follows
            // the root directory, and at the end of sector 22, across the start of sector 23
            byte[] image = new byte[(31 * 2048) + 20];
            WriteDirectory(image, 20, Concat(SharingProtocolIndicator(0), ContinuationArea(location, offset, length)));
            PosixFileAttributes().CopyTo(image, 21 * 2048);
            PosixFileAttributes().CopyTo(image, (22 * 2048) + 2040);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var entries = GetRecords(actual, 20)[0].SystemUseEntries;
            Assert.Equal(new[] { typeof(SharingProtocolIndicator), typeof(ContinuationArea) }, entries.Select(e => e.GetType()));
        }

        [Fact]
        public void ParseDirectoryDescriptors_ContinuationAreaLoop_StopsAtLimit()
        {
            // The Continuation Area at sector 22 points to itself
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, Concat(SharingProtocolIndicator(0), ContinuationArea(22, 0, 28)));
            ContinuationArea(22, 0, 28).CopyTo(image, 22 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var entries = GetRecords(actual, 20)[0].SystemUseEntries;
            Assert.Equal(2 + 32, entries.Length);
            Assert.All(entries.Skip(1), e => Assert.IsType<ContinuationArea>(e));
        }

        [Fact]
        public void ParseDirectoryDescriptors_EntryDoesNotFit_StopsReading()
        {
            // A padding byte after an odd-length entry moves the next signature by one byte
            byte[] image = new byte[32 * 2048];
            byte[] timeStamps = Entry("TF", 1, [0x06], [124, 11, 25, 8, 30, 0, 4], [124, 11, 26, 9, 31, 1, 4]);
            byte[] systemUse = Concat(timeStamps, [0x00], Entry("PD", 1, [0x00, 0x00]));
            WriteDirectory(image, 20, SharingProtocolIndicator(0), ("FILE;1", 30, NoFlags, systemUse));

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            var file = GetRecords(actual, 20)[2];
            Assert.IsType<TimeStamps>(Assert.Single(file.SystemUseEntries));
            Assert.Equal(systemUse, file.SystemUse);
        }

        [Fact]
        public void ParseDirectoryDescriptors_SkipLongerThanSystemUse_ReadsNoEntries()
        {
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, SharingProtocolIndicator(200), ("FILE;1", 30, NoFlags, PosixFileAttributes()));

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Empty(GetRecords(actual, 20)[2].SystemUseEntries);
        }

        [Fact]
        public void ParseDirectoryDescriptors_RootShorterThanFirstRecord_ReadsEmptyDirectory()
        {
            // The root directory and its "." record give a length of 20 bytes, shorter than the "." record
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, SharingProtocolIndicator(0));
            WriteBothEndian(image, (20 * 2048) + 10, 20);
            var vd = CreatePrimaryVolumeDescriptor(20);
            vd.RootDirectoryRecord.ExtentLength = 20;

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [vd]);

            Assert.NotNull(actual);
            Assert.Empty(GetRecords(actual, 20));
        }

        #endregion

        /// <summary>
        /// Create a System Use entry with the given signature and version, followed by the given fields
        /// </summary>
        private static byte[] Entry(string signature, byte version, params byte[][] fields)
        {
            byte[] entry = [.. Encoding.ASCII.GetBytes(signature), 0, version, .. fields.SelectMany(field => field)];
            entry[2] = (byte)entry.Length;
            return entry;
        }

        /// <summary>
        /// Create a Sharing Protocol Indicator entry
        /// </summary>
        private static byte[] SharingProtocolIndicator(byte bytesSkipped)
            => Entry("SP", 1, [0xBE, 0xEF, bytesSkipped]);

        /// <summary>
        /// Create a Continuation Area entry
        /// </summary>
        private static byte[] ContinuationArea(int location, int offset, int length)
            => Entry("CE", 1, BothEndian(location), BothEndian(offset), BothEndian(length));

        /// <summary>
        /// Create a 36-byte POSIX file attributes entry of a regular file
        /// </summary>
        private static byte[] PosixFileAttributes()
            => Entry("PX", 1, BothEndian(0x81A4), BothEndian(1), BothEndian(0), BothEndian(0));

        /// <summary>
        /// Create a 32-bit value in both-endian format
        /// </summary>
        private static byte[] BothEndian(int value)
        {
            byte[] bytes = new byte[8];
            WriteBothEndian(bytes, 0, value);
            return bytes;
        }

        /// <summary>
        /// Concatenate byte arrays
        /// </summary>
        private static byte[] Concat(params byte[][] parts)
            => [.. parts.SelectMany(part => part)];

        /// <summary>
        /// Get the directory records of a parsed directory extent
        /// </summary>
        private static DirectoryRecord[] GetRecords(Dictionary<int, FileExtent> directories, int sector)
            => Assert.IsType<DirectoryExtent>(directories[sector]).DirectoryRecords;

        /// <summary>
        /// Write a one-sector directory extent with self and parent records and the given entries
        /// </summary>
        private static void WriteDirectory(byte[] image, int sector, byte[] currentSystemUse, params (string Name, int Location, FileFlags Flags, byte[] SystemUse)[] entries)
        {
            int offset = sector * 2048;
            offset = WriteRecord(image, offset, Constants.CurrentDirectory, sector, FileFlags.DIRECTORY, currentSystemUse);
            offset = WriteRecord(image, offset, Constants.ParentDirectory, sector, FileFlags.DIRECTORY, []);
            foreach (var (name, location, flags, systemUse) in entries)
            {
                offset = WriteRecord(image, offset, Encoding.ASCII.GetBytes(name), location, flags, systemUse);
            }
        }

        /// <summary>
        /// Write a directory record with the given System Use field for a one-sector extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags, byte[] systemUse)
        {
            int length = 33 + identifier.Length + (identifier.Length % 2 == 0 ? 1 : 0) + systemUse.Length;
            systemUse.CopyTo(image, offset + length - systemUse.Length);
            return WriteRecord(image, offset, identifier, location, flags, length);
        }
    }
}
