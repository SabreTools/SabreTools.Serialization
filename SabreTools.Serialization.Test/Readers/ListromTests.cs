using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
{
    public class ListromTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Listrom();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Listrom();
            var serializer = new SabreTools.Serialization.Writers.Listrom();

            // Build the data
            Data.Models.Listrom.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.Listrom.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Assert.NotNull(newMf.Set);
            Assert.Equal(2, newMf.Set.Length);

            ValidateDevice(newMf.Set[0]);
            ValidateDriver(newMf.Set[1]);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.Listrom.MetadataFile Build()
        {
            var romGood = new Data.Models.Listrom.Row
            {
                Name = "XXXXXX",
                Size = "12345",
                Bad = false,
                CRC = Hashing.ZeroHash.CRC32Str,
                SHA1 = Hashing.ZeroHash.SHA1Str,
                NoGoodDumpKnown = false,
            };

            var romBad = new Data.Models.Listrom.Row
            {
                Name = "XXXXXX",
                Size = "12345",
                Bad = true,
                CRC = Hashing.ZeroHash.CRC32Str,
                SHA1 = Hashing.ZeroHash.SHA1Str,
                NoGoodDumpKnown = false,
            };

            var diskGoodMd5 = new Data.Models.Listrom.Row
            {
                Name = "XXXXXX",
                Bad = false,
                MD5 = Hashing.ZeroHash.MD5Str,
                SHA1 = null,
                NoGoodDumpKnown = false,
            };

            var diskGoodSha1 = new Data.Models.Listrom.Row
            {
                Name = "XXXXXX",
                Bad = false,
                MD5 = null,
                SHA1 = Hashing.ZeroHash.SHA1Str,
                NoGoodDumpKnown = false,
            };

            var diskBad = new Data.Models.Listrom.Row
            {
                Name = "XXXXXX",
                Bad = false,
                MD5 = Hashing.ZeroHash.MD5Str,
                SHA1 = Hashing.ZeroHash.SHA1Str,
                NoGoodDumpKnown = true,
            };

            var device = new Data.Models.Listrom.Set()
            {
                Device = "XXXXXX",
                Row = [romGood, romBad],
            };

            var driver = new Data.Models.Listrom.Set()
            {
                Driver = "XXXXXX",
                Row = [diskGoodMd5, diskGoodSha1, diskBad],
            };

            return new Data.Models.Listrom.MetadataFile
            {
                Set = [device, driver],
            };
        }

        /// <summary>
        /// Validate a Set
        /// </summary>
        private static void ValidateDevice(Data.Models.Listrom.Set? set)
        {
            Assert.NotNull(set);
            Assert.Equal("XXXXXX", set.Device);

            Assert.NotNull(set.Row);
            Assert.Equal(2, set.Row.Length);

            ValidateGoodRom(set.Row[0]);
            ValidateBadRom(set.Row[1]);
        }

        /// <summary>
        /// Validate a Set
        /// </summary>
        private static void ValidateDriver(Data.Models.Listrom.Set? set)
        {
            Assert.NotNull(set);
            Assert.Equal("XXXXXX", set.Driver);

            Assert.NotNull(set.Row);
            Assert.Equal(3, set.Row.Length);

            ValidateGoodMd5Disk(set.Row[0]);
            ValidateGoodSha1Disk(set.Row[1]);
            ValidateBadDisk(set.Row[2]);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateGoodRom(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.Equal("12345", row.Size);
            Assert.False(row.Bad);
            Assert.Equal(Hashing.ZeroHash.CRC32Str, row.CRC);
            Assert.Equal(Hashing.ZeroHash.SHA1Str, row.SHA1);
            Assert.False(row.NoGoodDumpKnown);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateBadRom(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.Equal("12345", row.Size);
            Assert.True(row.Bad);
            Assert.Equal(Hashing.ZeroHash.CRC32Str, row.CRC);
            Assert.Equal(Hashing.ZeroHash.SHA1Str, row.SHA1);
            Assert.False(row.NoGoodDumpKnown);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateGoodMd5Disk(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.False(row.Bad);
            Assert.Equal(Hashing.ZeroHash.MD5Str, row.MD5);
            Assert.Null(row.SHA1);
            Assert.False(row.NoGoodDumpKnown);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateGoodSha1Disk(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.False(row.Bad);
            Assert.Null(row.MD5);
            Assert.Equal(Hashing.ZeroHash.SHA1Str, row.SHA1);
            Assert.False(row.NoGoodDumpKnown);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateBadDisk(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.False(row.Bad);
            Assert.Null(row.MD5);
            Assert.Null(row.SHA1);
            Assert.True(row.NoGoodDumpKnown);
        }
    }
}
