using System.Text;

namespace SabreTools.Data.Extensions
{
    public static class CFB
    {
        /// <summary>
        /// Decode a MIME-encoded stream name stored as a byte array
        /// </summary>
        /// <param name="input">Byte array to decode</param>
        /// <returns>Decoded string on success, null otherwise</returns>
        /// <remarks>Adapted from LibMSI</remarks>
        public static string? DecodeStreamName(this byte[]? input)
        {
            // Ignore invalid inputs
            if (input == null)
                return null;
            else if (input.Length == 0)
                return string.Empty;

            int count = 0;
            int p = 0; // inputBytes[0]

            byte[] output = new byte[input.Length + 1];
            int q = 0; // output[0]
            while (p < input.Length && input[p] != 0)
            {
                byte ch = input[p];
                if ((ch == 0xe3 && input[p + 1] >= 0xa0) || (ch == 0xe4 && input[p + 1] < 0xa0))
                {
                    // UTF-8 encoding of 0x3800..0x47ff.
                    output[q++] = (byte)Mime2Utf(input[p + 2] & 0x7f);
                    output[q++] = (byte)Mime2Utf(input[p + 1] ^ 0xa0);
                    p += 3;
                    count += 2;
                    continue;
                }

                if (ch == 0xe4 && input[p + 1] == 0xa0)
                {
                    // UTF-8 encoding of 0x4800..0x483f.
                    output[q++] = (byte)Mime2Utf(input[p + 2] & 0x7f);
                    p += 3;
                    count++;
                    continue;
                }

                output[q++] = input[p++];
                if (ch >= 0xc1)
                    output[q++] = input[p++];
                if (ch >= 0xe0)
                    output[q++] = input[p++];
                if (ch >= 0xf0)
                    output[q++] = input[p++];

                count++;
            }

            output[q] = 0;
            return Encoding.UTF8.GetString(output);
        }

        /// <summary>
        /// Decode a MIME-encoded stream name stored as a string
        /// </summary>
        /// <param name="input">String to decode</param>
        /// <returns>Decoded string on success, null otherwise</returns>
        /// <remarks>Adapted from LibMSI</remarks>
        public static string? DecodeStreamName(this string? input)
        {
            // Ignore invalid inputs
            if (input == null)
                return null;
            else if (input.Length == 0)
                return string.Empty;

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return inputBytes.DecodeStreamName();
        }

        /// <summary>
        /// Decode a single MIME-encoded byte
        /// </summary>
        /// <param name="b">Byte to decode</param>
        /// <returns>Decoded byte on success, `_` otherwise</returns>
        /// <remarks>Adapted from LibMSI</remarks>
        private static int Mime2Utf(int b)
        {
            return b switch
            {
                < 10 => b + '0',
                < 10 + 26 => b - 10 + 'A',
                < 10 + 26 + 26 => b - 10 - 26 + 'a',
                10 + 26 + 26 => '.',
                _ => '_'
            };
        }
    }
}
