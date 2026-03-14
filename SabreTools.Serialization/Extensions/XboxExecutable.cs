using SabreTools.IO.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class XboxExecutable
    {
        /// <summary>
        /// Convert a 4-byte value to a formatted XBE title ID
        /// </summary>
        public static string? ToFormattedXBETitleID(this byte[]? data)
        {
            // Ignore invalid data
            if (data is null || data.Length < 4)
                return null;

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
