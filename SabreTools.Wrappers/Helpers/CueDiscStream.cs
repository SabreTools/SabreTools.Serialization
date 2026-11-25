using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.CueSheets;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// A read-only seekable stream over the track files of a cue sheet that reads like a CD drive with 2048-byte
    /// blocks: one block per sector address, sector 0 at the start of the first track file
    /// </summary>
    /// <remarks>
    /// Mode 1 and Mode 2 Form 1 sectors give their user data. Mode 2 Form 2 sectors give the first 2048 of their 2324
    /// data bytes, as a drive answers READ(10) there. Audio, CD+G and sectors without a valid header read as zeros, as
    /// does the gap between two sessions (lead-out, lead-in and pregap). Sessions come from the REM SESSION lines of
    /// the cue sheet. When a session starts within a file, the indexes from there on are disc addresses that count the
    /// gap: IsoBuster and ImgBurn write the gap into the file as empty sectors, and the image file of
    /// DiscImageCreator leaves it out. The file holds the gap when the sector at INDEX 01 of the first track of the
    /// session has that address in its header. The high-density area of a GD-ROM is not supported.
    /// </remarks>
    public sealed class CueDiscStream : Stream
    {
        /// <summary>
        /// Sectors from the end of the first session to the first track of the second: lead-out 6750, lead-in 4500, pregap 150
        /// </summary>
        private const long FirstSessionGap = 11400;

        /// <summary>
        /// Sectors from the end of a later session to the first track of the next: lead-out 2250, lead-in 4500, pregap 150
        /// </summary>
        private const long LaterSessionGap = 6900;

        /// <summary>
        /// Size of a block as read from a drive
        /// </summary>
        private const int BlockSize = 2048;

        /// <summary>
        /// Sync pattern at the start of a raw data sector
        /// </summary>
        private static readonly byte[] Sync = [0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00];

        /// <summary>
        /// Part of a track file that maps onto consecutive sector addresses
        /// </summary>
        private sealed class Extent
        {
            public Stream File = Stream.Null;
            public long FileOffset;
            public long Start;
            public long Count;
            public int SectorSize;
            public CueTrackDataType DataType;
        }

        private readonly Stream[] _files;
        private readonly Extent[] _extents;
        private readonly long[] _starts;
        private readonly byte[] _raw = new byte[2352];
        private readonly byte[] _block = new byte[BlockSize];
        private long _cachedSector = -1;
        private long _position;

        /// <summary>
        /// Sector address of the start of each session, in session order: 0 for the first session, INDEX 01 of the
        /// first track for a later one
        /// </summary>
        public long[] SessionStarts { get; }

        /// <summary>
        /// Create a stream over already opened track files
        /// </summary>
        /// <param name="cueSheet">Cue sheet describing the track files</param>
        /// <param name="files">Readable, seekable track files in the order of the FILE lines; disposed with this stream</param>
        public CueDiscStream(CueSheet cueSheet, Stream[] files)
        {
            if (files.Length != cueSheet.Files.Length)
                throw new ArgumentException("One stream per FILE line is required", nameof(files));

            foreach (var cueFile in cueSheet.Files)
            {
                foreach (var track in cueFile.Tracks)
                {
                    if (track.DensityArea == CueDensityArea.HighDensity)
                        throw new InvalidDataException("The high-density area of a GD-ROM is not supported");
                }
            }

            _files = files;
            List<Extent> extents = [];
            List<long> sessionStarts = [];

            // Sector of the current file within all files laid end to end, and the address of that sector; the first
            // track file starts at sector 0, also when INDEX 01 of the first track comes later
            long fileStart = 0;
            long offset = 0;
            int session = 0;
            for (int i = 0; i < files.Length; i++)
            {
                CueTrack[] tracks = cueSheet.Files[i].Tracks;
                int sectorSize = GetSectorSize(tracks[0].DataType);
                long fileSectors = files[i].Length / sectorSize;
                var fileExtents = new Extent[tracks.Length];

                // Sectors of the session gaps within this file, which its indexes count and its data does not hold
                long skipped = 0;

                for (int j = 0; j < tracks.Length; j++)
                {
                    if (GetSectorSize(tracks[j].DataType) != sectorSize)
                        throw new InvalidDataException("Tracks of one file must share the sector size");

                    long first = GetSector(tracks[j].Indices[0]);
                    long index01 = GetIndex01(tracks[j]);

                    if (sessionStarts.Count == 0)
                    {
                        session = tracks[j].Session;
                        sessionStarts.Add(0);
                    }
                    else if (tracks[j].Session != session)
                    {
                        long gap = sessionStarts.Count == 1 ? FirstSessionGap : LaterSessionGap;
                        if (j == 0)
                        {
                            // A session in files of its own, as in the cue sheets of redump and redumper: the
                            // previous session ends where this track's first index starts
                            long previousEnd = fileStart + first + offset;
                            offset = previousEnd + gap - (fileStart + index01);
                        }
                        else if (!HasAddress(files[i], index01 * sectorSize, fileStart + index01 + offset))
                        {
                            // A session within a file that leaves out the gap, as the image file of DiscImageCreator
                            skipped += gap;
                        }

                        session = tracks[j].Session;
                        sessionStarts.Add(fileStart + index01 + offset);
                    }

                    fileExtents[j] = new Extent
                    {
                        File = files[i],
                        FileOffset = (first - skipped) * sectorSize,
                        Start = fileStart + first + offset,
                        SectorSize = sectorSize,
                        DataType = tracks[j].DataType,
                    };
                }

                // A track ends in the file where the next one starts
                for (int j = 0; j < tracks.Length; j++)
                {
                    long end = j + 1 < tracks.Length ? fileExtents[j + 1].FileOffset / sectorSize : fileSectors;
                    fileExtents[j].Count = end - (fileExtents[j].FileOffset / sectorSize);
                    if (fileExtents[j].Count > 0)
                        extents.Add(fileExtents[j]);
                }

                fileStart += fileSectors;
                offset += skipped;
            }

            _extents = [.. extents];
            _starts = new long[_extents.Length];
            for (int i = 0; i < _extents.Length; i++)
            {
                _starts[i] = _extents[i].Start;
            }

            SessionStarts = [.. sessionStarts];
        }

        /// <summary>
        /// Open the track files of a cue sheet, relative to the directory of the cue sheet
        /// </summary>
        /// <param name="cuePath">Path to the cue sheet</param>
        /// <returns>A disc stream on success, null on failure</returns>
        public static CueDiscStream? Create(string cuePath)
        {
            List<Stream> files = [];
            try
            {
                CueSheet? cueSheet;
                using (var cueStream = File.OpenRead(cuePath))
                {
                    cueSheet = new Serialization.Readers.CueSheet().Deserialize(cueStream);
                }

                if (cueSheet is null)
                    return null;

                string directory = Path.GetDirectoryName(Path.GetFullPath(cuePath)) ?? string.Empty;
                foreach (var cueFile in cueSheet.Files)
                {
                    files.Add(File.OpenRead(Path.Combine(directory, cueFile.FileName)));
                }

                return new CueDiscStream(cueSheet, [.. files]);
            }
            catch
            {
                foreach (var file in files)
                {
                    file.Dispose();
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                if (_extents.Length == 0)
                    return 0;

                Extent last = _extents[_extents.Length - 1];
                return (last.Start + last.Count) * BlockSize;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get => _position;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _position = value;
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count > buffer.Length - offset)
                throw new ArgumentException("offset + count exceeds buffer length");

            // Nothing is read at or past the end
            count = (int)Math.Min(count, Length - _position);
            int totalRead = 0;
            while (totalRead < count)
            {
                long sector = _position / BlockSize;
                int inBlock = (int)(_position % BlockSize);
                int chunk = Math.Min(count - totalRead, BlockSize - inBlock);

                if (sector != _cachedSector)
                {
                    ReadBlock(sector);
                    _cachedSector = sector;
                }

                Array.Copy(_block, inBlock, buffer, offset + totalRead, chunk);
                totalRead += chunk;
                _position += chunk;
            }

            return totalRead;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            if (position < 0)
                throw new IOException("Seek position cannot be negative.");

            _position = position;
            return _position;
        }

        /// <inheritdoc/>
        public override void Flush() { }

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            foreach (var file in _files)
            {
                file.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Fill the block buffer with what a drive returns for a sector address
        /// </summary>
        private void ReadBlock(long sector)
        {
            Array.Clear(_block, 0, BlockSize);

            int index = Array.BinarySearch(_starts, sector);
            if (index < 0)
                index = ~index - 1;

            Extent extent = _extents[index];
            if (sector >= extent.Start + extent.Count)
                return;
            if (extent.DataType == CueTrackDataType.AUDIO || extent.DataType == CueTrackDataType.CDG)
                return;

            extent.File.Seek(extent.FileOffset + ((sector - extent.Start) * extent.SectorSize), SeekOrigin.Begin);
            if (!ReadRaw(extent.File, extent.SectorSize))
                return;

            int dataStart = GetUserDataStart(extent.SectorSize);
            if (dataStart >= 0)
                Array.Copy(_raw, dataStart, _block, 0, BlockSize);
        }

        /// <summary>
        /// Check whether the raw sector at a file offset has a valid sync and a sector address in its header
        /// </summary>
        private bool HasAddress(Stream file, long fileOffset, long sector)
        {
            file.Seek(fileOffset, SeekOrigin.Begin);
            if (!ReadRaw(file, 16) || !HasSync())
                return false;

            // The header holds minutes, seconds and frames in BCD, counted from 150 sectors before sector 0
            long address = (((FromBcd(_raw[12]) * 60) + FromBcd(_raw[13])) * 75) + FromBcd(_raw[14]) - 150;
            return address == sector;
        }

        /// <summary>
        /// Read bytes from the current position of a file into the raw sector buffer
        /// </summary>
        /// <returns>True if all bytes were read, false if the file ended before</returns>
        private bool ReadRaw(Stream file, int count)
        {
            int read = 0;
            while (read < count)
            {
                int n = file.Read(_raw, read, count - read);
                if (n == 0)
                    return false;

                read += n;
            }

            return true;
        }

        /// <summary>
        /// Get where the 2048 user bytes start in the raw sector buffer, -1 if the sector has none
        /// </summary>
        private int GetUserDataStart(int sectorSize)
        {
            if (sectorSize == BlockSize)
                return 0;

            // Mode 2 without sync and header: the subheader comes first
            if (sectorSize == 2336)
                return 8;

            if (!HasSync())
                return -1;

            return _raw[15] switch
            {
                1 => 16,
                2 => 24,
                _ => -1,
            };
        }

        /// <summary>
        /// Check whether the raw sector buffer starts with the sync pattern of a data sector
        /// </summary>
        private bool HasSync()
        {
            for (int i = 0; i < Sync.Length; i++)
            {
                if (_raw[i] != Sync[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Convert a binary-coded decimal byte to its value
        /// </summary>
        private static int FromBcd(byte value)
            => ((value >> 4) * 10) + (value & 0x0F);

        /// <summary>
        /// Get the size of a sector in a track file
        /// </summary>
        private static int GetSectorSize(CueTrackDataType dataType)
        {
            return dataType switch
            {
                CueTrackDataType.AUDIO => 2352,
                CueTrackDataType.CDG => 2448,
                CueTrackDataType.MODE1_2048 => 2048,
                CueTrackDataType.MODE1_2352 => 2352,
                CueTrackDataType.MODE2_2336 => 2336,
                CueTrackDataType.MODE2_2352 => 2352,
                CueTrackDataType.CDI_2336 => 2336,
                CueTrackDataType.CDI_2352 => 2352,
                _ => 2352,
            };
        }

        /// <summary>
        /// Get the sector within the track file of an index
        /// </summary>
        private static long GetSector(CueIndex index)
            => (((index.Minutes * 60) + index.Seconds) * 75) + index.Frames;

        /// <summary>
        /// Get the sector within the track file of INDEX 01
        /// </summary>
        private static long GetIndex01(CueTrack track)
        {
            foreach (var index in track.Indices)
            {
                if (index.Index == 1)
                    return GetSector(index);
            }

            throw new InvalidDataException("Every track needs an INDEX 01");
        }
    }
}
