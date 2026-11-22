using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.XDVDFS;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class XDVDFSTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = XDVDFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = XDVDFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = XDVDFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = XDVDFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = XDVDFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = XDVDFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyFile_Extract()
        {
            var wrapper = CreateWrapper(FileRecord("DATA.BIN", 40, 10), FileRecord("EMPTY.TXT", 41, 0));

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(SectorData(40, 10), File.ReadAllBytes(Path.Combine(output, "DATA.BIN")));
                Assert.Empty(File.ReadAllBytes(Path.Combine(output, "EMPTY.TXT")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void EmptyFileBeforeFileAtSameSector_Extract()
        {
            var wrapper = CreateWrapper(FileRecord("A.TXT", 40, 0), FileRecord("B.BIN", 40, 10));

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Empty(File.ReadAllBytes(Path.Combine(output, "A.TXT")));
                Assert.Equal(SectorData(40, 10), File.ReadAllBytes(Path.Combine(output, "B.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void EmptyFileAfterFileAtSameSector_Extract()
        {
            var wrapper = CreateWrapper(FileRecord("A.BIN", 40, 10), FileRecord("B.TXT", 40, 0));

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(SectorData(40, 10), File.ReadAllBytes(Path.Combine(output, "A.BIN")));
                Assert.Empty(File.ReadAllBytes(Path.Combine(output, "B.TXT")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        #region Helpers

        /// <summary>
        /// Sector of the root directory
        /// </summary>
        private const uint RootDirectory = 33;

        /// <summary>
        /// Create a wrapper for a volume with the given root directory records, whose sectors are filled with the low byte of their sector number
        /// </summary>
        private static XDVDFS CreateWrapper(params DirectoryRecord[] records)
        {
            byte[] data = new byte[64 * Constants.SectorSize];
            for (int sector = 0; sector < 64; sector++)
            {
                data.AsSpan(sector * Constants.SectorSize, Constants.SectorSize).Fill((byte)sector);
            }

            var volume = new Volume
            {
                VolumeDescriptor = new VolumeDescriptor { RootOffset = RootDirectory },
                DirectoryDescriptors = new Dictionary<uint, DirectoryDescriptor>
                {
                    [RootDirectory] = new DirectoryDescriptor { DirectoryRecords = records },
                },
            };

            return new XDVDFS(volume, data);
        }

        /// <summary>
        /// Directory record of a file with no other attributes
        /// </summary>
        private static DirectoryRecord FileRecord(string name, uint sector, uint size)
        {
            byte[] filename = Encoding.ASCII.GetBytes(name);
            return new DirectoryRecord
            {
                ExtentOffset = sector,
                ExtentSize = size,
                FileFlags = FileFlags.NORMAL,
                FilenameLength = (byte)filename.Length,
                Filename = filename,
            };
        }

        /// <summary>
        /// Data of a file recorded at the given sector
        /// </summary>
        private static byte[] SectorData(int sector, int size)
            => [.. Enumerable.Repeat((byte)sector, size)];

        #endregion
    }
}
