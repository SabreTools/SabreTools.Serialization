using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Models.BFPK;

namespace SabreTools.Serialization.Wrappers
{
    public class BFPK : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BFPK Archive";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Files"/>
        public FileEntry[] Files => Model.Files ?? [];

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public BFPK(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public BFPK(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BFPK archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK? Create(byte[]? data, int offset)
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
        /// Create a BFPK archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var archive = Deserializers.BFPK.DeserializeStream(data);
                if (archive == null)
                    return null;

                return new BFPK(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the BFPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file information
            var file = Files[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > GetEndOfFile())
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.Name ?? $"file{index}");
                using FileStream fs = File.OpenWrite(filePath);

                // Read the data block
                var data = ReadFromDataSource(offset, compressedSize);
                if (data == null)
                    return false;

                // If we have uncompressed data
                if (compressedSize == file.UncompressedSize)
                {
                    fs.Write(data, 0, compressedSize);
                }
                else
                {
                    MemoryStream ms = new MemoryStream(data);
                    ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                    zs.CopyTo(fs);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
