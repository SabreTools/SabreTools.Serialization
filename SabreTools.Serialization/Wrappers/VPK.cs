using System;
using System.IO;
using SabreTools.IO.Extensions;
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
                ushort archiveCount = 0;
                foreach (var di in Model.DirectoryItems ?? [])
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
                var file = Deserializers.VPK.DeserializeStream(data);
                if (file == null)
                    return null;

                return new VPK(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory items
            if (Model.DirectoryItems == null || Model.DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Model.DirectoryItems.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory items
            if (Model.DirectoryItems == null || Model.DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= Model.DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = Model.DirectoryItems[index];
            if (directoryItem.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
            byte[] data = [];
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (ArchiveFilenames == null || ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrEmpty(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
                var archiveStream = default(Stream);
                try
                {
                    // Open the archive
                    archiveStream = File.Open(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (data != null && directoryItem.PreloadData != null)
                    data = [.. directoryItem.PreloadData, .. data];
            }

            // If there is nothing to write out
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrEmpty(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
