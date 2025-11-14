using System;
using System.IO;
using System.Text.RegularExpressions;
using SabreTools.Data.Models.InstallShieldCabinet;
using SabreTools.Hashing;
using SabreTools.IO.Compression.zlib;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.InstallShieldCabinet.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldCabinet : IExtractable
    {
        #region Extension Properties

        /// <summary>
        /// Reference to the next cabinet header
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public InstallShieldCabinet? Next { get; set; }

        /// <summary>
        /// Reference to the next previous header
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public InstallShieldCabinet? Prev { get; set; }

        /// <summary>
        /// Volume index ID, 0 for headers
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public ushort VolumeID { get; set; }

        #endregion

        #region Extraction State

        /// <summary>
        /// Base filename path for related CAB files
        /// </summary>
        internal string? FilenamePattern { get; set; }

        #endregion

        #region Constants

        /// <summary>
        /// Default buffer size
        /// </summary>
        private const int BUFFER_SIZE = 64 * 1024;

        /// <summary>
        /// Maximum size of the window in bits
        /// </summary>
        private const int MAX_WBITS = 15;

        #endregion

        #region Cabinet Set

        /// <summary>
        /// Open a cabinet set for reading, if possible
        /// </summary>
        /// <param name="pattern">Filename pattern for matching cabinet files</param>
        /// <returns>Wrapper representing the set, null on error</returns>
        public static InstallShieldCabinet? OpenSet(string? pattern)
        {
            // An invalid pattern means no cabinet files
            if (string.IsNullOrEmpty(pattern))
                return null;

            // Create a placeholder wrapper for output
            InstallShieldCabinet? set = null;

            // Loop until there are no parts left
            bool iterate = true;
            InstallShieldCabinet? previous = null;
            for (ushort i = 1; iterate; i++)
            {
                var file = OpenFileForReading(pattern, i, HEADER_SUFFIX);
                if (file != null)
                    iterate = false;
                else
                    file = OpenFileForReading(pattern, i, CABINET_SUFFIX);

                if (file == null)
                    break;

                var current = Create(file);
                if (current == null)
                    break;

                current.VolumeID = i;
                if (previous != null)
                {
                    previous.Next = current;
                    current.Prev = previous;
                }
                else
                {
                    set = current;
                    previous = current;
                }
            }

            // Set the pattern, if possible
            set?.FilenamePattern = pattern;

            return set;
        }

        /// <summary>
        /// Open the numbered cabinet set volume
        /// </summary>
        /// <param name="volumeId">Volume ID, 1-indexed</param>
        /// <returns>Wrapper representing the volume on success, null otherwise</returns>
        public InstallShieldCabinet? OpenVolume(ushort volumeId, out Stream? volumeStream)
        {
            // Normalize the volume ID for odd cases
            if (volumeId == ushort.MinValue || volumeId == ushort.MaxValue)
                volumeId = 1;

            // Try to open the file as a stream
            volumeStream = OpenFileForReading(FilenamePattern, volumeId, CABINET_SUFFIX);
            if (volumeStream == null)
            {
                Console.Error.WriteLine($"Failed to open input cabinet file {volumeId}");
                return null;
            }

            // Try to parse the stream into a cabinet
            var volume = Create(volumeStream);
            if (volume == null)
            {
                Console.Error.WriteLine($"Failed to open input cabinet file {volumeId}");
                return null;
            }

            // Set the volume ID and return
            volume.VolumeID = volumeId;
            return volume;
        }

        /// <summary>
        /// Open a cabinet file for reading
        /// </summary>
        /// <param name="index">Cabinet part index to be opened</param>
        /// <param name="suffix">Cabinet files suffix (e.g. `.cab`)</param>
        /// <returns>A Stream representing the cabinet part, null on error</returns>
        public Stream? OpenFileForReading(int index, string suffix)
            => OpenFileForReading(FilenamePattern, index, suffix);

        /// <summary>
        /// Create the generic filename pattern to look for from the input filename
        /// </summary>
        /// <returns>String representing the filename pattern for a cabinet set, null on error</returns>
        private static string? CreateFilenamePattern(string filename)
        {
            string? pattern = null;
            if (string.IsNullOrEmpty(filename))
                return pattern;

            string? directory = Path.GetDirectoryName(Path.GetFullPath(filename));
            if (directory != null)
                pattern = Path.Combine(directory, Path.GetFileNameWithoutExtension(filename));
            else
                pattern = Path.GetFileNameWithoutExtension(filename);

            return new Regex(@"\d+$").Replace(pattern, string.Empty);
        }

        /// <summary>
        /// Open a cabinet file for reading
        /// </summary>
        /// <param name="pattern">Filename pattern for matching cabinet files</param>
        /// <param name="index">Cabinet part index to be opened</param>
        /// <param name="suffix">Cabinet files suffix (e.g. `.cab`)</param>
        /// <returns>A Stream representing the cabinet part, null on error</returns>
        private static Stream? OpenFileForReading(string? pattern, int index, string suffix)
        {
            // An invalid pattern means no cabinet files
            if (string.IsNullOrEmpty(pattern))
                return null;

            // Attempt lower-case extension
            string filename = $"{pattern}{index}.{suffix}";
            if (File.Exists(filename))
                return File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Attempt upper-case extension
            filename = $"{pattern}{index}.{suffix.ToUpperInvariant()}";
            if (File.Exists(filename))
                return File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return null;
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Open the full set if possible
            var cabinet = this;
            if (Filename != null)
            {
                // Get the name of the first cabinet file or header
                string pattern = CreateFilenamePattern(Filename)!;
                bool cabinetHeaderExists = File.Exists(pattern + "1.hdr");
                bool shouldScanCabinet = cabinetHeaderExists
                    ? Filename.Equals(pattern + "1.hdr", StringComparison.OrdinalIgnoreCase)
                    : Filename.Equals(pattern + "1.cab", StringComparison.OrdinalIgnoreCase);

                // If we have anything but the first file
                if (!shouldScanCabinet)
                    return false;

                // Open the set from the pattern
                cabinet = OpenSet(pattern);
            }

            // If the cabinet set could not be opened
            if (cabinet == null)
                return false;

            try
            {
                for (int i = 0; i < cabinet.FileCount; i++)
                {
                    try
                    {
                        // Check if the file is valid first
                        if (!cabinet.FileIsValid(i))
                            continue;

                        // Ensure directory separators are consistent
                        string filename = cabinet.GetFileName(i) ?? $"BAD_FILENAME{i}";
                        if (Path.DirectorySeparatorChar == '\\')
                            filename = filename.Replace('/', '\\');
                        else if (Path.DirectorySeparatorChar == '/')
                            filename = filename.Replace('\\', '/');

                        // Ensure the full output directory exists
                        filename = Path.Combine(outputDirectory, filename);
                        var directoryName = Path.GetDirectoryName(filename);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        cabinet.FileSave(i, filename, includeDebug);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.Error.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Save the file at the given index to the filename specified
        /// </summary>
        public bool FileSave(int index, string filename, bool includeDebug, bool useOld = false)
        {
            // Get the file descriptor
            if (!TryGetFileDescriptor(index, out var fileDescriptor) || fileDescriptor == null)
                return false;

            // If the file is split
            if (fileDescriptor.LinkFlags == LinkFlags.LINK_PREV)
                return FileSave((int)fileDescriptor.LinkPrevious, filename, includeDebug, useOld);

            // Get the reader at the index
            var reader = Reader.Create(this, index, fileDescriptor);
            if (reader == null)
                return false;

            // Create the output file and hasher
            using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            var md5 = new HashWrapper(HashType.MD5);

            long readBytesLeft = (long)GetReadableBytes(fileDescriptor);
            long writeBytesLeft = (long)GetWritableBytes(fileDescriptor);
            byte[] inputBuffer;
            byte[] outputBuffer = new byte[BUFFER_SIZE];
            long totalWritten = 0;

            // Read while there are bytes remaining
            while (readBytesLeft > 0 && writeBytesLeft > 0)
            {
                long bytesToWrite = BUFFER_SIZE;
                int result;

                // Handle compressed files
#if NET20 || NET35
                if ((fileDescriptor.Flags & FileFlags.FILE_COMPRESSED) != 0)
#else
                if (fileDescriptor.Flags.HasFlag(FileFlags.FILE_COMPRESSED))
#endif
                {
                    // Attempt to read the length value
                    byte[] lengthArr = new byte[sizeof(ushort)];
                    if (!reader.Read(lengthArr, 0, lengthArr.Length))
                    {
                        Console.Error.WriteLine($"Failed to read {lengthArr.Length} bytes of file {index} ({GetFileName(index)}) from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        fs?.Close();
                        return false;
                    }

                    // Attempt to read the specified number of bytes
                    ushort bytesToRead = BitConverter.ToUInt16(lengthArr, 0);
                    inputBuffer = new byte[BUFFER_SIZE + 1];
                    if (!reader.Read(inputBuffer, 0, bytesToRead))
                    {
                        Console.Error.WriteLine($"Failed to read {lengthArr.Length} bytes of file {index} ({GetFileName(index)}) from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        fs?.Close();
                        return false;
                    }

                    // Add a null byte to make inflate happy
                    inputBuffer[bytesToRead] = 0;
                    ulong readBytes = (ulong)(bytesToRead + 1);

                    // Uncompress into a buffer
                    if (useOld)
                        result = UncompressOld(outputBuffer, ref bytesToWrite, inputBuffer, ref readBytes);
                    else
                        result = Uncompress(outputBuffer, ref bytesToWrite, inputBuffer, ref readBytes);

                    // If we didn't get a positive result that's not a data error (false positives)
                    if (result != zlibConst.Z_OK && result != zlibConst.Z_DATA_ERROR)
                    {
                        Console.Error.WriteLine($"Decompression failed with code {result.ToZlibConstName()}. bytes_to_read={bytesToRead}, volume={fileDescriptor.Volume}, read_bytes={readBytes}");
                        reader.Dispose();
                        fs?.Close();
                        return false;
                    }

                    // Set remaining bytes
                    readBytesLeft -= 2;
                    readBytesLeft -= bytesToRead;
                }

                // Handle uncompressed files
                else
                {
                    bytesToWrite = Math.Min(readBytesLeft, BUFFER_SIZE);
                    if (!reader.Read(outputBuffer, 0, (int)bytesToWrite))
                    {
                        Console.Error.WriteLine($"Failed to write {bytesToWrite} bytes from input cabinet file {fileDescriptor.Volume}");
                        reader.Dispose();
                        fs?.Close();
                        return false;
                    }

                    // Set remaining bytes
                    readBytesLeft -= (uint)bytesToWrite;
                }

                // Hash and write the next block
                bytesToWrite = Math.Min(bytesToWrite, writeBytesLeft);
                md5.Process(outputBuffer, 0, (int)bytesToWrite);
                fs?.Write(outputBuffer, 0, (int)bytesToWrite);

                totalWritten += bytesToWrite;
                writeBytesLeft -= bytesToWrite;
            }

            // Validate the number of bytes written
            if ((long)fileDescriptor.ExpandedSize != totalWritten)
                if (includeDebug) Console.WriteLine($"Expanded size of file {index} ({GetFileName(index)}) expected to be {fileDescriptor.ExpandedSize}, but was {totalWritten}");

            // Finalize output values
            md5.Terminate();
            reader?.Dispose();
            fs?.Close();

            // Validate the data written, if required
            if (MajorVersion >= 6)
            {
                string expectedMd5 = BitConverter.ToString(fileDescriptor.MD5);
                expectedMd5 = expectedMd5.ToLowerInvariant().Replace("-", string.Empty);

                string? actualMd5 = md5.CurrentHashString;
                if (actualMd5 == null || actualMd5 != expectedMd5)
                {
                    Console.Error.WriteLine($"MD5 checksum failure for file {index} ({GetFileName(index)})");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Save the file at the given index to the filename specified as raw
        /// </summary>
        public bool FileSaveRaw(int index, string filename)
        {
            // Get the file descriptor
            if (!TryGetFileDescriptor(index, out var fileDescriptor) || fileDescriptor == null)
                return false;

            // If the file is split
            if (fileDescriptor.LinkFlags == LinkFlags.LINK_PREV)
                return FileSaveRaw((int)fileDescriptor.LinkPrevious, filename);

            // Get the reader at the index
            var reader = Reader.Create(this, index, fileDescriptor);
            if (reader == null)
                return false;

            // Create the output file
            using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            ulong bytesLeft = GetReadableBytes(fileDescriptor);
            byte[] outputBuffer = new byte[BUFFER_SIZE];

            // Read while there are bytes remaining
            while (bytesLeft > 0)
            {
                ulong bytesToWrite = Math.Min(bytesLeft, BUFFER_SIZE);
                if (!reader.Read(outputBuffer, 0, (int)bytesToWrite))
                {
                    Console.Error.WriteLine($"Failed to read {bytesToWrite} bytes from input cabinet file {fileDescriptor.Volume}");
                    reader.Dispose();
                    fs?.Close();
                    return false;
                }

                // Set remaining bytes
                bytesLeft -= (uint)bytesToWrite;

                // Write the next block
                fs.Write(outputBuffer, 0, (int)bytesToWrite);
            }

            // Finalize output values
            reader.Dispose();
            fs?.Close();
            return true;
        }

        /// <summary>
        /// Uncompress a source byte array to a destination
        /// </summary>
        private unsafe static int Uncompress(byte[] dest, ref long destLen, byte[] source, ref ulong sourceLen)
        {
            fixed (byte* sourcePtr = source)
            fixed (byte* destPtr = dest)
            {
                var stream = new ZLib.z_stream_s
                {
                    next_in = sourcePtr,
                    avail_in = (uint)sourceLen,
                    next_out = destPtr,
                    avail_out = (uint)destLen,
                };

                // make second parameter negative to disable checksum verification
                int err = ZLib.inflateInit2_(stream, -MAX_WBITS, ZLib.zlibVersion(), source.Length);
                if (err != zlibConst.Z_OK)
                    return err;

                err = ZLib.inflate(stream, 1);
                if (err != zlibConst.Z_OK && err != zlibConst.Z_STREAM_END)
                {
                    ZLib.inflateEnd(stream);
                    return err;
                }

                destLen = stream.total_out;
                sourceLen = stream.total_in;
                return ZLib.inflateEnd(stream);
            }
        }

        /// <summary>
        /// Uncompress a source byte array to a destination (old version)
        /// </summary>
        private unsafe static int UncompressOld(byte[] dest, ref long destLen, byte[] source, ref ulong sourceLen)
        {
            fixed (byte* sourcePtr = source)
            fixed (byte* destPtr = dest)
            {
                var stream = new ZLib.z_stream_s
                {
                    next_in = sourcePtr,
                    avail_in = (uint)sourceLen,
                    next_out = destPtr,
                    avail_out = (uint)destLen,
                };

                destLen = 0;
                sourceLen = 0;

                // make second parameter negative to disable checksum verification
                int err = ZLib.inflateInit2_(stream, -MAX_WBITS, ZLib.zlibVersion(), source.Length);
                if (err != zlibConst.Z_OK)
                    return err;

                while (stream.avail_in > 1)
                {
                    err = ZLib.inflate(stream, 1);
                    if (err != zlibConst.Z_OK)
                    {
                        ZLib.inflateEnd(stream);
                        return err;
                    }
                }

                destLen = stream.total_out;
                sourceLen = stream.total_in;
                return ZLib.inflateEnd(stream);
            }
        }

        #endregion

        #region Obfuscation

        /// <summary>
        /// Deobfuscate a buffer
        /// </summary>
        public static void Deobfuscate(byte[] buffer, long size, ref uint offset)
        {
            offset = Deobfuscate(buffer, size, offset);
        }

        /// <summary>
        /// Deobfuscate a buffer with a seed value
        /// </summary>
        /// <remarks>Seed is 0 at file start</remarks>
        public static uint Deobfuscate(byte[] buffer, long size, uint seed)
        {
            for (int i = 0; size > 0; size--, i++, seed++)
            {
                buffer[i] = (byte)(ROR8(buffer[i] ^ 0xd5, 2) - (seed % 0x47));
            }

            return seed;
        }

        /// <summary>
        /// Obfuscate a buffer
        /// </summary>
        public static void Obfuscate(byte[] buffer, long size, ref uint offset)
        {
            offset = Obfuscate(buffer, size, offset);
        }

        /// <summary>
        /// Obfuscate a buffer with a seed value
        /// </summary>
        /// <remarks>Seed is 0 at file start</remarks>
        public static uint Obfuscate(byte[] buffer, long size, uint seed)
        {
            for (int i = 0; size > 0; size--, i++, seed++)
            {
                buffer[i] = (byte)(ROL8(buffer[i] ^ 0xd5, 2) + (seed % 0x47));
            }

            return seed;
        }

        /// <summary>
        /// Rotate Right 8
        /// </summary>
        private static int ROR8(int x, byte n) => (x >> n) | (x << (8 - n));

        /// <summary>
        /// Rotate Left 8
        /// </summary>
        private static int ROL8(int x, byte n) => (x << n) | (x >> (8 - n));

        #endregion

        #region Helper Classes

        /// <summary>
        /// Helper to read a single file from a cabinet set
        /// </summary>
        private class Reader : IDisposable
        {
            #region Private Instance Variables

            /// <summary>
            /// Cabinet file to read from
            /// </summary>
            private readonly InstallShieldCabinet _cabinet;

            /// <summary>
            /// Currently selected index
            /// </summary>
            private readonly uint _index;

            /// <summary>
            /// File descriptor defining the currently selected index
            /// </summary>
            private readonly FileDescriptor _fileDescriptor;

            /// <summary>
            /// Offset in the data where the file exists
            /// </summary>
            private ulong _dataOffset;

            /// <summary>
            /// Number of bytes left in the current volume
            /// </summary>
            private ulong _volumeBytesLeft;

            /// <summary>
            /// Handle to the current volume stream
            /// </summary>
            private Stream? _volumeFile;

            /// <summary>
            /// Current volume header
            /// </summary>
            private VolumeHeader? _volumeHeader;

            /// <summary>
            /// Current volume ID
            /// </summary>
            private ushort _volumeId;

            /// <summary>
            /// Offset for obfuscation seed
            /// </summary>
            private uint _obfuscationOffset;

            #endregion

            #region Constructors

            private Reader(InstallShieldCabinet cabinet, uint index, FileDescriptor fileDescriptor)
            {
                _cabinet = cabinet;
                _index = index;
                _fileDescriptor = fileDescriptor;
            }

            #endregion

            /// <summary>
            /// Create a new <see cref="Reader"> from an existing cabinet, index, and file descriptor
            /// </summary>
            public static Reader? Create(InstallShieldCabinet cabinet, int index, FileDescriptor fileDescriptor)
            {
                var reader = new Reader(cabinet, (uint)index, fileDescriptor);
                for (; ; )
                {
                    // If the volume is invalid
                    if (!reader.OpenVolume(fileDescriptor.Volume))
                    {
                        Console.Error.WriteLine($"Failed to open volume {fileDescriptor.Volume}");
                        return null;
                    }
                    else if (reader._volumeFile == null || reader._volumeHeader == null)
                    {
                        Console.Error.WriteLine($"Volume {fileDescriptor.Volume} is invalid");
                        return null;
                    }

                    // Start with the correct volume for IS5 cabinets
                    if (reader._cabinet.MajorVersion <= 5 && index > (int)reader._volumeHeader.LastFileIndex)
                    {
                        // Normalize the volume ID for odd cases
                        if (fileDescriptor.Volume == ushort.MinValue || fileDescriptor.Volume == ushort.MaxValue)
                            fileDescriptor.Volume = 1;

                        fileDescriptor.Volume++;
                        continue;
                    }

                    break;
                }

                return reader;
            }

            /// <summary>
            /// Dispose of the current object
            /// </summary>
            public void Dispose()
            {
                _volumeFile?.Close();
            }

            #region Reading

            /// <summary>
            /// Read a certain number of bytes from the current volume
            /// </summary>
            public bool Read(byte[] buffer, int start, long size)
            {
                long bytesLeft = size;
                while (bytesLeft > 0)
                {
                    // Open the next volume, if necessary
                    if (_volumeBytesLeft == 0)
                    {
                        if (!OpenNextVolume(out _))
                            return false;
                    }

                    // Get the number of bytes to read from this volume
                    int bytesToRead = (int)Math.Min(bytesLeft, (long)_volumeBytesLeft);
                    if (bytesToRead == 0)
                        break;

                    // Read as much as possible from this volume
                    if (bytesToRead != _volumeFile!.Read(buffer, start, bytesToRead))
                        return false;

                    // Set the number of bytes left
                    bytesLeft -= bytesToRead;
                    _volumeBytesLeft -= (uint)bytesToRead;
                }

#if NET20 || NET35
                if ((_fileDescriptor.Flags & FileFlags.FILE_OBFUSCATED) != 0)
#else
                if (_fileDescriptor.Flags.HasFlag(FileFlags.FILE_OBFUSCATED))
#endif
                    Deobfuscate(buffer, size, ref _obfuscationOffset);

                return true;
            }

            /// <summary>
            /// Open the next volume based on the current index
            /// </summary>
            private bool OpenNextVolume(out ushort nextVolume)
            {
                nextVolume = (ushort)(_volumeId + 1);
                return OpenVolume(nextVolume);
            }

            /// <summary>
            /// Open the volume at the inputted index
            /// </summary>
            private bool OpenVolume(ushort volume)
            {
                // Read the volume from the cabinet set
                var next = _cabinet.OpenVolume(volume, out var volumeStream);
                if (next?.VolumeHeader == null || volumeStream == null)
                {
                    Console.Error.WriteLine($"Failed to open input cabinet file {volume}");
                    return false;
                }

                // Assign the next items
                _volumeFile?.Close();
                _volumeFile = volumeStream;
                _volumeHeader = next.VolumeHeader;

                // Enable support for split archives for IS5
                if (_cabinet.MajorVersion == 5)
                {
                    if (_index < (_cabinet.FileCount - 1)
                        && _index == _volumeHeader.LastFileIndex
                        && _volumeHeader.LastFileSizeCompressed != _fileDescriptor.CompressedSize)
                    {
                        _fileDescriptor.Flags |= FileFlags.FILE_SPLIT;
                    }
                    else if (_index > 0
                        && _index == _volumeHeader.FirstFileIndex
                        && _volumeHeader.FirstFileSizeCompressed != _fileDescriptor.CompressedSize)
                    {
                        _fileDescriptor.Flags |= FileFlags.FILE_SPLIT;
                    }
                }

                ulong volumeBytesLeftCompressed, volumeBytesLeftExpanded;
#if NET20 || NET35
            if ((_fileDescriptor.Flags & FileFlags.FILE_SPLIT) != 0)
#else
                if (_fileDescriptor.Flags.HasFlag(FileFlags.FILE_SPLIT))
#endif
                {
                    if (_index == _volumeHeader.LastFileIndex && _volumeHeader.LastFileOffset != 0x7FFFFFFF)
                    {
                        // can be first file too
                        _dataOffset = _volumeHeader.LastFileOffset;
                        volumeBytesLeftExpanded = _volumeHeader.LastFileSizeExpanded;
                        volumeBytesLeftCompressed = _volumeHeader.LastFileSizeCompressed;
                    }
                    else if (_index == _volumeHeader.FirstFileIndex)
                    {
                        _dataOffset = _volumeHeader.FirstFileOffset;
                        volumeBytesLeftExpanded = _volumeHeader.FirstFileSizeExpanded;
                        volumeBytesLeftCompressed = _volumeHeader.FirstFileSizeCompressed;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    _dataOffset = _fileDescriptor.DataOffset;
                    volumeBytesLeftExpanded = _fileDescriptor.ExpandedSize;
                    volumeBytesLeftCompressed = _fileDescriptor.CompressedSize;
                }

#if NET20 || NET35
                if ((_fileDescriptor.Flags & FileFlags.FILE_COMPRESSED) != 0)
#else
                if (_fileDescriptor.Flags.HasFlag(FileFlags.FILE_COMPRESSED))
#endif
                    _volumeBytesLeft = volumeBytesLeftCompressed;
                else
                    _volumeBytesLeft = volumeBytesLeftExpanded;

                _volumeFile.SeekIfPossible((long)_dataOffset, SeekOrigin.Begin);
                _volumeId = volume;

                return true;
            }

            #endregion
        }

        #endregion
    }
}
