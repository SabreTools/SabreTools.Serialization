using System;
using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class FDS : BaseBinaryWriter<Data.Models.NES.FDS>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.NES.FDS? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();

            // Write header
            if (!WriteHeader(obj.Header, stream))
                return null;

            // Write data
            if (!WriteRom(obj.Data, stream))
                return null;

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WriteHeader(FDSHeader? obj, Stream stream)
        {
            if (obj is null)
            {
                if (Debug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write header data");

                // Bytes 0-3
                stream.Write(obj.IdentificationString);
                stream.Flush();

                // Byte 4
                stream.Write(obj.DiskSides);
                stream.Flush();

                // Byte 5-15
                stream.Write(obj.Padding);
                stream.Flush();

                // Header extracted
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write rom data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WriteRom(byte[]? obj, Stream stream)
        {
            if (obj is null || obj.Length == 0)
            {
                if (Debug) Console.WriteLine("ROM data was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write ROM data");

                stream.Write(obj);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
