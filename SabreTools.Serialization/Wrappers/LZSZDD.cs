using System;
using System.IO;
using SabreTools.IO.Compression.SZDD;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public class LZSZDD : WrapperBase<SZDDFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, SZDD variant";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="SZDDHeader.LastChar"/>
        public char LastChar => Model.Header?.LastChar ?? '\0';

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZSZDD(SZDDFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public LZSZDD(SZDDFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an LZ (SZDD variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (SZDD variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (SZDD variant) wrapper on success, null on failure</returns>
        public static LZSZDD? Create(byte[]? data, int offset)
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
        /// Create a LZ (SZDD variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (SZDD variant)</param>
        /// <returns>An LZ (SZDD variant) wrapper on success, null on failure</returns>
        public static LZSZDD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.LZSZDD.DeserializeStream(data);
                if (file == null)
                    return null;

                return new LZSZDD(file, data);
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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the contents extracted, false otherwise</returns>
        public bool Extract(string filename, string outputDirectory, bool includeDebug)
        {
            // Get the length of the compressed data
            long compressedSize = Length - 14;
            if (compressedSize < 14)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadFromDataSource(14, (int)compressedSize);
            if (contents == null)
                return false;

            // Get the decompressor
            var decompressor = Decompressor.CreateSZDD(contents);
            if (decompressor == null)
                return false;

            // Create the output file
            filename = GetExpandedName(filename).TrimEnd('\0');

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

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

        /// <summary>
        /// Get the full name of the input file
        /// </summary>
        private string GetExpandedName(string input)
        {
            // If the extension is missing
            string extension = Path.GetExtension(input).TrimStart('.');
            if (string.IsNullOrEmpty(extension))
                return Path.GetFileNameWithoutExtension(input);

            // If the extension is a single character
            if (extension.Length == 1)
            {
                if (extension == "_" || extension == "$")
                    return $"{Path.GetFileNameWithoutExtension(input)}.{char.ToLower(LastChar)}";

                return Path.GetFileNameWithoutExtension(input);
            }

            // If the extension isn't formatted
            if (!extension.EndsWith("_"))
                return Path.GetFileNameWithoutExtension(input);

            // Handle replacing characters
            char c = (char.IsUpper(input[0]) ? char.ToLower(LastChar) : char.ToUpper(LastChar));
            string text2 = extension.Substring(0, extension.Length - 1) + c;
            return Path.GetFileNameWithoutExtension(input) + "." + text2;
        }

        #endregion
    }
}
