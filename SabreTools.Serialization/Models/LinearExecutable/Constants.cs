namespace SabreTools.Serialization.Models.LinearExecutable
{
    public static class Constants
    {
        public static readonly byte[] DebugInformationSignatureBytes = [0x4e, 0x42, 0x30];

        public const string DebugInformationSignatureString = "NB0";

        public static readonly byte[] LESignatureBytes = [0x4c, 0x45];

        public const string LESignatureString = "LE";

        public const ushort LESignatureUInt16 = 0x454c;

        public static readonly byte[] LXSignatureBytes = [0x4c, 0x58];

        public const string LXSignatureString = "LX";

        public const ushort LXSignatureUInt16 = 0x584c;
    }
}