using System;
using System.IO;
using System.Text.RegularExpressions;
using SabreTools.IO.Compression.zlib;
using SabreTools.Models.InstallShieldCabinet;
using static SabreTools.Models.InstallShieldCabinet.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase<Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Cabinet";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Cabinet.CommonHeader"/>
        public CommonHeader? CommonHeader => Model.CommonHeader;

        /// <inheritdoc cref="Cabinet.Components"/>
        public Component[]? Components => Model.Components;

        /// <summary>
        /// Number of components in the cabinet set
        /// </summary>
        public int ComponentCount => Components?.Length ?? 0;

        /// <summary>
        /// Number of directories in the cabinet set
        /// </summary>
        public ushort DirectoryCount => Model.Descriptor?.DirectoryCount ?? 0;

        /// <inheritdoc cref="Cabinet.DirectoryNames"/>
        public string[]? DirectoryNames => Model.DirectoryNames;

        /// <summary>
        /// Number of files in the cabinet set
        /// </summary>
        public uint FileCount => Model.Descriptor?.FileCount ?? 0;

        /// <inheritdoc cref="Cabinet.FileDescriptors"/>
        public FileDescriptor[]? FileDescriptors => Model.FileDescriptors;

        /// <inheritdoc cref="Cabinet.FileGroups"/>
        public FileGroup[]? FileGroups => Model.FileGroups;

        /// <summary>
        /// Number of file groups in the cabinet set
        /// </summary>
        public int FileGroupCount => Model.FileGroups?.Length ?? 0;

        /// <summary>
        /// Indicates if Unicode strings are used
        /// </summary>
        public bool IsUnicode => MajorVersion >= 17;

        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion => Model.GetMajorVersion();

        /// <inheritdoc cref="Cabinet.VolumeHeader"/>
        public VolumeHeader? VolumeHeader => Model.VolumeHeader;

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

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public InstallShieldCabinet(Cabinet? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a InstallShield Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static InstallShieldCabinet? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var cabinet = Deserializers.InstallShieldCabinet.DeserializeStream(data);
                if (cabinet == null)
                    return null;

                return new InstallShieldCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Cabinet Set

        /// <summary>
        /// Create the generic filename pattern to look for from the input filename
        /// </summary>
        /// <returns>String representing the filename pattern for a cabinet set, null on error</returns>
        public static string? CreateFilenamePattern(string filename)
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
            for (int i = 1; iterate; i++)
            {
                var file = OpenFileForReading(pattern, i, HEADER_SUFFIX);
                if (file != null)
                    iterate = false;
                else
                    file = OpenFileForReading(pattern, i, CABINET_SUFFIX);

                if (file == null)
                    break;

                var header = Create(file);
                if (header == null)
                    break;

                if (previous != null)
                    previous.Next = header;
                else
                    previous = set = header;
            }

            return set;
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

        #region Component

        /// <summary>
        /// Get the component name at a given index, if possible
        /// </summary>
        public string? GetComponentName(int index)
        {
            if (Components == null)
                return null;

            if (index < 0 || index >= ComponentCount)
                return null;

            var component = Components[index];
            if (component?.Identifier == null)
                return null;

            return component.Identifier.Replace('\\', '/');
        }

        #endregion

        #region Directory

        /// <summary>
        /// Get the directory name at a given index, if possible
        /// </summary>
        public string? GetDirectoryName(int index)
        {
            if (DirectoryNames == null)
                return null;

            if (index < 0 || index >= DirectoryNames.Length)
                return null;

            return DirectoryNames[index];
        }

        /// <summary>
        /// Get the directory index for the given file index
        /// </summary>
        /// <returns>Directory index if found, UInt32.MaxValue on error</returns>
        public uint GetDirectoryIndexFromFile(int index)
        {
            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor != null)
                return descriptor.DirectoryIndex;
            else
                return uint.MaxValue;
        }

        #endregion

        #region File

        /// <summary>
        /// Returns if the file at a given index is marked as valid
        /// </summary>
        public bool FileIsValid(int index)
        {
            if (index < 0 || index > FileCount)
                return false;

            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor == null)
                return false;

#if NET20 || NET35
            if ((descriptor.Flags & FileFlags.FILE_INVALID) != 0)
#else
            if (descriptor.Flags.HasFlag(FileFlags.FILE_INVALID))
