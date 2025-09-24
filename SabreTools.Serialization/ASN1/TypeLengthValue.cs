using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.ASN1
{
    /// <summary>
    /// ASN.1 type/length/value class that all types are based on
    /// </summary>
    public class TypeLengthValue
    {
        /// <summary>
        /// The ASN.1 type
        /// </summary>
        public ASN1Type Type { get; private set; }

        /// <summary>
        /// Length of the value
        /// </summary>
        public ulong Length { get; private set; }

        /// <summary>
        /// Generic value associated with <see cref="Type"/>
        /// </summary>
        public object? Value { get; private set; }

        /// <summary>
        /// Manual constructor
        /// </summary>
        public TypeLengthValue(ASN1Type type, ulong length, object? value)
        {
            Type = type;
            Length = length;
            Value = value;
        }

        /// <summary>
        /// Read from the source data array at an index
        /// </summary>
        /// <param name="data">Byte array representing data to read</param>
        /// <param name="index">Index within the array to read at</param>
        public TypeLengthValue(byte[] data, ref int index)
        {
            // If the data is invalid
            if (data.Length == 0)
                throw new InvalidDataException(nameof(data));
            if (index < 0 || index >= data.Length)
                throw new IndexOutOfRangeException(nameof(index));

            using var stream = new MemoryStream(data);
            stream.Seek(index, SeekOrigin.Begin);
            if (!Parse(stream))
                throw new InvalidDataException(nameof(data));
        }

        /// <summary>
        /// Read from the source data stream
        /// </summary>
        /// <param name="data">Stream representing data to read</param>
        public TypeLengthValue(Stream data)
        {
            if (!Parse(data))
                throw new InvalidDataException(nameof(data));
        }

        /// <summary>
        /// Format the TLV as a string
        /// </summary>
        /// <param name="paddingLevel">Padding level of the item when formatting</param>
        /// <returns>String representing the TLV, if possible</returns>
        public string Format(int paddingLevel = 0)
        {
            // Create the left-padding string
            string padding = new(' ', paddingLevel);

            // Create the string builder
            var formatBuilder = new StringBuilder();

            // Append the type
            formatBuilder.Append($"{padding}Type: {Type}");
            if (Type == ASN1Type.V_ASN1_EOC)
                return formatBuilder.ToString();

            // Append the length
            formatBuilder.Append($", Length: {Length}");
            if (Length == 0)
                return formatBuilder.ToString();

            // If we have a constructed type
#if NET20 || NET35
            if ((Type & ASN1Type.V_ASN1_CONSTRUCTED) != 0)
#else
            if (Type.HasFlag(ASN1Type.V_ASN1_CONSTRUCTED))
#endif
            {
                if (Value is not TypeLengthValue[] valueAsObjectArray)
                {
                    formatBuilder.Append(", Value: [INVALID DATA TYPE]");
                    return formatBuilder.ToString();
                }

                formatBuilder.Append(", Value:\n");
                for (int i = 0; i < valueAsObjectArray.Length; i++)
                {
                    var child = valueAsObjectArray[i];
                    string childString = child.Format(paddingLevel + 1);
                    formatBuilder.Append($"{childString}\n");
                }

                return formatBuilder.ToString().TrimEnd('\n');
            }

            // Get the value as a byte array
            if (Value is not byte[] valueAsByteArray)
            {
                formatBuilder.Append(", Value: [INVALID DATA TYPE]");
                return formatBuilder.ToString();
            }
            else if (valueAsByteArray.Length == 0)
            {
                formatBuilder.Append(", Value: [NO DATA]");
                return formatBuilder.ToString();
            }

            // If we have a primitive type
            switch (Type)
            {
                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-boolean"/>
                case ASN1Type.V_ASN1_BOOLEAN:
                    if (Length > 1)
                        formatBuilder.Append($" [Expected length of 1]");
                    else if (valueAsByteArray.Length > 1)
                        formatBuilder.Append($" [Expected value length of 1]");

                    bool booleanValue = valueAsByteArray[0] != 0x00;
                    formatBuilder.Append($", Value: {booleanValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-integer"/>
                case ASN1Type.V_ASN1_INTEGER:
                    Array.Reverse(valueAsByteArray);
                    var integerValue = new BigInteger(valueAsByteArray);
                    formatBuilder.Append($", Value: {integerValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bit-string"/>
                case ASN1Type.V_ASN1_BIT_STRING:
                    // TODO: Read into a BitArray and print that out instead?
                    int unusedBits = valueAsByteArray[0];
                    if (unusedBits == 0)
                        formatBuilder.Append($", Value with {unusedBits} unused bits");
                    else
                        formatBuilder.Append($", Value with {unusedBits} unused bits: {BitConverter.ToString(valueAsByteArray, 1).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-octet-string"/>
                case ASN1Type.V_ASN1_OCTET_STRING:
                    formatBuilder.Append($", Value: {BitConverter.ToString(valueAsByteArray).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-object-identifier"/>
                /// <see cref="http://snmpsharpnet.com/index.php/2009/03/02/ber-encoding-and-decoding-oid-values/"/>
                case ASN1Type.V_ASN1_OBJECT:
                    // Derive array of values
                    ulong[] objectNodes = ObjectIdentifier.ParseDERIntoArray(valueAsByteArray, Length);

                    // Append the dot and modified OID-IRI notations
                    string? dotNotationString = ObjectIdentifier.ParseOIDToDotNotation(objectNodes);
                    string? oidIriString = ObjectIdentifier.ParseOIDToOIDIRINotation(objectNodes);
                    formatBuilder.Append($", Value: {dotNotationString} ({oidIriString})");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-utf8string"/>
                case ASN1Type.V_ASN1_UTF8STRING:
                    formatBuilder.Append($", Value: {Encoding.UTF8.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-printablestring"/>
                case ASN1Type.V_ASN1_PRINTABLESTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                //case ASN1Type.V_ASN1_T61STRING:
                case ASN1Type.V_ASN1_TELETEXSTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-ia5string"/>
                case ASN1Type.V_ASN1_IA5STRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                case ASN1Type.V_ASN1_UTCTIME:
                    string utctimeString = Encoding.ASCII.GetString(valueAsByteArray);
                    if (DateTime.TryParse(utctimeString, out DateTime utctimeDateTime))
                        formatBuilder.Append($", Value: {utctimeDateTime:yyyy-MM-dd HH:mm:ss}");
                    else
                        formatBuilder.Append($", Value: {utctimeString}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bmpstring"/>
                case ASN1Type.V_ASN1_BMPSTRING:
                    formatBuilder.Append($", Value: {Encoding.Unicode.GetString(valueAsByteArray)}");
                    break;

                default:
                    formatBuilder.Append($", Value: {BitConverter.ToString(valueAsByteArray).Replace('-', ' ')}");
                    break;
            }

            // Return the formatted string
            return formatBuilder.ToString();
        }

        /// <summary>
        /// Parse a stream into TLV data
        /// </summary>
        /// <param name="data">Stream representing data to read</param>
        /// <returns>Indication if parsing was successful</returns>
        private bool Parse(Stream data)
        {
            // If the data is invalid
            if (data.Length == 0 || !data.CanRead)
                return false;
            if (data.Position < 0 || data.Position >= data.Length)
                throw new IndexOutOfRangeException(nameof(data));

            // Get the type and modifiers
            Type = (ASN1Type)data.ReadByteValue();

            // If we have an end indicator, we just return
            if (Type == ASN1Type.V_ASN1_EOC)
                return true;

            // Get the length of the value
            Length = ReadLength(data);

            // Read the value
#if NET20 || NET35
            if ((Type & ASN1Type.V_ASN1_CONSTRUCTED) != 0)
#else
            if (Type.HasFlag(ASN1Type.V_ASN1_CONSTRUCTED))
#endif
            {
                var valueList = new List<TypeLengthValue>();

                long currentIndex = data.Position;
                while (data.Position < currentIndex + (long)Length)
                {
                    valueList.Add(new TypeLengthValue(data));
                }

                Value = valueList.ToArray();
            }
            else
            {
                // TODO: Get more granular based on type
                Value = data.ReadBytes((int)Length);
            }

            return true;
        }

        /// <summary>
        /// Reads the length field for a type
        /// </summary>
        /// <param name="data">Stream representing data to read</param>
        /// <returns>The length value read from the array</returns>
        private static ulong ReadLength(Stream data)
        {
            // If the data is invalid
            if (data.Length == 0 || !data.CanRead)
                throw new InvalidDataException(nameof(data));
            if (data.Position < 0 || data.Position >= data.Length)
                throw new IndexOutOfRangeException(nameof(data));

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
                    throw new InvalidOperationException();
            }

            return fullLength;
        }
    }
}
