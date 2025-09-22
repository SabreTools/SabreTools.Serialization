using System;
using System.IO;
using SabreTools.IO.Compression.SZDD;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZSZDD : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
            => Extract(string.Empty, outputDirectory, includeDebug);

        /// <inheritdoc cref="Extract(string, bool)"/>
        /// <param name="filename">Original name of the file to convert to the output name</param>
        public bool Extract(string filename, string outputDirectory, bool includeDebug)
        {
            // Ensure the filename
            if (filename.Length == 0 && Filename != null)
                filename = Filename;

            // Get the length of the compressed data
            long compressedSize = Length - 14;
            if (compressedSize < 14)
                return false;

            // Read in the data as an array
            byte[]? contents = ReadRangeFromSource(14, (int)compressedSize);
            if (contents.Length == 0)
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
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
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
    }
}
