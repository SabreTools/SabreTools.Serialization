using System;
using SabreTools.Numerics.Extensions;

// TODO: Remove when IO is updated
namespace SabreTools.Wrappers
{
    /// <summary>
    /// Lagged Fibonacci Generator matching Dolphin's LaggedFibonacciGenerator exactly.
    /// Used to regenerate Nintendo's deterministic "junk" padding data in disc images.
    /// RVZ format identifies junk regions and stores only a 68-byte seed (17 uint words)
    /// instead of the full data, enabling significant compression of padding areas.
    /// </summary>
    public class LaggedFibonacciGenerator
    {
        /// <summary>
        ///
        /// </summary>
        private const int LFG_K = 521;

        /// <summary>
        ///
        /// </summary>
        private const int LFG_J = 32;

        /// <summary>
        /// Size of the LFG output buffer in bytes (LFG_K * 4 = 2084)
        /// </summary>
        public const int BUFFER_BYTES = LFG_K * 4;

        /// <summary>
        /// Size of the seed in 32-bit words (68 bytes total)
        /// </summary>
        public const int SEED_SIZE = 17;

        /// <summary>
        ///
        /// </summary>
        private readonly uint[] _buffer = new uint[LFG_K];

        /// <summary>
        ///
        /// </summary>
        private int _position = 0;

        /// <summary>
        /// Initializes the generator from a 17-element uint seed array.
        /// Each seed word is treated as a raw LE uint from the file (Dolphin: reinterpret_cast then swap32).
        /// </summary>
        public void SetSeed(uint[] seed)
        {
            if (seed == null || seed.Length < SEED_SIZE)
                throw new ArgumentException($"Seed must contain at least {SEED_SIZE} uint values.", nameof(seed));

            // Reinterpret LE bytes as BE (Dolphin swap32)
            _position = 0;
            for (int i = 0; i < SEED_SIZE; i++)
            {
                _buffer[i] = SwapU32(seed[i]);
            }

            Initialize(false);
        }

        /// <summary>
        /// Initializes the generator from a 68-byte seed (17 BE uint values as in the RVZ file).
        /// </summary>
        /// <remarks>Matches Dolphin: m_buffer[i] = Common::swap32(seed + i * 4).</remarks>
        public void SetSeed(byte[] seedBytes)
        {
            if (seedBytes == null || seedBytes.Length < SEED_SIZE * 4)
                throw new ArgumentException($"Seed must be {SEED_SIZE * 4} bytes.", nameof(seedBytes));

            _position = 0;
            int offset = 0;
            for (int i = 0; i < SEED_SIZE; i++)
            {
                _buffer[i] = seedBytes.ReadUInt32BigEndian(ref offset);
            }

            Initialize(false);
        }

        /// <summary>
        /// Skips forward by <paramref name="count"/> bytes in the output stream.
        /// </summary>
        /// <remarks>Matches Dolphin: LaggedFibonacciGenerator::Forward(size_t count).</remarks>
        public void Forward(int count)
        {
            _position += count;
            while (_position >= BUFFER_BYTES)
            {
                ForwardStep();
                _position -= BUFFER_BYTES;
            }
        }

        /// <summary>
        /// Generates <paramref name="count"/> junk bytes and returns them.
        /// </summary>
        public byte[] GetBytes(int count)
        {
            byte[] output = new byte[count];
            GetBytes(count, output, 0);
            return output;
        }

        /// <summary>
        /// Generates junk bytes into <paramref name="output"/> starting at <paramref name="outputOffset"/>.
        /// Matches Dolphin: LaggedFibonacciGenerator::GetBytes using memcpy pattern.
        /// </summary>
        /// <remarks></remarks>
        public void GetBytes(int count, byte[] output, int outputOffset)
        {
            while (count > 0)
            {
                int length = Math.Min(count, BUFFER_BYTES - _position);
                Buffer.BlockCopy(_buffer, _position, output, outputOffset, length);
                _position += length;
                count -= length;
                outputOffset += length;

                if (_position == BUFFER_BYTES)
                {
                    ForwardStep();
                    _position = 0;
                }
            }
        }

