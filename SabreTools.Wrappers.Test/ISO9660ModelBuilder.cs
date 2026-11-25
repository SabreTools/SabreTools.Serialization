using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Wrappers.Test
{
    /// <summary>
    /// Builds ISO9660 models and their data in memory
    /// </summary>
    internal static class ISO9660ModelBuilder
    {
        public const int BlockLength = 2048;

        /// <summary>
        /// File flags of a plain file record
        /// </summary>
        private const FileFlags NoFlags = 0;

        /// <summary>
        /// Data of 64 blocks that are filled with the low byte of their block number
        /// </summary>
        public static byte[] CreateData()
        {
            byte[] data = new byte[64 * BlockLength];
            for (int block = 0; block < 64; block++)
            {
                data.AsSpan(block * BlockLength, BlockLength).Fill((byte)block);
            }

            return data;
        }

        /// <summary>
        /// Paths of all extracted files and directories, relative to the output directory and sorted
        /// </summary>
        public static string[] Entries(string outputDirectory)
            => [.. Directory.GetFileSystemEntries(outputDirectory, "*", SearchOption.AllDirectories)
                .Select(path => Path.GetRelativePath(outputDirectory, path).Replace(Path.DirectorySeparatorChar, '/'))
                .OrderBy(path => path, StringComparer.Ordinal)];

        /// <summary>
        /// Data of a 10-byte file recorded at the given block
        /// </summary>
        public static byte[] ExpectedData(int location)
            => [.. Enumerable.Repeat((byte)location, 10)];

        /// <summary>
        /// Directory extent with current and parent directory records and the given records
        /// </summary>
        public static DirectoryExtent Listing(int location, int parent, params DirectoryRecord[] records)
            => new() { DirectoryRecords = [DirectoryEntry(Constants.CurrentDirectory, location), DirectoryEntry(Constants.ParentDirectory, parent), .. records] };

        /// <summary>
        /// Directory record of a subdirectory
        /// </summary>
        public static DirectoryRecord DirectoryEntry(string name, int location, Encoding? encoding = null)
            => DirectoryEntry((encoding ?? Encoding.ASCII).GetBytes(name), location);

        /// <summary>
        /// Directory record of a subdirectory
        /// </summary>
        public static DirectoryRecord DirectoryEntry(byte[] identifier, int location)
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
        public static DirectoryRecord FileEntry(string name, int location, Encoding? encoding = null)
        {
            return new DirectoryRecord
            {
                ExtentLocation = location,
                ExtentLength = 10,
                FileFlags = NoFlags,
                FileIdentifier = (encoding ?? Encoding.ASCII).GetBytes($"{name};1"),
            };
        }

        /// <summary>
        /// Path table record of a directory
        /// </summary>
        public static PathTableRecord PathTableEntry(int location, short parent, string name, Encoding? encoding = null)
        {
            return new PathTableRecord
            {
                ExtentLocation = location,
                ParentDirectoryNumber = parent,
                DirectoryIdentifier = (encoding ?? Encoding.ASCII).GetBytes(name),
            };
        }
    }
}
