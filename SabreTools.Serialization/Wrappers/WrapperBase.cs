using System;
using System.IO;
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

        /// <summary>
        /// Lock for accessing <see cref="_dataSource"/> 
        /// </summary>
        protected readonly object _dataSourceLock = new();

        #endregion

        #region Byte Array Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(byte[] data)
            : this(data, 0, data.Length)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(byte[] data, int offset)
            : this(data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a byte array
        /// </summary>
        protected WrapperBase(byte[] data, int offset, int length)
        {
            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _dataSource = new ViewStream(data, offset, length);
        }

        #endregion

        #region Stream Constructors

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        /// <remarks>Uses the current stream position as the offset</remarks>
        protected WrapperBase(Stream data)
            : this(data, data.Position, data.Length - data.Position)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        protected WrapperBase(Stream data, long offset)
            : this(data, offset, data.Length - offset)
        {
        }

        /// <summary>
        /// Construct a new instance of the wrapper from a Stream
        /// </summary>
        protected WrapperBase(Stream data, long offset, long length)
        {
            if (!data.CanSeek || !data.CanRead)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _dataSource = new ViewStream(data, offset, length);
        }

        #endregion

        #region Data

        /// <summary>
        /// Read a number of bytes from an offset fomr the data source, if possible
        /// </summary>
        /// <param name="offset">Offset within the data source to start reading</param>
        /// <param name="length">Number of bytes to read from the offset</param>
        /// <returns>Filled byte array on success, null on error</returns>
        /// <remarks>
        /// This method locks the data source to avoid potential conflicts in reading
        /// from the data source. This should be the preferred way of reading in cases
        /// where there may be multiple threads accessing the wrapper.
        /// 
        /// This method will return an empty array if the length is greater than what is left
        /// in the stream. This is different behavior than a normal stream read that would
        /// attempt to read as much as possible, returning the amount of bytes read.
        /// </remarks>
        protected byte[] ReadRangeFromSource(long offset, int length)
        {
            lock (_dataSourceLock)
            {
                return _dataSource.ReadFrom(offset, length, retainPosition: true) ?? [];
            }
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
