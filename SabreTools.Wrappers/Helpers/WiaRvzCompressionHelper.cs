using System;
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.IO;
using SabreTools.IO.Extensions;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.ZStandard;
#endif
using SabreTools.Data.Models.WIA;

// TODO: Move to IO
namespace SabreTools.Wrappers
{
    /// <summary>
    /// Compress and decompress helpers for WIA / RVZ group and table data.
    /// Mirrors Dolphin's WIACompression.cpp: Bzip2, LZMA (raw, no stream header), LZMA2, and Zstd.
    /// </summary>
    internal static class WiaRvzCompressionHelper
    {
        /// <summary>
        /// Dictionary sizes per compression level 1-9 (index 0 unused).
        /// </summary>
        /// <remarks>Mirrors Dolphin WIACompression.cpp dict_size choices.</remarks>
        private static readonly int[] DictSizes =
        [
            0,         // 0: unused
            1 << 16,   // 1:  64 KiB
            1 << 20,   // 2:   1 MiB
            1 << 22,   // 3:   4 MiB
            1 << 22,   // 4:   4 MiB
            1 << 23,   // 5:   8 MiB
            1 << 23,   // 6:   8 MiB
            1 << 24,   // 7:  16 MiB
            1 << 25,   // 8:  32 MiB
            1 << 26,   // 9:  64 MiB
        ];

        /// <summary>
        ///
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int GetDictSize(int level)
            => DictSizes[Math.Max(1, Math.Min(9, level))];

        /// <summary>
        /// Returns the raw LZMA2 dict-size property byte for a given dictionary size.
        /// </summary>
        private static uint Lzma2DictSize(byte p) => (uint)((2 | (p & 1)) << ((p / 2) + 11));

        /// <summary>
        ///
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static byte EncodeLzma2DictSize(uint d)
        {
            byte e = 0;
            while (e < 40 && d > Lzma2DictSize(e))
            {
                e++;
            }

            return e;
        }

        /// <summary>
        /// Fills the compressor-data bytes for <see cref="WiaHeader2.CompressorData"/>
        /// and <see cref="WiaHeader2.CompressorDataSize"/>.
        /// LZMA: 5 bytes. LZMA2: 1 byte. Others: 0 bytes.
        /// </summary>
        internal static void GetCompressorData(WiaRvzCompressionType type, int level, out byte[] propData, out byte propSize)
        {
            propData = new byte[7];
            int dictSize = GetDictSize(level);

            switch (type)
            {
                case WiaRvzCompressionType.LZMA:
                    propData[0] = 0x5D; // propByte for default pb=2,lp=0,lc=3
                    propData[1] = (byte)dictSize;
                    propData[2] = (byte)(dictSize >> 8);
                    propData[3] = (byte)(dictSize >> 16);
                    propData[4] = (byte)(dictSize >> 24);
                    propSize = 5;
                    break;

                case WiaRvzCompressionType.LZMA2:
                    propData[0] = EncodeLzma2DictSize((uint)dictSize);
                    propSize = 1;
                    break;

                // All cases below default to 0
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                case WiaRvzCompressionType.Bzip2:
                case WiaRvzCompressionType.Zstd:
                default:
                    propSize = 0;
                    break;
            }
        }

        /// <summary>
        /// Compress <paramref name="data"/> using the specified algorithm.
        /// </summary>
        internal static byte[] Compress(WiaRvzCompressionType type,
            byte[] data,
            int offset,
            int length,
            int level,
            byte[] compressorData,
            byte compressorDataSize)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            switch (type)
            {
                case WiaRvzCompressionType.Bzip2:
                    return CompressBzip2(data, offset, length);
                case WiaRvzCompressionType.LZMA:
                    return CompressLzma(data, offset, length, level, isLzma2: false);
                case WiaRvzCompressionType.LZMA2:
                    return CompressLzma(data, offset, length, level, isLzma2: true);
                case WiaRvzCompressionType.Zstd:
                    return CompressZstd(data, offset, length, level);

                // Do not use compression
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                default:
                    throw new ArgumentException($"Cannot compress type {type}", nameof(type));
            }
#else
            throw new PlatformNotSupportedException("WIA/RVZ compression requires .NET 4.6.2 or later.");
#endif
        }

