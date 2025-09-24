using System;
using System.Numerics;
using System.Text;

namespace SabreTools.Serialization.Extensions
{
    public static class TypeLengthValue
    {
        /// <summary>
        /// Format a TypeLengthValue as a string
        /// </summary>
        /// <param name="paddingLevel">Padding level of the item when formatting</param>
        /// <returns>String representing the TypeLengthValue, if possible</returns>
        public static string Format(this ASN1.TypeLengthValue tlv, int paddingLevel = 0)
        {
            // Create the left-padding string
            string padding = new(' ', paddingLevel);

            // Create the string builder
            var formatBuilder = new StringBuilder();

            // Append the type
            formatBuilder.Append($"{padding}Type: {tlv.Type}");
            if (tlv.Type == ASN1.ASN1Type.V_ASN1_EOC)
                return formatBuilder.ToString();

            // Append the length
            formatBuilder.Append($", Length: {tlv.Length}");
            if (tlv.Length == 0)
                return formatBuilder.ToString();

            // If we have a constructed type
#if NET20 || NET35
            if ((tlv.Type & ASN1.ASN1Type.V_ASN1_CONSTRUCTED) != 0)
#else
            if (tlv.Type.HasFlag(ASN1.ASN1Type.V_ASN1_CONSTRUCTED))
#endif
            {
                if (tlv.Value is not ASN1.TypeLengthValue[] valueAsObjectArray)
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
            if (tlv.Value is not byte[] valueAsByteArray)
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
            switch (tlv.Type)
            {
                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-boolean"/>
                case ASN1.ASN1Type.V_ASN1_BOOLEAN:
                    if (tlv.Length > 1)
                        formatBuilder.Append($" [Expected length of 1]");
                    else if (valueAsByteArray.Length > 1)
                        formatBuilder.Append($" [Expected value length of 1]");

                    bool booleanValue = valueAsByteArray[0] != 0x00;
                    formatBuilder.Append($", Value: {booleanValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-integer"/>
                case ASN1.ASN1Type.V_ASN1_INTEGER:
                    Array.Reverse(valueAsByteArray);
                    var integerValue = new BigInteger(valueAsByteArray);
                    formatBuilder.Append($", Value: {integerValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bit-string"/>
                case ASN1.ASN1Type.V_ASN1_BIT_STRING:
                    // TODO: Read into a BitArray and print that out instead?
                    int unusedBits = valueAsByteArray[0];
                    if (unusedBits == 0)
                        formatBuilder.Append($", Value with {unusedBits} unused bits");
                    else
                        formatBuilder.Append($", Value with {unusedBits} unused bits: {BitConverter.ToString(valueAsByteArray, 1).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-octet-string"/>
                case ASN1.ASN1Type.V_ASN1_OCTET_STRING:
                    formatBuilder.Append($", Value: {BitConverter.ToString(valueAsByteArray).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-object-identifier"/>
                /// <see cref="http://snmpsharpnet.com/index.php/2009/03/02/ber-encoding-and-decoding-oid-values/"/>
                case ASN1.ASN1Type.V_ASN1_OBJECT:
                    // Derive array of values
                    ulong[] objectNodes = ASN1.ObjectIdentifier.ParseDERIntoArray(valueAsByteArray, tlv.Length);

                    // Append the dot and modified OID-IRI notations
                    string? dotNotationString = ASN1.ObjectIdentifier.ParseOIDToDotNotation(objectNodes);
                    string? oidIriString = ASN1.ObjectIdentifier.ParseOIDToOIDIRINotation(objectNodes);
                    formatBuilder.Append($", Value: {dotNotationString} ({oidIriString})");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-utf8string"/>
                case ASN1.ASN1Type.V_ASN1_UTF8STRING:
                    formatBuilder.Append($", Value: {Encoding.UTF8.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-printablestring"/>
                case ASN1.ASN1Type.V_ASN1_PRINTABLESTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                //case ASN1Type.V_ASN1_T61STRING:
                case ASN1.ASN1Type.V_ASN1_TELETEXSTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-ia5string"/>
                case ASN1.ASN1Type.V_ASN1_IA5STRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                case ASN1.ASN1Type.V_ASN1_UTCTIME:
                    string utctimeString = Encoding.ASCII.GetString(valueAsByteArray);
                    if (DateTime.TryParse(utctimeString, out DateTime utctimeDateTime))
                        formatBuilder.Append($", Value: {utctimeDateTime:yyyy-MM-dd HH:mm:ss}");
                    else
                        formatBuilder.Append($", Value: {utctimeString}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bmpstring"/>
                case ASN1.ASN1Type.V_ASN1_BMPSTRING:
                    formatBuilder.Append($", Value: {Encoding.Unicode.GetString(valueAsByteArray)}");
                    break;

                default:
                    formatBuilder.Append($", Value: {BitConverter.ToString(valueAsByteArray).Replace('-', ' ')}");
                    break;
            }

            // Return the formatted string
            return formatBuilder.ToString();
        }
    }
}