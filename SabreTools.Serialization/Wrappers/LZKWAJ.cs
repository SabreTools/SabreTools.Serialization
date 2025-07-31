using System;
using System.IO;
using SabreTools.IO.Compression.SZDD;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public class LZKWAJ : WrapperBase<KWAJFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, KWAJ variant";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="KWAJHeader.CompressionType"/>
        public KWAJCompressionType CompressionType => Model.Header?.CompressionType ?? KWAJCompressionType.NoCompression;

        /// <inheritdoc cref="KWAJHeader.DataOffset"/>
        public ushort DataOffset => Model.Header?.DataOffset ?? 0;

        /// <inheritdoc cref="KWAJHeaderExtensions.FileName"/>
        public string? FileName => Model.HeaderExtensions?.FileName;

        /// <inheritdoc cref="KWAJHeaderExtensions.FileExtension"/>
        public string? FileExtension => Model.HeaderExtensions?.FileExtension;

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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the contents extracted, false otherwise</returns>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the length of the compressed data
            long compressedSize = Length - DataOffset;
            if (compressedSize < DataOffset)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadFromDataSource(DataOffset, (int)compressedSize);
            if (contents == null)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateKWAJ(contents, CompressionType);
            if (decompressor == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            string filename = FileName ?? "tempfile";
            if (FileExtension != null)
                filename += $".{FileExtension}";

            // Ensure directory separators are consistent
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
                decompressor.CopyTo(fs);
                fs.Flush();
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
