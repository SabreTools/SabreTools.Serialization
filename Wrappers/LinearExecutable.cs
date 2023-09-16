﻿using System;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class LinearExecutable : WrapperBase<Models.LinearExecutable.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Linear Executable (LE/LX)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public LinearExecutable(Models.LinearExecutable.Executable model, byte[] data, int offset)
#else
        public LinearExecutable(Models.LinearExecutable.Executable? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public LinearExecutable(Models.LinearExecutable.Executable model, Stream data)
#else
        public LinearExecutable(Models.LinearExecutable.Executable? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }/// <summary>
         /// Create an LE/LX executable from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the executable</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
#if NET48
        public static LinearExecutable Create(byte[] data, int offset)
#else
        public static LinearExecutable? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an LE/LX executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
#if NET48
        public static LinearExecutable Create(Stream data)
#else
        public static LinearExecutable? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = new Streams.LinearExecutable().Deserialize(data);
            if (executable == null)
                return null;

            try
            {
                return new LinearExecutable(executable, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region REMOVE -- DO NOT USE

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns>Byte array representing the range, null on error</returns>
        [Obsolete]
#if NET48
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
#else
        public byte[]? ReadArbitraryRange(int rangeStart = -1, int length = -1)
#endif
        {
            // If we have an unset range start, read from the start of the source
            if (rangeStart == -1)
                rangeStart = 0;

            // If we have an unset length, read the whole source
            if (length == -1)
            {
                switch (_dataSource)
                {
                    case DataSource.ByteArray:
#if NET48
                        length = _byteArrayData.Length - _byteArrayOffset;
#else
                        length = _byteArrayData!.Length - _byteArrayOffset;
#endif
                        break;

                    case DataSource.Stream:
#if NET48
                        length = (int)_streamData.Length;
#else
                        length = (int)_streamData!.Length;
#endif
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}