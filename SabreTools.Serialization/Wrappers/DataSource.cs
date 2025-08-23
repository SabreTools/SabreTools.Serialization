using System;
using System.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Represents the data source backing the wrapper
    /// </summary>
    public class DataSource
    {
        #region Properties

        /// <summary>
        /// Filename from the source, if possible
        /// </summary>
        /// <returns>String representing the filename on success, null otherwise</returns>
        /// <remarks>This only works if the source was a <see cref="FileStream"/></remarks>
        public string? Filename
        {
            get
            {
                // Only streams can have a filename
                if (_dataSourceType != DataSourceType.Stream)
                    return null;

                // Only file streams can have a filename
                if (_streamData == null || _streamData is not FileStream fs)
                    return null;

                // Return the name
                return fs.Name;
            }
        }

        /// <summary>
        /// Usable length of the underlying data
        /// </summary>
        /// <returns>The usable length on success, -1 on error</returns>
        public long Length
        {
            get
            {
                return _dataSourceType switch
                {
                    DataSourceType.ByteArray => _byteArrayData!.Length - _initialPosition,
                    DataSourceType.Stream => _streamData!.Length - _initialPosition,

                    // Everything else is invalid
                    _ => -1,
                };
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Source of the original data
        /// </summary>
        private readonly DataSourceType _dataSourceType = DataSourceType.UNKNOWN;

        /// <summary>
        /// Lock object for reading from the source
        /// </summary>
        private readonly object _streamDataLock = new();

        /// <summary>
        /// Initial position of the data source
        /// </summary>
        /// <remarks>Populated for both <see cref="DataSourceType.ByteArray"/> and <see cref="DataSourceType.Stream"/></remarks>
        protected long _initialPosition = 0;

        /// <summary>
        /// Source byte array data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSourceType"/> is <see cref="DataSourceType.ByteArray"/></remarks>
        protected byte[]? _byteArrayData = null;

        /// <summary>
        /// Source Stream data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSourceType"/> is <see cref="DataSourceType.Stream"/></remarks>
        protected Stream? _streamData = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new DataSource from a Stream
        /// </summary>
        /// <param name="data"></param>
        public DataSource(Stream data)
        {
            _dataSourceType = DataSourceType.Stream;
            _initialPosition = data.Position;
            _streamData = data;
        }

        /// <summary>
        /// Construct a new DataSource from a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public DataSource(byte[] data, int offset)
        {
            _dataSourceType = DataSourceType.ByteArray;
            _initialPosition = offset;
            _byteArrayData = data;
        }

        #endregion

        #region Data

        /// <summary>
        /// Return the underlying data as a stream
        /// </summary>
        /// <returns>Stream representing the data source on success, null on error</returns>
        public Stream? AsStream()
        {
            return _dataSourceType switch
            {
                DataSourceType.ByteArray => new MemoryStream(_byteArrayData!, (int)_initialPosition, (int)Length),
                DataSourceType.Stream => _streamData, // TODO: This should be wrapped better
                _ => null,
            };
        }

        /// <summary>
        /// Read data from the source
        /// </summary>
        /// <param name="position">Position in the source to read from</param>
        /// <param name="length">Length of the requested data</param>
        /// <returns>Byte array containing the requested data, null on error</returns>
        public byte[]? Read(int position, int length)
        {
            // Validate the data source
            if (!IsValid())
                return null;

            // Validate the requested segment
            if (!SegmentValid(position, length))
                return null;

            try
            {
                // Read and return the data
                byte[]? sectionData = null;
                switch (_dataSourceType)
                {
                    case DataSourceType.ByteArray:
                        sectionData = new byte[length];
                        Array.Copy(_byteArrayData!, _initialPosition + position, sectionData, 0, length);
                        break;

                    case DataSourceType.Stream:
                        lock (_streamDataLock)
                        {
                            long currentLocation = _streamData!.Position;
                            _streamData.Seek(_initialPosition + position, SeekOrigin.Begin);
                            sectionData = _streamData.ReadBytes(length);
                            _streamData.Seek(currentLocation, SeekOrigin.Begin);
                            break;
                        }
                }

                return sectionData;

            }
            catch
            {
                // Absorb the error
                return null;
            }
        }

        /// <summary>
        /// Validate the backing data source
        /// </summary>
        /// <returns>True if the data source is valid, false otherwise</returns>
        private bool IsValid()
        {
            return _dataSourceType switch
            {
                // Byte array data requires both a valid array and offset
                DataSourceType.ByteArray => _byteArrayData != null && _initialPosition >= 0,

                // Stream data requires both a valid stream
                DataSourceType.Stream => _streamData != null && _initialPosition >= 0 && _streamData.CanRead && _streamData.CanSeek,

                // Everything else is invalid
                _ => false,
            };
        }

        /// <summary>
        /// Check if a data segment is valid in the data source 
        /// </summary>
        /// <param name="position">Position in the source</param>
        /// <param name="length">Length of the data to check</param>
        /// <returns>True if the positional data is valid, false otherwise</returns>
        private bool SegmentValid(int position, int length)
        {
            // Validate the data souece
            if (!IsValid())
                return false;

            // If we have an invalid position
            if (position < 0 || position >= Length)
                return false;

            return _dataSourceType switch
            {
                DataSourceType.ByteArray => _initialPosition + position + length <= _byteArrayData!.Length,
                DataSourceType.Stream => _initialPosition + position + length <= _streamData!.Length,

                // Everything else is invalid
                _ => false,
            };
        }

        #endregion
    }
}