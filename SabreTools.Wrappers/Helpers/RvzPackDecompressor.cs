using System;
using SabreTools.Numerics.Extensions;

// TODO: Remove when IO is updated
namespace SabreTools.Wrappers
{
    /// <summary>
    /// Decompressor for RVZ packed format.
    /// RVZ uses run-length encoding to store real data and junk data efficiently:
    /// - Real data: size (4 bytes) + data bytes
    /// - Junk data: size with high bit set (4 bytes) + 68-byte seed -> regenerate using LFG
    /// </summary>
    internal class RvzPackDecompressor
    {
        /// <summary>
        ///
        /// </summary>
        private readonly byte[] _packedData;

        /// <summary>
        ///
        /// </summary>
        private readonly uint _rvzPackedSize;

        /// <summary>
        ///
        /// </summary>
        private long _dataOffset;

        /// <summary>
        ///
        /// </summary>
        private readonly LaggedFibonacciGenerator _lfg;

        /// <summary>
        ///
        /// </summary>
        private int _inPosition = 0;

        /// <summary>
        ///
        /// </summary>
        private uint _currentSize = 0;

        /// <summary>
        ///
        /// </summary>
        private bool _currentIsJunk = false;

        /// <summary>
        /// Creates a new RVZ pack decompressor.
        /// </summary>
        /// <param name="packedData">The packed RVZ data</param>
        /// <param name="rvzPackedSize">Expected size of packed data (for validation)</param>
        /// <param name="dataOffset">Offset in the virtual disc (for LFG alignment)</param>
        public RvzPackDecompressor(byte[] packedData, uint rvzPackedSize, long dataOffset)
        {
            _packedData = packedData;
            _rvzPackedSize = rvzPackedSize;
            _dataOffset = dataOffset;
            _lfg = new LaggedFibonacciGenerator();
        }

        /// <summary>
        /// Decompresses the packed data into the output buffer.
        /// </summary>
        /// <param name="output">Destination buffer</param>
        /// <param name="outputOffset">Offset in destination buffer</param>
        /// <param name="count">Number of bytes to decompress</param>
        /// <returns>Number of bytes actually decompressed</returns>
        public int Decompress(byte[] output, int outputOffset, int count)
        {
            int totalWritten = 0;

            while (totalWritten < count && !IsDone())
            {
                if (_currentSize == 0)
                {
                    if (!ReadNextSegment())
                        break;
                }

                int bytesToWrite = Math.Min((int)_currentSize, count - totalWritten);
                if (_currentIsJunk)
                {
                    _lfg.GetBytes(bytesToWrite, output, outputOffset + totalWritten);
                }
                else
                {
                    Array.Copy(_packedData, _inPosition, output, outputOffset + totalWritten, bytesToWrite);
                    _inPosition += bytesToWrite;
                }

                _currentSize -= (uint)bytesToWrite;
                totalWritten += bytesToWrite;
                _dataOffset += bytesToWrite;
            }

            return totalWritten;
        }

        /// <summary>
        /// Checks if decompression is complete.
        /// </summary>
        public bool IsDone() => _currentSize == 0 && _inPosition >= _rvzPackedSize;

        /// <summary>
        /// Read the next segment of packed data
        /// </summary>
        /// <returns>True if the segment was read and cached, false otherwise</returns>
        private bool ReadNextSegment()
        {
            if (_inPosition + 4 > _packedData.Length)
                return false;

            // Size field is big-endian u32; high bit signals junk data
            uint sizeField = _packedData.ReadUInt32BigEndian(ref _inPosition);

            _currentIsJunk = (sizeField & 0x80000000) != 0;
            _currentSize = sizeField & 0x7FFFFFFF;

            if (_currentIsJunk)
            {
                if (_inPosition + (LaggedFibonacciGenerator.SEED_SIZE * 4) > _packedData.Length)
                    return false;

                byte[] seed = new byte[LaggedFibonacciGenerator.SEED_SIZE * 4];
                Array.Copy(_packedData, _inPosition, seed, 0, seed.Length);
                _inPosition += seed.Length;

                _lfg.SetSeed(seed);

                // Advance LFG to the correct position within the buffer.
                // Dolphin: lfg.m_position_bytes = data_offset % (LFG_K * sizeof(u32))
                int offsetInBuffer = (int)(_dataOffset % LaggedFibonacciGenerator.BUFFER_BYTES);
                if (offsetInBuffer > 0)
                    _lfg.Forward(offsetInBuffer);
            }

            return true;
        }
    }
}
