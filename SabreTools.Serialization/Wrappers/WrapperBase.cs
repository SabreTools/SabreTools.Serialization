using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.IO.Streams;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public abstract class WrapperBase : IWrapper
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public string Description() => DescriptionString;

        /// <summary>
        /// Description of the object
        /// </summary>
        public abstract string DescriptionString { get; }

        #endregion

        #region Properties

        /// <inheritdoc cref="ViewStream.Filename"/>
        public string? Filename => _dataSource.Filename;

        /// <inheritdoc cref="ViewStream.Length"/>
        public long Length => _dataSource.Length;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Source of the original data
        /// </summary>
        protected readonly ViewStream _dataSource;

#if NETCOREAPP
        /// <summary>
        /// JSON serializer options for output printing
        /// </summary>
        protected System.Text.Json.JsonSerializerOptions _jsonSerializerOptions
        {
            get
            {
#if NETCOREAPP3_1
                var serializer = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
#else
                var serializer = new System.Text.Json.JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
#endif
                serializer.Converters.Add(new ConcreteAbstractSerializer());
                serializer.Converters.Add(new ConcreteInterfaceSerializer());
                serializer.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                return serializer;
            }
        }
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(byte[]? data, int offset)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            _dataSource = new ViewStream(data, offset, data.Length - offset);
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        protected WrapperBase(Stream? data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (!data.CanSeek || !data.CanRead)
                throw new ArgumentOutOfRangeException(nameof(data));

            _dataSource = new ViewStream(data, data.Position, data.Length - data.Position);
        }

        #endregion

        #region Data

        /// <summary>
        /// Read data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <returns>Byte array containing the requested data, null on error</returns>
        public byte[]? ReadFromDataSource(int position, int length)
        {
            // Validate the requested segment
            if (!_dataSource.SegmentValid(position, length))
                return null;

            try
            {
                long currentLocation = _dataSource.Position;

                _dataSource.Seek(position, SeekOrigin.Begin);
                byte[] sectionData = _dataSource.ReadBytes(length);
                _dataSource.Seek(currentLocation, SeekOrigin.Begin);

                return sectionData;

            }
            catch
            {
                // Absorb the error
                return null;
            }
        }

        /// <summary>
        /// Read string data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <param name="charLimit">Number of characters needed to be a valid string</param>
        /// <returns>String list containing the requested data, null on error</returns>
        public List<string>? ReadStringsFromDataSource(int position, int length, int charLimit = 5)
        {
            // Read the data as a byte array first
            byte[]? sourceData = ReadFromDataSource(position, length);
            if (sourceData == null)
                return null;

            // Check for ASCII strings
            var asciiStrings = ReadStringsWithEncoding(sourceData, charLimit, Encoding.ASCII);

            // Check for UTF-8 strings
            // We are limiting the check for Unicode characters with a second byte of 0x00 for now
            var utf8Strings = ReadStringsWithEncoding(sourceData, charLimit, Encoding.UTF8);

            // Check for Unicode strings
            // We are limiting the check for Unicode characters with a second byte of 0x00 for now
            var unicodeStrings = ReadStringsWithEncoding(sourceData, charLimit, Encoding.Unicode);

            // Ignore duplicate strings across encodings
            List<string> sourceStrings = [.. asciiStrings, .. utf8Strings, .. unicodeStrings];

            // Sort the strings and return
            sourceStrings.Sort();
            return sourceStrings;
        }

        /// <summary>
        /// Read string data from the source with an encoding
        /// </summary>
        /// <param name="sourceData">Byte array representing the source data</param>
        /// <param name="charLimit">Number of characters needed to be a valid string</param>
        /// <param name="encoding">Character encoding to use for checking</param>
        /// <returns>String list containing the requested data, empty on error</returns>
        /// <remarks>TODO: Move to IO?</remarks>
#if NET20
        private static List<string> ReadStringsWithEncoding(byte[] sourceData, int charLimit, Encoding encoding)
#else
        private static HashSet<string> ReadStringsWithEncoding(byte[] sourceData, int charLimit, Encoding encoding)
#endif
        {
            // If we have an invalid character limit, default to 5
            if (charLimit <= 0)
                charLimit = 5;

            // Create the string hash set to return
#if NET20
            var sourceStrings = new List<string>();
#else
            var sourceStrings = new HashSet<string>();
#endif

            // Setup cached data
            int sourceDataIndex = 0;
            List<char> cachedChars = [];

            // Check for strings
            while (sourceDataIndex < sourceData.Length)
            {
                // Read the next character
                char ch = encoding.GetChars(sourceData, sourceDataIndex, 1)[0];

                // If we have a control character or an invalid byte
                bool isValid = !char.IsControl(ch) && (ch & 0xFF00) == 0;
                if (!isValid)
                {
                    // If we have no cached string
                    if (cachedChars.Count == 0)
                    {
                        sourceDataIndex++;
                        continue;
                    }

                    // If we have a cached string greater than the limit
                    if (cachedChars.Count >= charLimit)
                        sourceStrings.Add(new string([.. cachedChars]));

                    cachedChars.Clear();
                    sourceDataIndex++;
                    continue;
                }

                // If a long repeating string is found, discard it
                if (cachedChars.Count >= 64 && cachedChars.TrueForAll(c => c == cachedChars[0]))
                {
                    cachedChars.Clear();
                    sourceDataIndex++;
                    continue;
                }

                // Append the character to the cached string
                cachedChars.Add(ch);
                sourceDataIndex++;
            }

            // If we have a cached string greater than the limit
            if (cachedChars.Count >= charLimit)
            {
                // Get the string from the cached characters
                string cachedString = new([.. cachedChars]);
                cachedString = cachedString.Trim();

                // Only include trimmed strings over the limit
                if (cachedString.Length >= charLimit)
                    sourceStrings.Add(cachedString);
            }

            return sourceStrings;
        }

        #endregion

        #region JSON Export

#if NETCOREAPP
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        public abstract string ExportJSON();
#endif

        #endregion
    }
}
