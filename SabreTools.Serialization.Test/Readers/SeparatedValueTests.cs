using System;
using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
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
            var serializer = new SabreTools.Serialization.Writers.SeparatedValue();

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
            var serializer = new SabreTools.Serialization.Writers.SeparatedValue();

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
                FileName = "XXXXXX",
                InternalName = "XXXXXX",
                Description = "XXXXXX",
                GameName = "XXXXXX",
                GameDescription = "XXXXXX",
                Type = "disk",
                DiskName = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Status = "XXXXXX",
            };

            var media = new Data.Models.SeparatedValue.Row
            {
                FileName = "XXXXXX",
                InternalName = "XXXXXX",
                Description = "XXXXXX",
                GameName = "XXXXXX",
                GameDescription = "XXXXXX",
                Type = "media",
                DiskName = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            var rom = new Data.Models.SeparatedValue.Row
            {
                FileName = "XXXXXX",
                InternalName = "XXXXXX",
                Description = "XXXXXX",
                GameName = "XXXXXX",
                GameDescription = "XXXXXX",
                Type = "rom",
                RomName = "XXXXXX",
                Size = "XXXXXX",
                CRC = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX",
                Status = "XXXXXX",
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
                Assert.True(SabreTools.Serialization.Writers.SeparatedValue.HeaderArrayExtended.SequenceEqual(header));
            else
                Assert.True(SabreTools.Serialization.Writers.SeparatedValue.HeaderArrayStandard.SequenceEqual(header));
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateDisk(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("disk", row.Type);
            Assert.NotNull(row.RomName); Assert.Empty(row.RomName);
            Assert.Equal("XXXXXX", row.DiskName);
            Assert.NotNull(row.Size); Assert.Empty(row.Size);
            Assert.NotNull(row.CRC); Assert.Empty(row.CRC);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
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
            Assert.Equal("XXXXXX", row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateMedia(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("media", row.Type);
            Assert.NotNull(row.RomName); Assert.Empty(row.RomName);
            Assert.Equal("XXXXXX", row.DiskName);
            Assert.NotNull(row.Size); Assert.Empty(row.Size);
            Assert.NotNull(row.CRC); Assert.Empty(row.CRC);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.SHA256);
            if (longHeader)
            {
                Assert.NotNull(row.SHA384); Assert.Empty(row.SHA384);
                Assert.NotNull(row.SHA512); Assert.Empty(row.SHA512);
                Assert.Equal("XXXXXX", row.SpamSum);
            }
            else
            {
                Assert.Null(row.SHA384);
                Assert.Null(row.SHA512);
                Assert.Null(row.SpamSum);
            }
            Assert.NotNull(row.Status); Assert.Empty(row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateRom(Data.Models.SeparatedValue.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("rom", row.Type);
            Assert.Equal("XXXXXX", row.RomName);
            Assert.NotNull(row.DiskName);
            Assert.Empty(row.DiskName);
            Assert.Equal("XXXXXX", row.Size);
            Assert.Equal("XXXXXX", row.CRC);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.SHA256);
            if (longHeader)
            {
                Assert.Equal("XXXXXX", row.SHA384);
                Assert.Equal("XXXXXX", row.SHA512);
                Assert.Equal("XXXXXX", row.SpamSum);
            }
            else
            {
                Assert.Null(row.SHA384);
                Assert.Null(row.SHA512);
                Assert.Null(row.SpamSum);
            }
            Assert.Equal("XXXXXX", row.Status);
        }
    }
}
