using System;
using System.Text;
using SabreTools.IO;
using SabreTools.Numerics.Extensions;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class SpoonInstallerTests
    {
        [Fact]
        public void ValidTable_Entries()
        {
            byte[] data = new byte[512];
            int offset = 0x100;
            WriteEntry(data, ref offset, "a.txt");
            WriteEntry(data, ref offset, "b.txt");
            WriteFooter(data, tablePointer: 0x100, entryCount: 2);

            // The wrapper hands the reader a ReadOnlyViewStream, whose end-relative seek the footer offset relies on
            var stream = new ReadOnlyViewStream(data, 0, data.Length);
            var deserializer = new SpoonInstaller();

            var actual = deserializer.Deserialize(stream);
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Entries.Length);
            Assert.Equal("a.txt", actual.Entries[0].Filename);
            Assert.Equal("b.txt", actual.Entries[1].Filename);
        }

        [Fact]
        public void EntryCountLargerThanData_NullWithoutTableAllocation()
        {
            // The last 24 bytes of any file are read as a footer; here they claim 10 million entries in 1 KiB
            byte[] data = new byte[1024];
            WriteFooter(data, tablePointer: 0x100, entryCount: 10_000_000);

            var stream = new ReadOnlyViewStream(data, 0, data.Length);
            var deserializer = new SpoonInstaller();

            long before = GC.GetAllocatedBytesForCurrentThread();
            var actual = deserializer.Deserialize(stream);
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Null(actual);
            Assert.True(allocated < 1024 * 1024, $"{allocated} bytes allocated");
        }

        #region Helpers

        /// <summary>
        /// Write a file entry with the given name at the offset
        /// </summary>
        private static void WriteEntry(byte[] data, ref int offset, string filename)
        {
            data.WriteLittleEndian(ref offset, 0x10u);
            data.WriteLittleEndian(ref offset, 4u);
            data.WriteLittleEndian(ref offset, 4u);
            data.WriteLittleEndian(ref offset, 0u);
            data.Write(ref offset, (byte)filename.Length);
            data.Write(ref offset, Encoding.ASCII.GetBytes(filename));
        }

        /// <summary>
        /// Write the table pointer and the entry count into the footer in the last 24 bytes
        /// </summary>
        private static void WriteFooter(byte[] data, uint tablePointer, uint entryCount)
        {
            int offset = data.Length - 16;
            data.WriteLittleEndian(ref offset, tablePointer);
            data.WriteLittleEndian(ref offset, entryCount);
        }

        #endregion
    }
}
