using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.CueSheets;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class CueSheetTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new CueSheet();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new CueSheet();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_ShortSheet_Written()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files =
                [
                    CreateFile("a (Track 1).bin", CreateTrack(1, CueTrackDataType.MODE1_2352)),
                ],
            };

            string[] expected =
            [
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_LongSheet_Written()
        {
            // Longer than the StreamWriter buffer
            var files = new CueFile[99];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = CreateFile($"a (Track {i + 1:D2}).bin", CreateTrack(i + 1, CueTrackDataType.AUDIO));
            }

            string actual = Serialize(new Data.Models.CueSheets.CueSheet { Files = files });

            Assert.EndsWith("FILE \"a (Track 99).bin\" BINARY\n  TRACK 99 AUDIO\n    INDEX 01 00:00:00\n", actual);
        }

        [Fact]
        public void SerializeStream_TextValues_QuotedUtf8()
        {
            var track = CreateTrack(1, CueTrackDataType.AUDIO);
            track.Performer = " ";
            track.Songwriter = "Rare Candy";
            track.Title = "Lumi\u00e8re";
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Performer = "Barry \"Epoch\" Topping",
                Songwriter = "Lee Perry",
                Title = "Clair Obscur: Expedition 33 ",
                Files = [CreateFile("a (Track 1).bin", track)],
            };

            string[] expected =
            [
                "PERFORMER \"Barry \"Epoch\" Topping\"",
                "SONGWRITER \"Lee Perry\"",
                "TITLE \"Clair Obscur: Expedition 33 \"",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    PERFORMER \" \"",
                "    SONGWRITER \"Rare Candy\"",
                "    TITLE \"Lumi\u00e8re\"",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_SessionPerFile_RemSessionBeforeFile()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files =
                [
                    CreateFile("a (Track 1).bin", CreateTrack(1, CueTrackDataType.AUDIO, session: 1)),
                    CreateFile("a (Track 2).bin", CreateTrack(2, CueTrackDataType.AUDIO, session: 1)),
                    CreateFile("a (Track 3).bin", CreateTrack(3, CueTrackDataType.MODE1_2352, session: 2)),
                ],
            };

            string[] expected =
            [
                "REM SESSION 01",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:00:00",
                "REM SESSION 02",
                "FILE \"a (Track 3).bin\" BINARY",
                "  TRACK 03 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_SessionInsideFile_RemSessionBeforeTrack()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files =
                [
                    CreateFile("a.bin",
                        CreateTrack(1, CueTrackDataType.AUDIO, session: 1),
                        CreateTrack(2, CueTrackDataType.MODE1_2352, session: 2)),
                ],
            };

            string[] expected =
            [
                "REM SESSION 01",
                "FILE \"a.bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "REM SESSION 02",
                "  TRACK 02 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_DensityAreas_RemBeforeFile()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files =
                [
                    CreateFile("a (Track 1).bin", CreateTrack(1, CueTrackDataType.MODE1_2352, densityArea: CueDensityArea.SingleDensity)),
                    CreateFile("a (Track 2).bin", CreateTrack(2, CueTrackDataType.AUDIO, densityArea: CueDensityArea.SingleDensity)),
                    CreateFile("a (Track 3).bin", CreateTrack(3, CueTrackDataType.MODE1_2352, densityArea: CueDensityArea.HighDensity)),
                ],
            };

            string[] expected =
            [
                "REM SINGLE-DENSITY AREA",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:00:00",
                "REM HIGH-DENSITY AREA",
                "FILE \"a (Track 3).bin\" BINARY",
                "  TRACK 03 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_Remarks_WrittenInPlace()
        {
            var track = CreateTrack(1, CueTrackDataType.MODE1_2352);
            track.Title = "Data";
            track.Remarks = ["Before the pregap"];
            track.PreGap = new PreGap { Seconds = 2 };
            track.Indices[0].Remarks = ["LBA: 0", ""];
            var file = CreateFile("a.bin", track);
            file.Remarks = ["ORIGINAL MEDIA-TYPE: CD-RW"];
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Catalog = "0000000000000",
                Remarks = ["GENRE Game"],
                Files = [file],
            };

            string[] expected =
            [
                "CATALOG 0000000000000",
                "REM GENRE Game",
                "FILE \"a.bin\" BINARY",
                "REM ORIGINAL MEDIA-TYPE: CD-RW",
                "  TRACK 01 MODE1/2352",
                "    TITLE \"Data\"",
                "REM Before the pregap",
                "    PREGAP 00:02:00",
                "    INDEX 01 00:00:00",
                "REM LBA: 0",
                "REM",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_NullRemarks_Skipped()
        {
            var track = CreateTrack(1, CueTrackDataType.MODE1_2352);
            track.Remarks = null!;
            track.Indices[0].Remarks = null!;
            var file = CreateFile("a.bin", track);
            file.Remarks = null!;
            file.Indices = null!;
            var cueSheet = new Data.Models.CueSheets.CueSheet { Remarks = null!, Files = [file] };

            string[] expected =
            [
                "FILE \"a.bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_RemSessionInRemarks_NotRepeated()
        {
            // Layout written by IsoBuster
            var first = CreateTrack(1, CueTrackDataType.AUDIO, session: 1);
            first.Indices[0].Remarks = ["LEAD-OUT 02:00:00 (*)", "SESSION 02        (*)"];
            var file = CreateFile("a.bin", first, CreateTrack(2, CueTrackDataType.MODE2_2352, session: 2));
            file.Remarks = ["SESSION 01        (*)"];
            var cueSheet = new Data.Models.CueSheets.CueSheet { Files = [file] };

            string[] expected =
            [
                "FILE \"a.bin\" BINARY",
                "REM SESSION 01        (*)",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "REM LEAD-OUT 02:00:00 (*)",
                "REM SESSION 02        (*)",
                "  TRACK 02 MODE2/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_RemSessionInRemarksDiffers_RemSessionWritten()
        {
            // The session of the track is written after a REM line with another session
            var first = CreateTrack(1, CueTrackDataType.AUDIO, session: 1);
            first.Indices[0].Remarks = ["SESSION 02"];
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files = [CreateFile("a.bin", first, CreateTrack(2, CueTrackDataType.AUDIO, session: 1))],
            };

            string[] expected =
            [
                "REM SESSION 01",
                "FILE \"a.bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "REM SESSION 02",
                "REM SESSION 01",
                "  TRACK 02 AUDIO",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_FileRemarks_RemSessionAfterThem()
        {
            // The REM lines of the file could note the session themselves
            var file = CreateFile("a.bin", CreateTrack(1, CueTrackDataType.AUDIO, session: 1));
            file.Remarks = ["ORIGINAL MEDIA-TYPE: CD-RW"];
            var cueSheet = new Data.Models.CueSheets.CueSheet { Files = [file] };

            string[] expected =
            [
                "FILE \"a.bin\" BINARY",
                "REM ORIGINAL MEDIA-TYPE: CD-RW",
                "REM SESSION 01",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_DensityAreaInRemarks_NotRepeated()
        {
            // Layout written by redump for a GD-ROM
            var first = CreateTrack(1, CueTrackDataType.MODE1_2352, densityArea: CueDensityArea.SingleDensity);
            first.Indices[0].Remarks = ["HIGH-DENSITY AREA", "Comment"];
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Remarks = ["SINGLE-DENSITY AREA"],
                Files =
                [
                    CreateFile("a (Track 1).bin", first),
                    CreateFile("a (Track 2).bin", CreateTrack(2, CueTrackDataType.MODE1_2352, densityArea: CueDensityArea.HighDensity)),
                ],
            };

            string[] expected =
            [
                "REM SINGLE-DENSITY AREA",
                "FILE \"a (Track 1).bin\" BINARY",
                "  TRACK 01 MODE1/2352",
                "    INDEX 01 00:00:00",
                "REM HIGH-DENSITY AREA",
                "REM Comment",
                "FILE \"a (Track 2).bin\" BINARY",
                "  TRACK 02 MODE1/2352",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_FileIndices_WrittenBeforeTracks()
        {
            // Layout written by Exact Audio Copy and whipper, with the gap of a track at the end of the previous file
            var second = CreateTrack(2, CueTrackDataType.AUDIO);
            second.Indices = [new CueIndex { Index = 0, Minutes = 6, Seconds = 32, Frames = 45 }];
            var third = CreateTrack(3, CueTrackDataType.AUDIO);
            third.Indices = [new CueIndex { Index = 0, Minutes = 4, Seconds = 11, Frames = 3 }];
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files =
                [
                    CreateFile("01.bin", CreateTrack(1, CueTrackDataType.AUDIO), second),
                    new CueFile
                    {
                        FileName = "02.bin",
                        Remarks = ["Before the index"],
                        Indices = [new CueIndex { Index = 1, Remarks = ["After the index"] }],
                        Tracks = [third],
                    },
                    new CueFile { FileName = "03.bin", Indices = [new CueIndex { Index = 1 }] },
                ],
            };

            string[] expected =
            [
                "FILE \"01.bin\" BINARY",
                "  TRACK 01 AUDIO",
                "    INDEX 01 00:00:00",
                "  TRACK 02 AUDIO",
                "    INDEX 00 06:32:45",
                "FILE \"02.bin\" BINARY",
                "REM Before the index",
                "    INDEX 01 00:00:00",
                "REM After the index",
                "  TRACK 03 AUDIO",
                "    INDEX 00 04:11:03",
                "FILE \"03.bin\" BINARY",
                "    INDEX 01 00:00:00",
            ];
            Assert.Equal(JoinLines(expected), Serialize(cueSheet));
        }

        [Fact]
        public void SerializeStream_FileWithoutTracksOrIndices_Throws()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files = [new CueFile { FileName = "a.bin", FileType = CueFileType.BINARY }],
            };

            var serializer = new CueSheet();
            Assert.Throws<ArgumentException>(() => serializer.SerializeStream(cueSheet));
        }

        [Fact]
        public void SerializeStream_NullTrack_Throws()
        {
            var cueSheet = new Data.Models.CueSheets.CueSheet
            {
                Files = [new CueFile { FileName = "a.bin", FileType = CueFileType.BINARY, Tracks = [null!] }],
            };

            var serializer = new CueSheet();
            Assert.Throws<InvalidDataException>(() => serializer.SerializeStream(cueSheet));
        }

        /// <summary>
        /// Create a binary file with the given tracks
        /// </summary>
        private static CueFile CreateFile(string name, params CueTrack[] tracks)
        {
            return new CueFile { FileName = name, FileType = CueFileType.BINARY, Tracks = tracks };
        }

        /// <summary>
        /// Create a track with index 01 at 00:00:00
        /// </summary>
        private static CueTrack CreateTrack(int number, CueTrackDataType dataType, int session = 0, CueDensityArea densityArea = CueDensityArea.None)
        {
            return new CueTrack
            {
                Number = number,
                DataType = dataType,
                Session = session,
                DensityArea = densityArea,
                Indices = [new CueIndex { Index = 1 }],
            };
        }

        /// <summary>
        /// Join the given lines, each ending with a line feed
        /// </summary>
        private static string JoinLines(string[] lines)
        {
            var builder = new StringBuilder();
            foreach (string line in lines)
            {
                builder.Append($"{line}\n");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serialize a cue sheet and decode the result with line feeds
        /// </summary>
        private static string Serialize(Data.Models.CueSheets.CueSheet cueSheet)
        {
            var stream = new CueSheet().SerializeStream(cueSheet);
            Assert.NotNull(stream);
            var bytes = new MemoryStream();
            stream.CopyTo(bytes);

            // Decoded without detecting a byte order mark, so one would show up as U+FEFF
            return Encoding.UTF8.GetString(bytes.ToArray()).Replace("\r\n", "\n");
        }
    }
}
