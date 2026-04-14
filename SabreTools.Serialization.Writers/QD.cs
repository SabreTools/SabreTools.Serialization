using System;
using System.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class QD : BaseBinaryWriter<Data.Models.NES.QD>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.NES.QD? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();

            // Write data
            if (!WriteRom(obj.Data, stream))
                return null;

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
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
