using System;
using System.Collections.Generic;
using System.IO;

namespace SabreTools.Serialization.ASN1
{
    /// <summary>
    /// ASN.1 Parser
    /// </summary>
    public static class AbstractSyntaxNotationOne
    {
        /// <summary>
        /// Parse a byte array into a DER-encoded ASN.1 structure
        /// </summary>
        /// <param name="data">Byte array representing the data</param>
        /// <param name="pointer">Current pointer into the data</param>
        public static List<TypeLengthValue> Parse(byte[] data, int pointer)
        {
            // If the data is invalid
            if (data.Length == 0)
                throw new InvalidDataException(nameof(data));
            if (pointer < 0 || pointer >= data.Length)
                throw new IndexOutOfRangeException(nameof(pointer));

            using var stream = new MemoryStream(data);
            stream.Seek(pointer, SeekOrigin.Begin);
            return Parse(stream);
        }

        /// <summary>
        /// Parse a stream into a DER-encoded ASN.1 structure
        /// </summary>
        /// <param name="data">Stream representing the data</param>
        public static List<TypeLengthValue> Parse(Stream data)
        {
            // If the data is invalid
            if (data.Length == 0 || !data.CanRead)
                throw new InvalidDataException(nameof(data));
            if (data.Position < 0 || data.Position >= data.Length)
                throw new IndexOutOfRangeException(nameof(data));

            // Create the deserializer
            var deserializer = new Deserializers.TypeLengthValue();

            // Create the output list to return
            var topLevelValues = new List<TypeLengthValue>();

            // Loop through the data and return all top-level values
            while (data.Position < data.Length)
            {
                var topLevelValue = deserializer.Deserialize(data);
                if (topLevelValue == null)
                    break;

                topLevelValues.Add(topLevelValue);
            }

            return topLevelValues;
        }
    }
}
