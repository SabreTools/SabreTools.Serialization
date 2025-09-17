using System;
using System.IO;
using static SabreTools.Models.VPK.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class VPK : WrapperBase<Models.VPK.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Valve Package File (VPK)";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
        public string[]? ArchiveFilenames
        {
            get
            {
                // Use the cached value if we have it
                if (_archiveFilenames != null)
                    return _archiveFilenames;

                // If we don't have a source filename
                if (string.IsNullOrEmpty(Filename))
                    return null;

                // If the filename is not the right format
                string extension = Path.GetExtension(Filename).TrimStart('.');
                string? directoryName = Path.GetDirectoryName(Filename);
                string fileName = directoryName == null
                    ? Path.GetFileNameWithoutExtension(Filename)
                    : Path.Combine(directoryName, Path.GetFileNameWithoutExtension(Filename));

                if (fileName.Length < 3)
                    return null;
                else if (fileName.Substring(fileName.Length - 3) != "dir")
                    return null;

                // Get the archive count
                ushort archiveCount = 0;
                foreach (var di in DirectoryItems ?? [])
                {
                    if (di.DirectoryEntry == null)
                        continue;
                    if (di.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
                        continue;

                    archiveCount = Math.Max(archiveCount, di.DirectoryEntry.ArchiveIndex);
                }

                // Build the list of archive filenames to populate
                _archiveFilenames = new string[archiveCount];

                // Loop through and create the archive filenames
                for (int i = 0; i < archiveCount; i++)
                {
                    // We need 5 digits to print a short, but we already have 3 for dir.
                    string archiveFileName = $"{fileName.Substring(0, fileName.Length - 3)}{i.ToString().PadLeft(3, '0')}.{extension}";
                    _archiveFilenames[i] = archiveFileName;
                }

                // Return the array
                return _archiveFilenames;
            }
        }

        /// <inheritdoc cref="Models.VPK.File.DirectoryItems"/>
        public Models.VPK.DirectoryItem[]? DirectoryItems => Model.DirectoryItems;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
        private string[]? _archiveFilenames = null;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public VPK(Models.VPK.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a VPK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VPK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
        public static VPK? Create(byte[]? data, int offset)
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
        /// Create a VPK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VPK</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
        public static VPK? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.VPK.DeserializeStream(data);
                if (model == null)
                    return null;

                return new VPK(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
