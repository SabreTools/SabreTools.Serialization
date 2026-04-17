using System;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Decompressor for RVZ packed format.
    /// RVZ uses run-length encoding to store real data and junk data efficiently:
    /// - Real data: size (4 bytes) + data bytes
    /// - Junk data: size with high bit set (4 bytes) + 68-byte seed → regenerate using LFG
    /// </summary>
    internal class RvzPackDecompressor
    {
        private readonly byte[] m_packed_data;
        private readonly uint m_rvz_packed_size;
        private long m_data_offset;
        private readonly LaggedFibonacciGenerator m_lfg;

        private int m_in_position = 0;
        private int m_out_position = 0;
        private uint m_current_size = 0;
        private bool m_current_is_junk = false;

        /// <summary>
        /// Creates a new RVZ pack decompressor.
        /// </summary>
        /// <param name="packedData">The packed RVZ data</param>
        /// <param name="rvzPackedSize">Expected size of packed data (for validation)</param>
        /// <param name="dataOffset">Offset in the virtual disc (for LFG alignment)</param>
        public RvzPackDecompressor(byte[] packedData, uint rvzPackedSize, long dataOffset)
        {
            m_packed_data    = packedData ?? throw new ArgumentNullException(nameof(packedData));
            m_rvz_packed_size = rvzPackedSize;
            m_data_offset    = dataOffset;
            m_lfg            = new LaggedFibonacciGenerator();
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
                if (m_current_size == 0)
                {
                    if (!ReadNextSegment())
                        break;
                }

                int bytesToWrite = Math.Min((int)m_current_size, count - totalWritten);

                if (m_current_is_junk)
                {
                    m_lfg.GetBytes(bytesToWrite, output, outputOffset + totalWritten);
                }
                else
                {
                    Array.Copy(m_packed_data, m_in_position, output, outputOffset + totalWritten, bytesToWrite);
                    m_in_position += bytesToWrite;
                }

                m_current_size   -= (uint)bytesToWrite;
                m_out_position   += bytesToWrite;
                totalWritten     += bytesToWrite;
                m_data_offset    += bytesToWrite;
            }

            return totalWritten;
        }

        /// <summary>
        /// Checks if decompression is complete.
        /// </summary>
        public bool IsDone() => m_current_size == 0 && m_in_position >= m_rvz_packed_size;

        private bool ReadNextSegment()
        {
            if (m_in_position + 4 > m_packed_data.Length)
                return false;

            // Size field is big-endian u32; high bit signals junk data
            uint sizeField = (uint)((m_packed_data[m_in_position]     << 24) |
                                    (m_packed_data[m_in_position + 1] << 16) |
                                    (m_packed_data[m_in_position + 2] <<  8) |
                                     m_packed_data[m_in_position + 3]);
            m_in_position += 4;

            m_current_is_junk = (sizeField & 0x80000000) != 0;
            m_current_size    = sizeField & 0x7FFFFFFF;

            if (m_current_is_junk)
            {
                if (m_in_position + LaggedFibonacciGenerator.SEED_SIZE * 4 > m_packed_data.Length)
                    return false;

                byte[] seed = new byte[LaggedFibonacciGenerator.SEED_SIZE * 4];
                Array.Copy(m_packed_data, m_in_position, seed, 0, seed.Length);
                m_in_position += seed.Length;

                m_lfg.SetSeed(seed);

                // Advance LFG to the correct position within the 32 KiB disc block
                const int BLOCK_SIZE = 0x8000;
                int offsetInBlock = (int)(m_data_offset % BLOCK_SIZE);
                if (offsetInBlock > 0)
                    m_lfg.Forward(offsetInBlock);
            }

            return true;
        }
    }
}
