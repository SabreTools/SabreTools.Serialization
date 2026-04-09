using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class SeparatedValueTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripShortTest()
        {
            // Get the serializer and deserializer
            var deserializer = new SeparatedValue();
            var serializer = new Writers.SeparatedValue();

            // Build the data
            Data.Models.SeparatedValue.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, ',', longHeader: false);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.SeparatedValue.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header, longHeader: false);
            Assert.NotNull(newMf.Row);
            Assert.Equal(3, newMf.Row.Length);

            ValidateDisk(newMf.Row[0], longHeader: false);
            ValidateMedia(newMf.Row[1], longHeader: false);
            ValidateRom(newMf.Row[2], longHeader: false);
        }

        [Fact]
        public void RoundTripLongTest()
        {
            // Get the serializer and deserializer
            var deserializer = new SeparatedValue();
            var serializer = new Writers.SeparatedValue();

            // Build the data
            Data.Models.SeparatedValue.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, ',', longHeader: true);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.SeparatedValue.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header, longHeader: true);
            Assert.NotNull(newMf.Row);
            Assert.Equal(3, newMf.Row.Length);

            ValidateDisk(newMf.Row[0], longHeader: true);
            ValidateMedia(newMf.Row[1], longHeader: true);
            ValidateRom(newMf.Row[2], longHeader: true);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.SeparatedValue.MetadataFile Build()
        {
            string[] header = ["header"];

            var disk = new Data.Models.SeparatedValue.Row
            {
                FileName = "filename",
                InternalName = "internalname",
                Description = "description",
                GameName = "gamename",
                GameDescription = "description",
                Type = "disk",
                DiskName = "diskname",
                MD5 = "md5",
                SHA1 = "sha1",
                Status = Data.Models.Metadata.ItemStatus.Good,
            };

            var media = new Data.Models.SeparatedValue.Row
            {
                FileName = "filename",
                InternalName = "internalname",
                Description = "description",
                GameName = "gamename",
                GameDescription = "description",
                Type = "media",
                DiskName = "diskname",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SpamSum = "spamsum",
            };

            var rom = new Data.Models.SeparatedValue.Row
            {
                FileName = "filename",
                InternalName = "internalname",
                Description = "description",
                GameName = "gamename",
                GameDescription = "description",
                Type = "rom",
                RomName = "romname",
                Size = 12345,
                CRC = "crc32",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum",
                Status = Data.Models.Metadata.ItemStatus.Good,
            };

            return new Data.Models.SeparatedValue.MetadataFile
            {
                Header = header,
                Row = [disk, media, rom],
            };
        }

        /// <summary>
        /// Validate a header
        /// </summary>
        private static void Validate(string[]? header, bool longHeader)
        {
            Assert.NotNull(header);
            if (longHeader)
                Assert.True(Writers.SeparatedValue.HeaderArrayExtended.SequenceEqual(header));
            else
                Assert.True(Writers.SeparatedValue.HeaderArrayStandard.SequenceEqual(header));
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateDisk(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("disk", row.Type);
            Assert.NotNull(row.RomName); Assert.Empty(row.RomName);
            Assert.Equal("diskname", row.DiskName);
            Assert.Null(row.Size);
            Assert.NotNull(row.CRC); Assert.Empty(row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.NotNull(row.CRC); Assert.Empty(row.CRC);
            if (longHeader)
            {
                Assert.NotNull(row.SHA384); Assert.Empty(row.SHA384);
                Assert.NotNull(row.SHA512); Assert.Empty(row.SHA512);
                Assert.NotNull(row.SpamSum); Assert.Empty(row.SpamSum);
            }
            else
            {
                Assert.Null(row.SHA384);
                Assert.Null(row.SHA512);
                Assert.Null(row.SpamSum);
            }

            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateMedia(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("media", row.Type);
            Assert.NotNull(row.RomName); Assert.Empty(row.RomName);
            Assert.Equal("diskname", row.DiskName);
            Assert.Null(row.Size);
            Assert.NotNull(row.CRC); Assert.Empty(row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.Equal("sha256", row.SHA256);
            if (longHeader)
            {
                Assert.NotNull(row.SHA384); Assert.Empty(row.SHA384);
                Assert.NotNull(row.SHA512); Assert.Empty(row.SHA512);
                Assert.Equal("spamsum", row.SpamSum);
            }
            else
            {
                Assert.Null(row.SHA384);
                Assert.Null(row.SHA512);
                Assert.Null(row.SpamSum);
            }

            Assert.Null(row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateRom(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("rom", row.Type);
            Assert.Equal("romname", row.RomName);
            Assert.NotNull(row.DiskName);
            Assert.Empty(row.DiskName);
            Assert.Equal(12345, row.Size);
            Assert.Equal("crc32", row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.Equal("sha256", row.SHA256);
            if (longHeader)
            {
                Assert.Equal("sha384", row.SHA384);
                Assert.Equal("sha512", row.SHA512);
                Assert.Equal("spamsum", row.SpamSum);
            }
            else
            {
                Assert.Null(row.SHA384);
                Assert.Null(row.SHA512);
                Assert.Null(row.SpamSum);
            }

            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, row.Status);
        }
    }
}
