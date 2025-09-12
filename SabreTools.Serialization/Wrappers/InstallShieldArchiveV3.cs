using System.Collections.Generic;
using System.IO;
using SabreTools.Models.InstallShieldArchiveV3;

namespace SabreTools.Serialization.Wrappers
{
    /// <remarks>
    /// Reference (de)compressor: https://www.sac.sk/download/pack/icomp95.zip
    /// </remarks>
    /// <see href="https://github.com/wfr/unshieldv3"/>
    public partial class InstallShieldArchiveV3 : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Archive V3";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.DirCount"/>
        public ushort DirCount => Model.Header?.DirCount ?? 0;

        /// <inheritdoc cref="Header.FileCount"/>
        public ushort FileCount => Model.Header?.FileCount ?? 0;

        /// <inheritdoc cref="Archive.Directories"/>
        public Models.InstallShieldArchiveV3.Directory[] Directories => Model.Directories ?? [];

        /// <inheritdoc cref="Archive.Files"/>
        public Models.InstallShieldArchiveV3.File[] Files => Model.Files ?? [];

        /// <summary>
        /// Map of all files to their parent directories by index
        /// </summary>
        public Dictionary<int, int> FileDirMap
        {
            get
            {
                // Return the prebuilt map
                if (_fileDirMap != null)
                    return _fileDirMap;

                // Build the file map
                _fileDirMap = [];

                int fileId = 0;
                for (int i = 0; i < Directories.Length; i++)
                {
                    var dir = Directories[i];
                    for (int j = 0; j < dir.FileCount; j++)
                    {
                        _fileDirMap[fileId++] = i;
                    }
                }

                return _fileDirMap;
            }
        }
        private Dictionary<int, int>? _fileDirMap = null;

        /// <summary>
        /// Map of all files found in the archive
        /// </summary>
        public Dictionary<string, Models.InstallShieldArchiveV3.File> FileNameMap
        {
            get
            {
                // Return the prebuilt map
                if (_fileNameMap != null)
                    return _fileNameMap;

                // Build the file map
                _fileNameMap = [];
                for (int fileIndex = 0; fileIndex < Files.Length; fileIndex++)
                {
                    // Get the current file
                    var file = Files[fileIndex];

                    // Get the parent directory
                    int dirIndex = FileDirMap[fileIndex];
                    if (dirIndex < 0 || dirIndex >= DirCount)
                        continue;

                    // Create the filename
                    string filename = Path.Combine(
                        Directories[dirIndex]?.Name ?? $"dir_{dirIndex}",
                        file.Name ?? $"file_{fileIndex}"
                    );

                    // Add to the map
                    _fileNameMap[filename] = file;
                }

                return _fileNameMap;
            }
        }
        private Dictionary<string, Models.InstallShieldArchiveV3.File>? _fileNameMap = null;

        /// <summary>
        /// Data offset for all archives
        /// </summary>
        private const uint DataStart = 255;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldArchiveV3(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public InstallShieldArchiveV3(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Archive V3 from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A archive wrapper on success, null on failure</returns>
        public static InstallShieldArchiveV3? Create(byte[]? data, int offset)
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
        /// Create a InstallShield Archive V3 from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A archive wrapper on success, null on failure</returns>
        public static InstallShieldArchiveV3? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.InstallShieldArchiveV3.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new InstallShieldArchiveV3(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
