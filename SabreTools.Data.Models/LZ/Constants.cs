namespace SabreTools.Data.Models.LZ
{
    public static class Constants
    {
        public const string KWAJPrefix = "KWAJ";

        public static readonly byte[] KWAJSignatureBytes = [0x4B, 0x57, 0x41, 0x4A, 0x88, 0xF0, 0x27, 0xD1];

        public const string QBasicPrefix = "SZ ";

        public static readonly byte[] QBasicSignatureBytes = [0x53, 0x5A, 0x20, 0x88, 0xF0, 0x27, 0x33, 0xD1];

        public const string SZDDPrefix = "SZDD";

        public static readonly byte[] SZDDSignatureBytes = [0x53, 0x5A, 0x44, 0x44, 0x88, 0xF0, 0x27, 0x33];
    }
}
