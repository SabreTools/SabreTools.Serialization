namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class Header4 : Header
    {
        public byte[] FileMD5 { get; set; } = new byte[0x10];

        public string Name { get; set; } = string.Empty;

        public byte[] HeaderMD5 { get; set; } = new byte[0x10];

        public uint HeaderLength { get; set; }

        public uint FileDataOffset { get; set; }

        public uint Dummy0 { get; set; }
    }
}
