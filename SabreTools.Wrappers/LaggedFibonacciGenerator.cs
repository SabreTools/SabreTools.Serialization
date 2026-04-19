using System;

namespace SabreTools.Wrappers
{
    /// <summary>
    /// Lagged Fibonacci Generator matching Dolphin's LaggedFibonacciGenerator exactly.
    /// Used to regenerate Nintendo's deterministic "junk" padding data in disc images.
    /// RVZ format identifies junk regions and stores only a 68-byte seed (17 u32 words)
    /// instead of the full data, enabling significant compression of padding areas.
    /// </summary>
    internal class LaggedFibonacciGenerator
    {
        private const int LFG_K = 521;
        private const int LFG_J = 32;

        /// <summary>Size of the LFG output buffer in bytes (LFG_K * 4 = 2084).</summary>
        public const int BUFFER_BYTES = LFG_K * 4;

        /// <summary>Size of the seed in 32-bit words (68 bytes total).</summary>
        public const int SEED_SIZE = 17;

        private readonly uint[] m_buffer = new uint[LFG_K];
        private int m_position_bytes = 0;

        /// <summary>
        /// Initializes the generator from a 17-element u32 seed array.
        /// Each seed word is treated as a raw LE u32 from the file (Dolphin: reinterpret_cast then swap32).
        /// </summary>
        public void SetSeed(uint[] seed)
        {
            if (seed == null || seed.Length < SEED_SIZE)
                throw new ArgumentException($"Seed must contain at least {SEED_SIZE} u32 values.", nameof(seed));

            m_position_bytes = 0;
            for (int i = 0; i < SEED_SIZE; i++)
                m_buffer[i] = SwapU32(seed[i]); // reinterpret LE bytes as BE (Dolphin swap32)
            Initialize(false);
        }

        /// <summary>
        /// Initializes the generator from a 68-byte seed (17 BE u32 values as in the RVZ file).
        /// Matches Dolphin: m_buffer[i] = Common::swap32(seed + i * 4).
        /// </summary>
        public void SetSeed(byte[] seedBytes)
        {
            if (seedBytes == null || seedBytes.Length < SEED_SIZE * 4)
                throw new ArgumentException($"Seed must be {SEED_SIZE * 4} bytes.", nameof(seedBytes));

            m_position_bytes = 0;
            for (int i = 0; i < SEED_SIZE; i++)
                m_buffer[i] = ReadBigEndianU32(seedBytes, i * 4);
            Initialize(false);
        }

        /// <summary>
        /// Skips forward by <paramref name="count"/> bytes in the output stream.
        /// Matches Dolphin: LaggedFibonacciGenerator::Forward(size_t count).
        /// </summary>
        public void Forward(int count)
        {
            m_position_bytes += count;
            while (m_position_bytes >= BUFFER_BYTES)
            {
                ForwardStep();
                m_position_bytes -= BUFFER_BYTES;
            }
        }

        /// <summary>Generates <paramref name="count"/> junk bytes and returns them.</summary>
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
        public void GetBytes(int count, byte[] output, int outputOffset)
        {
            while (count > 0)
            {
                int length = Math.Min(count, BUFFER_BYTES - m_position_bytes);
                Buffer.BlockCopy(m_buffer, m_position_bytes, output, outputOffset, length);
                m_position_bytes += length;
                count -= length;
                outputOffset += length;

                if (m_position_bytes == BUFFER_BYTES)
                {
                    ForwardStep();
                    m_position_bytes = 0;
                }
            }
        }

        /// <summary>
        /// Returns a single junk byte at the current position, advancing by one byte.
        /// Matches Dolphin: LaggedFibonacciGenerator::GetByte.
        /// </summary>
        internal byte GetByte()
        {
            int wordIdx    = m_position_bytes / 4;
            int byteInWord = m_position_bytes % 4;
            byte result = (byte)(m_buffer[wordIdx] >> (byteInWord * 8)); // LE byte order

            m_position_bytes++;
            if (m_position_bytes == BUFFER_BYTES)
            {
                ForwardStep();
                m_position_bytes = 0;
            }

            return result;
        }

