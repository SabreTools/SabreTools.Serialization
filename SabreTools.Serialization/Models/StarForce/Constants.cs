namespace SabreTools.Data.Models.StarForce
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = [0x53, 0x46, 0x46, 0x53];

        public const string SignatureString = "SFFS";

        public const uint SignatureUInt32 = 0x53464653;
    }
}