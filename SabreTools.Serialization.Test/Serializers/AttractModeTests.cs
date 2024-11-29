using System.IO;
using SabreTools.Hashing;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class AttractModeTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new AttractMode();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new AttractMode();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Valid_Filled()
        {
            // Create the object for serialization
            var dat = Build();

            // Deserialize the file
            var actual = AttractMode.SerializeStream(dat);
            actual?.Seek(0, SeekOrigin.Begin);

            // Validate the values
            Assert.NotNull(actual);
            string? actualHash = HashTool.GetStreamHash(actual, HashType.SHA1);
            Assert.NotNull(actualHash);
            Assert.Equal("21e2e21306dca19a580fde4e273f63a063c2d117", actualHash);
        }

        private static Models.AttractMode.MetadataFile Build()
        {
            string[] header = ["header"];

            var row = new Models.AttractMode.Row
            {
                Name = "XXXXXX",
                Title = "XXXXXX",
                Emulator = "XXXXXX",
                CloneOf = "XXXXXX",
                Year = "XXXXXX",
                Manufacturer = "XXXXXX",
                Category = "XXXXXX",
                Players = "XXXXXX",
                Rotation = "XXXXXX",
                Control = "XXXXXX",
                Status = "XXXXXX",
                DisplayCount = "XXXXXX",
                DisplayType = "XXXXXX",
                AltRomname = "XXXXXX",
                AltTitle = "XXXXXX",
                Extra = "XXXXXX",
                Buttons = "XXXXXX",
                Favorite = "XXXXXX",
                Tags = "XXXXXX",
                PlayedCount = "XXXXXX",
                PlayedTime = "XXXXXX",
                FileIsAvailable = "XXXXXX",
            };

            return new Models.AttractMode.MetadataFile
            {
                Header = header,
                Row = [row],
            };
        }
    }
}