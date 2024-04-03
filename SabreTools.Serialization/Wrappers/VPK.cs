using System.IO;
using System.Linq;
using static SabreTools.Models.VPK.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public class VPK : WrapperBase<Models.VPK.File>
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
                if (!(_streamData is FileStream fs) || string.IsNullOrEmpty(fs.Name))
                    return null;

                // If the filename is not the right format
                string extension = Path.GetExtension(fs.Name).TrimStart('.');
                string? directoryName = Path.GetDirectoryName(fs.Name);
                string fileName = directoryName == null
                    ? Path.GetFileNameWithoutExtension(fs.Name)
                    : Path.Combine(directoryName, Path.GetFileNameWithoutExtension(fs.Name));

                if (fileName.Length < 3)
                    return null;
                else if (fileName.Substring(fileName.Length - 3) != "dir")
                    return null;

                // Get the archive count
                int archiveCount = this.Model.DirectoryItems == null
                    ? 0
                    : this.Model.DirectoryItems
                        .Select(di => di?.DirectoryEntry)
                        .Select(de => de?.ArchiveIndex ?? 0)
                        .Where(ai => ai != HL_VPK_NO_ARCHIVE)
                        .Max();

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

        #endregion

        #region Instance Variables

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
        private string[]? _archiveFilenames = null;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public VPK(Models.VPK.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public VPK(Models.VPK.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a VPK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VPK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
        public static VPK? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
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
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new Streams.VPK().DeserializeImpl(data);
            if (file == null)
                return null;

            try
            {
                return new VPK(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}