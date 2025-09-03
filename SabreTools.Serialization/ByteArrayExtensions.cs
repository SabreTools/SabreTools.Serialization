using System.Collections.Generic;
using System.Text;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization
{
    // TODO: Move this to IO
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Read string data from the source
        /// </summary>
        /// <param name="charLimit">Number of characters needed to be a valid string, default 5</param>
        /// <returns>String list containing the requested data, null on error</returns>
        public static List<string>? ReadStringsFrom(this byte[]? input, int charLimit = 5)
        {
            // Validate the data
            if (input == null)
                return null;

            // Check for ASCII strings
            var asciiStrings = input.ReadStringsWithEncoding(charLimit, Encoding.ASCII);

            // Check for UTF-8 strings
            // We are limiting the check for Unicode characters with a second byte of 0x00 for now
            var utf8Strings = input.ReadStringsWithEncoding(charLimit, Encoding.UTF8);

            // Check for Unicode strings
            // We are limiting the check for Unicode characters with a second byte of 0x00 for now
            var unicodeStrings = input.ReadStringsWithEncoding(charLimit, Encoding.Unicode);

            // Ignore duplicate strings across encodings
            List<string> sourceStrings = [.. asciiStrings, .. utf8Strings, .. unicodeStrings];

            // Sort the strings and return
            sourceStrings.Sort();
            return sourceStrings;
        }
    }
}