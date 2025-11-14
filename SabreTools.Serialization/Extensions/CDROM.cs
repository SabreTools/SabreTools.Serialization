using System;
using System.IO;
using SabreTools.Data.Models.CDROM;
using SabreTools.IO.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class CDROM
    {
        /// <summary>
        /// Get the sector mode for a CD-ROM stream
        /// </summary>
        /// <param name="stream">Stream to derive the sector mode from</param>
        /// <returns>Sector mode from the stream on success, <see cref="SectorMode.UNKNOWN"/> on error</returns>
        public static SectorMode GetSectorMode(this Stream stream)
        {
            try
            {
                byte modeByte = stream.ReadByteValue();
                if (modeByte == 0)
                {
                    return SectorMode.MODE0;
                }
                else if (modeByte == 1)
                {
                    return SectorMode.MODE1;
                }
                else if (modeByte == 2)
                {
                    stream.SeekIfPossible(2, SeekOrigin.Current);
                    byte submode = stream.ReadByteValue();
                    if ((submode & 0x20) == 0x20)
                        return SectorMode.MODE2_FORM2;
                    else
                        return SectorMode.MODE2_FORM1;
                }
                else
                {
                    return SectorMode.UNKNOWN;
                }
            }
            catch
            {
                // Ignore the actual error
                return SectorMode.UNKNOWN;
            }

        }

        /// <summary>
        /// Get the user data size for a sector mode
        /// </summary>
        /// <param name="mode">Sector mode to get a value for</param>
        /// <returns>User data size, if possible</returns>
        public static long GetUserDataSize(this SectorMode mode)
        {
            return mode switch
            {
                SectorMode.MODE0 => Constants.Mode0DataSize,
                SectorMode.MODE1 => Constants.Mode1DataSize,
                SectorMode.MODE2 => Constants.Mode0DataSize,
                SectorMode.MODE2_FORM1 => Constants.Mode2Form1DataSize,
                SectorMode.MODE2_FORM2 => Constants.Mode2Form2DataSize,
                _ => Constants.Mode0DataSize,
            };
        }

        /// <summary>
        /// Get the user data end offset for a sector mode
        /// </summary>
        /// <param name="mode">Sector mode to get a value for</param>
        /// <returns>User data end offset, if possible</returns>
        public static long GetUserDataEnd(this SectorMode mode)
        {
            return mode switch
            {
                SectorMode.MODE0 => Constants.Mode0UserDataEnd, // TODO: Support flexible sector length (2352)
                SectorMode.MODE1 => Constants.Mode1UserDataEnd,
                SectorMode.MODE2 => Constants.Mode0UserDataEnd, // TODO: Support flexible sector length (2352)
                SectorMode.MODE2_FORM1 => Constants.Mode2Form1UserDataEnd,
                SectorMode.MODE2_FORM2 => Constants.Mode2Form2UserDataEnd, // TODO: Support flexible sector length (2348)
                _ => Constants.Mode0UserDataEnd,
            };
        }

        /// <summary>
        /// Get the user data start offset for a sector mode
        /// </summary>
        /// <param name="mode">Sector mode to get a value for</param>
        /// <returns>User data start offset, if possible</returns>
        public static long GetUserDataStart(this SectorMode mode)
        {
            return mode switch
            {
                SectorMode.MODE0 => Constants.Mode0UserDataStart,
                SectorMode.MODE1 => Constants.Mode1UserDataStart,
                SectorMode.MODE2 => Constants.Mode0UserDataStart,
                SectorMode.MODE2_FORM1 => Constants.Mode2Form1UserDataStart,
                SectorMode.MODE2_FORM2 => Constants.Mode2Form2UserDataStart,
                _ => Constants.Mode0UserDataStart,
            };
        }

        /// <summary>
        /// Creates a stream that provides only the user data of a CDROM stream
        /// </summary>
        public class ISO9660Stream : Stream
        {
            // Base CDROM stream (2352-byte sector)
            private readonly Stream _baseStream;

            // State variables
            private long _position = 0;
            private SectorMode _currentMode = SectorMode.UNKNOWN;
            private long _userDataStart = Constants.Mode1UserDataStart;
            private long _userDataEnd = Constants.Mode1UserDataEnd;
            private long _isoSectorSize = Constants.Mode1DataSize;

            public ISO9660Stream(Stream inputStream)
            {
                if (!inputStream.CanSeek || !inputStream.CanRead)
                    throw new ArgumentException("Stream must be readable and seekable.", nameof(inputStream));

                _baseStream = inputStream;
            }

            /// <inheritdoc/>
            public override bool CanRead => _baseStream.CanRead;

            /// <inheritdoc/>
            public override bool CanSeek => _baseStream.CanSeek;

            /// <inheritdoc/>
            public override bool CanWrite => false;

            /// <inheritdoc/>
            public override void Flush() => _baseStream.Flush();

            /// <inheritdoc/>
            public override long Length
                => (_baseStream.Length / Constants.CDROMSectorSize) * _isoSectorSize;

            /// <inheritdoc/>
            public override void SetLength(long value)
                => throw new NotSupportedException("Setting the length of this stream is not supported.");

            /// <inheritdoc/>
            public override void Write(byte[] buffer, int offset, int count)
                => throw new NotSupportedException("Writing to this stream is not supported.");

            /// <inheritdoc/>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _baseStream.Dispose();

                base.Dispose(disposing);
            }

            /// <inheritdoc/>
            public override long Position
            {
                // Get the position of the underlying ISO9660 stream
                get
                {
                    // Get the user data location based on the current sector mode
                    SetState(_position);

                    // Get the number of ISO sectors before current position
                    long isoPosition = (_position / Constants.CDROMSectorSize) * _isoSectorSize;

                    // Add the within-sector position
                    long remainder = _position % Constants.CDROMSectorSize;
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

            /// <inheritdoc/>
            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalRead = 0;
                int remaining = count;

                while (remaining > 0 && _position < _baseStream.Length)
                {
                    // Determine location of current sector
                    long baseStreamOffset = _position - (_position % Constants.CDROMSectorSize);

                    // Set the current sector's mode and user data location
                    SetState(baseStreamOffset);

                    // Deal with case where base position is not in ISO stream
                    long remainder = _position % Constants.CDROMSectorSize;
                    long sectorOffset = remainder - _userDataStart;
                    if (remainder < _userDataStart)
                    {
                        baseStreamOffset += _userDataStart;
                        sectorOffset = 0;
                        _position += _userDataStart;
                    }
                    else if (remainder >= _userDataEnd)
                    {
                        baseStreamOffset += Constants.CDROMSectorSize;
                        sectorOffset = 0;
                        _position += Constants.CDROMSectorSize - _userDataEnd + _userDataStart;
                    }
                    else
                    {
                        baseStreamOffset += remainder;
                    }

                    // Sanity check on read location before seeking
                    if (baseStreamOffset < 0 || baseStreamOffset > _baseStream.Length)
                        throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to seek outside the stream boundaries.");

                    // Seek to target position in base CDROM stream
                    _baseStream.SeekIfPossible(baseStreamOffset, SeekOrigin.Begin);

                    // Read the remaining bytes, up to max of one ISO sector (2048 bytes)
                    int bytesToRead = (int)Math.Min(remaining, _isoSectorSize - sectorOffset);

                    // Don't overshoot end of stream
                    bytesToRead = (int)Math.Min(bytesToRead, _baseStream.Length - _position);

                    // Finish reading if no more bytes to be read
                    if (bytesToRead <= 0)
                        break;

                    // Read up to 2048 bytes from base CDROM stream
                    int bytesRead = _baseStream.Read(buffer, offset + totalRead, bytesToRead);

                    // Update state for base stream
                    _position = _baseStream.Position;
                    if (bytesToRead == (_isoSectorSize - sectorOffset))
                        _position += (Constants.CDROMSectorSize - _userDataEnd) + _userDataStart;

                    // Update state for ISO stream
                    totalRead += bytesRead;
                    remaining -= bytesRead;

                    if (bytesRead == 0)
                        break;
                }

                return totalRead;
            }

            /// <inheritdoc/>
            public override long Seek(long offset, SeekOrigin origin)
            {
                // Get the intended position for the ISO9660 stream
                long targetPosition = origin switch
                {
                    SeekOrigin.Begin => offset,
                    SeekOrigin.Current => Position + offset,
                    SeekOrigin.End => Length + offset,
                    _ => throw new ArgumentException("Invalid SeekOrigin.", nameof(origin)),
                };

                // Get the number of ISO sectors before current position
                long newPosition = (targetPosition / _isoSectorSize) * Constants.CDROMSectorSize;

                // Set the current sector's mode and user data location
                SetState(newPosition);

                // Add the within-sector position
                newPosition += _userDataStart + (targetPosition % _isoSectorSize);
                if (newPosition < 0 || newPosition > _baseStream.Length)
                    throw new ArgumentOutOfRangeException(nameof(offset), "Attempted to seek outside the stream boundaries.");

                _position = _baseStream.SeekIfPossible(newPosition, SeekOrigin.Begin);
                return Position;
            }

            /// <summary>
            /// Update the current stream state based on the location
            /// </summary>
            /// <param name="sectorLocation">Sector location to update from</param>
            private void SetState(long sectorLocation)
            {
                long current = _baseStream.Position;
                long modePosition = sectorLocation - (sectorLocation % Constants.CDROMSectorSize) + 15;

                // Get the current sector mode
                _baseStream.SeekIfPossible(modePosition, SeekOrigin.Begin);
                _currentMode = _baseStream.GetSectorMode();

                // Set the user data location variables
                _userDataStart = _currentMode.GetUserDataStart();
                _userDataEnd = _currentMode.GetUserDataEnd();
                // _isoSectorSize = _currentMode.GetUserDataSize();

                // Reset the stream position
                _baseStream.SeekIfPossible(current, SeekOrigin.Begin);
            }
        }
    }
}
