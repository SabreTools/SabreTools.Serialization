using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Data.Models.ISO9660.SystemUseEntries;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ModelBuilder;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660RockRidgeTests
    {
        /// <summary>
        /// Block of the root directory of the primary volume descriptor
        /// </summary>
        private const int RootDirectory = 20;

        /// <summary>
        /// Block of the root directory of the supplementary volume descriptor
        /// </summary>
        private const int JolietRootDirectory = 21;

        [Fact]
        public void Extract_RockRidgeNames_UsedForFilesAndDirectories()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("READ_ME.TXT", 25), Name("Read me;1 \u00FC.txt")),
                    WithEntries(DirectoryEntry("SUB_DIR", 30), Name("Sub dir"))),
                [30] = Listing(30, RootDirectory, WithEntries(FileEntry("FILE.TXT", 26), Name("file.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "Read me;1 \u00FC.txt", "Sub dir", "Sub dir/file.txt" }, Entries(outputDirectory));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(outputDirectory, "Read me;1 \u00FC.txt")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_RockRidgeNameInSeveralEntries_JoinsThem()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("LONGNAME.TXT", 25), Name("a long ", AlternateNameFlags.CONTINUE), Name("name.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "a long name.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData(AlternateNameFlags.CURRENT)]
        [InlineData(AlternateNameFlags.PARENT)]
        [InlineData(AlternateNameFlags.HOST)]
        [InlineData(AlternateNameFlags.CONTINUE | AlternateNameFlags.CURRENT)]
        public void Extract_RockRidgeNameEntryWithFlags_Skipped(AlternateNameFlags flags)
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("FILE.TXT", 25), Name("skipped", flags), Name("file.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "file.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyRockRidgeName_UsesFileIdentifier()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(FileEntry("FILE.TXT", 25), Name(string.Empty))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_UnsafeRockRidgeName_ReplacesCharacters()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("FILE.TXT", 25), Name("../FILE.TXT")),
                    WithEntries(DirectoryEntry("DIR", 30), Name("../DIR"))),
                [30] = Listing(30, RootDirectory, FileEntry("OTHER.TXT", 26)),
            };

            string outputDirectory = Extract(CreateVolume(directories));
            string parentDirectory = Path.GetDirectoryName(outputDirectory)!;

            Assert.Equal(new[] { "out", "out/.._DIR", "out/.._DIR/OTHER.TXT", "out/.._FILE.TXT" }, Entries(parentDirectory));
            Directory.Delete(parentDirectory, recursive: true);
        }

        [Fact]
        public void Extract_NoSharingProtocolIndicator_IgnoresRockRidgeEntries()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory,
                    WithEntries(FileEntry("FILE.TXT", 25), Name("file.txt"), Link(Component("target"))),
                    WithEntries(FileEntry("LINK", 26), new ChildLink { ChildDirectoryLocation = 30 }),
                    WithEntries(DirectoryEntry("MOVED", 30), new RelocatedDirectory())),
                [30] = Listing(30, RootDirectory, FileEntry("OTHER.TXT", 27)),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "FILE.TXT", "LINK", "MOVED", "MOVED/OTHER.TXT" }, Entries(outputDirectory));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(outputDirectory, "FILE.TXT")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData(0xBE, 0xEE)]
        [InlineData(0xEF, 0xBE)]
        public void Extract_SharingProtocolIndicatorWithOtherCheckBytes_IgnoresRockRidgeNames(byte first, byte second)
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RootListing(RootDirectory, [new SharingProtocolIndicator { CheckBytes = [first, second] }],
                    WithEntries(FileEntry("FILE.TXT", 25), Name("file.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SharingProtocolIndicatorNotFirstEntry_IgnoresRockRidgeNames()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RootListing(RootDirectory, [new UnknownEntry(), SharingProtocol()],
                    WithEntries(FileEntry("FILE.TXT", 25), Name("file.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_RootDirectoryWithoutRecords_ExtractsNothing()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = new DirectoryExtent(),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Empty(Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SupplementaryVolumeDescriptorWithRockRidgeNames_UsesFileIdentifiers()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, FileEntry("FILE.TXT", 25)),
                [JolietRootDirectory] = RockRidgeListing(JolietRootDirectory,
                    WithEntries(FileEntry("Joliet.txt", 25, Encoding.BigEndianUnicode), Name("rock ridge.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories, joliet: true));

            Assert.Equal(new[] { "Joliet.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_PrimaryVolumeDescriptorWithRockRidgeNames_ExtractedBeforeJoliet()
        {
            // Only the file at block 27 is not recorded in the primary volume descriptor
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("LONG_NAM.TXT", 25), Name("long name.txt")),
                    FileEntry("NO_NAME.TXT", 26)),
                [JolietRootDirectory] = Listing(JolietRootDirectory, JolietRootDirectory,
                    FileEntry("Long Name.txt", 25, Encoding.BigEndianUnicode),
                    FileEntry("No Name.txt", 26, Encoding.BigEndianUnicode),
                    FileEntry("Only Joliet.txt", 27, Encoding.BigEndianUnicode)),
            };

            string outputDirectory = Extract(CreateVolume(directories, joliet: true));

            Assert.Equal(new[] { "NO_NAME.TXT", "Only Joliet.txt", "long name.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SharingProtocolIndicatorWithoutRockRidgeNames_ExtractsJolietFirst()
        {
            // Apple hybrid discs use the System Use Sharing Protocol for the Apple extensions, without Rock Ridge names
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, FileEntry("FILE.TXT", 25)),
                [JolietRootDirectory] = Listing(JolietRootDirectory, JolietRootDirectory, FileEntry("Long File Name.txt", 25, Encoding.BigEndianUnicode)),
            };

            string outputDirectory = Extract(CreateVolume(directories, joliet: true));

            Assert.Equal(new[] { "Long File Name.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_RelocatedDirectory_ExtractedInPlaceOfChildLink()
        {
            // DEEP was relocated to RR_MOVED, and the file DEEP links to it, like mkisofs records directories deeper than 8 levels
            var childLink = new DirectoryRecord { ExtentLocation = 40, FileIdentifier = Encoding.ASCII.GetBytes("DEEP") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(childLink, Name("deep"), new ChildLink { ChildDirectoryLocation = 31 }),
                    WithEntries(DirectoryEntry("RR_MOVED", 30), Name("rr_moved"))),
                [30] = Listing(30, RootDirectory, WithEntries(DirectoryEntry("DEEP", 31), Name("deep"), new RelocatedDirectory())),
                [31] = Listing(31, 30, WithEntries(FileEntry("FILE.TXT", 25), Name("file.txt"))),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "RR_MOVED"), PathTableEntry(31, 2, "DEEP")];

            string outputDirectory = Extract(CreateVolume(directories, pathTable: pathTable));

            Assert.Equal(new[] { "deep", "deep/file.txt", "rr_moved" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SymbolicLinkToParentDirectory_WritesRelativePath()
            => AssertLinkTarget("../lib", Link(Component(ComponentFlags.PARENT), Component("lib")));

        [Fact]
        public void Extract_SymbolicLinkToCurrentDirectory_WritesRelativePath()
            => AssertLinkTarget("./run", Link(Component(ComponentFlags.CURRENT), Component("run")));

        [Fact]
        public void Extract_SymbolicLinkToRootDirectory_WritesAbsolutePath()
            => AssertLinkTarget("/usr/bin", Link(Component(ComponentFlags.ROOT), Component("usr"), Component("bin")));

        [Fact]
        public void Extract_SymbolicLinkToOnlyRootDirectory_WritesRootDirectory()
            => AssertLinkTarget("/", Link(Component(ComponentFlags.ROOT)));

        [Fact]
        public void Extract_SymbolicLinkComponentWithOtherFlags_WritesOnlySeparator()
            => AssertLinkTarget("/name", Link(Component(ComponentFlags.VOLROOT), Component("name")));

        [Fact]
        public void Extract_SymbolicLinkWithContinuedComponent_JoinsComponent()
            => AssertLinkTarget("long name", Link(Component("long ", ComponentFlags.CONTINUE), Component("name")));

        [Fact]
        public void Extract_SymbolicLinkInSeveralEntries_SeparatesComponents()
            => AssertLinkTarget("usr/bin", Link(SymbolicLinkFlags.CONTINUE, Component("usr")), Link(Component("bin")));

        [Fact]
        public void Extract_SymbolicLinkInSeveralEntriesWithContinuedComponent_JoinsComponent()
            => AssertLinkTarget("name", Link(SymbolicLinkFlags.CONTINUE, Component("na", ComponentFlags.CONTINUE)), Link(Component("me")));

        [Fact]
        public void Extract_SymbolicLinkInSeveralEntriesAfterRootDirectory_WritesAbsolutePath()
            => AssertLinkTarget("/usr", Link(SymbolicLinkFlags.CONTINUE, Component(ComponentFlags.ROOT)), Link(Component("usr")));

        [Fact]
        public void Extract_SymbolicLinkWithFileData_WritesOnlyPath()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(FileEntry("LINK", 25), Name("link"), Link(Component("target")))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(Encoding.ASCII.GetBytes("target"), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SymbolicLinkWithExtentPastEnd_WritesPath()
        {
            // The extent of a symbolic link holds no data, so it can run past the last of the 64 blocks
            var link = new DirectoryRecord { ExtentLocation = 63, ExtentLength = 2 * BlockLength, FileIdentifier = Encoding.ASCII.GetBytes("LINK.;1") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(link, Name("link"), Link(Component("target")))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(Encoding.ASCII.GetBytes("target"), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SymbolicLinkBeforeFileOfSameExtent_ExtractsBoth()
        {
            // Like mkisofs, the empty extent of the symbolic link is the extent of the file after it
            var link = new DirectoryRecord { ExtentLocation = 25, FileIdentifier = Encoding.ASCII.GetBytes("LINK.;1") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(link, Name("link"), Link(Component("file.txt"))),
                    WithEntries(FileEntry("FILE.TXT", 25), Name("file.txt"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "file.txt", "link" }, Entries(outputDirectory));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(outputDirectory, "file.txt")));
            Assert.Equal(Encoding.ASCII.GetBytes("file.txt"), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_JolietRecordOfSymbolicLink_Skipped()
        {
            // Like mkisofs, the Joliet directory records the symbolic link as an empty file of the same name
            var link = new DirectoryRecord { ExtentLocation = 25, FileIdentifier = Encoding.ASCII.GetBytes("LINK.;1") };
            var jolietLink = new DirectoryRecord { ExtentLocation = 25, FileIdentifier = Encoding.BigEndianUnicode.GetBytes("link") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(link, Name("link"), Link(Component("target")))),
                [JolietRootDirectory] = Listing(JolietRootDirectory, JolietRootDirectory, jolietLink),
            };

            string outputDirectory = Extract(CreateVolume(directories, joliet: true));

            Assert.Equal(new[] { "link" }, Entries(outputDirectory));
            Assert.Equal(Encoding.ASCII.GetBytes("target"), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AgainAfterOutputRemoved_WritesSymbolicLinkAgain()
        {
            var link = new DirectoryRecord { ExtentLocation = 25, FileIdentifier = Encoding.ASCII.GetBytes("LINK.;1") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(link, Name("link"), Link(Component("target")))),
            };

            var iso = new ISO9660(CreateVolume(directories), CreateData());
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "out");
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));
            Directory.Delete(outputDirectory, recursive: true);
            Directory.CreateDirectory(outputDirectory);

            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            Assert.Equal(Encoding.ASCII.GetBytes("target"), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_RockRidgeNameTaken_UsesFileIdentifier()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("A.TXT", 25), Name("same")),
                    WithEntries(FileEntry("B.TXT", 26), Name("same")),
                    WithEntries(DirectoryEntry("C", 30), Name("same")),
                    WithEntries(DirectoryEntry("D", 31), Name("folder")),
                    WithEntries(DirectoryEntry("E", 32), Name("folder"))),
                [30] = Listing(30, RootDirectory, FileEntry("F.TXT", 27)),
                [31] = Listing(31, RootDirectory, FileEntry("G.TXT", 28)),
                [32] = Listing(32, RootDirectory, FileEntry("H.TXT", 29)),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            string[] expected = ["B.TXT", "C", "C/F.TXT", "E", "E/H.TXT", "folder", "folder/G.TXT", "same"];
            Assert.Equal(expected, Entries(outputDirectory));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(outputDirectory, "same")));
            Assert.Equal(ExpectedData(26), File.ReadAllBytes(Path.Combine(outputDirectory, "B.TXT")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_RockRidgeNamesDifferingInCase_ExtractsBoth()
        {
            // On a file system that ignores case, the second name is taken by the first
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("A.TXT", 25), Name("Name")),
                    WithEntries(FileEntry("B.TXT", 26), Name("name"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            string[] expected = IgnoresCase(Path.GetDirectoryName(outputDirectory)!) ? ["B.TXT", "Name"] : ["Name", "name"];
            Assert.Equal(expected, Entries(outputDirectory));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(outputDirectory, "Name")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_FileInMultipleExtentsWithRockRidgeName_JoinsExtents()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory,
                    WithEntries(FileEntry("BIG.BIN", 25, flags: FileFlags.MULTI_EXTENT), Name("big.bin")),
                    WithEntries(FileEntry("BIG.BIN", 26), Name("big.bin"))),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "big.bin" }, Entries(outputDirectory));
            byte[] expectedData = [.. ExpectedData(25), .. ExpectedData(26)];
            Assert.Equal(expectedData, File.ReadAllBytes(Path.Combine(outputDirectory, "big.bin")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        #region Helpers

        /// <summary>
        /// Assert that a symbolic link with the given entries is extracted as a file with the path it points to
        /// </summary>
        /// <remarks>Like the symbolic links of mkisofs, the directory record has an empty extent</remarks>
        private static void AssertLinkTarget(string expected, params SymbolicLink[] entries)
        {
            var link = new DirectoryRecord { ExtentLocation = 25, FileIdentifier = Encoding.ASCII.GetBytes("LINK.;1") };
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = RockRidgeListing(RootDirectory, WithEntries(link, [Name("link"), .. entries])),
            };

            string outputDirectory = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "link" }, Entries(outputDirectory));
            Assert.Equal(Encoding.ASCII.GetBytes(expected), File.ReadAllBytes(Path.Combine(outputDirectory, "link")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        /// <summary>
        /// Extract a volume whose blocks are filled with the low byte of their block number into the directory "out"
        /// of a new directory, and return the directory "out"
        /// </summary>
        private static string Extract(Volume volume)
        {
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "out");
            Directory.CreateDirectory(outputDirectory);
            Assert.True(new ISO9660(volume, CreateData()).Extract(outputDirectory, includeDebug: false));
            return outputDirectory;
        }

        /// <summary>
        /// Create a volume with a primary volume descriptor, optionally a supplementary volume descriptor,
        /// their directories and an optional path table of the primary volume descriptor
        /// </summary>
        private static Volume CreateVolume(Dictionary<int, FileExtent> directories, bool joliet = false, PathTableRecord[]? pathTable = null)
        {
            VolumeDescriptor[] volumeDescriptors =
            [
                new PrimaryVolumeDescriptor
                {
                    LogicalBlockSize = BlockLength,
                    RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, RootDirectory),
                },
            ];

            if (joliet)
            {
                volumeDescriptors =
                [
                    .. volumeDescriptors,
                    new SupplementaryVolumeDescriptor
                    {
                        LogicalBlockSize = BlockLength,
                        RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, JolietRootDirectory),
                    },
                ];
            }

            return new Volume
            {
                VolumeDescriptorSet = volumeDescriptors,
                PathTableGroups = pathTable is null ? [] : [new PathTableGroup { PathTableL = pathTable }],
                DirectoryDescriptors = directories,
            };
        }

        /// <summary>
        /// Indicates if the file system of a directory ignores the case of names
        /// </summary>
        private static bool IgnoresCase(string directory)
        {
            string path = Path.Combine(directory, "probe");
            File.WriteAllBytes(path, []);
            bool ignoresCase = File.Exists(Path.Combine(directory, "PROBE"));
            File.Delete(path);
            return ignoresCase;
        }

        /// <summary>
        /// Root directory extent whose "." record holds the given System Use entries
        /// </summary>
        private static DirectoryExtent RootListing(int location, BaseEntry[] entries, params DirectoryRecord[] records)
        {
            var listing = Listing(location, location, records);
            listing.DirectoryRecords[0].SystemUseEntries = entries;
            return listing;
        }

        /// <summary>
        /// Root directory extent of a directory hierarchy that uses the System Use Sharing Protocol
        /// </summary>
        private static DirectoryExtent RockRidgeListing(int location, params DirectoryRecord[] records)
            => RootListing(location, [SharingProtocol()], records);

        /// <summary>
        /// Sharing Protocol Indicator with the check bytes of SUSP
        /// </summary>
        private static SharingProtocolIndicator SharingProtocol()
            => new() { CheckBytes = [0xBE, 0xEF] };

        /// <summary>
        /// Directory record with the given System Use entries
        /// </summary>
        private static DirectoryRecord WithEntries(DirectoryRecord dr, params BaseEntry[] entries)
        {
            dr.SystemUseEntries = entries;
            return dr;
        }

        /// <summary>
        /// Rock Ridge "NM" entry with a name in UTF-8
        /// </summary>
        private static AlternateName Name(string name, AlternateNameFlags flags = 0)
            => new() { Flags = flags, NameContent = Encoding.UTF8.GetBytes(name) };

        /// <summary>
        /// Rock Ridge "SL" entry with the given component records
        /// </summary>
        private static SymbolicLink Link(params SymbolicLinkComponent[] components)
            => Link(0, components);

        /// <summary>
        /// Rock Ridge "SL" entry with flags and the given component records
        /// </summary>
        private static SymbolicLink Link(SymbolicLinkFlags flags, params SymbolicLinkComponent[] components)
            => new() { Flags = flags, Components = components };

        /// <summary>
        /// Component record of a symbolic link with a name
        /// </summary>
        private static SymbolicLinkComponent Component(string content, ComponentFlags flags = 0)
            => new() { Flags = flags, ComponentLength = (byte)content.Length, Content = Encoding.ASCII.GetBytes(content) };

        /// <summary>
        /// Component record of a symbolic link without a name, like the root directory
        /// </summary>
        private static SymbolicLinkComponent Component(ComponentFlags flags)
            => new() { Flags = flags };

        #endregion
    }
}