        /// <summary>
        /// Returns a single junk byte at the current position, advancing by one byte.
        /// Matches Dolphin: LaggedFibonacciGenerator::GetByte.
        /// </summary>
        /// <remarks></remarks>
        internal byte GetByte()
        {
            int wordIdx = _position / 4;
            int byteInWord = _position % 4;
            byte result = (byte)(_buffer[wordIdx] >> (byteInWord * 8)); // LE byte order

            _position++;
            if (_position == BUFFER_BYTES)
            {
                ForwardStep();
                _position = 0;
            }

            return result;
        }

        #region Private forward/backward state steps

        /// <summary>
        /// Full buffer state step forward
        /// </summary>
        /// <remarks>
        /// Dolphin: Forward() (no args).
        /// for i in [0,J):   buf[i] ^= buf[i + K - J]  (= buf[i + 489])
        /// for i in [J,K):   buf[i] ^= buf[i - J]       (= buf[i - 32])
        /// </remarks>
        private void ForwardStep()
        {
            for (int i = 0; i < LFG_J; i++)
            {
                _buffer[i] ^= _buffer[i + LFG_K - LFG_J];
            }

            for (int i = LFG_J; i < LFG_K; i++)
            {
                _buffer[i] ^= _buffer[i - LFG_J];
            }
        }

        /// <summary>
        /// Partial or full buffer state step backward — undoes ForwardStep.
        /// </summary>
        /// <remarks>Dolphin: Backward(size_t start_word, size_t end_word).</remarks>
        private void Backward(int startWord = 0, int endWord = LFG_K)
        {
            int loopEnd = Math.Max(LFG_J, startWord);

            // Undo second loop of ForwardStep (reversed)
            for (int i = Math.Min(endWord, LFG_K); i > loopEnd; i--)
            {
                _buffer[i - 1] ^= _buffer[i - 1 - LFG_J];
            }

            // Undo first loop of ForwardStep (reversed)
            for (int i = Math.Min(endWord, LFG_J); i > startWord; i--)
            {
                _buffer[i - 1] ^= _buffer[i - 1 + LFG_K - LFG_J];
            }
        }

        /// <summary>
        /// Recovers the original 17-word seed from the current buffer state and outputs it
        /// as LE uint values into <paramref name="seedOut"/>.
        /// </summary>
        /// <remarks>Dolphin: Reinitialize(uint seed_out[]).</remarks>
        private bool Reinitialize(uint[] seedOut)
        {
            for (int i = 0; i < 4; i++)
            {
                Backward();
            }

            // Swap all words back to big-endian representation
            for (int i = 0; i < LFG_K; i++)
            {
                _buffer[i] = SwapU32(_buffer[i]);
            }

            // Reconstruct bits 16-17 for the first SEED_SIZE words
            for (int i = 0; i < SEED_SIZE; i++)
            {
                _buffer[i] = (_buffer[i] & 0xFF00FFFF)
                            | ((_buffer[i] << 2) & 0x00FC0000)
                            | (((_buffer[i + 16] ^ _buffer[i + 15]) << 9) & 0x00030000);
            }

            // Output seed as LE uint values (swap32 converts BE->LE)
            for (int i = 0; i < SEED_SIZE; i++)
            {
                seedOut[i] = SwapU32(_buffer[i]);
            }

            return Initialize(true);
        }

        /// <summary>
        /// Fills m_buffer[SEED_SIZE..K-1] from the first SEED_SIZE words, applies the output
        /// transform, and runs 4× ForwardStep.  When <paramref name="checkExisting"/> is true,
        /// verifies the data in m_buffer[SEED_SIZE..] matches the recurrence.
        /// </summary>
        /// <remarks>Dolphin: Initialize(bool check_existing_data).</remarks>
        private bool Initialize(bool checkExisting)
        {
            for (int i = SEED_SIZE; i < LFG_K; i++)
            {
                uint calculated = (_buffer[i - 17] << 23)
                                ^ (_buffer[i - 16] >> 9)
                                ^ _buffer[i - 1];

                if (checkExisting)
                {
                    uint actual = (_buffer[i] & 0xFF00FFFF) | ((_buffer[i] << 2) & 0x00FC0000);
                    if ((calculated & 0xFFFCFFFF) != actual)
                        return false;
                }

                _buffer[i] = calculated;
            }

            // Output transform: each word -> swap32((x & 0xFF00FFFF) | ((x >> 2) & 0x00FF0000))
            for (int i = 0; i < LFG_K; i++)
            {
                _buffer[i] = SwapU32((_buffer[i] & 0xFF00FFFF) | ((_buffer[i] >> 2) & 0x00FF0000));
            }

            for (int i = 0; i < 4; i++)
            {
                ForwardStep();
            }

            return true;
        }