        // -------------------------------------------------------------------
        // Private forward/backward state steps
        // -------------------------------------------------------------------

        /// <summary>
        /// Full buffer state step forward — Dolphin: Forward() (no args).
        /// for i in [0,J):   buf[i] ^= buf[i + K - J]  (= buf[i + 489])
        /// for i in [J,K):   buf[i] ^= buf[i - J]       (= buf[i - 32])
        /// </summary>
        private void ForwardStep()
        {
            for (int i = 0; i < LFG_J; i++)
                m_buffer[i] ^= m_buffer[i + LFG_K - LFG_J];
            for (int i = LFG_J; i < LFG_K; i++)
                m_buffer[i] ^= m_buffer[i - LFG_J];
        }

        /// <summary>
        /// Partial or full buffer state step backward — undoes ForwardStep.
        /// Dolphin: Backward(size_t start_word, size_t end_word).
        /// </summary>
        private void Backward(int startWord = 0, int endWord = LFG_K)
        {
            int loopEnd = Math.Max(LFG_J, startWord);

            // Undo second loop of ForwardStep (reversed)
            for (int i = Math.Min(endWord, LFG_K); i > loopEnd; i--)
                m_buffer[i - 1] ^= m_buffer[i - 1 - LFG_J];

            // Undo first loop of ForwardStep (reversed)
            for (int i = Math.Min(endWord, LFG_J); i > startWord; i--)
                m_buffer[i - 1] ^= m_buffer[i - 1 + LFG_K - LFG_J];
        }

        /// <summary>
        /// Recovers the original 17-word seed from the current buffer state and outputs it
        /// as LE u32 values into <paramref name="seedOut"/>.
        /// Dolphin: Reinitialize(u32 seed_out[]).
        /// </summary>
        private bool Reinitialize(uint[] seedOut)
        {
            for (int i = 0; i < 4; i++)
                Backward();

            // Swap all words back to big-endian representation
            for (int i = 0; i < LFG_K; i++)
                m_buffer[i] = SwapU32(m_buffer[i]);

            // Reconstruct bits 16-17 for the first SEED_SIZE words
            for (int i = 0; i < SEED_SIZE; i++)
            {
                m_buffer[i] = (m_buffer[i] & 0xFF00FFFF)
                            | ((m_buffer[i] << 2) & 0x00FC0000)
                            | (((m_buffer[i + 16] ^ m_buffer[i + 15]) << 9) & 0x00030000);
            }

            // Output seed as LE u32 values (swap32 converts BE→LE)
            for (int i = 0; i < SEED_SIZE; i++)
                seedOut[i] = SwapU32(m_buffer[i]);

            return Initialize(true);
        }

        /// <summary>
        /// Fills m_buffer[SEED_SIZE..K-1] from the first SEED_SIZE words, applies the output
        /// transform, and runs 4× ForwardStep.  When <paramref name="checkExisting"/> is true,
        /// verifies the data in m_buffer[SEED_SIZE..] matches the recurrence.
        /// Dolphin: Initialize(bool check_existing_data).
        /// </summary>
        private bool Initialize(bool checkExisting)
        {
            for (int i = SEED_SIZE; i < LFG_K; i++)
            {
                uint calculated = (m_buffer[i - 17] << 23)
                                ^ (m_buffer[i - 16] >> 9)
                                ^ m_buffer[i - 1];

                if (checkExisting)
                {
                    uint actual = (m_buffer[i] & 0xFF00FFFF) | ((m_buffer[i] << 2) & 0x00FC0000);
                    if ((calculated & 0xFFFCFFFF) != actual)
                        return false;
                }

                m_buffer[i] = calculated;
            }

            // Output transform: each word → swap32((x & 0xFF00FFFF) | ((x >> 2) & 0x00FF0000))
            for (int i = 0; i < LFG_K; i++)
                m_buffer[i] = SwapU32((m_buffer[i] & 0xFF00FFFF) | ((m_buffer[i] >> 2) & 0x00FF0000));

            for (int i = 0; i < 4; i++)
                ForwardStep();

            return true;
        }

