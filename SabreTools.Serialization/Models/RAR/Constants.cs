namespace SabreTools.Serialization.Models.RAR
{
    public static class Constants
    {
        public static readonly byte[] OldSignatureBytes = [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00];

        public static readonly byte[] NewSignatureBytes = [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00];
    }
}