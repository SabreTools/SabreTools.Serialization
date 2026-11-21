using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.CueSheets;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class CueDiscStreamTests
    {
        private static readonly byte[] Sync = [0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00];

        #region Sessions

        [Fact]
        public void EnhancedCd_SessionGaps()
        {
            // Session 1: two audio tracks, the second with a pregap; session 2: one Mode 2 track with a pregap in its
            // file; session 3: one Mode 1 track
            var cueSheet = Sheet(
                FileOf(Track(1, CueTrackDataType.AUDIO, 1, 0)),
                FileOf(Track(2, CueTrackDataType.AUDIO, 1, 0, 2)),
                FileOf(Track(3, CueTrackDataType.MODE2_2352, 2, 0, 2)),
                FileOf(Track(4, CueTrackDataType.MODE1_2352, 3, 0)));
            using var stream = new CueDiscStream(cueSheet,
            [
                Bin(Audio(), Audio(), Audio(), Audio()),
                Bin(Audio(), Audio(), Audio(), Audio(), Audio()),
                Bin([.. Enumerable.Range(0, 8).Select(i => Mode2((byte)(0x30 + i), form2: false))]),
                Bin(Mode1(0x40), Mode1(0x41), Mode1(0x42)),
            ]);

            // Session 1 ends at 9; 9 + 11400 = 11409; session 2 ends at 11415; 11415 + 6900 = 18315
            Assert.Equal([0, 11409, 18315], stream.SessionStarts);
            Assert.Equal(18318L * 2048, stream.Length);
            Assert.Equal(Block(0), ReadBlockAt(stream, 0));
            Assert.Equal(Block(0), ReadBlockAt(stream, 9));
            Assert.Equal(Block(0), ReadBlockAt(stream, 11406));
            Assert.Equal(Block(0x30), ReadBlockAt(stream, 11407));
            Assert.Equal(Block(0x32), ReadBlockAt(stream, 11409));
            Assert.Equal(Block(0x37), ReadBlockAt(stream, 11414));
            Assert.Equal(Block(0), ReadBlockAt(stream, 11415));
            Assert.Equal(Block(0x40), ReadBlockAt(stream, 18315));
            Assert.Equal(Block(0x42), ReadBlockAt(stream, 18317));
        }

        [Fact]
        public void SessionChangeInsideFile_IndexesAreDiscAddresses()
        {
            // One file for both sessions, as in the image cue sheet of DiscImageCreator: session 1 ends at 10, the
            // indexes of session 2 are disc addresses, the file holds no gap
            var cueSheet = Sheet(FileOf(
                Track(1, CueTrackDataType.MODE1_2352, 1, 0),
                Track(2, CueTrackDataType.MODE1_2352, 2, 11410),
                Track(3, CueTrackDataType.MODE1_2352, 2, 11412)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin([.. Enumerable.Range(0, 15).Select(i => Mode1((byte)(0x60 + i)))])]);

            Assert.Equal([0, 11410], stream.SessionStarts);
            Assert.Equal(11415L * 2048, stream.Length);
            Assert.Equal(Block(0x69), ReadBlockAt(stream, 9));
            Assert.Equal(Block(0), ReadBlockAt(stream, 10));
            Assert.Equal(Block(0x6A), ReadBlockAt(stream, 11410));
            Assert.Equal(Block(0x6C), ReadBlockAt(stream, 11412));
            Assert.Equal(Block(0x6E), ReadBlockAt(stream, 11414));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SessionChangeInsideFile_FileHoldsGap(bool pregap)
        {
            // One file for both sessions, as from IsoBuster and ImgBurn: the indexes of session 2 are disc addresses,
            // the file holds the gap as empty sectors, and the sector at 11410 has 02:34:10 (11410 + 150) in its header;
            // with a pregap, INDEX 00 of track 2 is at 11260, in the gap
            var cueSheet = Sheet(FileOf(
                Track(1, CueTrackDataType.MODE1_2352, 1, 0),
                Track(2, CueTrackDataType.MODE1_2352, 2, pregap ? [11260, 11410] : [11410])));
            byte[] image = new byte[11415 * 2352];
            for (int i = 0; i < 10; i++)
            {
                Put(image, i, Mode1((byte)(0x60 + i)));
            }

            for (int i = 0; i < 5; i++)
            {
                Put(image, 11410 + i, Mode1((byte)(0x6A + i), 0x02, 0x34, (byte)(0x10 + i)));
            }

            using var stream = new CueDiscStream(cueSheet, [new MemoryStream(image)]);

            Assert.Equal([0, 11410], stream.SessionStarts);
            Assert.Equal(11415L * 2048, stream.Length);
            Assert.Equal(Block(0x69), ReadBlockAt(stream, 9));
            Assert.Equal(Block(0), ReadBlockAt(stream, 10));
            Assert.Equal(Block(0), ReadBlockAt(stream, 11409));
            Assert.Equal(Block(0x6A), ReadBlockAt(stream, 11410));
            Assert.Equal(Block(0x6E), ReadBlockAt(stream, 11414));
        }

        [Theory]
        [InlineData(0x05, 0x06, 0x10, true)]
        [InlineData(0x02, 0x34, 0x10, false)]
        public void SessionChangeInsideFile_NoAddressAtIndex_GapLeftOut(byte minutes, byte seconds, byte frames, bool sync)
        {
            // One file without the gap, as from DiscImageCreator, that is long enough to have a sector at 11410: there,
            // the sector 11400 further on the disc has 05:06:10 (22810 + 150) in its header, or a sector without a
            // valid sync has 02:34:10
            var cueSheet = Sheet(FileOf(
                Track(1, CueTrackDataType.MODE1_2352, 1, 0),
                Track(2, CueTrackDataType.MODE1_2352, 2, 11410)));
            byte[] atIndex = Mode1(0x6B, minutes, seconds, frames);
            if (!sync)
                atIndex[1] = 0x00;

            byte[] image = new byte[11411 * 2352];
            for (int i = 0; i < 10; i++)
            {
                Put(image, i, Mode1((byte)(0x60 + i)));
            }

            Put(image, 10, Mode1(0x6A, 0x02, 0x34, 0x10));
            Put(image, 11410, atIndex);
            using var stream = new CueDiscStream(cueSheet, [new MemoryStream(image)]);

            Assert.Equal([0, 11410], stream.SessionStarts);
            Assert.Equal(22811L * 2048, stream.Length);
            Assert.Equal(Block(0), ReadBlockAt(stream, 10));
            Assert.Equal(Block(0x6A), ReadBlockAt(stream, 11410));
            Assert.Equal(Block(sync ? (byte)0x6B : (byte)0), ReadBlockAt(stream, 22810));
        }

        [Fact]
        public void SessionChangeInsideFile_NextFileAfterGap()
        {
            // Sessions 1 and 2 in one file, session 3 in the next file
            var cueSheet = Sheet(
                FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0), Track(2, CueTrackDataType.MODE1_2352, 2, 11410)),
                FileOf(Track(3, CueTrackDataType.MODE1_2352, 3, 0)));
            using var stream = new CueDiscStream(cueSheet,
            [
                Bin([.. Enumerable.Range(0, 15).Select(i => Mode1((byte)(0x60 + i)))]),
                Bin(Mode1(0x70)),
            ]);

            // Session 2 ends at 11415; 11415 + 6900 = 18315
            Assert.Equal([0, 11410, 18315], stream.SessionStarts);
            Assert.Equal(18316L * 2048, stream.Length);
            Assert.Equal(Block(0x6E), ReadBlockAt(stream, 11414));
            Assert.Equal(Block(0), ReadBlockAt(stream, 11415));
            Assert.Equal(Block(0x70), ReadBlockAt(stream, 18315));
        }

        [Fact]
        public void NoRemSession_OneSession()
        {
            var cueSheet = Sheet(
                FileOf(Track(1, CueTrackDataType.MODE1_2352, 0, 0)),
                FileOf(Track(2, CueTrackDataType.AUDIO, 0, 0, 2)));
            using var stream = new CueDiscStream(cueSheet, [Bin(Mode1(0x01), Mode1(0x02)), Bin(Audio(), Audio(), Audio())]);

            Assert.Equal([0], stream.SessionStarts);
            Assert.Equal(5L * 2048, stream.Length);
            Assert.Equal(Block(0x02), ReadBlockAt(stream, 1));
        }

        [Fact]
        public void FirstTrackPregap_StartsAtSectorZero()
        {
            // INDEX 00 of the first track holds sectors 0 and 1, INDEX 01 is at sector 2
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0, 2)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin([.. Enumerable.Range(0, 5).Select(i => Mode1((byte)(0x70 + i)))])]);

            Assert.Equal([0], stream.SessionStarts);
            Assert.Equal(5L * 2048, stream.Length);
            Assert.Equal(Block(0x70), ReadBlockAt(stream, 0));
            Assert.Equal(Block(0x72), ReadBlockAt(stream, 2));
            Assert.Equal(Block(0x74), ReadBlockAt(stream, 4));
        }

        [Fact]
        public void SingleDensityArea_Read()
        {
            var track = Track(1, CueTrackDataType.MODE1_2352, 0, 0);
            track.DensityArea = CueDensityArea.SingleDensity;
            using var stream = new CueDiscStream(Sheet(FileOf(track)), [Bin(Mode1(0x01))]);

            Assert.Equal(Block(0x01), ReadBlockAt(stream, 0));
        }

        [Fact]
        public void HighDensityArea_Throws()
        {
            // The high-density area of a GD-ROM starts at a sector address that the cue sheet does not give
            var first = Track(1, CueTrackDataType.MODE1_2352, 0, 0);
            first.DensityArea = CueDensityArea.SingleDensity;
            var second = Track(2, CueTrackDataType.MODE1_2352, 0, 0);
            second.DensityArea = CueDensityArea.HighDensity;
            var cueSheet = Sheet(FileOf(first), FileOf(second));

            Assert.Throws<InvalidDataException>(() => new CueDiscStream(cueSheet, [Bin(Mode1(0x01)), Bin(Mode1(0x02))]));
        }

        #endregion

        #region Sector formats

        [Theory]
        [InlineData(0, 0xA0)]
        [InlineData(1, 0xA1)]
        [InlineData(2, 0xA2)]
        [InlineData(3, 0x00)]
        [InlineData(4, 0x00)]
        public void RawSector_UserData(int sector, int expected)
        {
            byte[] badSync = Mode1(0xA3);
            badSync[5] = 0x00;
            byte[] mode0 = Mode1(0xA4);
            mode0[15] = 0;

            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE2_2352, 1, 0)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin(Mode1(0xA0), Mode2(0xA1, form2: false), Mode2(0xA2, form2: true), badSync, mode0)]);

            Assert.Equal(Block((byte)expected), ReadBlockAt(stream, sector));
        }

        [Fact]
        public void AudioTrack_Zeros()
        {
            // Audio holding bytes that look like a Mode 1 sector still has no user data
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.AUDIO, 1, 0)));
            using var stream = new CueDiscStream(cueSheet, [Bin(Mode1(0xB0))]);

            Assert.Equal(Block(0), ReadBlockAt(stream, 0));
        }

        [Fact]
        public void CdgTrack_ZerosAndSectorSize()
        {
            // 25 sectors of 2448 bytes would count as 26 sectors of 2352 bytes
            byte[] cdgSector = [.. Mode1(0xB1), .. new byte[96]];
            var cueSheet = Sheet(
                FileOf(Track(1, CueTrackDataType.CDG, 1, 0)),
                FileOf(Track(2, CueTrackDataType.MODE1_2352, 1, 0)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin([.. Enumerable.Repeat(cdgSector, 25)]), Bin(Mode1(0xB2))]);

            Assert.Equal(26L * 2048, stream.Length);
            Assert.Equal(Block(0), ReadBlockAt(stream, 0));
            Assert.Equal(Block(0xB2), ReadBlockAt(stream, 25));
        }

        [Fact]
        public void Mode1_2048_UserData()
        {
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2048, 1, 0)));
            using var stream = new CueDiscStream(cueSheet, [Bin(Block(0xC0), Block(0xC1))]);

            Assert.Equal(2L * 2048, stream.Length);
            Assert.Equal(Block(0xC1), ReadBlockAt(stream, 1));
        }

        [Theory]
        [InlineData(CueTrackDataType.MODE2_2336)]
        [InlineData(CueTrackDataType.CDI_2336)]
        public void Mode2_2336_UserData(CueTrackDataType dataType)
        {
            var cueSheet = Sheet(FileOf(Track(1, dataType, 1, 0)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin(Mode2(0xD0, form2: false).Skip(16).ToArray(), Mode2(0xD1, form2: true).Skip(16).ToArray())]);

            Assert.Equal(2L * 2048, stream.Length);
            Assert.Equal(Block(0xD0), ReadBlockAt(stream, 0));
            Assert.Equal(Block(0xD1), ReadBlockAt(stream, 1));
        }

        #endregion

        #region Invalid input

        [Fact]
        public void FileCountMismatch_Throws()
        {
            var cueSheet = Sheet(
                FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0)),
                FileOf(Track(2, CueTrackDataType.MODE1_2352, 1, 0)));

            Assert.Throws<ArgumentException>(() => new CueDiscStream(cueSheet, [Bin(Mode1(0x01))]));
        }

        [Fact]
        public void MixedSectorSizeInFile_Throws()
        {
            var cueSheet = Sheet(FileOf(
                Track(1, CueTrackDataType.MODE1_2048, 1, 0),
                Track(2, CueTrackDataType.AUDIO, 1, 2)));

            Assert.Throws<InvalidDataException>(() => new CueDiscStream(cueSheet, [Bin(Block(0x01), Block(0x02))]));
        }

        [Fact]
        public void TrackWithoutIndex01_Throws()
        {
            var track = Track(1, CueTrackDataType.MODE1_2352, 1, 0);
            track.Indices[0].Index = 0;

            Assert.Throws<InvalidDataException>(() => new CueDiscStream(Sheet(FileOf(track)), [Bin(Mode1(0x01))]));
        }

        [Fact]
        public void EmptyFile_EmptyStream()
        {
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0)));
            using var stream = new CueDiscStream(cueSheet, [Bin()]);

            Assert.Equal(0, stream.Length);
            Assert.Equal(0, stream.Read(new byte[16], 0, 16));
        }

        [Fact]
        public void TrackFileTooShort_MissingSectorsZeros()
        {
            // Track 2 starts after the end of the file
            var cueSheet = Sheet(FileOf(
                Track(1, CueTrackDataType.MODE1_2352, 1, 0),
                Track(2, CueTrackDataType.MODE1_2352, 1, 10)));
            using var stream = new CueDiscStream(cueSheet,
                [Bin([.. Enumerable.Range(0, 5).Select(i => Mode1((byte)(0x10 + i)))])]);

            Assert.Equal(10L * 2048, stream.Length);
            Assert.Equal(Block(0x14), ReadBlockAt(stream, 4));
            Assert.Equal(Block(0), ReadBlockAt(stream, 7));
        }

        #endregion

        #region Stream

        [Fact]
        public void Read_UnalignedAcrossBlocks()
        {
            using var stream = TwoBlockStream();
            stream.Position = 2038;

            byte[] buffer = new byte[40];
            Assert.Equal(30, stream.Read(buffer, 5, 30));

            byte[] expected = [.. new byte[5], .. Enumerable.Repeat<byte>(0x01, 10), .. Enumerable.Repeat<byte>(0x02, 20), .. new byte[5]];
            Assert.Equal(expected, buffer);
            Assert.Equal(2068, stream.Position);
        }

        [Fact]
        public void Read_AtEnd_Partial()
        {
            using var stream = TwoBlockStream();
            stream.Position = stream.Length - 100;

            Assert.Equal(100, stream.Read(new byte[4096], 0, 4096));
            Assert.Equal(0, stream.Read(new byte[4096], 0, 4096));
            Assert.Equal(stream.Length, stream.Position);
        }

        [Fact]
        public void Read_PastEnd_Zero()
        {
            using var stream = TwoBlockStream();
            stream.Position = stream.Length + 5000;

            Assert.Equal(0, stream.Read(new byte[16], 0, 16));
        }

        [Fact]
        public void Read_SameBlock_OneFileRead()
        {
            var file = new CountingStream(Concat(Mode1(0x01), Mode1(0x02)));
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0)));
            using var stream = new CueDiscStream(cueSheet, [file]);

            byte[] buffer = new byte[1];
            for (int i = 0; i < 2048; i++)
            {
                Assert.Equal(1, stream.Read(buffer, 0, 1));
                Assert.Equal(0x01, buffer[0]);
            }

            Assert.Equal(1, file.Reads);
        }

        [Fact]
        public void Seek_Origins()
        {
            using var stream = TwoBlockStream();

            Assert.Equal(10, stream.Seek(10, SeekOrigin.Begin));
            Assert.Equal(15, stream.Seek(5, SeekOrigin.Current));
            Assert.Equal(stream.Length - 1, stream.Seek(-1, SeekOrigin.End));
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(0, (SeekOrigin)3));
        }

        [Fact]
        public void Seek_Negative_Throws()
        {
            using var stream = TwoBlockStream();
            stream.Position = 10;

            Assert.Throws<IOException>(() => stream.Seek(-11, SeekOrigin.Current));
            Assert.Equal(10, stream.Position);
        }

        [Fact]
        public void Read_NullBuffer_Throws()
        {
            using var stream = TwoBlockStream();

            var exception = Assert.Throws<ArgumentNullException>(() => stream.Read(null!, 0, 16));
            Assert.Equal("buffer", exception.ParamName);
        }

        [Theory]
        [InlineData(-1, 16, "offset")]
        [InlineData(0, -1, "count")]
        public void Read_NegativeArgument_Throws(int offset, int count, string paramName)
        {
            using var stream = TwoBlockStream();

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(new byte[16], offset, count));
            Assert.Equal(paramName, exception.ParamName);
        }

        [Theory]
        [InlineData(0, 4096)]
        [InlineData(1, int.MaxValue)]
        public void Read_PastBufferEnd_ThrowsBeforeReading(int offset, int count)
        {
            using var stream = TwoBlockStream();

            Assert.Throws<ArgumentException>(() => stream.Read(new byte[4000], offset, count));
            Assert.Equal(0, stream.Position);
        }

        [Fact]
        public void Position_Negative_Throws()
        {
            using var stream = TwoBlockStream();

            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);
        }

        [Fact]
        public void Dispose_DisposesFiles()
        {
            var file = Bin(Mode1(0x01));
            var stream = new CueDiscStream(Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2352, 1, 0))), [file]);

            stream.Dispose();

            Assert.False(file.CanRead);
        }

        #endregion

        #region Create

        [Fact]
        public void Create_TrackFilesNextToCue()
        {
            string directory = CreateTempDirectory();
            try
            {
                File.WriteAllBytes(Path.Combine(directory, "Disc (Track 1).bin"), Concat(Mode1(0x01), Mode1(0x02)));
                File.WriteAllBytes(Path.Combine(directory, "Disc (Track 2).bin"), Mode1(0x03));
                string cuePath = WriteCue(directory, "Disc (Track 1).bin", "Disc (Track 2).bin");

                using var stream = CueDiscStream.Create(cuePath);

                Assert.NotNull(stream);
                Assert.Equal([0, 11402], stream.SessionStarts);
                Assert.Equal(Block(0x03), ReadBlockAt(stream, 11402));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        [Fact]
        public void Create_InvalidCue_Null()
        {
            string directory = CreateTempDirectory();
            try
            {
                string cuePath = Path.Combine(directory, "Disc.cue");
                File.WriteAllBytes(cuePath, [.. Enumerable.Repeat<byte>(0xFF, 1024)]);

                Assert.Null(CueDiscStream.Create(cuePath));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        [Fact]
        public void Create_MissingTrackFile_NullAndOpenedFilesClosed()
        {
            string directory = CreateTempDirectory();
            try
            {
                string firstPath = Path.Combine(directory, "Disc (Track 1).bin");
                File.WriteAllBytes(firstPath, Mode1(0x01));
                string cuePath = WriteCue(directory, "Disc (Track 1).bin", "Disc (Track 2).bin");

                Assert.Null(CueDiscStream.Create(cuePath));

                // Fails while the first track file is still open
                using var exclusive = new FileStream(firstPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        #endregion

        #region Helpers

        private sealed class CountingStream(byte[] data) : MemoryStream(data)
        {
            public int Reads { get; private set; }

            public override int Read(byte[] buffer, int offset, int count)
            {
                Reads++;
                return base.Read(buffer, offset, count);
            }
        }

        private static CueSheet Sheet(params CueFile[] files)
            => new() { Files = files };

        private static CueFile FileOf(params CueTrack[] tracks)
            => new() { FileName = "Track.bin", FileType = CueFileType.BINARY, Tracks = tracks };

        /// <summary>
        /// Track with INDEX 01 at the last given sector and INDEX 00 at the one before, if given
        /// </summary>
        private static CueTrack Track(int number, CueTrackDataType dataType, int session, params int[] sectors)
        {
            var indices = new CueIndex[sectors.Length];
            for (int i = 0; i < sectors.Length; i++)
            {
                indices[i] = new CueIndex
                {
                    Index = i + 2 - sectors.Length,
                    Minutes = sectors[i] / 75 / 60,
                    Seconds = sectors[i] / 75 % 60,
                    Frames = sectors[i] % 75,
                };
            }

            return new CueTrack { Number = number, DataType = dataType, Session = session, Indices = indices };
        }

        private static CueDiscStream TwoBlockStream()
        {
            var cueSheet = Sheet(FileOf(Track(1, CueTrackDataType.MODE1_2048, 1, 0)));
            return new CueDiscStream(cueSheet, [Bin(Block(0x01), Block(0x02))]);
        }

        private static MemoryStream Bin(params byte[][] sectors)
            => new(Concat(sectors));

        private static void Put(byte[] image, int position, byte[] sector)
            => Array.Copy(sector, 0, image, position * 2352, sector.Length);

        private static byte[] Concat(params byte[][] parts)
            => [.. parts.SelectMany(p => p)];

        private static byte[] Block(byte value)
            => [.. Enumerable.Repeat(value, 2048)];

        private static byte[] Audio()
            => [.. Enumerable.Repeat<byte>(0x55, 2352)];

        /// <summary>
        /// Raw Mode 1 sector: sync, header with the BCD address, user data filled with the value, EDC/ECC bytes 0xEE
        /// </summary>
        private static byte[] Mode1(byte value, byte minutes = 0x00, byte seconds = 0x02, byte frames = 0x00)
        {
            byte[] sector = [.. Enumerable.Repeat<byte>(0xEE, 2352)];
            Array.Copy(Sync, sector, Sync.Length);
            sector[12] = minutes;
            sector[13] = seconds;
            sector[14] = frames;
            sector[15] = 1;
            Array.Copy(Block(value), 0, sector, 16, 2048);
            return sector;
        }

        /// <summary>
        /// Raw Mode 2 sector: sync, header, subheader, 2048 bytes filled with the value, the rest 0xEE
        /// </summary>
        private static byte[] Mode2(byte value, bool form2)
        {
            byte[] sector = [.. Enumerable.Repeat<byte>(0xEE, 2352)];
            Array.Copy(Sync, sector, Sync.Length);
            sector[12] = 0x00;
            sector[13] = 0x02;
            sector[14] = 0x00;
            sector[15] = 2;
            byte submode = form2 ? (byte)0x20 : (byte)0x08;
            byte[] subheader = [0x00, 0x00, submode, 0x00, 0x00, 0x00, submode, 0x00];
            Array.Copy(subheader, 0, sector, 16, subheader.Length);
            Array.Copy(Block(value), 0, sector, 24, 2048);
            return sector;
        }

        private static byte[] ReadBlockAt(Stream stream, long sector)
        {
            byte[] block = new byte[2048];
            stream.Seek(sector * 2048, SeekOrigin.Begin);
            int read = 0;
            while (read < block.Length)
            {
                int n = stream.Read(block, read, block.Length - read);
                Assert.True(n > 0);
                read += n;
            }

            return block;
        }

        private static string CreateTempDirectory()
        {
            string directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);
            return directory;
        }

        /// <summary>
        /// Write a cue sheet with one Mode 1 track per file, one session per file
        /// </summary>
        private static string WriteCue(string directory, params string[] fileNames)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < fileNames.Length; i++)
            {
                builder.Append($"REM SESSION {i + 1:D2}\n");
                builder.Append($"FILE \"{fileNames[i]}\" BINARY\n");
                builder.Append($"  TRACK {i + 1:D2} MODE1/2352\n");
                builder.Append("    INDEX 01 00:00:00\n");
            }

            string cuePath = Path.Combine(directory, "Disc.cue");
            File.WriteAllText(cuePath, builder.ToString());
            return cuePath;
        }

        #endregion
    }
}
