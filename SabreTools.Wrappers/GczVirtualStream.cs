using System;
using System.IO;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// A read-only seekable stream that decompresses GCZ blocks on demand.
    /// Avoids loading the entire decompressed disc image into memory.
    /// </summary>
    internal sealed class GczVirtualStream : Stream
    {
        private readonly GCZ _gcz;
        private long _position;

        // Single-block cache to avoid re-decompressing on adjacent reads within the same block.
        private int _cachedBlockIndex = -1;
        private byte[]? _cachedBlock;

        public GczVirtualStream(GCZ gcz)
        {
            _gcz = gcz ?? throw new ArgumentNullException(nameof(gcz));
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => (long)_gcz.DataSize;
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

        private byte[]? GetBlock(int blockIndex)
        {
            if (_cachedBlockIndex == blockIndex)
                return _cachedBlock;

            byte[]? block = _gcz.DecompressBlock(blockIndex);
            _cachedBlockIndex = blockIndex;
            _cachedBlock = block;
            return block;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos;
            switch (origin)
            {
                case SeekOrigin.Begin:   newPos = offset; break;
                case SeekOrigin.Current: newPos = _position + offset; break;
                case SeekOrigin.End:     newPos = Length + offset; break;
                default: throw new ArgumentOutOfRangeException(nameof(origin));
            }

            if (newPos < 0)
                throw new IOException("Seek position cannot be negative.");

            _position = newPos;
            return _position;
        }

        public override void Flush() { }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
