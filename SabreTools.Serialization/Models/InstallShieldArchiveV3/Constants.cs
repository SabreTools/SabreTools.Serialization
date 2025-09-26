namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    public static class Constants
    {
        public const uint HeaderSignature = 0x8C655D13;

        public static readonly byte[] HeaderSignatureBytes = [0x13, 0x5D, 0x65, 0x8C];
    }
}