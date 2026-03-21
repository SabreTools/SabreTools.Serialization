namespace SabreTools.Data.Models.BZip2
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = [0x42, 0x5A, 0x68];

        public const string SignatureString = "BZh";
    }
}
