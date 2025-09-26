namespace SabreTools.Serialization.Models.BZip2
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = [0x42, 0x52, 0x68];

        public const string SignatureString = "BRh";
    }
}