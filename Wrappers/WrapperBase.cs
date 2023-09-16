using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public abstract class WrapperBase<T> : IWrapper
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

        /// <summary>
        /// Internal model
        /// </summary>
#if NET48
        public T Model { get; private set; }
#else
        public T Model { get; init; }
#endif

        #endregion

        #region Instance Variables

        /// <summary>
        /// Source of the original data
        /// </summary>
        protected DataSource _dataSource = DataSource.UNKNOWN;

        /// <summary>
        /// Source byte array data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
#if NET48
        protected byte[] _byteArrayData = null;
#else
        protected byte[]? _byteArrayData = null;
#endif

        /// <summary>
        /// Source byte array data offset
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
        protected int _byteArrayOffset = -1;

        /// <summary>
        /// Source Stream data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.Stream"/></remarks>
#if NET48
        protected Stream _streamData = null;
#else
        protected Stream? _streamData = null;
#endif

#if NET6_0_OR_GREATER

        /// <summary>
        /// JSON serializer options for output printing
        /// </summary>
        protected System.Text.Json.JsonSerializerOptions _jsonSerializerOptions
        {
            get
            {
                var serializer = new System.Text.Json.JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
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
#if NET48
        protected WrapperBase(T model, byte[] data, int offset)
#else
        protected WrapperBase(T? model, byte[]? data, int offset)
#endif
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            this.Model = model;
            _dataSource = DataSource.ByteArray;
            _byteArrayData = data;
            _byteArrayOffset = offset;
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
#if NET48
        protected WrapperBase(T model, Stream data)
#else
        protected WrapperBase(T? model, Stream? data)
#endif
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0 || !data.CanSeek || !data.CanRead)
                throw new ArgumentOutOfRangeException(nameof(data));

            this.Model = model;
            _dataSource = DataSource.Stream;
            _streamData = data;
        }

        #endregion

        #region Data

        /// <summary>
        /// Validate the backing data source
        /// </summary>
        /// <returns>True if the data source is valid, false otherwise</returns>
        protected bool DataSourceIsValid()
        {
            switch (_dataSource)
            {
                // Byte array data requires both a valid array and offset
                case DataSource.ByteArray:
                    return _byteArrayData != null && _byteArrayOffset >= 0;

                // Stream data requires both a valid stream
                case DataSource.Stream:
                    return _streamData != null && _streamData.CanRead && _streamData.CanSeek;

                // Everything else is invalid
                case DataSource.UNKNOWN:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if a data segment is valid in the data source 
        /// </summary>
        /// <param name="position">Position in the source</param>
        /// <param name="length">Length of the data to check</param>
        /// <returns>True if the positional data is valid, false otherwise</returns>
        protected bool SegmentValid(int position, int length)
        {
            // Validate the data souece
            if (!DataSourceIsValid())
                return false;

            // If we have an invalid position
            if (position < 0 || position >= GetEndOfFile())
                return false;

            switch (_dataSource)
            {
                case DataSource.ByteArray:
#if NET48
                    return _byteArrayOffset + position + length <= _byteArrayData.Length;
#else
                    return _byteArrayOffset + position + length <= _byteArrayData!.Length;
#endif

                case DataSource.Stream:
#if NET48
                    return position + length <= _streamData.Length;
#else
                    return position + length <= _streamData!.Length;
#endif

                // Everything else is invalid
                case DataSource.UNKNOWN:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Read data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <returns>Byte array containing the requested data, null on error</returns>
#if NET48
        protected byte[] ReadFromDataSource(int position, int length)
#else
        protected byte[]? ReadFromDataSource(int position, int length)
#endif
        {
            // Validate the data source
            if (!DataSourceIsValid())
                return null;

            // Validate the requested segment
            if (!SegmentValid(position, length))
                return null;

            // Read and return the data
#if NET48
            byte[] sectionData = null;
#else
            byte[]? sectionData = null;
#endif
            switch (_dataSource)
            {
                case DataSource.ByteArray:
                    sectionData = new byte[length];
#if NET48
                    Array.Copy(_byteArrayData, _byteArrayOffset + position, sectionData, 0, length);
#else
                    Array.Copy(_byteArrayData!, _byteArrayOffset + position, sectionData, 0, length);
#endif
                    break;

                case DataSource.Stream:
#if NET48
                    long currentLocation = _streamData.Position;
#else
                    long currentLocation = _streamData!.Position;
#endif
                    _streamData.Seek(position, SeekOrigin.Begin);
                    sectionData = _streamData.ReadBytes(length);
                    _streamData.Seek(currentLocation, SeekOrigin.Begin);
                    break;
            }

            return sectionData;
        }

        /// <summary>
        /// Read string data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <param name="charLimit">Number of characters needed to be a valid string</param>
        /// <returns>String list containing the requested data, null on error</returns>
#if NET48
        protected List<string> ReadStringsFromDataSource(int position, int length, int charLimit = 5)
#else
        protected List<string>? ReadStringsFromDataSource(int position, int length, int charLimit = 5)
#endif
        {
            // Read the data as a byte array first
#if NET48
            byte[] sourceData = ReadFromDataSource(position, length);
#else
            byte[]? sourceData = ReadFromDataSource(position, length);
#endif
            if (sourceData == null)
                return null;

            // If we have an invalid character limit, default to 5
            if (charLimit <= 0)
                charLimit = 5;

            // Create the string list to return
            var sourceStrings = new List<string>();

            // Setup cached data
            int sourceDataIndex = 0;
            string cachedString = string.Empty;

            // Check for ASCII strings
            while (sourceDataIndex < sourceData.Length)
            {
                // If we have a control character or an invalid byte
                if (sourceData[sourceDataIndex] < 0x20 || sourceData[sourceDataIndex] > 0x7F)
                {
                    // If we have no cached string
                    if (cachedString.Length == 0)
                    {
                        sourceDataIndex++;
                        continue;
                    }

                    // If we have a cached string greater than the limit
                    if (cachedString.Length >= charLimit)
                        sourceStrings.Add(cachedString);

                    cachedString = string.Empty;
                    sourceDataIndex++;
                    continue;
                }

                // All other characters get read in
                cachedString += Encoding.ASCII.GetString(sourceData, sourceDataIndex, 1);
                sourceDataIndex++;
            }

            // If we have a cached string greater than the limit
            if (cachedString.Length >= charLimit)
                sourceStrings.Add(cachedString);

            // Reset cached data
            sourceDataIndex = 0;
            cachedString = string.Empty;

            // We are limiting the check for Unicode characters with a second byte of 0x00 for now

            // Check for Unicode strings
            while (sourceDataIndex < sourceData.Length)
            {
                // Unicode characters are always 2 bytes
                if (sourceDataIndex == sourceData.Length - 1)
                    break;

                ushort ch = BitConverter.ToUInt16(sourceData, sourceDataIndex);

                // If we have a null terminator or "invalid" character
                if (ch == 0x0000 || (ch & 0xFF00) != 0)
                {
                    // If we have no cached string
                    if (cachedString.Length == 0)
                    {
                        sourceDataIndex += 2;
                        continue;
                    }

                    // If we have a cached string greater than the limit
                    if (cachedString.Length >= charLimit)
                        sourceStrings.Add(cachedString);

                    cachedString = string.Empty;
                    sourceDataIndex += 2;
                    continue;
                }

                // All other characters get read in
                cachedString += Encoding.Unicode.GetString(sourceData, sourceDataIndex, 2);
                sourceDataIndex += 2;
            }

            // If we have a cached string greater than the limit
            if (cachedString.Length >= charLimit)
                sourceStrings.Add(cachedString);

            // Deduplicate the string list for storage
            sourceStrings = sourceStrings.Distinct().OrderBy(s => s).ToList();

            // TODO: Complete implementation of string finding
            return sourceStrings;
        }

        /// <summary>
        /// Get the ending offset of the source
        /// </summary>
        /// <returns>Value greater than 0 for a valid end of file, -1 on error</returns>
        protected int GetEndOfFile()
        {
            // Validate the data souece
            if (!DataSourceIsValid())
                return -1;

            // Return the effective endpoint
            switch (_dataSource)
            {
                case DataSource.ByteArray:
#if NET48
                    return _byteArrayData.Length - _byteArrayOffset;
#else
                    return _byteArrayData!.Length - _byteArrayOffset;
#endif

                case DataSource.Stream:
#if NET48
                    return (int)_streamData.Length;
#else
                    return (int)_streamData!.Length;
#endif

                case DataSource.UNKNOWN:
                default:
                    return -1;
            }
        }

        #endregion

        #region JSON Export

#if NET6_0_OR_GREATER
        /// <summary>
        /// Export the item information as JSON
        /// </summary>
        public string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        #endregion
    }
}