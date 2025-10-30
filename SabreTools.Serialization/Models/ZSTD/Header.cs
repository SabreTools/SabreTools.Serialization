namespace SabreTools.Data.Models.ZSTD
{
    /// <summary>
    /// Header
    /// </summary>
    /// <see cref="https://datatracker.ietf.org/doc/html/rfc8878#section-3.1.1-3.2"/>
    public sealed class Header
    {
        /// <summary>
        /// Despite never being referred to as such, and being hard to find in the documentation, the least significant
        /// byte changed with versions until 0.8.
        /// 0.1 used 0x1E, then it was 0.2(0x22)-0.8(0x28)
        /// </summary>
        /// <see cref="https://github.com/facebook/zstd/issues/713"/>
        public byte VersionByte;

        /// <summary>
        /// "0x?? 0xB5 0x2F 0xFD"
        /// </summary>
        public byte[]? Magic;
    }
}