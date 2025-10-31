namespace SabreTools.Data.Models.GZIP
{
    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/>
    public static class Constants
    {
        public const byte ID1 = 0x1F;

        public const byte ID2 = 0x8B;

        public static readonly byte[] SignatureBytes = [0x1F, 0x8B];

        public static readonly byte[] TorrentGZHeader = [0x1F, 0x8B, 0x08, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x00];
    }
}