        // -------------------------------------------------------------------
        // Static seed-recovery API (used by RvzPackDecompressor)
        // -------------------------------------------------------------------

        /// <summary>
        /// Attempts to recover a 17-word seed from disc data starting at
        /// <paramref name="dataStart"/> within <paramref name="data"/>.
        /// <paramref name="size"/> is the number of bytes to match (up to the next 32 KiB boundary).
        /// <paramref name="dataOffsetMod"/> is <c>discOffset % 0x8000</c> — the offset within
        /// the current LFG cycle.
        /// Returns the number of bytes that were successfully reconstructed (0 = not junk data).
        /// Matches Dolphin: LaggedFibonacciGenerator::GetSeed(u8*, size_t, size_t, u32[]).
        /// </summary>
        public static int GetSeed(byte[] data, int dataStart, int size, int dataOffsetMod, uint[] seedOut)
        {
            if (size <= 0 || dataStart < 0 || dataStart + size > data.Length)
                return 0;

            // Skip any bytes before the next u32-aligned boundary
            int bytesToSkip = (4 - (dataOffsetMod % 4)) % 4;
            if (bytesToSkip >= size)
                return 0;

            int u32DataStart  = dataStart + bytesToSkip;
            int u32Size       = (size - bytesToSkip) / 4;
            int u32DataOffset = (dataOffsetMod + bytesToSkip) / 4;

            if (u32Size < LFG_K)
                return 0;

            // Read disc bytes as LE u32 values (Dolphin: reinterpret_cast<const u32*>)
            uint[] u32Data = new uint[u32Size];
            for (int i = 0; i < u32Size; i++)
                u32Data[i] = ReadLittleEndianU32(data, u32DataStart + (i * 4));

            LaggedFibonacciGenerator lfg = new LaggedFibonacciGenerator();
            if (!GetSeed_u32(u32Data, u32Size, u32DataOffset, lfg, seedOut))
                return 0;

            // Set position to data_offset % BUFFER_BYTES and count matching bytes from data[dataStart]
            lfg.m_position_bytes = dataOffsetMod % BUFFER_BYTES;

            int reconstructed = 0;
            for (int i = 0; i < size && lfg.GetByte() == data[dataStart + i]; i++)
                reconstructed++;

            return reconstructed;
        }

        /// <summary>
        /// Inner u32-level seed recovery.
        /// Dolphin: GetSeed(const u32* data, size_t size, size_t data_offset, LFG*, u32[]).
        /// </summary>
        private static bool GetSeed_u32(uint[] data, int size, int dataOffset,
                                        LaggedFibonacciGenerator lfg, uint[] seedOut)
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
            Array.Copy(data, 0,                        lfg.m_buffer, dataOffsetModK, LFG_K - dataOffsetModK);
            if (dataOffsetModK > 0)
                Array.Copy(data, LFG_K - dataOffsetModK, lfg.m_buffer, 0,             dataOffsetModK);

            lfg.Backward(0, dataOffsetModK);

            for (int i = 0; i < dataOffsetDivK; i++)
                lfg.Backward();

            if (!lfg.Reinitialize(seedOut))
                return false;

            for (int i = 0; i < dataOffsetDivK; i++)
                lfg.ForwardStep();

            return true;
        }

        // -------------------------------------------------------------------
        // Endian helpers
        // -------------------------------------------------------------------

        internal static uint ReadBigEndianU32(byte[] data, int offset) =>
            (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);

        private static uint ReadLittleEndianU32(byte[] data, int offset) =>
            (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));

        internal static uint SwapU32(uint value) =>
            (value << 24) | ((value << 8) & 0x00FF0000) | ((value >> 8) & 0x0000FF00) | (value >> 24);
    }
}
