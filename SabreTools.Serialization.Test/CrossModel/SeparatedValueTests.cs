using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class SeparatedValueTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.SeparatedValue();

            // Build the data
            Models.SeparatedValue.MetadataFile mf = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.SeparatedValue.MetadataFile? newMf = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header);
            Assert.NotNull(newMf.Row);
            Assert.Equal(3, newMf.Row.Length);

            ValidateDisk(newMf.Row[0]);
            ValidateMedia(newMf.Row[1]);
            ValidateRom(newMf.Row[2]);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.SeparatedValue.MetadataFile Build()
        {
            string[] header = ["header"];

            var disk = new Models.SeparatedValue.Row
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

            var media = new Models.SeparatedValue.Row
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

            var rom = new Models.SeparatedValue.Row
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

            return new Models.SeparatedValue.MetadataFile
            {
                Header = header,
                Row = [disk, media, rom],
            };
        }

        /// <summary>
        /// Validate a header
        /// </summary>
        private static void Validate(string[]? header)
        {
            Assert.NotNull(header);
            string column = Assert.Single(header);
            Assert.Equal("header", column);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateDisk(Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("disk", row.Type);
            Assert.Equal("XXXXXX", row.DiskName);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateMedia(Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("media", row.Type);
            Assert.Equal("XXXXXX", row.DiskName);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.SHA256);
            Assert.Equal("XXXXXX", row.SpamSum);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateRom(Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.FileName);
            Assert.Equal("XXXXXX", row.InternalName);
            Assert.Equal("XXXXXX", row.Description);
            Assert.Equal("XXXXXX", row.GameName);
            Assert.Equal("XXXXXX", row.GameDescription);
            Assert.Equal("rom", row.Type);
            Assert.Equal("XXXXXX", row.RomName);
            Assert.Equal("XXXXXX", row.Size);
            Assert.Equal("XXXXXX", row.CRC);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.SHA256);
            Assert.Equal("XXXXXX", row.SHA384);
            Assert.Equal("XXXXXX", row.SHA512);
            Assert.Equal("XXXXXX", row.SpamSum);
            Assert.Equal("XXXXXX", row.Status);
        }
    }
}