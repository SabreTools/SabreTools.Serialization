using System;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Represents the data source backing the wrapper
    /// </summary>
    public class ViewStream : Stream
    {
        #region Properties

        /// <summary>
        /// Filename from the source, if possible
        /// </summary>
        public string? Filename
        {
            get
            {
                // Only file streams can have a filename
                if (_streamData is not FileStream fs)
                    return null;

                // Return the name
                return fs.Name;
            }
        }

        /// <inheritdoc/>
        public override long Length => _length;

        /// <inheritdoc/>
        public override long Position
        {
            get { return _streamData.Position - _initialPosition; }
            set { _streamData.Position = value + _initialPosition; }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Lock object for reading from the source
        /// </summary>
        private readonly object _streamDataLock = new();

        /// <summary>
        /// Initial position of the data source
        /// </summary>
        protected long _initialPosition;

        /// <summary>
        /// Usable length of the underlying data
        /// </summary>
        protected long _length;

        /// <summary>
        /// Source Stream data
        /// </summary>
        protected Stream _streamData;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new ViewStream from a Stream
        /// </summary>
        public ViewStream(Stream data, long offset, long length)
        {
            _streamData = data;
            _initialPosition = offset;
            _length = length;
        }

        /// <summary>
        /// Construct a new ViewStream from a byte array
        /// </summary>
        public ViewStream(byte[] data, long offset, long length)
        {
            _streamData = new MemoryStream(data, (int)offset, (int)length);
            _initialPosition = 0;
            _length = length;
        }

        #endregion

        #region Data

        /// <summary>
        /// Check if a data segment is valid in the data source 
        /// </summary>
        /// <param name="offset">Position in the source</param>
        /// <param name="count">Length of the data to check</param>
        /// <returns>True if the positional data is valid, false otherwise</returns>
        public bool SegmentValid(int offset, int count)
        {
            // If we have an invalid position
            if (offset < 0 || offset >= Length)
                return false;

            return _initialPosition + offset + count <= Length;
        }

        #endregion

        #region Stream Implementations

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override bool CanSeek => _streamData.CanSeek;

        /// <inheritdoc/>
        public override void Flush() => _streamData.Flush();

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Validate the requested segment
            if (!SegmentValid(offset, count))
                return 0;

            try
            {
                // Correct the read offset
                offset += (int)_initialPosition;
                lock (_streamDataLock)
                {
                    return _streamData.Read(buffer, offset, count);
                }

            }
            catch
            {
                // Absorb the error
                return 0;
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            // Handle the "seek"
            switch (origin)
            {
                case SeekOrigin.Begin: Position = offset; break;
                case SeekOrigin.Current: Position += offset; break;
                case SeekOrigin.End: Position = _length + offset - 1; break;
                default: throw new ArgumentException($"Invalid value for {nameof(origin)}");
            }
            ;

            // Handle out-of-bounds seeks
            if (Position < 0)
                Position = 0;
            else if (Position >= _length)
                Position = _length - 1;

            return Position;
        }

         /// <inheritdoc/>
        public override void SetLength(long value)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotImplementedException();

        #endregion
    }
}