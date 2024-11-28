using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class PAK : WrapperBase<Models.PAK.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Package File (PAK)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PAK(Models.PAK.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PAK(Models.PAK.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PAK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the PAK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK? Create(byte[]? data, int offset)
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
        /// Create a PAK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the PAK</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Deserializers.PAK.DeserializeStream(data);
            if (file == null)
                return null;

            try
            {
                return new PAK(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the PAK to an output directory
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
        /// Extract a file from the PAK to an output directory by index
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
            if (directoryItem == null)
                return false;

            // Read the item data
            var data = ReadFromDataSource((int)directoryItem.ItemOffset, (int)directoryItem.ItemLength);
            if (data == null)
                return false;

            // Create the filename
            var filename = directoryItem.ItemName;

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