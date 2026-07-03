namespace SabreTools.Data.Models.Steam2Installer
{
    public static class Constants
    {
        public static readonly byte[] SimSignatureBytes = [0x1F, 0x4C, 0xD0, 0x3F];

        /// <remarks>All other values in the structure are little endian, assuming this is too</remarks>
        public const uint SimSignatureUInt32 = 0x3FD04C1F;
    }
}
