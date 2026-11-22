using System.Collections.Generic;
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
        public void IndicesBeforeTrack_KeptOnFile()
        {
            // Layout written by Exact Audio Copy and whipper, with the gap of a track at the end of the previous file
            string[] lines =
            [
                "FILE \"01.wav\" WAVE",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
                "    INDEX 00 06:32:45",
                "FILE \"02.wav\" WAVE",
                "    INDEX 01 00:00:00",
                "  TRACK 03 AUDIO",
                "    INDEX 00 04:11:03",
                "FILE \"03.wav\" WAVE",
                "    INDEX 01 00:00:00",
                "FILE \"04.wav\" WAVE",
                "  TRACK 04 AUDIO",
                "    INDEX 01 00:00:00",
                "  TRACK 05 AUDIO",
                "    INDEX 00 02:02:22",
                "FILE \"05.wav\" WAVE",
                "    INDEX 01 00:00:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal(5, actual.Files.Length);
            Assert.Empty(actual.Files[0].Indices);
            Assert.Equal([1, 2], actual.Files[0].Tracks.Select(t => t.Number));
            var index = Assert.Single(actual.Files[0].Tracks[1].Indices);
            Assert.Equal((0, 6, 32, 45), (index.Index, index.Minutes, index.Seconds, index.Frames));
            Assert.Equal(1, Assert.Single(actual.Files[1].Indices).Index);
            Assert.Equal(3, Assert.Single(actual.Files[1].Tracks).Number);
            Assert.Equal(1, Assert.Single(actual.Files[2].Indices).Index);
            Assert.Empty(actual.Files[2].Tracks);
            Assert.Equal([4, 5], actual.Files[3].Tracks.Select(t => t.Number));
            Assert.Equal(1, Assert.Single(actual.Files[4].Indices).Index);
            Assert.Empty(actual.Files[4].Tracks);
        }

        [Fact]
        public void IndicesBeforeTrack_RemarksKept()
        {
            string[] lines =
            [
                "FILE \"01.wav\" WAVE",
                "  TRACK 01 AUDIO",
                "    INDEX 00 00:00:00",
                "FILE \"02.wav\" WAVE",
                "REM Before the index",
                "    INDEX 01 00:00:00",
                "REM After the index",
                "REM Second line",
                "    INDEX 02 00:01:00",
                "REM After the second index",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:02:00",
                "UNKNOWN",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Files.Length);
            var file = actual.Files[1];
            Assert.Equal(["Before the index"], file.Remarks);
            Assert.Equal([1, 2], file.Indices.Select(i => i.Index));
            Assert.Equal(["After the index", "Second line"], file.Indices[0].Remarks);
            Assert.Equal(["After the second index"], file.Indices[1].Remarks);
            Assert.Equal(2, Assert.Single(file.Tracks).Number);
        }

        [Fact]
        public void IndicesBeforeTrack_WrittenBack()
        {
            string[] lines =
            [
                "FILE \"01.wav\" WAVE",
                "  TRACK 01 AUDIO",
                "    PERFORMER \"Performer\"",
                "    TITLE \"Track01\"",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
                "    INDEX 00 06:32:45",
                "FILE \"02.wav\" WAVE",
                "    INDEX 01 00:00:00",
                "REM Comment",
                "FILE \"03.wav\" WAVE",
                "  TRACK 03 AUDIO",
                "    INDEX 01 00:00:00",
            ];

            Assert.Equal(lines, ReadAndWrite(lines));
        }

        [Theory]
        [InlineData("AUDIO", CueTrackDataType.AUDIO, null)]
        [InlineData("audio", CueTrackDataType.AUDIO, null)]
        [InlineData("MODE1/2352", CueTrackDataType.MODE1_2352, null)]
        [InlineData("MODEx/2xxx", CueTrackDataType.AUDIO, "MODEx/2xxx")]
        public void DataType_UnknownKeptAsText(string dataType, CueTrackDataType expected, string? expectedUnknown)
        {
            string[] lines =
            [
                "FILE \"01.wav\" WAVE",
                $"  TRACK 01 {dataType}",
                "    INDEX 01 00:00:00",
            ];

            var track = Assert.Single(GetTracks(lines));
            Assert.Equal(expected, track.DataType);
            Assert.Equal(expectedUnknown, track.UnknownDataType);
        }

        [Fact]
        public void DataType_Unknown_WrittenBack()
        {
            // Exact Audio Copy writes MODEx/2xxx for a data track
            string[] lines =
            [
                "FILE \"01.iso\" BINARY",
                "  TRACK 01 MODEx/2xxx",
                "    INDEX 01 00:00:00",
                "FILE \"02.wav\" WAVE",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:00:00",
                "  TRACK 03 MODEx/2xxx",
                "    INDEX 00 03:21:52",
            ];

            Assert.Equal(lines, ReadAndWrite(lines));
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
        public void MultiLineTextValues_Joined()
        {
            // CD-Text values can contain a newline, which is written as it is between the quotes
            string[] lines =
            [
                "PERFORMER \"Artist Name",
                "\"",
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    TITLE \"Line 1 ",
                "Line 2\"",
                "",
                "    SONGWRITER \"",
                "Name\"",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:02:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal("Artist Name\n", actual.Performer);
            var file = Assert.Single(actual.Files);
            Assert.Equal(2, file.Tracks.Length);
            Assert.Equal("Line 1 \nLine 2", file.Tracks[0].Title);
            Assert.Equal("\nName", file.Tracks[0].Songwriter);
            Assert.Single(file.Tracks[0].Indices);
            Assert.Equal(2, file.Tracks[1].Number);
        }

        [Fact]
        public void UnclosedTextValues_Kept()
        {
            // A value that is not closed before the next command keeps its quote
            string[] lines =
            [
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    TITLE \"",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
                "    TITLE \"Unclosed",
                "    INDEX 01 00:02:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            var file = Assert.Single(actual.Files);
            Assert.Equal(2, file.Tracks.Length);
            Assert.Equal("\"", file.Tracks[0].Title);
            Assert.Single(file.Tracks[0].Indices);
            Assert.Equal("\"Unclosed", file.Tracks[1].Title);
            Assert.Single(file.Tracks[1].Indices);
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

        [Fact]
        public void Latin1Text_Kept()
        {
            // CD-Text bytes as DiscImageCreator writes them for Clockwork Aquario - Original Soundtrack
            string[] lines =
            [
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    TITLE \"Blast\u00fcfEm Up (Boss!!)\"",
                "    INDEX 01 00:00:00",
            ];
            var stream = CreateStream(lines, Encoding.GetEncoding(28591));

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal("Blast\u00fcfEm Up (Boss!!)", actual.Files[0].Tracks[0].Title);
        }

        [Fact]
        public void Latin1TextAtEnd_Kept()
        {
            // The last byte starts a UTF-8 sequence that never ends
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(
                "FILE \"Disc Name (Track 1).bin\" BINARY\n  TRACK 01 AUDIO\n    INDEX 01 00:00:00\n    TITLE Caf\u00e9");
            var stream = new MemoryStream(bytes);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal("Caf\u00e9", actual.Files[0].Tracks[0].Title);
        }

        [Fact]
        public void Utf8TextAcrossReadBuffer_Kept()
        {
            // The two bytes of the last letter are on both sides of the first 4096 bytes
            string start = "FILE \"Disc Name (Track 1).bin\" BINARY\n  TRACK 01 AUDIO\n    INDEX 01 00:00:00\n    TITLE \"";
            string title = $"{new string('a', 4095 - start.Length)}\u00e8";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes($"{start}{title}\"\n"));

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal(title, actual.Files[0].Tracks[0].Title);
        }

        [Fact]
        public void StreamAfterStart_ReadFromPosition()
        {
            string[] lines =
            [
                "TITLE \"Before\"",
                "FILE \"Disc Name (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
            ];
            var stream = CreateStream(lines);
            stream.Position = "TITLE \"Before\"\n".Length;

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Null(actual.Title);
            Assert.Single(actual.Files);
        }

        [Fact]
        public void Remarks_None_Zero()
        {
            string[] lines =
            [
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];

            var tracks = GetTracks(lines);

            Assert.Equal([0, 0], tracks.Select(t => t.Session));
            Assert.Equal([CueDensityArea.None, CueDensityArea.None], tracks.Select(t => t.DensityArea));
        }

        [Fact]
        public void Remarks_RemSessionBeforeFile_Parsed()
        {
            // Layout written by redump: the second REM SESSION follows the indices of the previous track
            string[] lines =
            [
                "REM SESSION 01",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    INDEX 00 00:00:00",
                "    INDEX 01 00:02:00",
                "REM SESSION 02",
                "FILE \"a (Track 3).bin\" BINARY",
                "  TRACK 03 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];

            Assert.Equal([1, 1, 2], GetTracks(lines).Select(t => t.Session));
        }

        [Fact]
        public void Remarks_RemSessionBeforeTrack_Parsed()
        {
            string[] lines =
            [
                "REM SESSION 01",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "REM SESSION 02",
                "  TRACK 02 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];

            Assert.Equal([1, 2], GetTracks(lines).Select(t => t.Session));
        }

        [Fact]
        public void Remarks_DensityAreas_Parsed()
        {
            // Layout written by redump for a GD-ROM
            string[] lines =
            [
                "REM SINGLE-DENSITY AREA",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    INDEX 00 00:00:00",
                "    INDEX 01 00:02:00",
                "REM HIGH-DENSITY AREA",
                "FILE \"a (Track 3).bin\" BINARY",
                "  TRACK 03 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];

            var tracks = GetTracks(lines);

            Assert.Equal(
                [CueDensityArea.SingleDensity, CueDensityArea.SingleDensity, CueDensityArea.HighDensity],
                tracks.Select(t => t.DensityArea));
            Assert.Equal([0, 0, 0], tracks.Select(t => t.Session));
        }

        [Fact]
        public void Remarks_OtherRem_Ignored()
        {
            string[] lines =
            [
                "REM",
                "REM SESSION",
                "REM DATE 1999",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
            ];

            var track = Assert.Single(GetTracks(lines));

            Assert.Equal(0, track.Session);
            Assert.Equal(CueDensityArea.None, track.DensityArea);
        }

        [Fact]
        public void Remarks_SessionNotNumber_Ignored()
        {
            string[] lines =
            [
                "REM SESSION 01",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "REM SESSION X",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:02:00",
            ];

            Assert.Equal([1, 1], GetTracks(lines).Select(t => t.Session));
        }

        [Fact]
        public void Remarks_Lines_KeptInPlace()
        {
            string[] lines =
            [
                "REM GENRE Game",
                "CATALOG 0000000000000",
                "REM",
                "FILE \"a (Track 1).bin\" BINARY",
                "REM ORIGINAL MEDIA-TYPE: CD-RW",
                "  TRACK 01 MODE1/2352",
                "    REM Before the index",
                "    FLAGS DCP",
                "    INDEX 01 00:00:00",
                "\tREM LBA: 0  ",
                "FILE \"a (Track 2).bin\" BINARY",
                "REM Second file",
                "  TRACK 02 AUDIO",
                "    INDEX 00 00:00:00",
                "    REM LBA: 4500",
                "    INDEX 01 00:02:00",
                "    REM LBA: 4650",
                "REM LEAD-OUT 02:00:00",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            Assert.Equal(["GENRE Game", ""], actual.Remarks);
            Assert.Equal(2, actual.Files.Length);
            Assert.Equal(["ORIGINAL MEDIA-TYPE: CD-RW"], actual.Files[0].Remarks);
            var track = Assert.Single(actual.Files[0].Tracks);
            Assert.Equal(["Before the index"], track.Remarks);
            Assert.Equal(["LBA: 0"], track.Indices[0].Remarks);
            Assert.Equal(["Second file"], actual.Files[1].Remarks);
            track = Assert.Single(actual.Files[1].Tracks);
            Assert.Empty(track.Remarks);
            Assert.Equal(["LBA: 4500"], track.Indices[0].Remarks);
            Assert.Equal(["LBA: 4650", "LEAD-OUT 02:00:00"], track.Indices[1].Remarks);
        }

        [Fact]
        public void Remarks_OtherRem_SessionAndDensityAreaKept()
        {
            string[] lines =
            [
                "REM SESSION 01",
                "REM HIGH-DENSITY AREA",
                "REM DATE 1999",
                "FILE \"a.bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];

            var track = Assert.Single(GetTracks(lines));

            Assert.Equal(1, track.Session);
            Assert.Equal(CueDensityArea.HighDensity, track.DensityArea);
        }

        [Fact]
        public void Remarks_BeforeUnknownLine_Kept()
        {
            string[] lines =
            [
                "FILE \"a.bin\" BINARY",
                "REM ORIGINAL MEDIA-TYPE: CD-RW",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "    REM LBA: 0",
                "    UNKNOWN",
            ];
            var stream = CreateStream(lines);

            var actual = new CueSheet().Deserialize(stream);

            Assert.NotNull(actual);
            var file = Assert.Single(actual.Files);
            Assert.Equal(["ORIGINAL MEDIA-TYPE: CD-RW"], file.Remarks);
            Assert.Equal(["LBA: 0"], Assert.Single(file.Tracks).Indices[0].Remarks);
        }

        [Fact]
        public void Remarks_DiscImageCreatorLayout_WrittenBack()
        {
            // Layout written by DiscImageCreator for a disc with two sessions
            string[] lines =
            [
                "REM SESSION 01",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    INDEX 00 00:00:00",
                "    INDEX 01 00:02:00",
                "REM LEAD-OUT 01:30:00",
                "REM SESSION 02",
                "REM LEAD-IN 01:00:00",
                "REM PREGAP 00:02:00",
                "FILE \"a (Track 3).bin\" BINARY",
                "  TRACK 03 MODE2/2352",
                "    INDEX 01 00:00:00",
            ];

            Assert.Equal(lines, ReadAndWrite(lines));
        }

        [Fact]
        public void Remarks_IsoBusterLayout_WrittenBack()
        {
            // Layout written by IsoBuster for a disc with two sessions, the written lines are not indented the same way
            string[] lines =
            [
                "FILE \"a.bin\" BINARY",
                "",
                "REM ORIGINAL MEDIA-TYPE: CD-RW",
                "",
                "  REM SESSION 01        (*)",
                "    TRACK 01 AUDIO",
                "      INDEX 01 00:00:00",
                "         REM LBA: 0",
                "    TRACK 02 AUDIO",
                "      INDEX 00 01:00:00",
                "         REM LBA: 4500",
                "      INDEX 01 01:02:00",
                "         REM LBA: 4650",
                "",
                "  REM LEAD-OUT 02:00:00 (*)",
                "  REM SESSION 02        (*)",
                "    TRACK 03 MODE2/2352",
                "      INDEX 01 04:32:00",
                "         REM LBA: 20400",
                "",
                "REM (*) SESSION commands are not supported by all applications",
            ];

            Assert.Equal([1, 1, 2], GetTracks(lines).Select(t => t.Session));
            Assert.Equal(lines.Select(l => l.Trim()).Where(l => l.Length > 0), ReadAndWrite(lines).Select(l => l.Trim()));
        }

        /// <summary>
        /// Read a cue sheet with the given lines, write it back and return the written lines
        /// </summary>
        private static string[] ReadAndWrite(string[] lines)
        {
            var cueSheet = new CueSheet().Deserialize(CreateStream(lines));
            Assert.NotNull(cueSheet);
            var stream = new Writers.CueSheet().SerializeStream(cueSheet);
            Assert.NotNull(stream);
            var reader = new StreamReader(stream);
            List<string> written = [];
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                written.Add(line);
            }

            return [.. written];
        }

        /// <summary>
        /// Read a cue sheet with the given lines and return the tracks of all files
        /// </summary>
        private static CueTrack[] GetTracks(string[] lines)
        {
            var stream = CreateStream(lines);
            var actual = new CueSheet().Deserialize(stream);
            Assert.NotNull(actual);
            return [.. actual.Files.SelectMany(f => f.Tracks)];
        }

        /// <summary>
        /// Create a stream with the given lines, each ending with a line feed, in UTF-8 or the given encoding
        /// </summary>
        private static MemoryStream CreateStream(string[] lines, Encoding? encoding = null)
        {
            var builder = new StringBuilder();
            foreach (string line in lines)
            {
                builder.Append($"{line}\n");
            }

            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(builder.ToString()));
        }
    }
}
