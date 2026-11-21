using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.CueSheets;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class CueSheetTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ValidStream_Parsed()
        {
            string[] lines =
            [
                "CATALOG 0000000000000",
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "FILE \"Disc Name (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    FLAGS DCP",
                "    INDEX 00 00:00:00",
                "    INDEX 01 00:02:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal("0000000000000", actual.Catalog);
            Assert.Equal(2, actual.Files.Length);
            Assert.Equal("Disc Name (Track 1).bin", actual.Files[0].FileName);
            Assert.Equal(CueFileType.BINARY, actual.Files[0].FileType);
            var track = Assert.Single(actual.Files[0].Tracks);
            Assert.Equal(1, track.Number);
            Assert.Equal(CueTrackDataType.MODE1_2352, track.DataType);
            track = Assert.Single(actual.Files[1].Tracks);
            Assert.Equal(2, track.Number);
            Assert.Equal(CueTrackDataType.AUDIO, track.DataType);
            Assert.Equal(CueTrackFlag.DCP, track.Flags);
            Assert.Equal(2, track.Indices.Length);
            Assert.Equal(1, track.Indices[1].Index);
            Assert.Equal(2, track.Indices[1].Seconds);
        }

        [Fact]
        public void LongStream_AllTracksParsed()
        {
            // Longer than the StreamReader buffer, so the underlying stream is fully read before the last FILE line
            var builder = new StringBuilder();
            for (int i = 1; i <= 99; i++)
            {
                builder.Append($"FILE \"Disc Name (Track {i:D2}).bin\" BINARY\n");
                builder.Append($"  TRACK {i:D2} AUDIO\n");
                builder.Append("    INDEX 01 00:00:00\n");
            }

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal(99, actual.Files.Length);
            Assert.Equal(99, actual.Files[98].Tracks[0].Number);
        }

        [Fact]
        public void TrackWithoutIndexAtEnd_Null()
        {
            string[] lines =
            [
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.Null(actual);
        }

        [Fact]
        public void TrackWithoutIndexAtEndOfFile_Null()
        {
            string[] lines =
            [
                "FILE \"Disc Name.bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.Null(actual);
        }

        [Fact]
        public void FileWithoutTrackAtEnd_Null()
        {
            string[] lines =
            [
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "FILE \"Disc Name (Track 2).bin\" BINARY",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.Null(actual);
        }

        [Fact]
        public void TextValues_QuotesInsideKept()
        {
            // CD-Text values from redump cuesheets; an odd number of quotes must not join the following lines
            string[] lines =
            [
                "PERFORMER \"Barry \"Epoch\" Topping\"",
                "SONGWRITER \"Lee \"Scratch\" Perry \"",
                "TITLE \"07 Mussorgsky - Pictures at an Exhibition \"The Hut on Fowl's Leg\"",
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    TITLE \"\"Fun\" with Mad Dummy\"\"",
                "    PERFORMER Twelve Inch 12\"",
                "    SONGWRITER \"Quoted\" Word",
                "    INDEX 01 00:00:00",
                "FILE \"Disc Name (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    TITLE \" \"",
                "    INDEX 01 00:00:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal("Barry \"Epoch\" Topping", actual.Performer);
            Assert.Equal("Lee \"Scratch\" Perry ", actual.Songwriter);
            Assert.Equal("07 Mussorgsky - Pictures at an Exhibition \"The Hut on Fowl's Leg", actual.Title);
            Assert.Equal(2, actual.Files.Length);
            var track = Assert.Single(actual.Files[0].Tracks);
            Assert.Equal("\"Fun\" with Mad Dummy\"", track.Title);
            Assert.Equal("Twelve Inch 12\"", track.Performer);
            Assert.Equal("\"Quoted\" Word", track.Songwriter);
            track = Assert.Single(actual.Files[1].Tracks);
            Assert.Equal(" ", track.Title);
        }

        [Fact]
        public void BlankLines_Skipped()
        {
            string[] lines =
            [
                "   ",
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "\t",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            var file = Assert.Single(actual.Files);
            Assert.Equal(1, Assert.Single(file.Tracks).Number);
        }

        [Fact]
        public void QuoteOnlyLine_Skipped()
        {
            string[] lines =
            [
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "\"",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:02:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            var file = Assert.Single(actual.Files);
            Assert.Equal(2, file.Tracks.Length);
        }

        /// <summary>
        /// Create a stream with the given lines, each ending with a line feed
        /// </summary>
        private static MemoryStream CreateStream(string[] lines)
        {
            var builder = new StringBuilder();
            foreach (string line in lines)
            {
                builder.Append($"{line}\n");
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));
        }
    }
}
