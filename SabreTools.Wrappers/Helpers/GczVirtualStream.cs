using System;
using System.IO;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// A read-only seekable stream that decompresses GCZ blocks on demand.
    /// Avoids loading the entire decompressed disc image into memory.
    /// </summary>
    public sealed class GczVirtualStream : Stream
    {
        /// <summary>
        /// GCZ wrapper used for reading
        /// </summary>
        private readonly GCZ _gcz;

        /// <summary>
        /// Virtual position within the stream
        /// </summary>
        private long _position;

        // Single-block cache to avoid re-decompressing on adjacent reads within the same block.
        private int _cachedBlockIndex = -1;
        private byte[]? _cachedBlock;

        public GczVirtualStream(GCZ gcz)
        {
            _gcz = gcz ?? throw new ArgumentNullException(nameof(gcz));
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => (long)_gcz.DataSize;

        /// <inheritdoc/>
        public override long Position
        {
            get => _position;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _position = value;
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentException("offset + count exceeds buffer length");

            long remaining = Length - _position;
            if (remaining <= 0 || count <= 0)
                return 0;

            count = (int)Math.Min(count, remaining);

            int totalRead = 0;
            uint blockSize = _gcz.BlockSize;

            while (totalRead < count && _position < Length)
            {
                int blockIndex = (int)(_position / blockSize);
                int offsetInBlock = (int)(_position % blockSize);

                byte[]? block = GetBlock(blockIndex);
                if (block is null)
                    break;

                int available = block.Length - offsetInBlock;
                int toCopy = Math.Min(count - totalRead, available);
                if (toCopy <= 0)
                    break;

                Array.Copy(block, offsetInBlock, buffer, offset + totalRead, toCopy);
                totalRead += toCopy;
                _position += toCopy;
            }

            return totalRead;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos;
            switch (origin)
            {
                case SeekOrigin.Begin: newPos = offset; break;
                case SeekOrigin.Current: newPos = _position + offset; break;
                case SeekOrigin.End: newPos = Length + offset; break;
                default: throw new ArgumentOutOfRangeException(nameof(origin));
            }

            if (newPos < 0)
                throw new IOException("Seek position cannot be negative.");

            _position = newPos;
            return _position;
        }

        /// <inheritdoc/>
        public override void Flush() { }

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>
        /// Decompress and retrieve a possibly-cached data block
        /// </summary>
        /// <param name="blockIndex">Block index to retrieve</param>
        /// <returns>Decompressed block data on success, null otherwise</returns>
        private byte[]? GetBlock(int blockIndex)
        {
            if (_cachedBlockIndex == blockIndex)
                return _cachedBlock;

            byte[]? block = _gcz.DecompressBlock(blockIndex);
            _cachedBlockIndex = blockIndex;
            _cachedBlock = block;
            return block;
        }
    }
}
