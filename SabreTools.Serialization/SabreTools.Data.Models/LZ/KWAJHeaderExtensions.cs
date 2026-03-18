namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// Additional information stored after the KWAJ header
    /// </summary>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public sealed class KWAJHeaderExtensions
    {
        /// <summary>
        /// Decompressed length of file
        /// </summary>
        public uint? DecompressedLength { get; set; }

        /// <summary>
        /// Unknown purpose
        /// </summary>
        public ushort? UnknownPurpose { get; set; }

        /// <summary>
        /// Length of <see cref="UnknownData"/>
        /// </summary>
        public ushort? UnknownDataLength { get; set; }

        /// <summary>
        /// Unknown purpose data whose length is defined
        /// by <see cref="UnknownDataLength"/>
        /// </summary>
        public byte[]? UnknownData { get; set; }

        /// <summary>
        /// Null-terminated string with max length 8: file name
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Null-terminated string with max length 3: file extension
        /// </summary>
        public string? FileExtension { get; set; }

        /// <summary>
        /// Length of <see cref="ArbitraryText"/>
        /// </summary>
        public ushort? ArbitraryTextLength { get; set; }

        /// <summary>
        /// Arbitrary text data whose length is defined
        /// by <see cref="ArbitraryTextLength"/>
        /// </summary>
        public byte[]? ArbitraryText { get; set; }
    }
}
