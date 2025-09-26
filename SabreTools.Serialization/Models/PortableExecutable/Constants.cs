namespace SabreTools.Data.Models.PortableExecutable
{
    // TODO: Add more constants around sizes
    // - Debug Directory entry size is a constant value of 28 (0x1C)
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = [0x50, 0x45, 0x00, 0x00];

        public const string SignatureString = "PE\0\0";

        public const uint SignatureUInt32 = 0x00004550;
    }
}