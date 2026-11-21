using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660DirectoryHierarchyTests
    {
        private const int BlockLength = 2048;

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

            Assert.Equal(new[] { "A", "A/FILE.TXT", "B", "B/FILE.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyFileSharingLocationInOneHierarchy_ExtractsEach()
        {
            // EMPTY.TXT is recorded with the location of README.TXT
            var emptyFile = FileEntry("EMPTY.TXT", 40);
            emptyFile.ExtentLength = 0;
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, emptyFile, FileEntry("README.TXT", 40)),
            };

            var volume = CreateVolume(directories);
            volume.VolumeDescriptorSet = [volume.VolumeDescriptorSet[0]];

            string outputDirectory = Extract(new ISO9660(volume, CreateData()));

            Assert.Equal(new[] { "EMPTY.TXT", "README.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_FileRecordedTwiceInOneHierarchy_IsExtractedOnce()
        {
            // The root directory lists README.TXT twice
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, FileEntry("README.TXT", 40), FileEntry("README.TXT", 40)),
            };

            var volume = CreateVolume(directories);
            volume.VolumeDescriptorSet = [volume.VolumeDescriptorSet[0]];

            string outputDirectory = Extract(new ISO9660(volume, CreateData()));

            Assert.Equal(new[] { "README.TXT" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyFileSharingLocationInEarlierHierarchy_IsNotExtractedAgain()
        {
            // Both volume descriptors record an empty file with the location of the file after it
            var emptyFile = FileEntry("EMPTY.TXT", 40);
            emptyFile.ExtentLength = 0;
            var unicodeEmptyFile = FileEntry("Empty file.txt", 40, Unicode);
            unicodeEmptyFile.ExtentLength = 0;
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, emptyFile, FileEntry("README.TXT", 40)),
                [SupplementaryRootDirectory] = Listing(SupplementaryRootDirectory, SupplementaryRootDirectory, unicodeEmptyFile, FileEntry("Read me.txt", 40, Unicode)),
            };

            string outputDirectory = Extract(new ISO9660(CreateVolume(directories), CreateData()));

            Assert.Equal(new[] { "Empty file.txt", "Read me.txt" }, Entries(outputDirectory));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyFileSharingLocationWithInterleavedFile_IsExtracted()
        {
            // EMPTY.TXT is recorded with the location of MOVIE.STR
            var emptyFile = FileEntry("EMPTY.TXT", 40);
            emptyFile.ExtentLength = 0;
            var interleavedFile = FileEntry("MOVIE.STR", 40);
            interleavedFile.FileUnitSize = 1;
            interleavedFile.InterleaveGapSize = 1;
            var directories = new Dictionary<int, FileExtent>
            {
                [PrimaryRootDirectory] = Listing(PrimaryRootDirectory, PrimaryRootDirectory, DirectoryEntry("MOVIES", 30)),
                [30] = Listing(30, PrimaryRootDirectory, emptyFile, interleavedFile),
            };

            var volume = CreateVolume(directories);
            volume.VolumeDescriptorSet = [volume.VolumeDescriptorSet[0]];

            string outputDirectory = Extract(new ISO9660(volume, CreateData()));

            Assert.True(File.Exists(Path.Combine(outputDirectory, "MOVIES", "EMPTY.TXT")));
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
        /// Data of 64 blocks that are filled with the low byte of their block number
        /// </summary>
        private static byte[] CreateData()
        {
            byte[] data = new byte[64 * BlockLength];
            for (int block = 0; block < 64; block++)
            {
                data.AsSpan(block * BlockLength, BlockLength).Fill((byte)block);
            }

            return data;
        }

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
        /// Paths of all extracted files and directories, relative to the output directory and sorted
        /// </summary>
        private static string[] Entries(string outputDirectory)
            => [.. Directory.GetFileSystemEntries(outputDirectory, "*", SearchOption.AllDirectories)
                .Select(path => Path.GetRelativePath(outputDirectory, path).Replace(Path.DirectorySeparatorChar, '/'))
                .OrderBy(path => path, StringComparer.Ordinal)];

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

        /// <summary>
        /// Directory extent with current and parent directory records and the given records
        /// </summary>
        private static DirectoryExtent Listing(int location, int parent, params DirectoryRecord[] records)
            => new() { DirectoryRecords = [DirectoryEntry(Constants.CurrentDirectory, location), DirectoryEntry(Constants.ParentDirectory, parent), .. records] };

        /// <summary>
        /// Directory record of a subdirectory
        /// </summary>
        private static DirectoryRecord DirectoryEntry(string name, int location, Encoding? encoding = null)
            => DirectoryEntry((encoding ?? Encoding.ASCII).GetBytes(name), location);

        /// <summary>
        /// Directory record of a subdirectory
        /// </summary>
        private static DirectoryRecord DirectoryEntry(byte[] identifier, int location)
        {
            return new DirectoryRecord
            {
                ExtentLocation = location,
                ExtentLength = BlockLength,
                FileFlags = FileFlags.DIRECTORY,
                FileIdentifier = identifier,
            };
        }

        /// <summary>
        /// Directory record of a 10-byte file
        /// </summary>
        private static DirectoryRecord FileEntry(string name, int location, Encoding? encoding = null)
        {
            return new DirectoryRecord
            {
                ExtentLocation = location,
                ExtentLength = 10,
                FileIdentifier = (encoding ?? Encoding.ASCII).GetBytes($"{name};1"),
            };
        }

        /// <summary>
        /// Path table record of a directory
        /// </summary>
        private static PathTableRecord PathTableEntry(int location, short parent, string name)
        {
            return new PathTableRecord
            {
                ExtentLocation = location,
                ParentDirectoryNumber = parent,
                DirectoryIdentifier = Encoding.ASCII.GetBytes(name),
            };
        }

        #endregion
    }
}
