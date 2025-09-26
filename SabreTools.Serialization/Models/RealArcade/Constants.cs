namespace SabreTools.Data.Models.RealArcade
{
    public static class Constants
    {
        public static readonly byte[] MezzanineSignatureBytes = [0x58, 0x5A, 0x69, 0x70, 0x32, 0x2E, 0x30];

        public const string MezzanineSignatureString = "XZip2.0";

        public static readonly byte[] RgsSignatureBytes = [0x52, 0x41, 0x53, 0x47, 0x49, 0x32, 0x2E, 0x30];

        public const string RgsSignatureString = "RASGI2.0";
    }
}