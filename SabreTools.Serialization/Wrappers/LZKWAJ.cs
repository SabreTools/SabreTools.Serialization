using System.IO;
using SabreTools.Compression.SZDD;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public class LZKWAJ : WrapperBase<KWAJFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, KWAJ variant";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an LZ (KWAJ variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (KWAJ variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (KWAJ variant) wrapper on success, null on failure</returns>
        public static LZKWAJ? Create(byte[]? data, int offset)
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
        /// Create a LZ (KWAJ variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (KWAJ variant)</param>
        /// <returns>An LZ (KWAJ variant) wrapper on success, null on failure</returns>
        public static LZKWAJ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.LZKWAJ.DeserializeStream(data);
                if (file == null)
                    return null;

                return new LZKWAJ(file, data);
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
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the contents extracted, false otherwise</returns>
        public bool Extract(string outputDirectory)
        {
            // Get the length of the compressed data
            long compressedSize = Length - Model.Header!.DataOffset;
            if (compressedSize < Model.Header.DataOffset)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadFromDataSource(Model.Header.DataOffset, (int)compressedSize);
            if (contents == null)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateKWAJ(contents, Model.Header!.CompressionType);
            if (decompressor == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            string filename = "tempfile";
            if (Model.HeaderExtensions?.FileName != null)
                filename = Model.HeaderExtensions.FileName;
            if (Model.HeaderExtensions?.FileExtension != null)
                filename += $".{Model.HeaderExtensions.FileExtension}";

            filename = Path.Combine(outputDirectory, filename);

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