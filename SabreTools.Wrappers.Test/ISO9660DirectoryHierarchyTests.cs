using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ModelBuilder;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660DirectoryHierarchyTests
    {
        /// <summary>
        /// Block of the root directory of the primary volume descriptor
        /// </summary>
        private const int PrimaryRootDirectory = 20;

        /// <summary>
        /// Block of the root directory of the supplementary volume descriptor
        /// </summary>
        private const int SupplementaryRootDirectory = 21;

        /// <summary>
        /// Encoding of the identifiers in the supplementary volume descriptor
        /// </summary>
        private static readonly Encoding Unicode = Encoding.BigEndianUnicode;

        [Fact]
        public void Extract_DirectoryWithFilesOfEarlierHierarchy_IsNotCreated()
        {
            // The supplementary volume descriptor is extracted first, it records BGM as Background music and INTRO as Intro
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("BGM", 30)),
                [30] = Listing(30, PrimaryRootDirectory, DirectoryEntry("INTRO", 31)),
                [31] = Listing(31, 30, FileEntry("START.WAV", 41)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Background music", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, DirectoryEntry("Intro", 33, Unicode)),
                [33] = Listing(33, 32, FileEntry("Start.wav", 41, Unicode)),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.Equal(new[] { "Background music", "Background music/Intro", "Background music/Intro/Start.wav" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_DirectoryWithOtherFile_IsCreated()
        {
            // EXTRA.TXT is only recorded in the primary volume descriptor
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("BGM", 30)),
                [30] = Listing(30, PrimaryRootDirectory, FileEntry("EXTRA.TXT", 42), FileEntry("THEME.WAV", 40)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Background music", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, FileEntry("Theme.wav", 40, Unicode)),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.Equal(new[] { "BGM", "BGM/EXTRA.TXT", "Background music", "Background music/Theme.wav" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyDirectory_IsCreated()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("EMPTY", 30)),
                [30] = Listing(30, PrimaryRootDirectory),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Empty folder", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.Equal(new[] { "EMPTY", "Empty folder" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_DirectoriesSharingFilesInOneHierarchy_CreatesEach()
        {
            // A and B list the same files
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("A", 30), DirectoryEntry("B", 30)),
                [30] = Listing(30, PrimaryRootDirectory, FileEntry("FILE.TXT", 40)),
            };

            var volume = CreateVolume(directories);
            volume.VolumeDescriptorSet = [volume.VolumeDescriptorSet[0]];

            string outputDirectory = Extract(new ISO9660(volume, CreateData()));

            Assert.Equal(new[] { "A", "A/FILE.TXT", "B" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AmbiguousDirectoryLocation_IsCreated()
        {
            // The big-endian location of BGM points to block 31, which holds EXTRA.WAV
            var bgm = DirectoryEntry("BGM", 30);
            bgm.ExtentLocation = new BothInt32(30, 31);
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, bgm),
                [30] = Listing(30, PrimaryRootDirectory, FileEntry("THEME.WAV", 40)),
                [31] = Listing(31, PrimaryRootDirectory, FileEntry("EXTRA.WAV", 41)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Background music", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, FileEntry("Theme.wav", 40, Unicode)),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.Equal(new[] { "BGM", "BGM/EXTRA.WAV", "Background music", "Background music/Theme.wav" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_DirectoryOnlyInPathTableWithFilesOfEarlierHierarchy_IsNotCreated()
        {
            // The root directory of the primary volume descriptor does not hold HIDDEN at block 30
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory),
                [30] = Listing(30, PrimaryRootDirectory, FileEntry("FILE.TXT", 40)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Hidden files", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, FileEntry("File.txt", 40, Unicode)),
            };

            var volume = CreateVolume(directories);
            volume.PathTableGroups = [new PathTableGroup { PathTableL = [PathTableEntry(PrimaryRootDirectory, 1, "\0"), PathTableEntry(30, 1, "HIDDEN")] }];

            string outputDirectory = Extract(new ISO9660(volume, CreateData()));

            Assert.Equal(new[] { "Hidden files", "Hidden files/File.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_DirectoryWithAssociatedFileOfEarlierHierarchy_IsCreated()
        {
            // The supplementary volume descriptor records the associated file of README as the file Read me in resource.frk
            var associatedFile = FileEntry("README", 41);
            associatedFile.FileFlags = FileFlags.ASSOCIATED_FILE;
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("MACAPP", 30)),
                [30] = Listing(30, PrimaryRootDirectory, associatedFile, FileEntry("README", 40)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Mac application", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, FileEntry("Read me", 40, Unicode), DirectoryEntry("resource.frk", 33, Unicode)),
                [33] = Listing(33, 32, FileEntry("Read me", 41, Unicode)),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.True(Directory.Exists(Path.Combine(outputDirectory, "MACAPP")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_Again_CreatesTheSameDirectories()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("BGM", 30)),
                [30] = Listing(30, PrimaryRootDirectory, FileEntry("THEME.WAV", 40)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, DirectoryEntry("Background music", 32, Unicode)),
                [32] = Listing(32, SupplementaryRootDirectory, FileEntry("Theme.wav", 40, Unicode)),
            };

            var iso = new ISO9660(CreateVolume(directories), CreateData());
            string firstOutputDirectory = Extract(iso);
            string secondOutputDirectory = Extract(iso);

            Assert.Equal(new[] { "Background music", "Background music/Theme.wav" }, Entries(firstOutputDirectory));
            Assert.Equal(Entries(firstOutputDirectory), Entries(secondOutputDirectory));
            Directory.Delete(firstOutputDirectory, recursive: true);
            Directory.Delete(secondOutputDirectory, recursive: true);
        }

        #region Helpers

        /// <summary>
        /// Extract a volume into a new directory
        /// </summary>
        private static string Extract(ISO9660 iso)
        {
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));
            return outputDirectory;
        }

        /// <summary>
        /// Create a volume with a primary and a supplementary volume descriptor and their directories
        /// </summary>
        private static Volume CreateVolume(Dictionary<int, FileExtent> directories)
        {
            return new Volume
            {
                VolumeDescriptorSet =
                [
                    new PrimaryVolumeDescriptor
                    {
                        LogicalBlockSize = BlockLength,
                        RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, PrimaryRootDirectory),
                    },
                    new SupplementaryVolumeDescriptor
                    {
                        LogicalBlockSize = BlockLength,
                        RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, SupplementaryRootDirectory),
                    },
                ],
                PathTableGroups = [],
                DirectoryDescriptors = directories,
            };
        }

        #endregion
    }
}
