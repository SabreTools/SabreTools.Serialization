using System;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class NewExecutable : WrapperBase<Models.NewExecutable.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "New Executable (NE)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NewExecutable(Models.NewExecutable.Executable? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public NewExecutable(Models.NewExecutable.Executable? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an NE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an NE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var executable = Deserializers.NewExecutable.DeserializeStream(data);
                if (executable == null)
                    return null;

                return new NewExecutable(executable, data);
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
        public byte[]? ReadArbitraryRange(int rangeStart = -1, int length = -1)
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
                        length = _byteArrayData!.Length - _byteArrayOffset;
                        break;

                    case DataSource.Stream:
                        length = (int)_streamData!.Length;
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}
