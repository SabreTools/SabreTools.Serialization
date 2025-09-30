namespace SabreTools.Data.Models.XZ
{
    public static class Constants
    {
        public static readonly byte[] HeaderSignatureBytes = [0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00];

        public static readonly byte[] FooterSignatureBytes = [0x59, 0x5A];
    }
}