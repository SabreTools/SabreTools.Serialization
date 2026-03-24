using System;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class XboxExecutableExtensions
    {
        /// <summary>
        /// Convert a UInt32 to a formatted XBE title ID
        /// </summary>
        public static string ToFormattedXBETitleID(this uint value)
        {
            // Convert to a byte array
            byte[] data = BitConverter.GetBytes(value);

            // Create the prefix
            string prefix1 = (0x41 <= data[3] && data[3] <= 0x5A) || (0x30 <= data[3] && data[3] <= 0x39)
                ? $"{(char)data[3]}"
                : $"{data[3]:X2}";
            string prefix2 = (0x41 <= data[2] && data[2] <= 0x5A) || (0x30 <= data[2] && data[2] <= 0x39)
                ? $"{(char)data[2]}"
                : $"{data[2]:X2}";

            // Create the serial
            int offset = 0;
            ushort serial = data.ReadUInt16LittleEndian(ref offset);

            // Assemble and return
            return $"{prefix1}{prefix2}-{serial:D3}";
        }
    }
}
