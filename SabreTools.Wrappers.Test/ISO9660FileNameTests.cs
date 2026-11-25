using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Data.Models.ISO9660;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ModelBuilder;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660FileNameTests
    {
        /// <summary>
        /// Block of the root directory of the primary volume descriptor
        /// </summary>
        private const int RootDirectory = 20;

        [Theory]
        [InlineData("../../FILE.TXT", ".._.._FILE.TXT", ".._.._FILE.TXT")]
        [InlineData("..\\..\\FILE.TXT", "..\\..\\FILE.TXT", ".._.._FILE.TXT")]
        [InlineData("SUB/FILE.TXT", "SUB_FILE.TXT", "SUB_FILE.TXT")]
        [InlineData("FI\0LE.TXT", "FI_LE.TXT", "FI_LE.TXT")]
        [InlineData("FI:LE.TXT", "FI:LE.TXT", "FI_LE.TXT")]
        [InlineData("..", "__", "__")]
        [InlineData(".", "_", "_")]
        public void Extract_UnsafeFileName_ReplacesCharacters(string name, string expected, string expectedOnWindows)
        {
            if (OperatingSystem.IsWindows())
                expected = expectedOnWindows;

            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, FileEntry(name, 25), FileEntry("OTHER.TXT", 26)),
            };

            string root = Extract(CreateVolume(directories));

            string[] expectedEntries = ["out", "out/sub", $"out/sub/{expected}", "out/sub/OTHER.TXT"];
            Assert.Equal(expectedEntries.OrderBy(path => path, StringComparer.Ordinal), Entries(root));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(root, "out", "sub", expected)));
            Directory.Delete(root, recursive: true);
        }

        [Theory]
        [InlineData("FILE.", "FILE.", "FILE")]
        [InlineData("FILE..", "FILE..", "FILE")]
        [InlineData("FILE ", "FILE ", "FILE")]
        [InlineData(". .", ". .", "___")]
        public void Extract_FileNameEndingInPeriodOrSpace_IsWritten(string name, string expected, string expectedOnWindows)
        {
            if (OperatingSystem.IsWindows())
                expected = expectedOnWindows;

            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, FileEntry(name, 25), FileEntry("OTHER.TXT", 26)),
            };

            string root = Extract(CreateVolume(directories));

            string[] expectedEntries = ["out", "out/sub", $"out/sub/{expected}", "out/sub/OTHER.TXT"];
            Assert.Equal(expectedEntries.OrderBy(path => path, StringComparer.Ordinal), Entries(root));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(root, "out", "sub", expected)));
            Directory.Delete(root, recursive: true);
        }

        [Theory]
        [InlineData("../..", ".._..", ".._")]
        [InlineData("..\\..", "..\\..", ".._")]
        [InlineData("..", "__", "__")]
        [InlineData(".", "_", "_")]
        public void Extract_UnsafeDirectoryName_ReplacesCharacters(string name, string expected, string expectedOnWindows)
        {
            if (OperatingSystem.IsWindows())
                expected = expectedOnWindows;

            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, DirectoryEntry(name, 30)),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            string root = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "out", "out/sub", $"out/sub/{expected}", $"out/sub/{expected}/FILE.TXT" }, Entries(root));
            Directory.Delete(root, recursive: true);
        }

        [Theory]
        [InlineData("DIR.", "DIR.", "DIR")]
        [InlineData("DIR..", "DIR..", "DIR")]
        [InlineData("DIR ", "DIR ", "DIR")]
        [InlineData("...", "...", "___")]
        [InlineData(". .", ". .", "___")]
        public void Extract_DirectoryNameEndingInPeriodOrSpace_IsWritten(string name, string expected, string expectedOnWindows)
        {
            if (OperatingSystem.IsWindows())
                expected = expectedOnWindows;

            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, DirectoryEntry(name, 30)),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            string root = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "out", "out/sub", $"out/sub/{expected}", $"out/sub/{expected}/FILE.TXT" }, Entries(root));
            Directory.Delete(root, recursive: true);
        }

        [Theory]
        [InlineData("../..", ".._..", ".._")]
        [InlineData("..\\..", "..\\..", ".._")]
        [InlineData("..", "__", "__")]
        [InlineData(".", "_", "_")]
        public void Extract_UnsafePathTableDirectoryName_ReplacesCharacters(string name, string expected, string expectedOnWindows)
        {
            if (OperatingSystem.IsWindows())
                expected = expectedOnWindows;

            // The root directory does not hold the directory at block 30
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory),
                [30] = Listing(30, RootDirectory, FileEntry("FILE.TXT", 26)),
            };

            PathTableRecord[] pathTable = [PathTableEntry(RootDirectory, 1, "\0"), PathTableEntry(30, 1, name)];

            string root = Extract(CreateVolume(directories, pathTable));

            Assert.Equal(new[] { "out", "out/sub", $"out/sub/{expected}", $"out/sub/{expected}/FILE.TXT" }, Entries(root));
            Directory.Delete(root, recursive: true);
        }

        [Fact]
        public void Extract_UnsafeAssociatedFileName_ReplacesCharacters()
        {
            var directories = new Dictionary<int, FileExtent>
            {
                [RootDirectory] = Listing(RootDirectory, RootDirectory, FileEntry("../FILE.TXT", 25, flags: FileFlags.ASSOCIATED_FILE), FileEntry("../FILE.TXT", 26)),
            };

            string root = Extract(CreateVolume(directories));

            Assert.Equal(new[] { "out", "out/sub", "out/sub/.._FILE.TXT", "out/sub/.._FILE.TXT.associated" }, Entries(root));
            Assert.Equal(ExpectedData(25), File.ReadAllBytes(Path.Combine(root, "out", "sub", ".._FILE.TXT.associated")));
            Directory.Delete(root, recursive: true);
        }

        #region Helpers

        /// <summary>
        /// Extract a volume whose blocks are filled with the low byte of their block number into the
        /// directory "out/sub" of a new directory, and return the new directory
        /// </summary>
        private static string Extract(Volume volume)
        {
            string root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string outputDirectory = Path.Combine(root, "out", "sub");
            Directory.CreateDirectory(outputDirectory);
            Assert.True(new ISO9660(volume, CreateData()).Extract(outputDirectory, includeDebug: false));
            return root;
        }

        /// <summary>
        /// Create a volume with a primary volume descriptor, its directories and an optional path table
        /// </summary>
        private static Volume CreateVolume(Dictionary<int, FileExtent> directories, PathTableRecord[]? pathTable = null)
        {
            return new Volume
            {
                VolumeDescriptorSet =
                [
                    new PrimaryVolumeDescriptor
                    {
                        LogicalBlockSize = BlockLength,
                        RootDirectoryRecord = DirectoryEntry(Constants.CurrentDirectory, RootDirectory),
                    },
                ],
                PathTableGroups = pathTable is null ? [] : [new PathTableGroup { PathTableL = pathTable }],
                DirectoryDescriptors = directories,
            };
        }

        #endregion
    }
}
