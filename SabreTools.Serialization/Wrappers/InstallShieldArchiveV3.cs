using System.Collections.Generic;
using System.IO;
using SabreTools.Models.InstallShieldArchiveV3;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldArchiveV3 : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Archive V3";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Directories"/>
        public Models.InstallShieldArchiveV3.Directory[] Directories => Model.Directories ?? [];

        /// <inheritdoc cref="Archive.Files"/>
        public Models.InstallShieldArchiveV3.File[] Files => Model.Files ?? [];

        /// <summary>
        /// Map of all directories found in the archive
        /// </summary>
        public Dictionary<string, Models.InstallShieldArchiveV3.File> FileMap
        {
            get
            {
                // Build the file map if not already
                if (_fileMap == null)
                {
                    _fileMap = [];
                    foreach (var file in Model.Files ?? [])
                    {
                        if (file?.Name == null)
                            continue;

                        _fileMap[file.Name] = file;
                    }
                }

                return _fileMap;
            }
        }
        private Dictionary<string, Models.InstallShieldArchiveV3.File>? _fileMap = null;

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
                var archive = Deserializers.InstallShieldArchiveV3.DeserializeStream(data);
                if (archive == null)
                    return null;

                return new InstallShieldArchiveV3(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
