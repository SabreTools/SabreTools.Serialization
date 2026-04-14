using System;
using System.IO;
using SabreTools.Data.Models.AtariLynx;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class AtariLynxCart : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".lnx")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Header is null || Model.Data is null || Model.Data.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Open the output file for writing
            using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

            // Header data
            if (!WriteHeader(fs, includeDebug))
                return false;

            // ROM data
            if (!WriteRom(fs, includeDebug))
                return false;

            return true;
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteHeader(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to extract header data");

            if (Header is null)
            {
                if (includeDebug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                // Bytes 0-3
                stream.Write(Header.Magic);
                stream.Flush();

                // Bytes 4-5
                stream.Write(Header.Bank0PageSize);
                stream.Flush();

                // Bytes 6-7
                stream.Write(Header.Bank0PageSize);
                stream.Flush();

                // Bytes 8-9
                stream.Write(Header.Version);
                stream.Flush();

                // Bytes 10-41
                stream.Write(Header.CartName);
                stream.Flush();

                // Bytes 42-57
                stream.Write(Header.Manufacturer);
                stream.Flush();

                // Byte 58
                stream.Write((byte)Header.Rotation);
                stream.Flush();

                // Bytes 59-63
                stream.Write(Header.Spare);
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
            if (includeDebug) Console.WriteLine("Attempting to extract ROM data");

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
