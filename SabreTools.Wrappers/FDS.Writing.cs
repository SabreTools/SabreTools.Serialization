using System;
using System.IO;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class FDS : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".fds")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Header is null || Model.Data is null || Model.Data.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Create and use the writer
            var writer = new Serialization.Writers.FDS { Debug = includeDebug };
            return writer.SerializeFile(Model, outputPath);
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteHeader(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write header data");

            if (Header is null)
            {
                if (includeDebug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                // Bytes 0-3
                stream.Write(Header.IdentificationString);
                stream.Flush();

                // Byte 4
                stream.Write(Header.DiskSides);
                stream.Flush();

                // Byte 5-15
                stream.Write(Header.Padding);
                stream.Flush();

                // Header extracted
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write rom data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteRom(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write ROM data");

            // Try to write the data
            try
            {
                stream.Write(Model.Data, 0, Model.Data.Length);
                stream.Flush();
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
