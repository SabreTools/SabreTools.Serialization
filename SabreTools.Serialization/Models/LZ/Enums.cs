using System;

namespace SabreTools.Serialization.Models.LZ
{
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public enum KWAJCompressionType : ushort
    {
        /// <summary>
        /// No compression
        /// </summary>
        NoCompression = 0,

        /// <summary>
        /// No compression, data is XORed with byte 0xFF
        /// </summary>
        NoCompressionXor = 1,

        /// <summary>
        /// The same compression method as the QBasic variant of SZDD
        /// </summary>
        QBasic = 2,

        /// <summary>
        /// LZ + Huffman "Jeff Johnson" compression
        /// </summary>
        LZH = 3,

        /// <summary>
        /// MS-ZIP
        /// </summary>
        MSZIP = 4,
    }

    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    [Flags]
    public enum KWAJHeaderFlags : ushort
    {
        /// <summary>
        /// Header extensions contains 4-byte decompressed length
        /// </summary>
        HasDecompressedLength = 0x0001,

        /// <summary>
        /// Header extensions contains 2-byte unknown value
        /// </summary>
        HasUnknownFlag = 0x0002,

        /// <summary>
        /// Header extensions contains 2-byte prefix followed by
        /// that many bytes of (unknown purpose) data
        /// </summary>
        HasPrefixedData = 0x0004,

        /// <summary>
        /// Header extensions contains null-terminated string of
        /// max length 8 representing the file name
        /// </summary>
        HasFileName = 0x0008,

        /// <summary>
        /// Header extensions contains null-terminated string of
        /// max length 3 representing the file name
        /// </summary>
        HasFileExtension = 0x0010,

        /// <summary>
        /// Header extensions contains 2-byte prefix followed by
        /// that many bytes of (arbitrary text) data
        /// </summary>
        HasAdditionalText = 0x0020,
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public enum ExpandCompressionType : byte
    {
        /// <summary>
        /// Only valid compression type: 'A'
        /// </summary>
        A = 0x41,
    }
}