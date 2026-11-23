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
