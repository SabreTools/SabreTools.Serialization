using System;
using System.IO;
using SabreTools.Data.Models.Atari7800;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class Atari7800Cart : BaseBinaryWriter<Cart>
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

                // Byte 0
                stream.Write(obj.HeaderVersion);
                stream.Flush();

                // Bytes 1-16
                stream.Write(obj.MagicText);
                stream.Flush();

                // Bytes 17-48
                stream.Write(obj.CartTitle);
                stream.Flush();

                // Bytes 49-52
                stream.Write(obj.RomSizeWithoutHeader);
                stream.Flush();

                // Bytes 53-54
                stream.Write((ushort)obj.CartType);
                stream.Flush();

                // Bytes 55-56
                stream.Write((byte)obj.Controller1Type);
                stream.Write((byte)obj.Controller2Type);
                stream.Flush();

                // Byte 57
                stream.Write((byte)obj.TVType);
                stream.Flush();

                // Byte 58
                stream.Write((byte)obj.SaveDevice);
                stream.Flush();

                // Bytes 59-62
                stream.Write(obj.Reserved);
                stream.Flush();

                // Byte 63
                stream.Write((byte)obj.SlotPassthroughDevice);
                stream.Flush();

                if (obj.HeaderVersion >= 4)
                {
                    // Byte 64
                    stream.Write((byte)obj.Mapper);
                    stream.Flush();

                    // Byte 65
                    stream.Write((byte)obj.MapperOptions);
                    stream.Flush();

                    // Byte 66-67
                    stream.Write((ushort)obj.AudioDevice);
                    stream.Flush();

                    // Byte 68-69
                    stream.Write((ushort)obj.Interrupt);
                    stream.Flush();

                    // Bytes 70-99
                    stream.Write(obj.Padding);
                    stream.Flush();
                }
                else
                {
                    // Bytes 64-99
                    stream.Write(obj.Padding);
                    stream.Flush();
                }

                // Bytes 100-127
                stream.Write(obj.EndMagicText);
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
