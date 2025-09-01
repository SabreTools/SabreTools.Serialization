using System;
using System.IO;
using SabreTools.Hashing;
using SabreTools.Matching;
using static SabreTools.Models.MoPaQ.Constants;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Handler for decrypting MoPaQ block and table data
    /// </summary>
    /// TODO: Should this live in IO? New `Encryption` namespace?
    public class MoPaQDecrypter
    {
        #region Private Instance Variables

        /// <summary>
        /// Buffer for encryption and decryption
        /// </summary>
        private readonly uint[] _stormBuffer = new uint[STORM_BUFFER_SIZE];

        #endregion

        public MoPaQDecrypter()
        {
            PrepareCryptTable();
        }

        /// <summary>
        /// Prepare the encryption table
        /// </summary>
        private void PrepareCryptTable()
        {
            uint seed = 0x00100001;
            for (uint index1 = 0; index1 < 0x100; index1++)
            {
                for (uint index2 = index1, i = 0; i < 5; i++, index2 += 0x100)
                {
                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp1 = (seed & 0xFFFF) << 0x10;

                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp2 = (seed & 0xFFFF);

                    _stormBuffer[index2] = (temp1 | temp2);
                }
            }
        }

        /// <summary>
        /// Load a table block by optionally decompressing and
        /// decrypting before returning the data.
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="offset">Data offset to parse</param>
        /// <param name="expectedHash">Optional MD5 hash for validation</param>
        /// <param name="compressedSize">Size of the table in the file</param>
        /// <param name="tableSize">Expected size of the table</param>
        /// <param name="key">Encryption key to use</param>
        /// <param name="realTableSize">Output represening the real table size</param>
        /// <returns>Byte array representing the processed table</returns>
        public byte[]? LoadTable(Stream data,
            long offset,
            byte[]? expectedHash,
            uint compressedSize,
            uint tableSize,
            uint key,
            out long realTableSize)
        {
            byte[]? tableData;
            byte[]? readBytes;
            long bytesToRead = tableSize;

            // Allocate the MPQ table
            tableData = readBytes = new byte[tableSize];

            // Check if the MPQ table is compressed
            if (compressedSize != 0 && compressedSize < tableSize)
            {
                // Allocate temporary buffer for holding compressed data
                readBytes = new byte[compressedSize];
                bytesToRead = compressedSize;
            }

            // Get the file offset from which we will read the table
            // Note: According to Storm.dll from Warcraft III (version 2002),
            // if the hash table position is 0xFFFFFFFF, no SetFilePointer call is done
            // and the table is loaded from the current file offset
            if (offset == 0xFFFFFFFF)
                offset = data.Position;

            // Is the sector table within the file?
            if (offset >= data.Length)
            {
                realTableSize = 0;
                return null;
            }

            // The hash table and block table can go beyond EOF.
            // Storm.dll reads as much as possible, then fills the missing part with zeros.
            // Abused by Spazzler map protector which sets hash table size to 0x00100000
            // Abused by NP_Protect in MPQs v4 as well
            if ((offset + bytesToRead) > data.Length)
                bytesToRead = (uint)(data.Length - offset);

            // Give the caller information that the table was cut
            realTableSize = bytesToRead;

            // If everything succeeded, read the raw table from the MPQ
            data.Seek(offset, SeekOrigin.Begin);
            _ = data.Read(readBytes, 0, (int)bytesToRead);

            // Verify the MD5 of the table, if present
            byte[]? actualHash = HashTool.GetByteArrayHashArray(readBytes, HashType.MD5);
            if (expectedHash != null && actualHash != null && !actualHash.EqualsExactly(expectedHash))
            {
                Console.WriteLine("Table is corrupt!");
                return null;
            }

            // First of all, decrypt the table
            if (key != 0)
                tableData = DecryptBlock(readBytes, bytesToRead, key);

            // If the table is compressed, decompress it
            if (compressedSize != 0 && compressedSize < tableSize)
            {
                Console.WriteLine("Table is compressed, it will not read properly!");
                return null;

                // TODO: Handle decompression
                // int cbOutBuffer = (int)tableSize;
                // int cbInBuffer = (int)compressedSize;

                // if (!SCompDecompress2(readBytes, &cbOutBuffer, tableData, cbInBuffer))
                //     errorCode = SErrGetLastError();

                // tableData = readBytes;
            }

            // Return the MPQ table
            return tableData;
        }

        /// <summary>
        /// Decrypt a single block of data
        /// </summary>
        public unsafe byte[] DecryptBlock(byte[] block, long length, uint key)
        {
            uint seed = 0xEEEEEEEE;

            uint[] castBlock = new uint[length >> 2];
            Buffer.BlockCopy(block, 0, castBlock, 0, (int)length);
            int castBlockPtr = 0;

            // Round to uints
            length >>= 2;

            while (length-- > 0)
            {
                seed += _stormBuffer[MPQ_HASH_KEY2_MIX + (key & 0xFF)];
                uint ch = castBlock[castBlockPtr] ^ (key + seed);

                key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
                seed = ch + seed + (seed << 5) + 3;
                castBlock[castBlockPtr++] = ch;
            }

            Buffer.BlockCopy(castBlock, 0, block, 0, block.Length >> 2);
            return block;
        }
    }
}