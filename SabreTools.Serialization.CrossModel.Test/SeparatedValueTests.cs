using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class SeparatedValueTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new SeparatedValue();

            // Build the data
            Data.Models.SeparatedValue.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.SeparatedValue.MetadataFile? newMf = serializer.Deserialize(metadata);

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
        private static void Validate(string[]? header)
        {
            Assert.NotNull(header);
            string column = Assert.Single(header);
            Assert.Equal("header", column);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateDisk(Data.Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("disk", row.Type);
            Assert.Null(row.RomName);
            Assert.Equal("diskname", row.DiskName);
            Assert.Null(row.Size);
            Assert.Null(row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.Null(row.SHA256);
            Assert.Null(row.SHA384);
            Assert.Null(row.SHA512);
            Assert.Null(row.SpamSum);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateMedia(Data.Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("media", row.Type);
            Assert.Null(row.RomName);
            Assert.Equal("diskname", row.DiskName);
            Assert.Null(row.Size);
            Assert.Null(row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.Equal("sha256", row.SHA256);
            Assert.Null(row.SHA384);
            Assert.Null(row.SHA512);
            Assert.Equal("spamsum", row.SpamSum);
            Assert.Null(row.Status);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateRom(Data.Models.SeparatedValue.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("filename", row.FileName);
            Assert.Equal("internalname", row.InternalName);
            Assert.Equal("description", row.Description);
            Assert.Equal("gamename", row.GameName);
            Assert.Equal("description", row.GameDescription);
            Assert.Equal("rom", row.Type);
            Assert.Equal("romname", row.RomName);
            Assert.Null(row.DiskName);
            Assert.Equal(12345, row.Size);
            Assert.Equal("crc32", row.CRC);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.Equal("sha256", row.SHA256);
            Assert.Equal("sha384", row.SHA384);
            Assert.Equal("sha512", row.SHA512);
            Assert.Equal("spamsum", row.SpamSum);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, row.Status);
        }
    }
}
