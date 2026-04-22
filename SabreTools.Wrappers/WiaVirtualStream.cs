using System;
using System.IO;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// A read-only seekable stream that decompresses WIA/RVZ groups on demand.
    /// Avoids loading the entire decompressed disc image into memory.
    /// </summary>
    internal sealed class WiaVirtualStream : Stream
    {
        private readonly WIA _wia;
        private long _position;

        public WiaVirtualStream(WIA wia)
        {
            _wia = wia ?? throw new ArgumentNullException(nameof(wia));
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => (long)_wia.IsoFileSize;
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
            int read = _wia.ReadVirtual(_position, buffer, offset, count);
            _position += read;
            return read;
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
