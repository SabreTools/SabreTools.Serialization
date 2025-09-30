namespace SabreTools.Data.Extensions
{
    public static class XZ
    {
        /// <summary>
        /// Decode a value from a variable-length integer
        /// </summary>
        /// <param name="value">Value to decode</param>
        /// <param name="maxSize">Maximum number of bytes to parse</param>
        /// <returns>UInt64 representing the decoded integer</returns>
        /// <see href="https://tukaani.org/xz/xz-file-format.txt"/>
        public static ulong DecodeVariableLength(this byte[] value, int maxSize)
        {
            if (maxSize <= 0)
                return 0;

            if (maxSize > 9)
                maxSize = 9;

            ulong output = value[0] & 0x7F;
            int i = 0;

            while (value[i++] & 0x80 != 0)
            {
                if (i >= maxSize || value[i] == 0x00)
                    return 0;

                output |= (value[i] & 0x7F) << (i * 7);
            }

            return output;
        }

        /// <summary>
        /// Encode a value to a variable-length integer
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Byte array representing the encoded integer</returns>
        /// <see href="https://tukaani.org/xz/xz-file-format.txt"/>
        public static byte[] EncodeVariableLength(this ulong value)
        {
            if (value > long.MaxValue / 2)
                return [];

            var output = new List<byte>();

            int i = 0;
            while (value >= 0x80)
            {
                output.Add((byte)value | 0x80);
                value >>= 7;
            }

            output.Add((byte)value);
            return [.. output];
        }
    }
}