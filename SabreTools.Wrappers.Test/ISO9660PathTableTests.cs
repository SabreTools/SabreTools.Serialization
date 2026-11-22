using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ModelBuilder;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660PathTableTests
    {
        /// <summary>
        /// Block of the root directory of the primary volume descriptor
        /// </summary>
        private const int RootDirectory = 20;

        [Fact]
        public void Extract_DirectoryOnlyInPathTable_ExtractsItsHierarchy()
        {
            // The root directory does not hold HIDDEN at block 30, which holds SUB at block 31,
            // and block 40 is not a parsed directory
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, FileEntry("ROOT.TXT", 25)),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26), DirectoryEntry("SUB", 31)),
                [31] = Listing(31, 30, FileEntry("DEEP.TXT", 27)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "HIDDEN"), PathTableEntry(40, 1, "GONE"), PathTableEntry(31, 2, "SUB")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Equal(new[] { "HIDDEN", "HIDDEN/FILE.TXT", "HIDDEN/SUB", "HIDDEN/SUB/DEEP.TXT", "ROOT.TXT" }, Entries(outputDirectory));
            Assert.Equal(ExpectedData(26), File.ReadAllBytes(Path.Combine(outputDirectory, "HIDDEN", "FILE.TXT")));
            Assert.Equal(ExpectedData(27), File.ReadAllBytes(Path.Combine(outputDirectory, "HIDDEN", "SUB", "DEEP.TXT")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_ReachedDirectoryUnderOtherName_ExtractsItOnce()
        {
            // The path table records SUB as OTHER
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, DirectoryEntry("SUB", 30)),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "OTHER")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Equal(new[] { "SUB", "SUB/FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SubdirectoryUnderOtherName_ExtractsItOnce()
        {
            // The path table records SUB in HIDDEN as OTHER
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [30] = Listing(30, RootDirectory, DirectoryEntry("SUB", 31)),
                [31] = Listing(31, 30, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "HIDDEN"), PathTableEntry(31, 2, "OTHER")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Equal(new[] { "HIDDEN", "HIDDEN/SUB", "HIDDEN/SUB/FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AmbiguousRootDirectoryLocation_ExtractsReachedDirectoryOnce()
        {
            // The big-endian root directory location points to block 21, which holds SUB
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [21] = Listing(21, 21, DirectoryEntry("SUB", 30)),
                [30] = Listing(30, 21, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "OTHER")];
            var volume = CreateVolume(RootDirectory, directories, pathTable);
            ((PrimaryVolumeDescriptor)volume.VolumeDescriptorSet[0]).RootDirectoryRecord.ExtentLocation = new BothInt32(RootDirectory, 21);

            string outputDirectory = Extract(volume);

            Assert.Equal(new[] { "SUB", "SUB/FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AmbiguousSubdirectoryLocation_ExtractsReachedDirectoryOnce()
        {
            // The big-endian location of SUB points to block 31, which holds FILE.TXT
            var sub = DirectoryEntry("SUB", 30);
            sub.ExtentLocation = new BothInt32(30, 31);
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, sub),
                [30] = Listing(30, RootDirectory),
                [31] = Listing(31, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "SUB"), PathTableEntry(31, 1, "OTHER")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Equal(new[] { "SUB", "SUB/FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_InvalidParentDirectoryNumber_Ignored()
        {
            // ZERO and FAR have no parent directory, so CHILD in ZERO has none either
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [30] = Listing(30, RootDirectory, FileEntry("A.TXT", 25)),
                [31] = Listing(31, RootDirectory, FileEntry("B.TXT", 26)),
                [32] = Listing(32, 30, FileEntry("C.TXT", 27)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 0, "ZERO"), PathTableEntry(31, 99, "FAR"), PathTableEntry(32, 2, "CHILD")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Empty(Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_PathTableEntryOfFileExtent_Ignored()
        {
            // Block 40 holds the extended attribute record of a file
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [40] = new FileExtent(),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(40, 1, "FILE")];

            string outputDirectory = Extract(CreateVolume(RootDirectory, directories, pathTable));

            Assert.Empty(Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_SupplementaryVolumeDescriptor_UsesItsOwnPathTable()
        {
            // The supplementary volume descriptor has its root directory at block 21 and Secret at block 31
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [21] = Listing(21, 21),
                [30] = Listing(30, RootDirectory, FileEntry("A.TXT", 25)),
                [31] = Listing(31, 21, FileEntry("b.txt", 26, Encoding.BigEndianUnicode)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "HIDDEN")];
            PathTableRecord[] unicodePathTable = [PathTableEntry(21, 1, "\0"), PathTableEntry(31, 1, "Secret", Encoding.BigEndianUnicode)];
            var volume = CreateVolume(RootDirectory, directories, pathTable);
            var svd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = BlockLength,
                RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, 21),
            };

            volume.VolumeDescriptorSet = [.. volume.VolumeDescriptorSet, svd];
            volume.PathTableGroups = [.. volume.PathTableGroups, new PathTableGroup { PathTableL = unicodePathTable }];

            string outputDirectory = Extract(volume);

            Assert.Equal(new[] { "HIDDEN", "HIDDEN/A.TXT", "Secret", "Secret/b.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_Twice_ExtractsDirectoryOnlyInPathTableAgain()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, "HIDDEN")];
            var iso = CreateWrapper(CreateVolume(RootDirectory, directories, pathTable));

            string firstOutputDirectory = Extract(iso);
            string secondOutputDirectory = Extract(iso);

            Assert.Equal(new[] { "HIDDEN", "HIDDEN/FILE.TXT" }, Entries(firstOutputDirectory));
            Assert.Equal(new[] { "HIDDEN", "HIDDEN/FILE.TXT" }, Entries(secondOutputDirectory));
            Directory.Delete(firstOutputDirectory, recursive: true);
            Directory.Delete(secondOutputDirectory, recursive: true);
        }

        #region Helpers

        /// <summary>
        /// Extract a volume whose blocks are filled with the low byte of their block number into a new directory
        /// </summary>
        private static string Extract(Volume volume)
            => Extract(CreateWrapper(volume));

        /// <summary>
        /// Extract a wrapper into a new directory
        /// </summary>
        private static string Extract(ISO9660 iso)
        {
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            iso.Extract(outputDirectory, includeDebug: false);
            return outputDirectory;
        }

        /// <summary>
        /// Create a wrapper of a volume whose blocks are filled with the low byte of their block number
        /// </summary>
        private static ISO9660 CreateWrapper(Volume volume)
            => new(volume, CreateData());

        /// <summary>
        /// Create a volume with a primary volume descriptor, its directories and one path table
        /// </summary>
        private static Volume CreateVolume(int rootDirectory, Dictionary<int, FileExtent> directories, PathTableRecord[] pathTable)
        {
            return new Volume
            {
                VolumeDescriptorSet =
                [
                    new PrimaryVolumeDescriptor
                    {
                        LogicalBlockSize = BlockLength,
                        RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, rootDirectory),
                    },
                ],
                PathTableGroups = [new PathTableGroup { PathTableL = pathTable }],
                DirectoryDescriptors = directories,
            };
        }

        #endregion
    }
}
