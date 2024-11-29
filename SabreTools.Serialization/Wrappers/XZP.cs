using System;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class XZP : WrapperBase<Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XZP(Models.XZP.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public XZP(Models.XZP.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP? Create(byte[]? data, int offset)
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
        /// Create a XZP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XZP</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.XZP.DeserializeStream(data);
                if (file == null)
                    return null;

                return new XZP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the XZP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory entries
            if (Model.DirectoryEntries == null || Model.DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Model.DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the XZP to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory entries
            if (Model.DirectoryEntries == null || Model.DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (Model.DirectoryItems == null || Model.DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= Model.DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = Model.DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = Array.Find(Model.DirectoryItems, di => di?.FileNameCRC == directoryEntry.FileNameCRC);
            if (directoryItem == null)
                return false;

            // Load the item data
            var data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
            if (data == null)
                return false;

            // Create the filename
            var filename = directoryItem.Name;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
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