        /// <summary>
        /// Decompress <paramref name="data"/> using the specified algorithm.
        /// </summary>
        internal static byte[] Decompress(WiaRvzCompressionType type,
            byte[] data,
            int offset,
            int length,
            byte[] compressorData,
            byte compressorDataSize)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            switch (type)
            {
                case WiaRvzCompressionType.Bzip2:
                    return DecompressBzip2(data, offset, length);

                case WiaRvzCompressionType.LZMA:
                    byte[] lzmaProps = new byte[compressorDataSize];
                    Array.Copy(compressorData, lzmaProps, compressorDataSize);
                    return DecompressLzma(data, offset, length, lzmaProps, isLzma2: false);

                case WiaRvzCompressionType.LZMA2:
                    byte[] lzma2Props = new byte[compressorDataSize];
                    Array.Copy(compressorData, lzma2Props, compressorDataSize);
                    return DecompressLzma(data, offset, length, lzma2Props, isLzma2: true);

                case WiaRvzCompressionType.Zstd:
                    return DecompressZstd(data, offset, length);

                // Do not use compression
                case WiaRvzCompressionType.None:
                case WiaRvzCompressionType.Purge:
                default:
                    throw new ArgumentException($"Cannot decompress type {type}", nameof(type));
            }
#else
            throw new PlatformNotSupportedException("WIA/RVZ decompression requires .NET 4.6.2 or later.");
#endif
        }

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Compress data using bzip2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressBzip2(byte[] data, int offset, int length)
        {
            using var outMs = new MemoryStream();
            using (var bz2 = BZip2Stream.Create(outMs, CompressionMode.Compress, false, true))
            {
                bz2.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using bzip2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressBzip2(byte[] data, int offset, int length)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var bz2 = BZip2Stream.Create(inMs, CompressionMode.Decompress, false, false);
            using var outMs = new MemoryStream();

            bz2.BlockCopy(outMs);

            return outMs.ToArray();
        }

        /// <summary>
        /// Compress data using LZMA/LZMA2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="level">LZMA/LZMA2 level</param>
        /// <param name="isLzma2">Indicates if LZMA2 is used</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressLzma(byte[] data, int offset, int length, int level, bool isLzma2)
        {
            int dictSize = GetDictSize(level);
            using var outMs = new MemoryStream();
            using (var lzma = LzmaStream.Create(new LzmaEncoderProperties(true, dictSize), isLzma2, outMs))
            {
                lzma.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using LZMA/LZMA2
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="props">LZMA properties</param>
        /// <param name="isLzma2">Indicates if LZMA2 is used</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressLzma(byte[] data, int offset, int length, byte[] props, bool isLzma2)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var lzma = LzmaStream.Create(props, inMs, length, -1, null, isLzma2, false);
            using var outMs = new MemoryStream();

            lzma.BlockCopy(outMs);

            return outMs.ToArray();
        }

        /// <summary>
        /// Compress data using Zstd
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <param name="level">Zstd level</param>
        /// <returns>Compressed data</returns>
        private static byte[] CompressZstd(byte[] data, int offset, int length, int level)
        {
            using var outMs = new MemoryStream();
            using (var zstd = new ZStandardStream(outMs, CompressionMode.Compress, level))
            {
                zstd.Write(data, offset, length);
            }

            return outMs.ToArray();
        }

        /// <summary>
        /// Decompress data using Zstd
        /// </summary>
        /// <param name="data">Source data</param>
        /// <param name="offset">Offset into the source data</param>
        /// <param name="length">Length within the source data</param>
        /// <returns>Uncompressed data</returns>
        private static byte[] DecompressZstd(byte[] data, int offset, int length)
        {
            using var inMs = new MemoryStream(data, offset, length);
            using var zstd = new ZStandardStream(inMs);
            using var outMs = new MemoryStream();

            zstd.BlockCopy(outMs);

            return outMs.ToArray();
        }
#endif
    }
}
