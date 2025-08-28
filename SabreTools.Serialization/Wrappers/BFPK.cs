using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.Models.BFPK;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public class BFPK : WrapperBase<Archive>, IExtractable
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
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.BFPK.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new BFPK(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the BFPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
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
            if (compressedSize > Length)
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = file.Name ?? $"file{index}";
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
                using FileStream fs = File.OpenWrite(filename);

                // Read the data block
                var data = ReadFromDataSource(offset, compressedSize);
                if (data == null)
                    return false;

                // If we have uncompressed data
                if (compressedSize == file.UncompressedSize)
                {
                    fs.Write(data, 0, compressedSize);
                    fs.Flush();
                }
                else
                {
                    using MemoryStream ms = new MemoryStream(data);
                    using ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                    zs.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }

        #endregion
    }
}
