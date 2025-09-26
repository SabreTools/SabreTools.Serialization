namespace SabreTools.Serialization.Models.PFF
{
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    public static class Constants
    {
        public static readonly byte[] Version0SignatureBytes = [0x50, 0x46, 0x46, 0x30];

        public const string Version0SignatureString = "PFF0";
        
        public const uint Version0HSegmentSize = 0x00000020;

        // Version 1 not confirmed
        // public const string Version1SignatureString = "PFF1";
        // public const uint Version1SegmentSize = 0x00000020;

        public static readonly byte[] Version2SignatureBytes = [0x50, 0x46, 0x46, 0x32];

        public const string Version2SignatureString = "PFF2";

        public const uint Version2SegmentSize = 0x00000020;

        public static readonly byte[] Version3SignatureBytes = [0x50, 0x46, 0x46, 0x33];

        public const string Version3SignatureString = "PFF3";

        public const uint Version3SegmentSize = 0x00000024;

        public static readonly byte[] Version4SignatureBytes = [0x50, 0x46, 0x46, 0x34];

        public const string Version4SignatureString = "PFF4";

        public const uint Version4SegmentSize = 0x00000028;

        public const string FooterKingTag = "KING";
    }
}