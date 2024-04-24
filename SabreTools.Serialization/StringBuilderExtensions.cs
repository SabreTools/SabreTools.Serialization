using System;
using System.Linq;
using System.Text;

namespace SabreTools.Serialization
{
    // TODO: Add extension for printing enums, if possible
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// Append a line containing a boolean to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, bool? value, string prefixString)
        {
            value ??= false;
            return sb.AppendLine($"{prefixString}: {value}");
        }

        /// <summary>
        /// Append a line containing a Char to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, char? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : value.Value.ToString());
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int8 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, sbyte? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt8 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, byte? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int16 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, short? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt16 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, ushort? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int32 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, int? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt32 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, uint? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int64 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, long? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt64 to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, ulong? value, string prefixString)
        {
            value ??= 0;
            string valueString = $"{value} (0x{value:X})";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a string to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, string? value, string prefixString)
        {
            value ??= string.Empty;
            string valueString = value ?? "[NULL]";
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Guid to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, Guid? value, string prefixString)
        {
            value ??= Guid.Empty;
            string valueString = value.Value.ToString();
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt8[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, byte[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : BitConverter.ToString(value).Replace('-', ' '));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt8[] value as a string to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, byte[]? value, string prefixString, Encoding encoding)
        {
            string valueString = (value == null ? "[NULL]" : encoding.GetString(value).Replace("\0", string.Empty));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Char[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, char[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(c => c.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int16[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, short[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(s => s.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt16[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, ushort[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(u => u.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int32[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, int[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(i => i.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt32[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, uint[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(u => u.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a Int64[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, long[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(l => l.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }

        /// <summary>
        /// Append a line containing a UInt64[] value to a StringBuilder
        /// </summary>
        public static StringBuilder AppendLine(this StringBuilder sb, ulong[]? value, string prefixString)
        {
            string valueString = (value == null ? "[NULL]" : string.Join(", ", value.Select(u => u.ToString()).ToArray()));
            return sb.AppendLine($"{prefixString}: {valueString}");
        }
    }
}