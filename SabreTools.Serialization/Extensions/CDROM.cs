using System;
using System.IO;
using SabreTools.Data.Models.CDROM;
using SabreTools.IO;
using SabreTools.IO.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class CDROM
    {
        /// <summary>
        /// Creates a stream that provides only the user data of a CDROM stream
        /// </summary>
        public class ISO9660Stream : Stream
        {
            // Constant variables
            private readonly Stream _baseStream;
            private const long _baseSectorSize = 2352;
            private long _isoSectorSize = 2048;
            // TODO: Support flexible sector size (MODE2_FORM2)

            // State variables
            private long _position = 0;
            private SectorMode _currentMode = SectorMode.UNKNOWN;
            private long _userDataStart = 16;
            private long _userDataEnd = 2064;

            public ISO9660Stream(Stream inputStream)
            {
                if (inputStream == null)
                    throw new ArgumentNullException("Stream cannot be null.", nameof(inputStream));
                else if (!inputStream.CanSeek || !inputStream.CanRead)
                    throw new ArgumentException("Stream must be readable and seekable.", nameof(inputStream));
                _baseStream = inputStream;
            }

            public override bool CanRead => true;
            public override bool CanSeek => true;
            public override bool CanWrite => false;

            public override void Flush()
            {
                _baseStream.Flush();
            }

            public override long Length
            {
                get
                {
                    return (_baseStream.Length / _baseSectorSize) * _isoSectorSize;
                }
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException("Setting the length of this stream is not supported.");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException("Writing to this stream is not supported.");
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _baseStream.Dispose();
                }
                base.Dispose(disposing);
            }

            public override long Position
            {
                // Get the position of the underlying ISO9660 stream
                get
                {
                    // Get the user data location based on the current sector mode
                    SetState(_position);

                    // Get the number of ISO sectors before current position
                    long isoPosition = (_position / _baseSectorSize) * _isoSectorSize;

                    // Add the within-sector position
                    long remainder = _position % _baseSectorSize;
                    if (remainder > _userDataEnd)
                        isoPosition += _isoSectorSize;
                    else if (remainder > _userDataStart)
                        isoPosition += remainder - _userDataStart;

                    return isoPosition;
                }
                set
                {
                    // Seek to the underlying ISO9660 position
                    Seek(value, SeekOrigin.Begin);
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                bool readEntireSector = false;
                int totalRead = 0;
                int remaining = count;

                while (remaining > 0 && _position < Length)
                {
                    // Determine location of current sector
                    long baseStreamOffset = (_position / _baseSectorSize) * _baseSectorSize;

                    // Set the current sector's mode and user data location
                    SetState(baseStreamOffset);

                    // Deal with case where base position is not in ISO stream
                    long remainder = _position % _baseSectorSize;
                    long sectorOffset = remainder - _userDataStart;
                    if (remainder < _userDataStart)
                    {
                        baseStreamOffset += _userDataStart;
                        sectorOffset = 0;
                        _position += _userDataStart;
                    }
                    else if (remainder >= _userDataEnd)
                    {
                        baseStreamOffset += _baseSectorSize;
                        sectorOffset = 0;
                        _position += _baseSectorSize - _userDataEnd + _userDataStart;
                    }
                    else
                        baseStreamOffset += remainder;

                    // Sanity check on read location before seeking
                    if (baseStreamOffset < 0 || baseStreamOffset > _baseStream.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to seek outside the stream boundaries.");
                    }

                    // Seek to target position in base CDROM stream
                    _baseStream.Seek(baseStreamOffset, SeekOrigin.Begin);

                    // Read the remaining bytes, up to max of one ISO sector (2048 bytes)
                    int bytesToRead = (int)Math.Min(remaining, _isoSectorSize - sectorOffset);

                    // Don't overshoot end of stream
                    bytesToRead = (int)Math.Min(bytesToRead, Length - _position);

                    if (bytesToRead == (_isoSectorSize - sectorOffset))
                        readEntireSector = true;
                    else
                        readEntireSector = false;

                    // Finish reading if no more bytes to be read
                    if (bytesToRead <= 0)
                        break;

                    // Read up to 2048 bytes from base CDROM stream
                    int bytesRead = _baseStream.Read(buffer, offset + totalRead, bytesToRead);

                    // Update state for base stream
                    _position = _baseStream.Position;
                    if (readEntireSector)
                        _position += (_baseSectorSize - _userDataEnd) + _userDataStart;

                    // Update state for ISO stream
                    totalRead += bytesRead;
                    remaining -= bytesRead;

                    if (bytesRead == 0)
                        break;
                }

                return totalRead;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                // Get the intended position for the ISO9660 stream
                long targetPosition;
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        targetPosition = offset;
                        break;
                    case SeekOrigin.Current:
                        targetPosition = Position + offset;
                        break;
                    case SeekOrigin.End:
                        targetPosition = Length + offset;
                        break;
                    default:
                        throw new ArgumentException("Invalid SeekOrigin.", nameof(origin));
                }

                // Get the number of ISO sectors before current position
                long newPosition = (targetPosition / _isoSectorSize) * _baseSectorSize;

                // Set the current sector's mode and user data location
                SetState(newPosition);

                // Add the within-sector position
                newPosition += _userDataStart + (targetPosition % _isoSectorSize);

                if (newPosition < 0 || newPosition > _baseStream.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to seek outside the stream boundaries.");
                }

                _position = _baseStream.Seek(newPosition, SeekOrigin.Begin);
                return Position;
            }

            private void SetState(long sectorLocation)
            {
                long modePosition = (sectorLocation - sectorLocation % _baseSectorSize) + 15;
                _baseStream.Seek(modePosition, SeekOrigin.Begin);
                byte modeByte = _baseStream.ReadByteValue();
                if (modeByte == 0)
                    _currentMode = SectorMode.MODE0;
                else if (modeByte == 1)
                    _currentMode = SectorMode.MODE1;
                else if (modeByte == 2)
                {
                    _baseStream.Seek(modePosition + 3, SeekOrigin.Begin);
                    byte submode = _baseStream.ReadByteValue();
                    if ((submode & 0x20) == 0x20)
                        _currentMode = SectorMode.MODE2_FORM2;
                    else
                        _currentMode = SectorMode.MODE2_FORM1;
                }
                else
                    _currentMode = SectorMode.UNKNOWN;

                // Set the user data location variables
                switch (_currentMode)
                {
                    case SectorMode.MODE1:
                        _userDataStart = 16;
                        _userDataEnd = 2064;
                        //_isoSectorSize = 2048;
                        return;

                    case SectorMode.MODE2_FORM1:
                        _userDataStart = 24;
                        _userDataEnd = 2072;
                        //_isoSectorSize = 2048;
                        return;

                    case SectorMode.MODE2_FORM2:
                        _userDataStart = 24;
                        _userDataEnd = 2072;
                        // TODO: Support flexible sector length
                        //_userDataEnd = 2348;
                        //_isoSectorSize = 2324;
                        return;

                    case SectorMode.MODE0:
                    case SectorMode.MODE2:
                        _userDataStart = 16;
                        _userDataEnd = 2064;
                        // TODO: Support flexible sector length
                        //_userDataEnd = 2352;
                        //_isoSectorSize = 2336;
                        return;

                    case SectorMode.UNKNOWN:
                        _userDataStart = 16;
                        _userDataEnd = 2064;
                        //_isoSectorSize = 2048;
                        return;
                }

                return;
            }
        }
    }
}
