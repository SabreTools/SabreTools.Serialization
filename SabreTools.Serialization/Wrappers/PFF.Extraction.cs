using System;
using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PFF : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // If we have no segments
            if (Segments == null || Segments.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Segments.Length; i++)
            {
                allExtracted &= ExtractSegment(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a segment from the PFF to an output directory by index
        /// </summary>
        /// <param name="index">Segment index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the segment extracted, false otherwise</returns>
        public bool ExtractSegment(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (FileCount == 0)
                return false;

            // If we have no segments
            if (Segments == null || Segments.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= Segments.Length)
                return false;

            // Get the read index and length
            var segment = Segments[index];
            int offset = (int)segment.FileLocation;
            int size = (int)segment.FileSize;

            try
            {
                // Ensure directory separators are consistent
                string filename = segment.FileName ?? $"file{index}";
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Create the output file
                using FileStream fs = File.OpenWrite(filename);

                // Read the data block
                var data = ReadRangeFromSource(offset, size);
                if (data == null)
                    return false;

                // Write the data -- TODO: Compressed data?
                fs.Write(data, 0, size);
                fs.Flush();

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
