using System.IO;
using SabreTools.IO.Compression.SZDD;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public class LZQBasic : WrapperBase<QBasicFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, QBasic variant";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZQBasic(QBasicFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an LZ (QBasic variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (QBasic variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (QBasic variant) wrapper on success, null on failure</returns>
        public static LZQBasic? Create(byte[]? data, int offset)
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
        /// Create a LZ (QBasic variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (QBasic variant)</param>
        /// <returns>An LZ (QBasic variant) wrapper on success, null on failure</returns>
        public static LZQBasic? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.LZQBasic.DeserializeStream(data);
                if (file == null)
                    return null;

                return new LZQBasic(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract the contents to an output directory
        /// </summary>
        /// <param name="filename">Original filename to use as a base</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the contents extracted, false otherwise</returns>
        public bool Extract(string outputDirectory)
        {
            // Get the length of the compressed data
            long compressedSize = Length - 12;
            if (compressedSize < 12)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadFromDataSource(12, (int)compressedSize);
            if (contents == null)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateQBasic(contents);
            if (decompressor == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            string filename = Path.Combine(outputDirectory, "tempfile.bin");

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                decompressor.CopyTo(fs);
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