using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Deserializers
{
    public class ASN1TypeLengthValue : BaseBinaryDeserializer<ASN1.TypeLengthValue>
    {
        /// <inheritdoc/>
        public override ASN1.TypeLengthValue? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var tlv = ParseTypeLengthValue(data);
                if (tlv == null)
                    return null;

                // Return the Type/Length/Value
                return tlv;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a TypeLengthValue
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled TypeLengthValue on success, null on error</returns>
        public ASN1.TypeLengthValue? ParseTypeLengthValue(Stream data)
        {
            var obj = new ASN1.TypeLengthValue();

            // Get the type and modifiers
            obj.Type = (ASN1.ASN1Type)data.ReadByteValue();

            // If we have an end indicator, we just return
            if (obj.Type == ASN1.ASN1Type.V_ASN1_EOC)
                return obj;

            // Get the length of the value
            ulong? length = ReadLength(data);
            if (length == null)
                return null;

            // Set the length
            obj.Length = length.Value;

            // Read the value
#if NET20 || NET35
            if ((obj.Type & ASN1.ASN1Type.V_ASN1_CONSTRUCTED) != 0)
#else
            if (obj.Type.HasFlag(ASN1.ASN1Type.V_ASN1_CONSTRUCTED))
#endif
            {
                var valueList = new List<ASN1.TypeLengthValue>();

                long currentIndex = data.Position;
                while (data.Position < currentIndex + (long)obj.Length)
                {
                    var value = ParseTypeLengthValue(data);
                    valueList.Add(value);
                }

                obj.Value = valueList.ToArray();
            }
            else
            {
                // TODO: Get more granular based on type
                obj.Value = data.ReadBytes((int)obj.Length);
            }

            return obj;
        }

        /// <summary>
        /// Reads the length field for a type
        /// </summary>
        /// <param name="data">Stream representing data to read</param>
        /// <returns>The length value read from the array</returns>
        private static ulong? ReadLength(Stream data)
        {
            // Read the first byte, assuming it's the length
            byte length = data.ReadByteValue();

            // If the bit 7 is not set, then use the value as it is
            if ((length & 0x80) == 0)
                return length;

            // Otherwise, use the value as the number of remaining bytes to read
            int bytesToRead = length & ~0x80;

            // Assemble the length based on byte count
            ulong fullLength = 0;
            switch (bytesToRead)
            {
                case 8:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 7;
                case 7:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 6;
                case 6:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 5;
                case 5:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 4;
                case 4:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 3;
                case 3:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 2;
                case 2:
                    fullLength |= data.ReadByteValue();
                    fullLength <<= 8;
                    goto case 1;
                case 1:
                    fullLength |= data.ReadByteValue();
                    break;

                default:
                    return null;
            }

            return fullLength;
        }
    }
}
