using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.VDF.Constants;

namespace SabreTools.Serialization.Readers
{
    /// <remarks>
    /// The VDF file format was used for a very wide scope of functions on steam. At the moment, VDF file support is
    /// only needed when it comes to parsing retail sku sis files, so the current parser is only aimed at supporting
    /// these files, as they're overall very consistent, and trying to test every usage of VDF files would be extremely
    /// time-consuming for little benefit. If parsing other usages of VDF files ever becomes necessary, this should be
    /// replaced with a general-purpose VDF parser.
    /// Most observations about sku sis files described here probably also apply to VDF files.
    /// </remarks>
    public class SkuSis : BaseBinaryReader<Data.Models.VDF.SkuSis>
    {
        /// <inheritdoc/>
        public override Data.Models.VDF.SkuSis? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Check if file contains the top level sku value, otherwise return null
                var signatureBytes = data.ReadBytes(5);
                if (!signatureBytes.EqualsExactly(SteamSimSidSisSignatureBytes)
                    && !signatureBytes.EqualsExactly(SteamCsmCsdSisSignatureBytes))
                {
                    return null;
                }

                data.SeekIfPossible(initialOffset, SeekOrigin.Begin);

                var jsonBytes = ParseSkuSis(data);
                if (jsonBytes is null)
                    return null;

                var deserializer = new SkuSisJson();
                var skuSisJson = deserializer.Deserialize(jsonBytes, 0);
                if (skuSisJson is null)
                    return null;

                return skuSisJson;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        ///  Handles deserialization of the json-modified VDF string into a json.
        /// </summary>
        /// <remarks>Requires VDF-to-JSON conversion, should not be public.</remarks>
        private class SkuSisJson : JsonFile<Data.Models.VDF.SkuSis>
        {
            #region IByteReader

            /// <remarks>All known sku sis files are observed to be ASCII</remarks>
            public override Data.Models.VDF.SkuSis? Deserialize(byte[]? data, int offset)
                => Deserialize(data, offset, new ASCIIEncoding());

            #endregion

            #region IFileReader

            /// <remarks>All known sku sis files are observed to be ASCII</remarks>
            public override Data.Models.VDF.SkuSis? Deserialize(string? path)
                => Deserialize(path, new ASCIIEncoding());

            #endregion

            #region IStreamReader

            /// <remarks>All known sku sis files are observed to be ASCII</remarks>
            public override Data.Models.VDF.SkuSis? Deserialize(Stream? data)
                => Deserialize(data, new ASCIIEncoding());

            #endregion
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static byte[]? ParseSkuSis(Stream data)
        {
            string json = "{\n"; // Sku sis files have no surrounding curly braces, which json doesn't allow
            const string delimiter = "\"\t\t\""; // KVPs are always quoted, and are delimited by two tabs

            // This closes the stream, but can't be easily avoided on earlier versions of dotnet
#if NET20 || NET35 || NET40
            var reader = new StreamReader(data, Encoding.ASCII);
#else
            var reader = new StreamReader(data, Encoding.ASCII, false, -1, true);
#endif
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line is null)
                    continue;

                // Curly braces are always on their own lines
                if (line.Contains("{"))
                {
                    json += "{\n";
                    continue;
                }
                else if (line.Contains("}"))
                {
                    json += line;
                    json += ",\n";
                    continue;
                }

                int index = line.IndexOf(delimiter, StringComparison.Ordinal);

                // If the delimiter isn't found, this is the start of an object with multiple KVPs and the next line
                // will be an opening curly brace line.
                if (index <= -1)
                {
                    json += line;
                    json += ": ";
                }
                else // If the delimiter is found, it's just a normal KVP
                {
                    json += line.Replace(delimiter, "\": \"");
                    json += ",\n";
                }
            }

            json += "\n}";
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            return jsonBytes;
        }
    }
}