#endif
                return false;

            if (descriptor.NameOffset == default)
                return false;

            if (descriptor.DataOffset == default)
                return false;

            return true;
        }

        /// <summary>
        /// Get the reported expanded file size for a given index
        /// </summary>
        public ulong GetExpandedFileSize(int index)
        {
            FileDescriptor? descriptor = GetFileDescriptor(index);
            if (descriptor != null)
                return descriptor.ExpandedSize;
            else
                return 0;
        }

        /// <summary>
        /// Get the file descriptor at a given index, if possible
        /// </summary>
        public FileDescriptor? GetFileDescriptor(int index)
        {
            if (FileDescriptors == null)
                return null;

            if (index < 0 || index >= FileDescriptors.Length)
                return null;

            return FileDescriptors[index];
        }

        /// <summary>
        /// Get the file descriptor at a given index, if possible
        /// </summary>
        /// <remarks>Verifies the file descriptor flags before returning</remarks>
        public bool TryGetFileDescriptor(int index, out FileDescriptor? fileDescriptor)
        {
            fileDescriptor = GetFileDescriptor(index);
            if (fileDescriptor == null)
            {
                Console.Error.WriteLine($"Failed to get file descriptor for file {index}");
                return false;
            }

#if NET20 || NET35
            if ((fileDescriptor.Flags & FileFlags.FILE_INVALID) != 0 || fileDescriptor.DataOffset == 0)
#else
            if (fileDescriptor.Flags.HasFlag(FileFlags.FILE_INVALID) || fileDescriptor.DataOffset == 0)
#endif
            {
                Console.Error.WriteLine($"File at {index} is marked as invalid");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the file name at a given index, if possible
        /// </summary>
        public string? GetFileName(int index)
        {
            var descriptor = GetFileDescriptor(index);
#if NET20 || NET35
            if (descriptor == null || (descriptor.Flags & FileFlags.FILE_INVALID) != 0)
#else
            if (descriptor == null || descriptor.Flags.HasFlag(FileFlags.FILE_INVALID))
#endif
                return null;

            return descriptor.Name;
        }

        /// <summary>
        /// Get the packed size of a file, if possible
        /// </summary>
        public static ulong GetReadableBytes(FileDescriptor? descriptor)
        {
            if (descriptor == null)
                return 0;

#if NET20 || NET35
            if ((descriptor.Flags & FileFlags.FILE_COMPRESSED) != 0)
#else
            if (descriptor.Flags.HasFlag(FileFlags.FILE_COMPRESSED))
#endif
                return descriptor.CompressedSize;
            else
                return descriptor.ExpandedSize;
        }

        #endregion

        #region File Group

        /// <summary>
        /// Get the file group at a given index, if possible
        /// </summary>
        public FileGroup? GetFileGroup(int index)
        {
            if (FileGroups == null)
                return null;

            if (index < 0 || index >= FileGroups.Length)
                return null;

            return FileGroups[index];
        }

        /// <summary>
        /// Get the file group at a given name, if possible
        /// </summary>
        public FileGroup? GetFileGroup(string name)
        {
            if (FileGroups == null)
                return null;

            return Array.Find(FileGroups, fg => fg != null && string.Equals(fg.Name, name));
        }

        /// <summary>
        /// Get the file group for the given file index, if possible
        /// </summary>
        public FileGroup? GetFileGroupFromFile(int index)
        {
            if (FileGroups == null)
                return null;

            if (index < 0 || index >= FileCount)
                return null;

            for (int i = 0; i < FileGroupCount; i++)
            {
                var fileGroup = GetFileGroup(i);
                if (fileGroup == null)
                    continue;

                if (fileGroup.FirstFile > index || fileGroup.LastFile < index)
                    continue;

                return fileGroup;
            }

            return null;
        }

        /// <summary>
        /// Get the file group name at a given index, if possible
        /// </summary>
        public string? GetFileGroupName(int index)
            => GetFileGroup(index)?.Name;

        /// <summary>
        /// Get the file group name at a given file index, if possible
        /// </summary>
        public string? GetFileGroupNameFromFile(int index)
            => GetFileGroupFromFile(index)?.Name;

        #endregion

        #region Extraction

        /// <summary>
        /// Uncompress a source byte array to a destination
        /// </summary>
        public unsafe static int Uncompress(byte[] dest, ref ulong destLen, byte[] source, ref ulong sourceLen)
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
                if (err != zlibConst.Z_STREAM_END)
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
        public unsafe static int UncompressOld(byte[] dest, ref ulong destLen, byte[] source, ref ulong sourceLen)
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
        private void Deobfuscate(byte[] buffer, long size, ref uint offset)
        {
            offset = Deobfuscate(buffer, size, offset);
        }

        /// <summary>
        /// Deobfuscate a buffer with a seed value
        /// </summary>
        /// <remarks>Seed is 0 at file start</remarks>
        private static uint Deobfuscate(byte[] buffer, long size, uint seed)
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
        private void Obfuscate(byte[] buffer, long size, ref uint offset)
        {
            offset = Obfuscate(buffer, size, offset);
        }

        /// <summary>
        /// Obfuscate a buffer with a seed value
        /// </summary>
        /// <remarks>Seed is 0 at file start</remarks>
        private static uint Obfuscate(byte[] buffer, long size, uint seed)
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
    }
}
