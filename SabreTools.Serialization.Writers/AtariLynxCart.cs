using System;
using System.IO;
using SabreTools.Data.Models.AtariLynx;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class AtariLynxCart : BaseBinaryWriter<Cart>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Cart? obj)
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
        public bool WriteHeader(Header? obj, Stream stream)
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
                stream.Write(obj.Magic);
                stream.Flush();

                // Bytes 4-5
                stream.Write(obj.Bank0PageSize);
                stream.Flush();

                // Bytes 6-7
                stream.Write(obj.Bank0PageSize);
                stream.Flush();

                // Bytes 8-9
                stream.Write(obj.Version);
                stream.Flush();

                // Bytes 10-41
                stream.Write(obj.CartName);
                stream.Flush();

                // Bytes 42-57
                stream.Write(obj.Manufacturer);
                stream.Flush();

                // Byte 58
                stream.Write((byte)obj.Rotation);
                stream.Flush();

                // Bytes 59-63
                stream.Write(obj.Spare);
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

                stream.Write(obj, 0, obj.Length);
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