        #endregion

        #region Static seed-recovery API (used by RvzPackDecompressor)

        /// <summary>
        /// Attempts to recover a 17-word seed from disc data starting at
        /// <paramref name="dataStart"/> within <paramref name="data"/>.
        /// <paramref name="size"/> is the number of bytes to match (up to the next 32 KiB boundary).
        /// <paramref name="dataOffsetMod"/> is discOffset % 0x8000 — the offset within
        /// the current LFG cycle.
        /// </summary>
        /// <returns>the number of bytes that were successfully reconstructed (0 = not junk data).</returns>
        /// <remarks>Matches Dolphin: LaggedFibonacciGenerator::GetSeed(u8*, size_t, size_t, uint[]).</remarks>
        public static int GetSeed(byte[] data, int dataStart, int size, int dataOffsetMod, uint[] seedOut)
        {
            if (size <= 0 || dataStart < 0 || dataStart + size > data.Length)
                return 0;

            // Skip any bytes before the next uint-aligned boundary
            int bytesToSkip = (4 - (dataOffsetMod % 4)) % 4;
            if (bytesToSkip >= size)
                return 0;

            int uintDataStart = dataStart + bytesToSkip;
            int uintSize = (size - bytesToSkip) / 4;
            int uintDataOffset = (dataOffsetMod + bytesToSkip) / 4;

            if (uintSize < LFG_K)
                return 0;

            // Read disc bytes as little-endian uint values (Dolphin: reinterpret_cast<const uint*>)
            uint[] uintData = new uint[uintSize];
            for (int i = 0; i < uintSize; i++)
            {
                uintData[i] = data.ReadUInt32LittleEndian(ref uintDataStart);
            }

            var lfg = new LaggedFibonacciGenerator();
            if (!GetSeed(uintData, uintSize, uintDataOffset, lfg, seedOut))
                return 0;

            // Set position to data_offset % BUFFER_BYTES and count matching bytes from data[dataStart]
            lfg._position = dataOffsetMod % BUFFER_BYTES;

            int reconstructed = 0;
            for (int i = 0; i < size && lfg.GetByte() == data[dataStart + i]; i++)
            {
                reconstructed++;
            }

            return reconstructed;
        }

        /// <summary>
        /// Inner UInt32-level seed recovery.
        /// </summary>
        /// <remarks>Dolphin: GetSeed(const uint* data, size_t size, size_t data_offset, LFG*, uint[]).</remarks>
        private static bool GetSeed(uint[] data, int size, int dataOffset, LaggedFibonacciGenerator lfg, uint[] seedOut)
        {
            if (size < LFG_K)
                return false;

            // Quick sanity check: bits 22-23 of swap32(x) must equal bits 20-21
            // (a property of the LFG output transform).
            for (int i = 0; i < LFG_K; i++)
            {
                uint x = SwapU32(data[i]);
                if ((x & 0x00C00000) != ((x >> 2) & 0x00C00000))
                    return false;
            }

            int dataOffsetModK = dataOffset % LFG_K;
            int dataOffsetDivK = dataOffset / LFG_K;

            // Rotate data into buffer so buffer[dataOffsetModK] = data[0]
            Array.Copy(data, 0, lfg._buffer, dataOffsetModK, LFG_K - dataOffsetModK);
            if (dataOffsetModK > 0)
                Array.Copy(data, LFG_K - dataOffsetModK, lfg._buffer, 0, dataOffsetModK);

            lfg.Backward(0, dataOffsetModK);

            for (int i = 0; i < dataOffsetDivK; i++)
            {
                lfg.Backward();
            }

            if (!lfg.Reinitialize(seedOut))
                return false;

            for (int i = 0; i < dataOffsetDivK; i++)
            {
                lfg.ForwardStep();
            }

            return true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Swap endinaness of a UInt32
        /// </summary>
        internal static uint SwapU32(uint value)
            => (value << 24) | ((value << 8) & 0x00FF0000) | ((value >> 8) & 0x0000FF00) | (value >> 24);

        #endregion
    }
}
